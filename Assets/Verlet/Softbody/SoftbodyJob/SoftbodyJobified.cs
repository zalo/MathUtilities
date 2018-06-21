using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Profiling;
using Unity.Jobs.LowLevel.Unsafe;

[RequireComponent(typeof(MeshFilter))]
public class SoftbodyJobified : MonoBehaviour {
  public Transform anchor;
  public Transform groundPlane;
  [Range(0.3f, 2f)]
  public float inflationAmount = 0.8f;
  [Range(1, 20)]
  public int solverIterations = 1;
  public bool transformFollowsRotation = false;

  public struct SoftbodyData : IDisposable {
    public NativeArray<Verlet.DistConstraint> constraintsArray;
    public NativeArray<Vector3> bodyVerts;
    public NativeArray<Vector4> kabschVerts;
    public NativeArray<Vector3> prevBodyVerts;
    public NativeArray<Vector4> accumulatedDisplacements;
    public NativeArray<Vector3Int> bodyTriangles;
    public NativeArray<Vector3> bodyNormals;
    public NativeArray<Vector3> renderNormals;
    public NativeArray<float> dilationDistance;

    public Matrix4x4 localToWorld, worldToLocal;
    public Vector3 scaledGravity, groundPlanePos, groundPlaneNormal;
    public float initialVolume, initialSurfaceArea, previousDeltaTime;

    public SoftbodyData(Mesh bodyMesh, Transform transform, List<Verlet.DistConstraint> constraintsList, Vector3 scaledGravity, Transform ground) {
      bodyVerts = new NativeArray<Vector3>(bodyMesh.vertices, Allocator.Persistent);
      kabschVerts = new NativeArray<Vector4>(Array.ConvertAll(bodyVerts.ToArray(), (p => new Vector4(p.x, p.y, p.z, 1f))), Allocator.Persistent);
      int[] triangles = bodyMesh.triangles; Vector3Int[] tris = new Vector3Int[triangles.Length / 3];
      for (int i = 0; i < tris.Length; i++) {
        tris[i] = new Vector3Int(triangles[i * 3], triangles[(i * 3) + 1], triangles[(i * 3) + 2]);
      }
      bodyTriangles = new NativeArray<Vector3Int>(tris, Allocator.Persistent);
      bodyNormals = new NativeArray<Vector3>(bodyMesh.normals, Allocator.Persistent);
      renderNormals = new NativeArray<Vector3>(bodyMesh.normals, Allocator.Persistent);
      prevBodyVerts = new NativeArray<Vector3>(new Vector3[bodyVerts.Length], Allocator.Persistent);
      accumulatedDisplacements = new NativeArray<Vector4>(new Vector4[bodyVerts.Length], Allocator.Persistent);
      for (int i = 0; i < bodyVerts.Length; i++) {
        prevBodyVerts[i] = transform.TransformPoint(bodyVerts[i]);
      }
      constraintsArray = new NativeArray<Verlet.DistConstraint>(constraintsList.ToArray(), Allocator.Persistent);
      dilationDistance = new NativeArray<float>(new float[1], Allocator.Persistent);

      initialVolume = Verlet.VolumeOfMesh(bodyVerts, bodyTriangles);
      initialSurfaceArea = 4f * 3.14159f;
      previousDeltaTime = 1f;
      dilationDistance[0] = 0f;
      this.scaledGravity = scaledGravity;
      localToWorld = transform.localToWorldMatrix;
      worldToLocal = transform.worldToLocalMatrix;
      groundPlanePos = ground.position;
      groundPlaneNormal = -ground.forward;
    }

    public void Dispose() {
      bodyVerts.Dispose();
      kabschVerts.Dispose();
      bodyNormals.Dispose();
      bodyTriangles.Dispose();
      renderNormals.Dispose();
      prevBodyVerts.Dispose();
      constraintsArray.Dispose();
      dilationDistance.Dispose();
      accumulatedDisplacements.Dispose();
    }
  }

  [ComputeJobOptimization]
  public struct ToWorldSpaceJob : IJobParallelFor {
    [ReadOnly]
    public Matrix4x4 localToWorld;
    public NativeArray<Vector3> bodyVerts;
    public void Execute(int i) {
      bodyVerts[i] = localToWorld.MultiplyPoint3x4(bodyVerts[i]);
    }
  }

