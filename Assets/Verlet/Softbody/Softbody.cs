using System;
using UnityEngine;
using System.Collections.Generic;

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
  protected List<Verlet.DistConstraint> constraints;
  protected Mesh bodyMesh;
  protected Vector3[] originalVerts;
  protected Vector3[] bodyVerts;
  protected Vector4[] kabschVerts;
  protected Vector3[] prevBodyVerts;
  protected Vector4[] accumulatedDisplacements;
  protected int[] bodyTriangles;
  protected Vector3[] bodyNormals;
  protected Vector3[] renderVerts;
  protected Vector3[] renderNormals;
  protected Vector3 scaledGravity;
  protected KabschSolver kabschSolver = new KabschSolver();
  protected float initialVolume = (4f / 3f) * Mathf.PI;
  protected float initialSurfaceArea = 4f * Mathf.PI;
  protected float previousDeltaTime = 1f;

  void Start() {
    //Initialize mesh and state variables
    MeshFilter filter = GetComponent<MeshFilter>();
    bodyMesh = Instantiate(filter.mesh);
    bodyMesh.MarkDynamic();
    bodyVerts = bodyMesh.vertices;
    kabschVerts = Array.ConvertAll(bodyVerts, (p => new Vector4(p.x, p.y, p.z, 1f)));
    originalVerts = bodyMesh.vertices;
    bodyTriangles = bodyMesh.triangles;
    bodyNormals = bodyMesh.normals;
    renderNormals = bodyMesh.normals;
    prevBodyVerts = new Vector3[bodyVerts.Length];
    accumulatedDisplacements = new Vector4[bodyVerts.Length];
    for (int i = 0; i < bodyVerts.Length; i++) {
      prevBodyVerts[i] = transform.TransformPoint(bodyVerts[i]);
    }
    filter.mesh = bodyMesh;

    //Create Distance Constraints from Triangles in Mesh
    constraints = new List<Verlet.DistConstraint>(bodyVerts.Length * 3);
    Verlet.setUpConstraints(bodyMesh, constraints, false);

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
      Verlet.resolveDistanceConstraints(constraints, ref bodyVerts, ref accumulatedDisplacements, 1);

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
    Matrix4x4 toWorldSpace = kabschSolver.SolveKabsch(originalVerts, kabschVerts, transformFollowsRotation);
    transform.position = toWorldSpace.GetVector3();
    transform.rotation = toWorldSpace.GetQuaternion();

    //Move the points into local space for rendering
    for (int i = 0; i < bodyVerts.Length; i++) {
      bodyVerts[i] = transform.InverseTransformPoint(bodyVerts[i]);
      renderNormals[i] = transform.InverseTransformDirection(bodyNormals[i]);
    }

    //Graphics
    bodyMesh.vertices = bodyVerts;
    bodyMesh.normals = renderNormals;
    bodyMesh.RecalculateBounds();
    bodyMesh.UploadMeshData(false);
  }
}
