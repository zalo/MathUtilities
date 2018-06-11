using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;

[RequireComponent(typeof(MeshFilter))]
public class Softbody : MonoBehaviour {
  public Transform anchor;
  public Transform groundPlane;
  [Range(0.3f, 2f)]
  public float inflationAmount = 0.8f;
  [Range(1, 10)]
  public int solverIterations = 1;
  public bool transformFollowsRotation = false;

  protected const bool fastButGarbage = false;
  protected List<Verlet.DistConstraint> constraintsList;
  protected NativeArray<Verlet.DistConstraint> constraintsArray;
  protected Mesh bodyMesh;
  protected Vector3[] originalVerts;
  protected NativeArray<Vector3> bodyVerts;
  protected NativeArray<Vector4> kabschVerts;
  protected NativeArray<Vector3> prevBodyVerts;
  protected NativeArray<Vector4> accumulatedDisplacements;
  protected NativeArray<Vector3Int> bodyTriangles;
  protected NativeArray<Vector3> bodyNormals;
  protected NativeArray<Vector3> renderNormals;
  protected Vector3 scaledGravity;
  protected KabschSolver kabschSolver = new KabschSolver();
  protected Vector3[] renderNormalsArray;
  protected Vector3[] bodyVertsArray;
  protected Vector4[] kabschVertsArray;
  protected float initialVolume = (4f / 3f) * Mathf.PI;
  protected float initialSurfaceArea = 4f * Mathf.PI;
  protected float previousDeltaTime = 1f;

  void Start() {
    //Initialize mesh and state variables
    MeshFilter filter = GetComponent<MeshFilter>();
    bodyMesh = Instantiate(filter.mesh);
    bodyMesh.MarkDynamic();
    bodyVerts = new NativeArray<Vector3>(bodyMesh.vertices, Allocator.Persistent);
    kabschVerts = new NativeArray<Vector4>(Array.ConvertAll(bodyVerts.ToArray(), (p => new Vector4(p.x, p.y, p.z, 1f))), Allocator.Persistent);
    originalVerts = bodyMesh.vertices;
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
    filter.mesh = bodyMesh;
    renderNormalsArray = new Vector3[renderNormals.Length];
    bodyVertsArray = new Vector3[bodyVerts.Length];
    kabschVertsArray = new Vector4[kabschVerts.Length];

    //Create Distance Constraints from Triangles in Mesh
    constraintsList = new List<Verlet.DistConstraint>(bodyVerts.Length * 3);
    Verlet.setUpConstraints(bodyMesh, constraintsList, false);
    constraintsArray = new NativeArray<Verlet.DistConstraint>(constraintsList.ToArray(), Allocator.Persistent);

    //Scale gravity by the size of this Mesh Renderer
    scaledGravity = new Vector3(Physics.gravity.x / transform.lossyScale.x, Physics.gravity.y / transform.lossyScale.y, Physics.gravity.z / transform.lossyScale.z);

    initialVolume = Verlet.VolumeOfMesh(bodyVerts, bodyTriangles);
  }

  void Update() {
    //Translate the points into world space
    for (int i = 0; i < bodyVerts.Length; i++) {
      bodyVerts[i] = transform.TransformPoint(bodyVerts[i]);
    }

    //Physics
    float currentDeltaTime = Mathf.Clamp(Time.deltaTime, 0.01f, previousDeltaTime * 1.4f);
    Verlet.Integrate(bodyVerts, prevBodyVerts, scaledGravity, currentDeltaTime, previousDeltaTime);
    previousDeltaTime = currentDeltaTime;

    //Anchor a point on the body
    if (anchor != null && anchor.gameObject.activeInHierarchy) {
      bodyVerts[0] = prevBodyVerts[0] = anchor.position;
    }

    for (int i = 0; i < solverIterations; i++) {
      //First, ensure that the surface area is what we think it is
      Verlet.resolveDistanceConstraints(constraintsArray, ref bodyVerts, ref accumulatedDisplacements, 1);

      //Next, set the volume of the soft body
      Verlet.setVolume(inflationAmount * initialVolume, bodyVerts, bodyNormals, bodyTriangles, initialSurfaceArea, true, fastButGarbage);
    }

    //Also sneak in a ground plane here:
    Vector3 groundPlanePos = groundPlane.position;
    Vector3 groundPlaneNormal = -groundPlane.forward;
    for (int j = 0; j < bodyVerts.Length; j++) {
      if (Vector3.Dot(bodyVerts[j] - groundPlanePos, groundPlaneNormal) < 0f) {
        bodyVerts[j] = Vector3.ProjectOnPlane(bodyVerts[j] - groundPlanePos, groundPlaneNormal) + groundPlanePos;
        bodyVerts[j] -= Vector3.ProjectOnPlane(bodyVerts[j] - prevBodyVerts[j], groundPlaneNormal) * 0.3f;
      }
    }

    //Calculate the the position and rotation of the body
    for (int i = 0; i < bodyVerts.Length; i++) { kabschVerts[i] = new Vector4(bodyVerts[i].x, bodyVerts[i].y, bodyVerts[i].z, 1f); };
    kabschVerts.CopyTo(kabschVertsArray);
    Matrix4x4 toWorldSpace = kabschSolver.SolveKabsch(originalVerts, kabschVertsArray, transformFollowsRotation);
    transform.position = toWorldSpace.GetVector3();
    transform.rotation = toWorldSpace.GetQuaternion();

    //Move the points into local space for rendering
    for (int i = 0; i < bodyVerts.Length; i++) {
      bodyVerts[i] = transform.InverseTransformPoint(bodyVerts[i]);
      renderNormals[i] = transform.InverseTransformDirection(bodyNormals[i]);
    }

    //Graphics
    bodyVerts.CopyTo(bodyVertsArray);
    renderNormals.CopyTo(renderNormalsArray);
    bodyMesh.vertices = bodyVertsArray;
    bodyMesh.normals = renderNormalsArray;
    bodyMesh.RecalculateBounds();
    bodyMesh.UploadMeshData(false);
  }

  void OnDestroy() {
    bodyVerts.Dispose();
    kabschVerts.Dispose();
    bodyTriangles.Dispose();
    bodyNormals.Dispose();
    renderNormals.Dispose();
    prevBodyVerts.Dispose();
    constraintsArray.Dispose();
    accumulatedDisplacements.Dispose();
  }
}