  [ComputeJobOptimization]
  public struct VerletIntegrateJob : IJobParallelFor {
    public NativeArray<Vector3> bodyVerts, prevBodyVerts;
    public Vector3 scaledGravity;
    public void Execute(int i) {
      Vector3 tempPos = bodyVerts[i];
      bodyVerts[i] += (bodyVerts[i] - prevBodyVerts[i]) + (scaledGravity / 3600f);
      prevBodyVerts[i] = tempPos;
    }
  }

  [ComputeJobOptimization]
  public struct AccumulateDistanceConstraintsJob : IJob {//ParallelFor {
    [ReadOnly]
    public NativeArray<Vector3> bodyVerts;
    public NativeArray<Vector4> accumulatedDisplacements;
    public NativeArray<Verlet.DistConstraint> constraintsArray;
    public void Execute(){//int i) {
      for (int i = 0; i < constraintsArray.Length; i++) {
        constraintsArray[i].ResolveConstraint(bodyVerts, ref accumulatedDisplacements);
      }
    }
  }

  [ComputeJobOptimization]
  public struct ApplyAccumulatedConstraintsJob : IJobParallelFor {
    public NativeArray<Vector3> bodyVerts;
    public NativeArray<Vector4> accumulatedDisplacements;
    public void Execute(int i) {
      if (accumulatedDisplacements[i] != new Vector4(0f, 0f, 0f, 0f)) { accumulatedDisplacements[i] /= accumulatedDisplacements[i][3]; }
      bodyVerts[i] += new Vector3(accumulatedDisplacements[i][0], accumulatedDisplacements[i][1], accumulatedDisplacements[i][2]);
      accumulatedDisplacements[i] = new Vector4(0f, 0f, 0f, 0f);
    }
  }

  //NOT PARALLEL KOSHER
  //hmm what if you were able to precalculate some soft of structure that allows each vert to know all the triangles connected to it
  //then you could have 1 job that just calculates the normal of each triangle, and stores that into an array(regular parallel for job)
  //and then another job that loops through each vertex

  [ComputeJobOptimization]
  public struct AccumulateNormalsJob : IJob{//ParallelFor {
    [ReadOnly]
    public NativeArray<Vector3> bodyVerts;
    [ReadOnly]
    public NativeArray<Vector3Int> bodyTriangles;
    public NativeArray<Vector3> bodyNormals;
    public void Execute(){//int i) {
      for (int i = 0; i < bodyTriangles.Length; i++) {
        Vector3 normal = Vector3.Cross(bodyVerts[bodyTriangles[i].x] - bodyVerts[bodyTriangles[i].y],
                                       bodyVerts[bodyTriangles[i].x] - bodyVerts[bodyTriangles[i].z]);
        bodyNormals[bodyTriangles[i].x] += normal;
        bodyNormals[bodyTriangles[i].y] += normal;
        bodyNormals[bodyTriangles[i].z] += normal;
      }
    }
  }

  [ComputeJobOptimization]
  public struct NormalizeNormalsJob : IJobParallelFor {
    public NativeArray<Vector3> bodyNormals;
    public void Execute(int i) {
      bodyNormals[i] = bodyNormals[i]/bodyNormals[i].magnitude;
    }
  }

  [ComputeJobOptimization]
  public struct CalculateSurfaceAreaAndVolumeJob : IJob {
    [ReadOnly]
    public NativeArray<Vector3> bodyVerts;
    [ReadOnly]
    public NativeArray<Vector3Int> bodyTriangles;
    [ReadOnly]
    public float initialVolume, initialSurfaceArea;
    [WriteOnly]
    public NativeArray<float> dilationDistance;
    public void Execute() {
      //First, Calculate Current Volume
      float sum = 0f;
      for (int i = 0; i < bodyTriangles.Length; i++) {
        sum += Vector3.Dot(bodyVerts[bodyTriangles[i].x],
               Vector3.Cross(bodyVerts[bodyTriangles[i].y],
                             bodyVerts[bodyTriangles[i].z])) * 0.166666f;
      }
      float curVolume = Mathf.Abs(sum);

      //And the distance we have to dilate each vert to acheive the desired volume...
      float deltaVolume = initialVolume - curVolume;
      deltaVolume = (/*explosionResistance &&*/ curVolume > initialVolume * 2f) ? 0f : deltaVolume; //Explosion resistance
      if (deltaVolume > 0 || /*equality*/ true) {
        sum = 0f;
        for (int i = 0; i < bodyTriangles.Length; i++) {
          sum += Vector3.Cross(bodyVerts[bodyTriangles[i].y] - bodyVerts[bodyTriangles[i].x],
                               bodyVerts[bodyTriangles[i].z] - bodyVerts[bodyTriangles[i].x]).magnitude * 0.5f;
        }
        float curSurfaceArea = Mathf.Abs(sum);

        dilationDistance[0] = deltaVolume / (initialSurfaceArea == 0f ? curSurfaceArea : initialSurfaceArea);
      }
    }
  }

