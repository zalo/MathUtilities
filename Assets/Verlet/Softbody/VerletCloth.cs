using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class VerletCloth : MonoBehaviour {
  public Transform anchor1;
  public Transform anchor2;

  List<Verlet.DistConstraint> constraints;
  Mesh clothMesh;
  Vector3[] clothVerts;
  Vector4[] prevClothVerts;
  Vector3 scaledGravity;
  float previousDeltaTime = 1f;

  void Start() {
    MeshFilter filter = GetComponent<MeshFilter>();
    clothMesh = Instantiate(filter.mesh);
    clothMesh.MarkDynamic();
    clothVerts = clothMesh.vertices;
    prevClothVerts = new Vector4[clothVerts.Length];
    for (int i = 0; i < clothVerts.Length; i++) {
      prevClothVerts[i] = new Vector4(clothVerts[i].x, clothVerts[i].y, clothVerts[i].z, 1f);
    }
    filter.mesh = clothMesh;

    //Create Distance Constraints from Triangles in Mesh
    constraints = new List<Verlet.DistConstraint>(clothVerts.Length * 3);
    Verlet.setUpConstraints(clothMesh, constraints, false);

    //Scale gravity by the size of this Mesh Renderer
    scaledGravity = new Vector3(Physics.gravity.x / transform.lossyScale.x, Physics.gravity.y / transform.lossyScale.y, Physics.gravity.z / transform.lossyScale.z);
  }

  void Update() {
    //Physics
    Verlet.Integrate(clothVerts, prevClothVerts, scaledGravity, Time.deltaTime, previousDeltaTime);
    previousDeltaTime = Time.deltaTime;

    //Anchor the Top Corner of the Cloth
    clothVerts[0] = prevClothVerts[0] = anchor1.position;
    clothVerts[10] = prevClothVerts[10] = anchor2.position;

    //Constraint Resolution
    Verlet.resolveDistanceConstraints(constraints, ref clothVerts, 1);

    //Graphics
    clothMesh.vertices = clothVerts;
    clothMesh.RecalculateNormals();
    clothMesh.RecalculateBounds();
    clothMesh.UploadMeshData(false);
  }
}