using UnityEngine;

public class SurfaceDeform : MonoBehaviour {
  public MeshFilter filter;
  public Transform[] controlPoints;
  [Range(0f, 10f)]
  public float weight = 1f;
  public bool useRotation = true;

  Vector3[] controlPointArray;
  SurfaceMeshDeformer deformer;
  float lastWeight = 0f;

  // Use this for initialization
  void Start() {
    if (filter == null) { filter = GetComponent<MeshFilter>(); }
    controlPointArray = new Vector3[controlPoints.Length];
    for (int i = 0; i < controlPoints.Length; i++) {
      controlPointArray[i] = controlPoints[i].position;
    }
    deformer = new SurfaceMeshDeformer(filter, controlPointArray, 1f);
  }

  // Update is called once per frame
  void Update() {
    //Only perform this step if the weight has changed
    if (weight != lastWeight) {
      deformer.updateWeights(weight);
      lastWeight = weight;
    }

    for (int i = 0; i < controlPoints.Length; i++) {
      controlPointArray[i] = controlPoints[i].position;
    }

    deformer.updateMeshDeformation(ref filter, controlPointArray, useRotation, weight);
  }
}
