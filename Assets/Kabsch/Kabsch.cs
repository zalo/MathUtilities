using UnityEngine;
public class Kabsch : MonoBehaviour {
  public Transform[] inPoints;
  public Transform[] referencePoints;
  Vector3[] points, refPoints;
  KabschSolver solver = new KabschSolver();

  //Set up the Input Points
  void Start() {
    points = new Vector3[inPoints.Length];
    refPoints = new Vector3[inPoints.Length];
    for (int i = 0; i < inPoints.Length; i++) {
      points[i] = inPoints[i].position;
    }
  }

  //Calculate the Kabsch Transform and Apply it to the input points
  void Update() {
    for (int i = 0; i < inPoints.Length; i++) {
      refPoints[i] = referencePoints[i].position;
    }

    Matrix4x4 kabschTransform = solver.SolveKabsch(points, refPoints);

    for (int i = 0; i < inPoints.Length; i++) {
      inPoints[i].position = kabschTransform.MultiplyPoint3x4(points[i]);
    }
  }
}

//Kabsch Implementation-----------------------------------------------------------
public class KabschSolver {
  Vector3[] QuatBasis = new Vector3[3];
  Vector3[] DataCovariance = new Vector3[3];
  Quaternion OptimalRotation = Quaternion.identity;
  public Matrix4x4 SolveKabsch(Vector3[] inPoints, Vector3[] refPoints, bool solveRotation = true) {
    if (inPoints.Length != refPoints.Length) { return Matrix4x4.identity; }

    //Calculate the centroid offset and construct the centroid-shifted point matrices
    Vector3 inCentroid = Vector3.zero; Vector3 refCentroid = Vector3.zero;
    for (int i = 0; i < inPoints.Length; i++) {
      inCentroid += inPoints[i];
      refCentroid += refPoints[i];
    }
    inCentroid /= inPoints.Length;
    refCentroid /= refPoints.Length;

    //Calculate the 3x3 covariance matrix, and the optimal rotation
    if (solveRotation) {
      extractRotation(TransposeMultSubtract(inPoints, refPoints, inCentroid, refCentroid, DataCovariance), ref OptimalRotation);
    }

    return Matrix4x4.TRS(refCentroid,  Quaternion.identity, Vector3.one) *
           Matrix4x4.TRS(Vector3.zero, OptimalRotation,     Vector3.one) *
           Matrix4x4.TRS(-inCentroid,  Quaternion.identity, Vector3.one);
  }

  //https://animation.rwth-aachen.de/media/papers/2016-MIG-StableRotation.pdf
  //Iteratively apply torque to the basis using Cross products (in place of SVD)
  void extractRotation(Vector3[] A, ref Quaternion q) {
    for (int iter = 0; iter < 9; iter++) {
      q.FillMatrixFromQuaternion(ref QuatBasis);
      Vector3 omega = (Vector3.Cross(QuatBasis[0], A[0]) +
                       Vector3.Cross(QuatBasis[1], A[1]) +
                       Vector3.Cross(QuatBasis[2], A[2])) *
       (1f / Mathf.Abs(Vector3.Dot(QuatBasis[0], A[0]) +
                       Vector3.Dot(QuatBasis[1], A[1]) +
                       Vector3.Dot(QuatBasis[2], A[2]) + 0.000000001f));

      float w = Mathf.Clamp(omega.magnitude * Mathf.Rad2Deg, -20f, 20f); //How aggressive each iteration should be
      if (w < 0.000000001f)
        break;
      q = Quaternion.AngleAxis(w, (1f / w) * omega) * q;
      q = Quaternion.Lerp(q, q, 0f); //Normalizes the Quaternion; critical for error suppression
    }
  }

  //Calculate Covariance Matrices --------------------------------------------------
  public static Vector3[] TransposeMultSubtract(Vector3[] vec1, Vector3[] vec2, Vector3 vec1Centroid, Vector3 vec2Centroid, Vector3[] covariance) {
    for (int i = 0; i < 3; i++) { //i is the row in this matrix
      covariance[i] = Vector3.zero;
      for (int j = 0; j < 3; j++) {//j is the column in the other matrix
        for (int k = 0; k < vec1.Length; k++) {//k is the column in this matrix
          covariance[i][j] += (vec1[k][i] - vec1Centroid[i]) * (vec2[k][j] - vec2Centroid[j]);
        }
      }
    }
    return covariance;
  }
}

public static class FromMatrixExtension {
  public static Vector3 GetVector3(this Matrix4x4 m) { return m.GetColumn(3); }
  public static Quaternion GetQuaternion(this Matrix4x4 m) {
    if (m.GetColumn(2) == m.GetColumn(1)) { return Quaternion.identity; }
    return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
  }
  public static void FillMatrixFromQuaternion(this Quaternion q, ref Vector3[] covariance) {
    covariance[0] = q * Vector3.right;
    covariance[1] = q * Vector3.up;
    covariance[2] = q * Vector3.forward;
  }
}
