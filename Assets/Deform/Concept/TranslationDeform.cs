using UnityEngine;

public class TranslationDeform : MonoBehaviour {
  public MeshFilter filter;
  public Transform[] controlPoints;
  public float weight = 1f;

  Mesh distortedPlane;
  float[] tempControlPointWeights;
  Vector3[] originalControlPoints;
  Vector3[] originalPlaneVerts;
  Vector3[] distortedPlaneVerts;

  Vector3[,] restingDisplacements;

  // Use this for initialization
  void Start () {
    if (filter == null) { filter = GetComponent<MeshFilter>(); }
    originalPlaneVerts = filter.mesh.vertices;
    distortedPlaneVerts = filter.mesh.vertices;
    distortedPlane = filter.mesh;
    originalControlPoints = new Vector3[controlPoints.Length];
    for (int i = 0; i < controlPoints.Length; i++) {
      originalControlPoints[i] = transform.InverseTransformPoint(controlPoints[i].position);
    }
  }
	
	// Update is called once per frame
	void Update () {
    for (int i = 0; i < originalPlaneVerts.Length; i++) {
      //Calculate the influence of each control point on this vertex
      float totalWeight = 0f;
      float[] tempControlPointWeights = new float[controlPoints.Length];
      for (int j = 0; j < controlPoints.Length; j++) {
        tempControlPointWeights[j] = 1f / Mathf.Pow(Vector3.Distance(originalPlaneVerts[i], originalControlPoints[j]), weight);
        totalWeight += tempControlPointWeights[j];
      }
      for (int j = 0; j < controlPoints.Length; j++) {
        tempControlPointWeights[j] /= totalWeight;
      }

      //Apply the weighted offsets to each vertex
      Vector3 displacement = Vector3.zero;
      for (int j = 0; j < controlPoints.Length; j++) {
        Vector3 currentControlPoint = transform.InverseTransformPoint(controlPoints[j].position);
        Vector3 translationalDisplacement = (currentControlPoint - originalControlPoints[j]);

        Vector3 vertToControl = (originalPlaneVerts[i] - originalControlPoints[j]);
        Vector3 rotationalDisplacement = (controlPoints[j].localRotation * vertToControl) - vertToControl;

        displacement += (translationalDisplacement + rotationalDisplacement) * tempControlPointWeights[j];
      }
      distortedPlaneVerts[i] = originalPlaneVerts[i] + displacement;
    }

    distortedPlane.vertices = distortedPlaneVerts;
    distortedPlane.UploadMeshData(false);
    filter.mesh = distortedPlane;
  }
}
