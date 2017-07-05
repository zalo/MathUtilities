using System.Collections.Generic;
using UnityEngine;

public class AverageQuaternion : MonoBehaviour {
  QuaternionAverager averageQuat = new QuaternionAverager();

  //Calculate the Average Quaternion and draw a coordinate basis
  void Update() {
    //Average this transform's rotation with the identity rotation
    //(Not terribly impressive, but it serves as a basic unit test)
    averageQuat.Add(Quaternion.identity);
    averageQuat.Add(transform.rotation);

    Quaternion average = averageQuat.CalculateAverage();

    Debug.DrawLine(Vector3.zero, average * Vector3.right, Color.red);
    Debug.DrawLine(Vector3.zero, average * Vector3.up, Color.green);
    Debug.DrawLine(Vector3.zero, average * Vector3.forward, Color.blue);

    averageQuat.Clear();
  }
}

//Calculate Non-Weighted Averages of quaternions via a hack using the kabsch solver 
public class QuaternionAverager {
  List<Vector3> inBases = new List<Vector3>();
  List<Vector3> refBases = new List<Vector3>();

  Vector3[] QuatBasis = new Vector3[3];
  Vector3[] DataCovariance = new Vector3[3];
  Quaternion OptimalRotation = Quaternion.identity;

  public void Add(Quaternion quat) {
    inBases.Add(quat * Vector3.right);
    inBases.Add(quat * Vector3.up);
    refBases.Add(Vector3.right);
    refBases.Add(Vector3.up);
  }

  public Quaternion CalculateAverage() {
    //Calculate the 3x3 covariance matrix, and the optimal rotation
    extractRotation(TransposeMultSubtract(refBases, inBases, DataCovariance), ref OptimalRotation);
    return OptimalRotation;
  }

  public void Clear() {
    inBases.Clear();
    refBases.Clear();
  }

  //https://animation.rwth-aachen.de/media/papers/2016-MIG-StableRotation.pdf
  //Iteratively apply torque to the basis using Cross products (in place of SVD)
  void extractRotation(Vector3[] A, ref Quaternion q) {
    for (int iter = 0; iter < 9; iter++) {
      q.FillMatrixFromQuaternion(ref QuatBasis);
      Vector3 omega = (Vector3.Cross(QuatBasis[0], A[0]) +
                       Vector3.Cross(QuatBasis[1], A[1]) +
                       Vector3.Cross(QuatBasis[2], A[2])) *
       (1f / Mathf.Abs(Vector3.Dot(  QuatBasis[0], A[0]) +
                       Vector3.Dot(  QuatBasis[1], A[1]) +
                       Vector3.Dot(  QuatBasis[2], A[2]) + 0.000000001f));

      float w = Mathf.Clamp(omega.magnitude * Mathf.Rad2Deg, -20f, 20f); //How aggressive each iteration should be
      if (w < 0.000000001f)
        break;
      q = Quaternion.AngleAxis(w, (1f / w) * omega) * q;
      q = Quaternion.Lerp(q, q, 0f); //Normalizes the Quaternion; critical for error suppression
    }
  }

  //Calculate Cross-Covariance Matrix --------------------------------------------------
  public static Vector3[] TransposeMultSubtract(List<Vector3> vec1, List<Vector3> vec2, Vector3[] covariance) {
    for (int i = 0; i < 3; i++) { //i is the row in this matrix
      covariance[i] = Vector3.zero;
      for (int j = 0; j < 3; j++) {//j is the column in the other matrix
        for (int k = 0; k < vec1.Count; k++) {//k is the column in this matrix
          covariance[i][j] += (vec1[k][i]) * (vec2[k][j]);
        }
      }
    }
    return covariance;
  }
}