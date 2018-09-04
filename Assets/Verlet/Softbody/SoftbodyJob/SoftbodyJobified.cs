using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;
using Unity.Jobs;
using Unity.Collections;
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
  public bool parallelNormals = false;
  public bool parallelConstraints = false;
  public bool useAreaVolumeAccumulator = false;

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
    public NativeArray<vertexTriangles> triangleGraph;
    public NativeArray<vertexConstraints> connectionGraph;
    public NativeArray<float> triangleVolumes;
    public NativeArray<float> triangleSurfaceAreas;
    public NativeAccumulator<float, Addition> volumeAccumulator;
    public NativeAccumulator<float, Addition> areaAccumulator;

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
      triangleVolumes = new NativeArray<float>(new float[tris.Length], Allocator.Persistent);
      triangleSurfaceAreas = new NativeArray<float>(new float[tris.Length], Allocator.Persistent);

      volumeAccumulator = new NativeAccumulator<float, Addition>(Allocator.Persistent);
      areaAccumulator = new NativeAccumulator<float, Addition>(Allocator.Persistent);

      initialVolume = Verlet.VolumeOfMesh(bodyVerts, bodyTriangles);
      initialSurfaceArea = 4f * 3.14159f;
      previousDeltaTime = 1f;
      dilationDistance[0] = 0f;
      this.scaledGravity = scaledGravity;
      localToWorld = transform.localToWorldMatrix;
      worldToLocal = transform.worldToLocalMatrix;
      groundPlanePos = ground.position;
      groundPlaneNormal = -ground.forward;

      //Set up parallel normal accumulation
      triangleGraph = new NativeArray<vertexTriangles>(new vertexTriangles[bodyVerts.Length], Allocator.Persistent);
      for (int i = 0; i < triangleGraph.Length; i++) {
        triangleGraph[i] = new vertexTriangles(-1);
      }
      for (int i = 0; i < tris.Length; i++) {
        triangleGraph[tris[i].x] = triangleGraph[tris[i].x].Add(tris[i]);
        triangleGraph[tris[i].y] = triangleGraph[tris[i].y].Add(tris[i]);
        triangleGraph[tris[i].z] = triangleGraph[tris[i].z].Add(tris[i]);
      }

      //Set up parallel constraint satisfaction
      connectionGraph = new NativeArray<vertexConstraints>(new vertexConstraints[bodyVerts.Length], Allocator.Persistent);
      for (int i = 0; i < connectionGraph.Length; i++) {
        connectionGraph[i] = new vertexConstraints();
      }
      for (int i = 0; i < constraintsArray.Length; i++) {
        Verlet.DistConstraint constraint = constraintsArray[i];
        connectionGraph[constraint.index1] = connectionGraph[constraint.index1].Add(constraint);
        int temp = constraint.index1; constraint.index1 = constraint.index2; constraint.index2 = temp;
        connectionGraph[constraint.index1] = connectionGraph[constraint.index1].Add(constraint);
      }
    }

    public void Dispose() {
      bodyVerts.Dispose();
      kabschVerts.Dispose();
      bodyNormals.Dispose();
      bodyTriangles.Dispose();
      renderNormals.Dispose();
      prevBodyVerts.Dispose();
      triangleGraph.Dispose();
      connectionGraph.Dispose();
      triangleVolumes.Dispose();
      areaAccumulator.Dispose();
      constraintsArray.Dispose();
      dilationDistance.Dispose();
      volumeAccumulator.Dispose();
      triangleSurfaceAreas.Dispose();
      accumulatedDisplacements.Dispose();
    }
  }

  [Unity.Burst.BurstCompile]
  public struct ToWorldSpaceJob : IJobParallelFor {
    [ReadOnly]
    public Matrix4x4 localToWorld;
    public NativeArray<Vector3> bodyVerts;
    public void Execute(int i) {
      bodyVerts[i] = localToWorld.MultiplyPoint3x4(bodyVerts[i]);
    }
  }

  [Unity.Burst.BurstCompile]
  public struct VerletIntegrateJob : IJobParallelFor {
    public NativeArray<Vector3> bodyVerts, prevBodyVerts;
    public Vector3 scaledGravity;
    public void Execute(int i) {
      Vector3 tempPos = bodyVerts[i];
      bodyVerts[i] += (bodyVerts[i] - prevBodyVerts[i]) + (scaledGravity / 3600f);
      prevBodyVerts[i] = tempPos;
    }
  }

  [Unity.Burst.BurstCompile]
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

  [Unity.Burst.BurstCompile]
  public struct ApplyAccumulatedConstraintsJob : IJobParallelFor {
    public NativeArray<Vector3> bodyVerts;
    public NativeArray<Vector4> accumulatedDisplacements;
    public void Execute(int i) {
      if (accumulatedDisplacements[i] != new Vector4(0f, 0f, 0f, 0f)) { accumulatedDisplacements[i] /= accumulatedDisplacements[i][3]; }
      bodyVerts[i] += new Vector3(accumulatedDisplacements[i][0], accumulatedDisplacements[i][1], accumulatedDisplacements[i][2]);
      accumulatedDisplacements[i] = new Vector4(0f, 0f, 0f, 0f);
    }
  }

  [Unity.Burst.BurstCompile]
  public struct CalculateDistanceConstraintsJob : IJobParallelFor {
    [ReadOnly]
    public NativeArray<Vector3> bodyVerts;
    [ReadOnly]
    public NativeArray<vertexConstraints> constraintsArray;
    public NativeArray<Vector4> accumulatedDisplacements;
    public void Execute(int i) {
      constraintsArray[i].c1.ResolveConstraint(bodyVerts, ref accumulatedDisplacements, false);
      constraintsArray[i].c2.ResolveConstraint(bodyVerts, ref accumulatedDisplacements, false);
      constraintsArray[i].c3.ResolveConstraint(bodyVerts, ref accumulatedDisplacements, false);
      constraintsArray[i].c4.ResolveConstraint(bodyVerts, ref accumulatedDisplacements, false);
      constraintsArray[i].c5.ResolveConstraint(bodyVerts, ref accumulatedDisplacements, false);
      constraintsArray[i].c6.ResolveConstraint(bodyVerts, ref accumulatedDisplacements, false);
    }
  }

  //NOT PARALLEL KOSHER
  //hmm what if you were able to precalculate some soft of structure that allows each vert to know all the triangles connected to it
  //then you could have 1 job that just calculates the normal of each triangle, and stores that into an array(regular parallel for job)
  //and then another job that loops through each vertex

  [Unity.Burst.BurstCompile]
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

  [Unity.Burst.BurstCompile]
  public struct NormalizeNormalsJob : IJobParallelFor {
    public NativeArray<Vector3> bodyNormals;
    public void Execute(int i) {
      bodyNormals[i] = bodyNormals[i]/bodyNormals[i].magnitude;
    }
  }

  [Unity.Burst.BurstCompile]
  public struct GatherNormalsJob : IJobParallelFor {
    [ReadOnly]
    public NativeArray<Vector3> bodyVerts;
    [ReadOnly]
    public NativeArray<vertexTriangles> vertexConnections;
    //[WriteOnly]
    public NativeArray<Vector3> bodyNormals;
    public void Execute(int i) {
      /*for (int j = 0; j < vertexConnections[i].numValid.x; j++) {
        bodyNormals[i] += Vector3.Cross(bodyVerts[vertexConnections[i][j].x] - bodyVerts[vertexConnections[i][j].y],
                                        bodyVerts[vertexConnections[i][j].x] - bodyVerts[vertexConnections[i][j].z]);
      }*/
      Vector3 normal = new Vector3(0f, 0f, 0f);
      if (vertexConnections[i].triangle1.x != -1) {
        normal += Vector3.Cross(bodyVerts[vertexConnections[i].triangle1.x] - bodyVerts[vertexConnections[i].triangle1.y],
                                bodyVerts[vertexConnections[i].triangle1.x] - bodyVerts[vertexConnections[i].triangle1.z]);
      }
      if (vertexConnections[i].triangle2.x != -1) {
        normal += Vector3.Cross(bodyVerts[vertexConnections[i].triangle2.x] - bodyVerts[vertexConnections[i].triangle2.y],
                                bodyVerts[vertexConnections[i].triangle2.x] - bodyVerts[vertexConnections[i].triangle2.z]);
      }
      if (vertexConnections[i].triangle3.x != -1) {
        normal += Vector3.Cross(bodyVerts[vertexConnections[i].triangle3.x] - bodyVerts[vertexConnections[i].triangle3.y],
                                bodyVerts[vertexConnections[i].triangle3.x] - bodyVerts[vertexConnections[i].triangle3.z]);
      }
      if (vertexConnections[i].triangle4.x != -1) {
        normal += Vector3.Cross(bodyVerts[vertexConnections[i].triangle4.x] - bodyVerts[vertexConnections[i].triangle4.y],
                                bodyVerts[vertexConnections[i].triangle4.x] - bodyVerts[vertexConnections[i].triangle4.z]);
      }
      if (vertexConnections[i].triangle5.x != -1) {
        normal += Vector3.Cross(bodyVerts[vertexConnections[i].triangle5.x] - bodyVerts[vertexConnections[i].triangle5.y],
                                bodyVerts[vertexConnections[i].triangle5.x] - bodyVerts[vertexConnections[i].triangle5.z]);
      }
      if (vertexConnections[i].triangle6.x != -1) {
        normal += Vector3.Cross(bodyVerts[vertexConnections[i].triangle6.x] - bodyVerts[vertexConnections[i].triangle6.y],
                                bodyVerts[vertexConnections[i].triangle6.x] - bodyVerts[vertexConnections[i].triangle6.z]);
      }

      bodyNormals[i] = normal / normal.magnitude;
    }
  }

  [Unity.Burst.BurstCompile]
  public struct AccumulateSurfaceAreaAndVolumeJob : IJobParallelFor {
    [ReadOnly]
    public NativeArray<Vector3> bodyVerts;
    [ReadOnly]
    public NativeArray<Vector3Int> bodyTriangles;
    [WriteOnly]
    public NativeArray<float> triangleVolumes, triangleSurfaceAreas;
    public void Execute(int i) {
      //First, Accumulate the Current Volume
      triangleVolumes[i] = Vector3.Dot(bodyVerts[bodyTriangles[i].x],
                                       Vector3.Cross(bodyVerts[bodyTriangles[i].y],
                                                     bodyVerts[bodyTriangles[i].z])) * 0.166666f;

      //Second, Accumulate the Current Surface Area
      triangleSurfaceAreas[i] = Vector3.Cross(bodyVerts[bodyTriangles[i].y] - bodyVerts[bodyTriangles[i].x],
                                              bodyVerts[bodyTriangles[i].z] - bodyVerts[bodyTriangles[i].x]).magnitude * 0.5f;

    }
  }

  [Unity.Burst.BurstCompile]
  public struct SurfaceAreaAndVolumeAccumulatorJob : IJobParallelFor {
    [ReadOnly]
    public NativeArray<Vector3> bodyVerts;
    [ReadOnly]
    public NativeArray<Vector3Int> bodyTriangles;
    public NativeAccumulator<float, Addition>.Concurrent volumeAccumulator, areaAccumulator;
    public void Execute(int i) {
      volumeAccumulator.Accumulate(Vector3.Dot(bodyVerts[bodyTriangles[i].x],
                                 Vector3.Cross(bodyVerts[bodyTriangles[i].y],
                                               bodyVerts[bodyTriangles[i].z])) * 0.166666f);

      areaAccumulator.Accumulate(Vector3.Cross(bodyVerts[bodyTriangles[i].y] - bodyVerts[bodyTriangles[i].x],
                                               bodyVerts[bodyTriangles[i].z] - bodyVerts[bodyTriangles[i].x]).magnitude * 0.5f);
    }
  }

  [Unity.Burst.BurstCompile]
  public struct CalculateSurfaceAreaAndVolumeJob : IJob {
    [ReadOnly]
    public NativeArray<float> triangleVolumes, triangleSurfaceAreas;
    [ReadOnly]
    public float initialVolume, initialSurfaceArea;
    [WriteOnly]
    public NativeArray<float> dilationDistance;
    public void Execute() {
      //First, Calculate Current Volume
      float curVolume = 0f;
      for (int i = 0; i < triangleVolumes.Length; i++) {
        curVolume += triangleVolumes[i];
      }
      curVolume = Mathf.Abs(curVolume);

      //And the distance we have to dilate each vert to acheive the desired volume...
      float deltaVolume = initialVolume - curVolume;
      deltaVolume = (/*explosionResistance &&*/ curVolume > initialVolume * 2f) ? 0f : deltaVolume; //Explosion resistance
      if (deltaVolume > 0 || /*equality*/ true) {
        float curSurfaceArea = 0f;
        for (int i = 0; i < triangleSurfaceAreas.Length; i++) {
          curSurfaceArea += triangleSurfaceAreas[0];
        }
        curSurfaceArea = Mathf.Abs(curSurfaceArea);

        dilationDistance[0] = deltaVolume / (initialSurfaceArea == 0f ? curSurfaceArea : initialSurfaceArea);
      }
    }
  }

  [Unity.Burst.BurstCompile]
  public struct CalculateDilationJob : IJob {
    [ReadOnly]
    public NativeAccumulator<float, Addition> volumeAccumulator, areaAccumulator;
    [ReadOnly]
    public float initialVolume, initialSurfaceArea;
    [WriteOnly]
    public NativeArray<float> dilationDistance;
    public void Execute() {
      //First, Calculate Current Volume
      float curVolume = Mathf.Abs(volumeAccumulator.Value);

      //And the distance we have to dilate each vert to acheive the desired volume...
      float deltaVolume = initialVolume - curVolume;
      deltaVolume = (/*explosionResistance &&*/ curVolume > initialVolume * 2f) ? 0f : deltaVolume; //Explosion resistance
      if (deltaVolume > 0 || /*equality*/ true) {
        float curSurfaceArea = Mathf.Abs(areaAccumulator.Value);

        dilationDistance[0] = deltaVolume / (initialSurfaceArea == 0f ? curSurfaceArea : initialSurfaceArea);
      }
    }
  }

  [Unity.Burst.BurstCompile]
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

  [Unity.Burst.BurstCompile]
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

  //[Unity.Burst.BurstCompile]
  private static Vector3 projectToPlane(Vector3 point, Vector3 normalizedPlaneNormal) {
    return point - (Vector3.Dot(point, normalizedPlaneNormal) * normalizedPlaneNormal);
  }

  [Unity.Burst.BurstCompile]
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

    int batchSize = 16;

    //Transform the points into world space
    JobHandle localToWorldHandle = new ToWorldSpaceJob () {
      localToWorld = transform.localToWorldMatrix,
      bodyVerts = softbodyData.bodyVerts
    }.Schedule(originalVerts.Length, batchSize);

    //Physics - Verlet Integration
    JobHandle verletHandle = new VerletIntegrateJob() {
      bodyVerts = softbodyData.bodyVerts,
      prevBodyVerts = softbodyData.prevBodyVerts,
      scaledGravity = softbodyData.scaledGravity
    }.Schedule(originalVerts.Length, batchSize, dependsOn: localToWorldHandle);

    JobHandle previousHandle = verletHandle;
    for (int i = 0; i < solverIterations; i++) {
      //First, ensure that the surface area is what we think it is
      JobHandle applyConstraints;
      if (parallelConstraints) {
        JobHandle calculateConstraints = new CalculateDistanceConstraintsJob() {
          bodyVerts = softbodyData.bodyVerts,
          accumulatedDisplacements = softbodyData.accumulatedDisplacements,
          constraintsArray = softbodyData.connectionGraph
        }.Schedule(originalVerts.Length, batchSize, dependsOn: previousHandle);
        applyConstraints = new ApplyAccumulatedConstraintsJob() {
          bodyVerts = softbodyData.bodyVerts,
          accumulatedDisplacements = softbodyData.accumulatedDisplacements
        }.Schedule(originalVerts.Length, batchSize, dependsOn: calculateConstraints);
      } else {
        JobHandle accumulateDistances = new AccumulateDistanceConstraintsJob() {
          bodyVerts = softbodyData.bodyVerts,
          accumulatedDisplacements = softbodyData.accumulatedDisplacements,
          constraintsArray = softbodyData.constraintsArray
        }.Schedule(dependsOn: previousHandle);
        applyConstraints = new ApplyAccumulatedConstraintsJob() {
          bodyVerts = softbodyData.bodyVerts,
          accumulatedDisplacements = softbodyData.accumulatedDisplacements
        }.Schedule(originalVerts.Length, batchSize, dependsOn: accumulateDistances);
      }

      //Next, set the volume of the soft body
      JobHandle calculateNormals;
      if (parallelNormals) {
        calculateNormals = new GatherNormalsJob() {
          bodyVerts = softbodyData.bodyVerts,
          vertexConnections = softbodyData.triangleGraph,
          bodyNormals = softbodyData.bodyNormals
        }.Schedule(originalVerts.Length, batchSize, dependsOn: applyConstraints);
      } else {
        JobHandle accumulateNormals = new AccumulateNormalsJob() {
          bodyVerts = softbodyData.bodyVerts,
          bodyTriangles = softbodyData.bodyTriangles,
          bodyNormals = softbodyData.bodyNormals
        }.Schedule(dependsOn: applyConstraints);
        calculateNormals = new NormalizeNormalsJob() {
          bodyNormals = softbodyData.bodyNormals
        }.Schedule(originalVerts.Length, batchSize, dependsOn: accumulateNormals);
      }

      JobHandle calculateDilationDistance;
      if (useAreaVolumeAccumulator) {
        JobHandle accumulateDilationDistance = new SurfaceAreaAndVolumeAccumulatorJob() {
          bodyVerts = softbodyData.bodyVerts,
          bodyTriangles = softbodyData.bodyTriangles,
          volumeAccumulator = softbodyData.volumeAccumulator,
          areaAccumulator = softbodyData.areaAccumulator
        }.Schedule(softbodyData.bodyTriangles.Length, batchSize, dependsOn: calculateNormals);

        calculateDilationDistance = new CalculateDilationJob() {
          volumeAccumulator = softbodyData.volumeAccumulator,
          areaAccumulator = softbodyData.areaAccumulator,
          initialVolume = softbodyData.initialVolume * inflationAmount,
          initialSurfaceArea = softbodyData.initialSurfaceArea,
          dilationDistance = softbodyData.dilationDistance
        }.Schedule(dependsOn: accumulateDilationDistance);
      } else {
        JobHandle accumulateDilationDistance = new AccumulateSurfaceAreaAndVolumeJob() {
          bodyVerts = softbodyData.bodyVerts,
          bodyTriangles = softbodyData.bodyTriangles,
          triangleVolumes = softbodyData.triangleVolumes,
          triangleSurfaceAreas = softbodyData.triangleSurfaceAreas
        }.Schedule(softbodyData.bodyTriangles.Length, batchSize, dependsOn: calculateNormals);

        //THIS IS THE LAST BIG ONE
        calculateDilationDistance = new CalculateSurfaceAreaAndVolumeJob() {
          triangleVolumes = softbodyData.triangleVolumes,
          triangleSurfaceAreas = softbodyData.triangleSurfaceAreas,
          initialVolume = softbodyData.initialVolume * inflationAmount,
          initialSurfaceArea = softbodyData.initialSurfaceArea,
          dilationDistance = softbodyData.dilationDistance
        }.Schedule(dependsOn: accumulateDilationDistance);
      }

      previousHandle = new ExtrudeNormalsJob() {
        bodyVerts = softbodyData.bodyVerts,
        bodyNormals = softbodyData.bodyNormals,
        dilationDistance = softbodyData.dilationDistance
      }.Schedule(originalVerts.Length, batchSize, dependsOn: calculateDilationDistance);
    }

    //Also sneak in a ground plane here:
    JobHandle groundPlaneHandle = new GroundCollideJob() {
      bodyVerts = softbodyData.bodyVerts,
      prevBodyVerts = softbodyData.prevBodyVerts,
      groundPlanePos = groundPlane.position,
      groundPlaneNormal = -groundPlane.forward
    }.Schedule(originalVerts.Length, batchSize, dependsOn: previousHandle);

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
    }.Schedule(originalVerts.Length, batchSize);
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


  //Calculate the connections from each vertex to each other vertex
  [StructLayout(LayoutKind.Sequential)]
  public struct vertexTriangles {
    public Vector3Int triangle1, triangle2, triangle3, triangle4, triangle5, triangle6;
    public vertexTriangles(int i = -1) {
      triangle1 = new Vector3Int(-1, -1, -1);
      triangle2 = new Vector3Int(-1, -1, -1);
      triangle3 = new Vector3Int(-1, -1, -1);
      triangle4 = new Vector3Int(-1, -1, -1);
      triangle5 = new Vector3Int(-1, -1, -1);
      triangle6 = new Vector3Int(-1, -1, -1);
    }
    public vertexTriangles Add(Vector3Int addThis) {
      Vector3Int negative = new Vector3Int(-1, -1, -1);
      if (triangle1 == negative) { triangle1 = addThis; return this; }
      if (triangle2 == negative) { triangle2 = addThis; return this; }
      if (triangle3 == negative) { triangle3 = addThis; return this; }
      if (triangle4 == negative) { triangle4 = addThis; return this; }
      if (triangle5 == negative) { triangle5 = addThis; return this; }
      if (triangle6 == negative) { triangle6 = addThis; return this; }
      return this;
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct vertexConstraints {
    public Verlet.DistConstraint c1, c2, c3, c4, c5, c6;
    int numValid;
    public bool Contains(int containsThis) {
      if(containsThis == c1.index2 ||
         containsThis == c2.index2 ||
         containsThis == c3.index2 ||
         containsThis == c4.index2 ||
         containsThis == c5.index2 ||
         containsThis == c6.index2) {
        return true;
      }
      return false;
    }
    public vertexConstraints Add(Verlet.DistConstraint addThis) {
      Verlet.DistConstraint dC = new Verlet.DistConstraint();
      if (c1 == dC) { c1 = addThis; numValid = 1; return this; }
      if (c2 == dC) { c2 = addThis; numValid = 2; return this; }
      if (c3 == dC) { c3 = addThis; numValid = 3; return this; }
      if (c4 == dC) { c4 = addThis; numValid = 4; return this; }
      if (c5 == dC) { c5 = addThis; numValid = 5; return this; }
      if (c6 == dC) { c6 = addThis; numValid = 6; return this; }
      return this;
    }
  }
}
