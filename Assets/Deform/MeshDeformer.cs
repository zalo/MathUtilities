using UnityEngine;
public class MeshDeformer {
  Vector3[] originalMeshVertices;
  Vector3[] distortedMeshVertices;

  Vector3[] originalControlPoints;
  Vector3[] currentControlPoints;
  Vector3[,] restingIntraControlPointDisplacements;
  Quaternion[] controlPointRotations;
  float[,] vertexControlWeights;

  public MeshDeformer(MeshFilter inputMeshFilter, Vector3[] controlPoints, float weight = 1f) {
    //Record the undeformed control point positions
    originalControlPoints = new Vector3[controlPoints.Length];
    currentControlPoints = new Vector3[controlPoints.Length];
    for (int i = 0; i < controlPoints.Length; i++) {
      originalControlPoints[i] = inputMeshFilter.transform.InverseTransformPoint(controlPoints[i]);
      currentControlPoints[i] = originalControlPoints[i];
    }
    controlPointRotations = new Quaternion[originalControlPoints.Length];
    for (int i = 0; i < controlPointRotations.Length; i++) {
      controlPointRotations[i] = Quaternion.identity;
    }
    initializeMeshDeformation(inputMeshFilter.mesh, originalControlPoints, ref originalMeshVertices,
                              ref restingIntraControlPointDisplacements, ref vertexControlWeights, weight);
  }

  //Updates the Deformation of an Initialized Mesh
  public void updateMeshDeformation(ref MeshFilter filter, Vector3[] controlPoints, bool useRotation = true, float weight = 1f, int iters = 10) {
    for (int i = 0; i < controlPoints.Length; i++) {
      currentControlPoints[i] = filter.transform.InverseTransformPoint(controlPoints[i]);
    }
    Mesh toDistort = filter.mesh;
    updateMeshDeformation(originalControlPoints, ref originalMeshVertices,
                          ref currentControlPoints, ref controlPointRotations, ref restingIntraControlPointDisplacements,
                          ref vertexControlWeights, ref distortedMeshVertices, useRotation, weight, iters);
    toDistort.vertices = distortedMeshVertices;
    toDistort.UploadMeshData(false);
    filter.mesh = toDistort;
  }

  //Updates the weighting of the control points on each vertex
  public void updateWeights(float weight) {
    calculateVertexWeights(originalMeshVertices, originalControlPoints, weight, ref vertexControlWeights);
  }

  //STATIC UTILITY METHODS BELOW

  //Precalculates the vertex weighting and control point relationships
  static void initializeMeshDeformation(Mesh inputMesh, Vector3[] originalControlPoints, ref Vector3[] originalMeshVertices,
                                        ref Vector3[,] restingIntraControlPointDisplacements, ref float[,] vertexControlWeights, float weight = 1f) {
    originalMeshVertices = inputMesh.vertices;
    //Record the relationships of the control points to each other
    restingIntraControlPointDisplacements = new Vector3[originalControlPoints.Length, originalControlPoints.Length];
    for (int i = 0; i < originalControlPoints.Length; i++) {
      for (int j = 0; j < originalControlPoints.Length; j++) {
        restingIntraControlPointDisplacements[i, j] = originalControlPoints[j] - originalControlPoints[i];
      }
    }
    calculateVertexWeights(originalMeshVertices, originalControlPoints, weight, ref vertexControlWeights);
  }

  //Updates the Deformation of a Mesh
  static void updateMeshDeformation(Vector3[] originalControlPoints, ref Vector3[] originalMeshVertices, ref Vector3[] currentControlPoints,
                                    ref Quaternion[] controlPointRotations, ref Vector3[,] restingIntraControlPointDisplacements,
                                    ref float[,] vertexControlWeights, ref Vector3[] distortedMeshVertices, bool useRotation = true, float weight = 1f, int iters = 50) {
    calculateControlPointRotations(currentControlPoints, originalControlPoints,
                                   restingIntraControlPointDisplacements, ref controlPointRotations, useRotation, iters);

    calculateVertexDisplacement(originalMeshVertices, originalControlPoints, currentControlPoints,
                                controlPointRotations, vertexControlWeights, ref distortedMeshVertices);
  }