  [ComputeJobOptimization]
  public struct ExtrudeNormalsJob : IJobParallelFor {
    public NativeArray<Vector3> bodyVerts;
    [ReadOnly]
    public NativeArray<Vector3> bodyNormals;
    [ReadOnly]
    public NativeArray<float> dilationDistance;
    public void Execute(int i) {
      bodyVerts[i] += bodyNormals[i] * dilationDistance[0];
    }
  }

  [ComputeJobOptimization]
  public struct GroundCollideJob : IJobParallelFor {
    public NativeArray<Vector3> bodyVerts;
    [ReadOnly]
    public NativeArray<Vector3> prevBodyVerts;
    [ReadOnly]
    public Vector3 groundPlanePos, groundPlaneNormal;
    public void Execute(int i) {
      if (Vector3.Dot(bodyVerts[i] - groundPlanePos, groundPlaneNormal) < 0f) {
        bodyVerts[i] = projectToPlane(bodyVerts[i]-groundPlanePos, groundPlaneNormal) + groundPlanePos;
        bodyVerts[i] -= projectToPlane(bodyVerts[i]-prevBodyVerts[i], groundPlaneNormal) * 0.3f;
      }
    }
  }

  private static Vector3 projectToPlane(Vector3 point, Vector3 normalizedPlaneNormal) {
    return point - (Vector3.Dot(point, normalizedPlaneNormal) * normalizedPlaneNormal);
  }

  [ComputeJobOptimization]
  public struct ToLocalSpaceJob : IJobParallelFor {
    [ReadOnly]
    public Matrix4x4 worldToLocal;
    public NativeArray<Vector3> bodyVerts, renderNormals, bodyNormals;
    public void Execute(int i) {
      bodyVerts[i] = worldToLocal.MultiplyPoint3x4(bodyVerts[i]);
      renderNormals[i] = worldToLocal.MultiplyVector(bodyNormals[i]);
    }
  }

  protected SoftbodyData softbodyData;
  protected const bool fastButGarbage = false;
  protected List<Verlet.DistConstraint> constraintsList;
  protected Mesh bodyMesh;
  protected Vector3[] originalVerts;
  protected Vector3[] renderNormalsArray;
  protected Vector3[] bodyVertsArray;
  protected Vector4[] kabschVertsArray;
  protected KabschSolver kabschSolver = new KabschSolver();
  protected Vector3 scaledGravity;
  protected bool priorDebuggerState;

  JobHandle groundCollide;

  void Start() {
    //Initialize mesh and state variables
    MeshFilter filter = GetComponent<MeshFilter>();
    bodyMesh = Instantiate(filter.mesh);
    bodyMesh.MarkDynamic();
    originalVerts = bodyMesh.vertices;
    filter.mesh = bodyMesh;
    renderNormalsArray = new Vector3[originalVerts.Length];
    bodyVertsArray = new Vector3[originalVerts.Length];
    kabschVertsArray = new Vector4[originalVerts.Length];

    //Create Distance Constraints from Triangles in Mesh
    constraintsList = new List<Verlet.DistConstraint>(originalVerts.Length * 3);
    Verlet.setUpConstraints(bodyMesh, constraintsList, false);

    softbodyData = new SoftbodyData(bodyMesh, transform, constraintsList, 
      new Vector3(Physics.gravity.x / transform.lossyScale.x, 
                  Physics.gravity.y / transform.lossyScale.y, 
                  Physics.gravity.z / transform.lossyScale.z), groundPlane);

    priorDebuggerState = JobsUtility.JobDebuggerEnabled;
    JobsUtility.JobDebuggerEnabled = false;
  }

