using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class VolumeExpander : MonoBehaviour {
  public Transform groundPlane;
  [Range(0.1f, 1.5f)]
  public float inflationAmount = 1.0f;
  [Range(1, 10)]
  public int solverIterations = 1;

  protected const bool fastButGarbage = false;
  protected List<Verlet.DistConstraint> constraints;
  protected Mesh bodyMesh;
  protected Vector3[] originalVerts;
  protected Vector3[] bodyVerts;
  protected Vector4[] kabschVerts;
  protected Vector4[] accumulatedDisplacements;
  protected int[] bodyTriangles;
  protected Vector3[] bodyNormals;
  protected Vector3[] renderVerts;
  protected Vector3[] renderNormals;
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
    accumulatedDisplacements = new Vector4[bodyVerts.Length];
    filter.mesh = bodyMesh;

    //Create Distance Constraints from Triangles in Mesh
    constraints = new List<Verlet.DistConstraint>(bodyVerts.Length * 3);
    Verlet.setUpConstraints(bodyMesh, constraints, false);

    initialVolume = Verlet.VolumeOfMesh(bodyVerts, bodyTriangles);
  }

  void Update() {
    //Translate the points into world space
    for (int i = 0; i < bodyVerts.Length; i++) {
      bodyVerts[i] = transform.TransformPoint(bodyVerts[i]);
    }

    for (int i = 0; i < solverIterations; i++) {
      //First, ensure that the surface area is what we think it is
      Verlet.resolveDistanceConstraints(constraints, ref bodyVerts, ref accumulatedDisplacements, 1);

      //Next, set the volume of the soft body
      Verlet.setVolume(inflationAmount * initialVolume, bodyVerts, bodyNormals, bodyTriangles, initialSurfaceArea, true, fastButGarbage);
    }

    //Also clamp to the intersection of capsules for shits
    for (int j = 0; j < bodyVerts.Length; j++) {
      Vector3 capOne = Constraints.ConstrainToCapsule(bodyVerts[j], Vector3.zero, Vector3.up, 0.25f) - bodyVerts[j];
      Vector3 capTwo = Constraints.ConstrainToCapsule(bodyVerts[j], Vector3.zero, Vector3.right * 1.15f, 0.25f) - bodyVerts[j];
      if(capOne.sqrMagnitude < capTwo.sqrMagnitude) {
        bodyVerts[j] += capOne;
      } else {
        bodyVerts[j] += capTwo;
      }
    }

    //Calculate the the position and rotation of the body
    for (int i = 0; i < bodyVerts.Length; i++) { kabschVerts[i] = new Vector4(bodyVerts[i].x, bodyVerts[i].y, bodyVerts[i].z, 1f); };
    Matrix4x4 toWorldSpace = kabschSolver.SolveKabsch(originalVerts, Array.ConvertAll(bodyVerts, (p => (Vector4)p)));
    transform.position = toWorldSpace.GetVector3();
    transform.rotation = toWorldSpace.GetQuaternion();

    //Move the points into local space for rendering
    for (int i = 0; i < bodyVerts.Length; i++) {
      bodyVerts[i] = transform.InverseTransformPoint(bodyVerts[i]);
      renderNormals[i] = transform.InverseTransformDirection(bodyNormals[i]);
    }

    Debug.Log(Verlet.VolumeOfMesh(bodyVerts, bodyTriangles));

    //Graphics
    bodyMesh.vertices = bodyVerts;
    bodyMesh.normals = renderNormals;
    bodyMesh.RecalculateBounds();
    bodyMesh.UploadMeshData(false);
  }
}