  //Calculate the influence of each control point on each vertex
  static void calculateVertexWeights(Vector3[] originalPlaneVerts, Vector3[] originalControlPoints, float weightFalloff, ref float[,] vertexControlWeights) {
    if (vertexControlWeights == null || vertexControlWeights.GetLength(0) != originalPlaneVerts.Length ||
                                        vertexControlWeights.GetLength(1) != originalControlPoints.Length) {
      vertexControlWeights = new float[originalPlaneVerts.Length, originalControlPoints.Length];
    }
    for (int i = 0; i < originalPlaneVerts.Length; i++) {
      float totalWeight = 0f;
      float[] tempControlPointWeights = new float[originalControlPoints.Length];
      for (int j = 0; j < originalControlPoints.Length; j++) {
        //This distance metric can be replaced by distance along the edges of the surface of the mesh for better results
        tempControlPointWeights[j] = 1f / Mathf.Pow(Vector3.Distance(originalPlaneVerts[i], originalControlPoints[j]) + 0.001f, weightFalloff);
        totalWeight += tempControlPointWeights[j];
      }

      //Normalize the weights
      for (int j = 0; j < originalControlPoints.Length; j++) {
        tempControlPointWeights[j] /= totalWeight;
        vertexControlWeights[i, j] = tempControlPointWeights[j];
      }
    }
  }

  //Calculate the rotation of each control point (this is a polar decomposition)...
  static void calculateControlPointRotations(Vector3[] currentControlPoints, Vector3[] originalControlPoints, Vector3[,] restingIntraControlPointDisplacements,
                                             ref Quaternion[] controlPointRotations, bool useRotation = true, int iters = 50) {
    if (controlPointRotations == null || controlPointRotations.Length != originalControlPoints.Length) { controlPointRotations = new Quaternion[originalControlPoints.Length]; }
    for (int j = 0; j < currentControlPoints.Length; j++) {
      if (useRotation) {
        for (int iteration = 0; iteration < iters; iteration++) {
          Vector3 omegaNumerator = Vector3.zero; float omegaDenominator = 0f;
          for (int k = 0; k < currentControlPoints.Length; k++) {
            if (j != k) {
              Vector3 rotatedDirection = (controlPointRotations[j] * restingIntraControlPointDisplacements[j, k]).normalized;
              Vector3 currentDirection = (currentControlPoints[k] - currentControlPoints[j]).normalized;
              omegaNumerator += Vector3.Cross(rotatedDirection, currentDirection);
              omegaDenominator += Vector3.Dot(rotatedDirection, currentDirection);
            }
          }
          Vector3 omega = omegaNumerator / Mathf.Abs(omegaDenominator + 0.000000001f);
          float w = omega.magnitude;
          if (w < 0.000000001f)
            break;
          controlPointRotations[j] = Quaternion.AngleAxis(w * Mathf.Rad2Deg, omega / w) * controlPointRotations[j];
          //controlPointRotations[j] = Quaternion.Lerp(controlPointRotations[j], controlPointRotations[j], 1f); //Normalizes the Quaternion
        }
      } else {
        controlPointRotations[j] = Quaternion.identity;
      }
    }
  }

  //Apply the weighted offsets to each vertex
  static void calculateVertexDisplacement(Vector3[] originalPlaneVerts, Vector3[] originalControlPoints, Vector3[] currentControlPoints,
                                          Quaternion[] controlPointRotations, float[,] controlPointWeights, ref Vector3[] distortedPlaneVerts) {
    if (distortedPlaneVerts == null || distortedPlaneVerts.Length != originalPlaneVerts.Length) { distortedPlaneVerts = new Vector3[originalPlaneVerts.Length]; }
    for (int i = 0; i < originalPlaneVerts.Length; i++) {
      Vector3 vertexDisplacement = Vector3.zero;
      for (int j = 0; j < currentControlPoints.Length; j++) {
        Vector3 translationalDisplacement = (currentControlPoints[j] - originalControlPoints[j]);

        Vector3 vertToControlOffset = (originalPlaneVerts[i] - originalControlPoints[j]);
        Vector3 rotationalDisplacement = (controlPointRotations[j] * vertToControlOffset) - vertToControlOffset;

        vertexDisplacement += (translationalDisplacement + rotationalDisplacement) * controlPointWeights[i, j];
      }

      distortedPlaneVerts[i] = originalPlaneVerts[i] + vertexDisplacement;
    }
  }
}