  void Update() {
    Profiler.BeginSample("Schedule Softbody Work", this);

    //Transform the points into world space
    JobHandle localToWorldHandle = new ToWorldSpaceJob () {
      localToWorld = transform.localToWorldMatrix,
      bodyVerts = softbodyData.bodyVerts
    }.Schedule(originalVerts.Length, 64);

    //Physics - Verlet Integration
    JobHandle verletHandle = new VerletIntegrateJob() {
      bodyVerts = softbodyData.bodyVerts,
      prevBodyVerts = softbodyData.prevBodyVerts,
      scaledGravity = softbodyData.scaledGravity
    }.Schedule(originalVerts.Length, 64, dependsOn: localToWorldHandle);

    JobHandle previousHandle = verletHandle;
    for (int i = 0; i < solverIterations; i++) {
      //First, ensure that the surface area is what we think it is
      JobHandle accumulateDistances = new AccumulateDistanceConstraintsJob() {
        bodyVerts = softbodyData.bodyVerts,
        accumulatedDisplacements = softbodyData.accumulatedDisplacements,
        constraintsArray = softbodyData.constraintsArray
      }.Schedule(dependsOn: previousHandle);
      JobHandle applyConstraints = new ApplyAccumulatedConstraintsJob() {
        bodyVerts = softbodyData.bodyVerts,
        accumulatedDisplacements = softbodyData.accumulatedDisplacements
      }.Schedule(originalVerts.Length, 64, dependsOn: accumulateDistances);

      //Next, set the volume of the soft body
      JobHandle accumulateNormals = new AccumulateNormalsJob() {
        bodyVerts = softbodyData.bodyVerts,
        bodyTriangles = softbodyData.bodyTriangles,
        bodyNormals = softbodyData.bodyNormals
      }.Schedule(dependsOn: applyConstraints);
      JobHandle normalizeNormals = new NormalizeNormalsJob() {
        bodyNormals = softbodyData.bodyNormals
      }.Schedule(originalVerts.Length, 64, dependsOn: accumulateNormals);
      JobHandle calculateDilationDistance = new CalculateSurfaceAreaAndVolumeJob() {
        bodyVerts = softbodyData.bodyVerts,
        bodyTriangles = softbodyData.bodyTriangles,
        initialVolume = softbodyData.initialVolume,
        initialSurfaceArea = softbodyData.initialSurfaceArea,
        dilationDistance = softbodyData.dilationDistance
      }.Schedule(dependsOn: normalizeNormals);
      previousHandle = new ExtrudeNormalsJob() {
        bodyVerts = softbodyData.bodyVerts,
        bodyNormals = softbodyData.bodyNormals,
        dilationDistance = softbodyData.dilationDistance
      }.Schedule(originalVerts.Length, 64, dependsOn: calculateDilationDistance);
    }

    //Also sneak in a ground plane here:
    JobHandle groundPlaneHandle = new GroundCollideJob() {
      bodyVerts = softbodyData.bodyVerts,
      prevBodyVerts = softbodyData.prevBodyVerts,
      groundPlanePos = groundPlane.position,
      groundPlaneNormal = -groundPlane.forward
    }.Schedule(originalVerts.Length, 64, dependsOn: previousHandle);

    Profiler.EndSample();

    groundPlaneHandle.Complete();

    //Calculate the the position and rotation of the body
    for (int i = 0; i < softbodyData.bodyVerts.Length; i++) {
      softbodyData.kabschVerts[i] = new Vector4(
        softbodyData.bodyVerts[i].x, 
        softbodyData.bodyVerts[i].y, 
        softbodyData.bodyVerts[i].z, 1f);
    };
    softbodyData.kabschVerts.CopyTo(kabschVertsArray);
    Matrix4x4 toWorldSpace = kabschSolver.SolveKabsch(originalVerts, kabschVertsArray, transformFollowsRotation);
    transform.position = toWorldSpace.GetVector3();
    transform.rotation = toWorldSpace.GetQuaternion();

    //Move the points into local space for rendering
    JobHandle toLocalHandle = new ToLocalSpaceJob() {
      bodyVerts = softbodyData.bodyVerts,
      bodyNormals = softbodyData.bodyNormals,
      renderNormals = softbodyData.renderNormals,
      worldToLocal = transform.worldToLocalMatrix
    }.Schedule(originalVerts.Length, 64);
    toLocalHandle.Complete();

    //Graphics
    softbodyData.bodyVerts.CopyTo(bodyVertsArray);
    softbodyData.renderNormals.CopyTo(renderNormalsArray);
    bodyMesh.vertices = bodyVertsArray;
    bodyMesh.normals = renderNormalsArray;
    bodyMesh.RecalculateBounds();
    bodyMesh.UploadMeshData(false);
  }

  void OnDestroy() {
    softbodyData.Dispose();
    JobsUtility.JobDebuggerEnabled = priorDebuggerState;
  }
}
