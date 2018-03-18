using UnityEngine;

public class Deform : MonoBehaviour {
  public MeshFilter filter;
  public Transform[] controlPoints;
  public float weight = 1f;
  public bool useRotation = true;

  Mesh distortedPlane;
  float[] tempControlPointWeights;
  Vector3[] originalControlPoints;
  Vector3[] originalPlaneVerts;
  Vector3[] distortedPlaneVerts;

  Vector3[,] restingIntraControlPointDisplacements;
  float[,] controlPointWeights;
  Quaternion[] controlPointRotations;

  // Use this for initialization
  void Start() {
    if (filter == null) { filter = GetComponent<MeshFilter>(); }
    originalPlaneVerts = filter.mesh.vertices;
    distortedPlaneVerts = filter.mesh.vertices;
    distortedPlane = filter.mesh;

    //Record the undeformed control point positions
    originalControlPoints = new Vector3[controlPoints.Length];
    for (int i = 0; i < controlPoints.Length; i++) {
      originalControlPoints[i] = transform.InverseTransformPoint(controlPoints[i].position);
    }

    //Record the relationships of the control points to each other
    restingIntraControlPointDisplacements = new Vector3[controlPoints.Length, controlPoints.Length];
    for (int i = 0; i < controlPoints.Length; i++) {
      for (int j = 0; j < controlPoints.Length; j++) {
        restingIntraControlPointDisplacements[i, j] = originalControlPoints[j] - originalControlPoints[i];
      }
    }

    //Calculate the influence of each control point on each vertex
    controlPointWeights = new float[originalPlaneVerts.Length, controlPoints.Length];
    for (int i = 0; i < originalPlaneVerts.Length; i++) {
      float totalWeight = 0f;
      float[] tempControlPointWeights = new float[controlPoints.Length];
      for (int j = 0; j < controlPoints.Length; j++) {
        tempControlPointWeights[j] = 1f / Mathf.Pow(Vector3.Distance(originalPlaneVerts[i], originalControlPoints[j]), weight);
        totalWeight += tempControlPointWeights[j];
      }

      //Normalize the weights
      for (int j = 0; j < controlPoints.Length; j++) {
        tempControlPointWeights[j] /= totalWeight;
        controlPointWeights[i, j] = tempControlPointWeights[j];
      }
    }

    controlPointRotations = new Quaternion[controlPoints.Length];
  }

  // Update is called once per frame
  void Update() {
    Vector3[] currentControlPoints = new Vector3[controlPoints.Length];
    for (int j = 0; j < controlPoints.Length; j++) {
      currentControlPoints[j] = transform.InverseTransformPoint(controlPoints[j].position);
    }

    //Calculate the rotation of each control point...
    for (int j = 0; j < controlPoints.Length; j++) {
      Vector3 averageRotation = Vector3.zero; int activePoints = 0;
      if (useRotation) {
        for (int k = 0; k < controlPoints.Length; k++) {
          if (j != k && controlPoints[k].gameObject.activeSelf) {
            Quaternion rot = Quaternion.FromToRotation(restingIntraControlPointDisplacements[j, k],
                                                       currentControlPoints[k] - currentControlPoints[j]);
            Vector3 axis; float angle;
            rot.ToAngleAxis(out angle, out axis);
            averageRotation += axis.normalized * angle;
            activePoints++;
          }
        }
        averageRotation /= activePoints;
      }
      controlPointRotations[j] = Quaternion.AngleAxis(averageRotation.magnitude, averageRotation.normalized);
    }

    //Apply the weighted offsets to each vertex
    for (int i = 0; i < originalPlaneVerts.Length; i++) {
      Vector3 vertexDisplacement = Vector3.zero;
      for (int j = 0; j < controlPoints.Length; j++) {
        Vector3 translationalDisplacement = (currentControlPoints[j] - originalControlPoints[j]);

        Vector3 vertToControlOffset = (originalPlaneVerts[i] - originalControlPoints[j]);
        Vector3 rotationalDisplacement = (controlPointRotations[j] * vertToControlOffset) - vertToControlOffset;

        vertexDisplacement += (translationalDisplacement + rotationalDisplacement) * controlPointWeights[i, j];
      }

      distortedPlaneVerts[i] = originalPlaneVerts[i] + vertexDisplacement;
    }

    distortedPlane.vertices = distortedPlaneVerts;
    distortedPlane.UploadMeshData(false);
    filter.mesh = distortedPlane;
  }
}
