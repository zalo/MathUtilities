using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

[RequireComponent(typeof(MeshFilter))]
public class VerletCloth : MonoBehaviour {
  public Transform anchor1;
  public Transform anchor2;

  List<Verlet.DistConstraint> constraintsList;
  NativeArray<Verlet.DistConstraint> constraintsArray;
  Mesh clothMesh;
  protected Vector3[] clothVertsArray;
  NativeArray<Vector3> clothVerts;
  NativeArray<Vector3> prevClothVerts;
  NativeArray<Vector4> accumulatedDisplacements;
  Vector3 scaledGravity;
  float previousDeltaTime = 1f;

  void Start() {
    MeshFilter filter = GetComponent<MeshFilter>();
    clothMesh = Instantiate(filter.mesh);
    clothMesh.MarkDynamic();
    clothVerts = new NativeArray<Vector3>(clothMesh.vertices, Allocator.Persistent);
    prevClothVerts = new NativeArray<Vector3>(clothMesh.vertices, Allocator.Persistent);
    accumulatedDisplacements = new NativeArray<Vector4>(new Vector4[clothVerts.Length], Allocator.Persistent);
    filter.mesh = clothMesh;

    //Create Distance Constraints from Triangles in Mesh
    constraintsList = new List<Verlet.DistConstraint>(clothVerts.Length * 3);
    Verlet.setUpConstraints(clothMesh, constraintsList, false);
    constraintsArray = new NativeArray<Verlet.DistConstraint>(constraintsList.ToArray(), Allocator.Persistent);

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
    Verlet.resolveDistanceConstraints(constraintsArray, ref clothVerts, ref accumulatedDisplacements, 1);

    //Graphics
    clothVerts.CopyTo(clothVertsArray);
    clothMesh.vertices = clothVertsArray;
    clothMesh.RecalculateNormals();
    clothMesh.RecalculateBounds();
    clothMesh.UploadMeshData(false);
  }

  void OnDestroy() {
    constraintsArray.Dispose();
    clothVerts.Dispose();
    prevClothVerts.Dispose();
    accumulatedDisplacements.Dispose();
  }
}