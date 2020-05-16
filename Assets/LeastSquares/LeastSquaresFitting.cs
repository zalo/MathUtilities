using UnityEngine;
using System.Collections.Generic;

public class LeastSquaresFitting : MonoBehaviour {
  public Transform parentTransform;

  public enum FitType { Line, LineFast, Plane, OrdinaryLine, OrdinaryParabola, OrdinaryCubic };
  [Tooltip("Performs a plane fit when checked and a line fit when unchecked")]
  public FitType fitType = FitType.Line;

  public Vector3 origin, direction;
  void OnDrawGizmos() {
    List<Vector3> points = new List<Vector3>(parentTransform.childCount);
    foreach(Transform child in parentTransform) {
      if (child.gameObject.activeInHierarchy) {
        points.Add(child.position);
      }
    }

    if (fitType == FitType.Line) {
      Fit.Line(points, out origin, ref direction, 1, true);
    } else if (fitType == FitType.LineFast) {
      Fit.LineFast(points, out origin, ref direction, 1, true);
    } else if (fitType == FitType.Plane) {
      Fit.Plane(points, out origin, out direction, 100, true);
    } else if (fitType == FitType.OrdinaryLine) {
      Fit.Polynomial(points, 1, true);
    } else if (fitType == FitType.OrdinaryParabola) {
      Fit.Polynomial(points, 2, true);
    } else if (fitType == FitType.OrdinaryCubic) {
      Fit.Polynomial(points, 3, true);
    }
  }
}


public static class Fit {
  //These techniques should be extensible to n-dimensions

  public static void Line(List<Vector3> points, out Vector3 origin,
                          ref Vector3 direction, int iters = 100, bool drawGizmos = false) {
    if (
    direction == Vector3.zero ||
    float.IsNaN(direction.x) ||
    float.IsInfinity(direction.x)) direction = Vector3.up;

    //Calculate Average
    origin = Vector3.zero;
    for (int i = 0; i < points.Count; i++) origin += points[i];
    origin /= points.Count;

    // Step the optimal fitting line approximation:
    for (int iter = 0; iter < iters; iter++) {
      Vector3 newDirection = Vector3.zero;
      foreach (Vector3 worldSpacePoint in points) {
        Vector3 point = worldSpacePoint - origin;
        newDirection += Vector3.Dot(direction, point) * point;
      }
      direction = newDirection.normalized;
    }

    if (drawGizmos) {
      Gizmos.color = Color.red;
      Gizmos.DrawRay(origin, direction * 2f);
      Gizmos.DrawRay(origin, -direction * 2f);
    }
  }

  public static void LineFast(List<Vector3> points, out Vector3 origin,
                          ref Vector3 direction, int iters = 10, bool drawGizmos = false) {
    if (
    direction == Vector3.zero ||
    float.IsNaN(direction.x) ||
    float.IsInfinity(direction.x)) direction = Vector3.up;

    //Mean Center the Points
    origin = Vector3.zero;
    for (int i = 0; i < points.Count; i++) origin += points[i];
    origin /= points.Count;
    for (int i = 0; i < points.Count; i++) points[i] -= origin;

    // Calculate the 3x3 Cross Covariance Matrix:
    Vector3[] crossCovariance = new Vector3[3];
    foreach(Vector3 p in points) {
      crossCovariance[0][0] += p[0] * p[0];
      crossCovariance[1][0] += p[1] * p[0];
      crossCovariance[2][0] += p[2] * p[0];
      crossCovariance[0][1] += p[0] * p[1];
      crossCovariance[1][1] += p[1] * p[1];
      crossCovariance[2][1] += p[2] * p[1];
      crossCovariance[0][2] += p[0] * p[2];
      crossCovariance[1][2] += p[1] * p[2];
      crossCovariance[2][2] += p[2] * p[2];
    }

    // Step the optimal fitting line approximation with Power Iteration:
    for (int iter = 0; iter < iters; iter++) {
      Vector3 newDirection = Vector3.zero;
      foreach (Vector3 basis in crossCovariance) newDirection += Vector3.Dot(direction, basis) * basis;
      direction = newDirection.normalized;
    }

    if (drawGizmos) {
      Gizmos.color = Color.green;
      Gizmos.DrawRay(origin,  direction * 2f);
      Gizmos.DrawRay(origin, -direction * 2f);
    }
  }

  public static void Plane(List<Vector3> points, out Vector3 position,
    out Vector3 normal, int iters = 200, bool drawGizmos = false) {

    //Find the primary principal axis
    Vector3 primaryDirection = Vector3.right;
    Line(points, out position, ref primaryDirection, iters / 2, false);

    //Flatten the points along that axis
    List<Vector3> flattenedPoints = new List<Vector3>(points);
    for (int i = 0; i < flattenedPoints.Count; i++)
      flattenedPoints[i] = Vector3.ProjectOnPlane(points[i] - position, primaryDirection) + position;

    //Find the secondary principal axis
    Vector3 secondaryDirection = Vector3.right;
    Line(flattenedPoints, out position, ref secondaryDirection, iters / 2, false);

    normal = Vector3.Cross(primaryDirection, secondaryDirection).normalized;

    if (drawGizmos) {
      Gizmos.color = Color.red;
      foreach (Vector3 point in points) Gizmos.DrawLine(point, Vector3.ProjectOnPlane(point - position, normal) + position);
      Gizmos.color = Color.blue;
      Gizmos.DrawRay(position, normal * 0.5f); Gizmos.DrawRay(position, -normal * 0.5f);
      Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.LookRotation(normal, primaryDirection), new Vector3(1f, 1f, 0.001f));
      Gizmos.DrawWireSphere(Vector3.zero, 1f);
      Gizmos.matrix = Matrix4x4.identity;
    }
  }

  public static float TimeAlongSegment(Vector3 position, Vector3 a, Vector3 b) {
    Vector3 ba = b - a;
    return Vector3.Dot(position - a, ba) / ba.sqrMagnitude;
  }

  public static Vector4 Polynomial(List<Vector3> points, uint orderUpToThree = 3, bool drawGizmos = false) {
    Matrix4x4 xMatrix = Matrix4x4.identity;
    for (int i = 0; i < orderUpToThree + 1; i++) {
      for (int j = 0; j < orderUpToThree + 1; j++) {
        if (xMatrix[j, i] == 1f) xMatrix[j, i] = 0f;
        for (int k = 0; k < points.Count; k++) {
          xMatrix[j, i] += Mathf.Pow(points[k].x, i + j);
        }
      }
    }

    Matrix4x4 yMatrix = Matrix4x4.zero;
    for (int i = 0; i < orderUpToThree + 1; i++) {
      for (int k = 0; k < points.Count; k++) {
        yMatrix[0, i] += points[k].y * Mathf.Pow(points[k].x, i);
      }
    }

    //TODO: Find a way to avoid calculating the inverse, which
    //becomes numerically unstable once any of the values exit
    //the single digits.  Gaussian Elimination or something.
    Vector4 coefficients = (yMatrix * xMatrix.inverse).GetRow(0);

    if (drawGizmos) {
      Gizmos.color = Color.white;
      points.Sort((x, y) => x.x.CompareTo(y.x));
      for (float x = points[0].x - 0.5f; x < points[points.Count - 1].x + 0.5f; x += 0.05f) {
        Gizmos.DrawLine(new Vector3(x,         coefficients.EvaluateCubic(x),         points[0].z),
                        new Vector3(x + 0.05f, coefficients.EvaluateCubic(x + 0.05f), points[0].z));
      }
      Gizmos.color = Color.red;
      foreach (Vector3 point in points) {
        Gizmos.DrawLine(point, new Vector3(point.x, coefficients.EvaluateCubic(point.x), point.z));
      }
    }

    return coefficients;
  }

  static float EvaluateCubic(this Vector4 coefficients, float x) {
    return coefficients[0] + 
          (coefficients[1] * x) + 
          (coefficients[2] * x * x) + 
          (coefficients[3] * x * x * x);
  }

  /// <summary>
  /// An analytic orthogonal regression technique that unfortunately only works in 2D
  /// https://en.wikipedia.org/wiki/Deming_regression#Orthogonal_regression
  /// </summary>
  public static void LineAnalyticBroken(List<Vector3> points, out Vector3 origin,
                    ref Vector3 direction, int iters = 100, bool drawGizmos = false) {
    if (
    direction == Vector3.zero ||
    float.IsNaN(direction.x) ||
    float.IsInfinity(direction.x)) direction = Vector3.up;

    //Calculate Average
    origin = Vector3.zero;
    for (int i = 0; i < points.Count; i++) origin += points[i];
    origin /= points.Count;

    // Attempt to solve for the fitting line analytically
    Quaternion accum = new Quaternion(0, 0, 0, 0);
    foreach (Vector3 worldSpacePoint in points) {
      Vector3 point = worldSpacePoint - origin;
      Quaternion complexPoint = new Quaternion(point.y, point.z, 0, point.x);
      Quaternion squaredComplexPoint = (complexPoint * complexPoint);
      accum = new Quaternion(squaredComplexPoint.x + accum.x,
                             squaredComplexPoint.y + accum.y,
                             0,//squaredComplexPoint.z + accum.z,
                             squaredComplexPoint.w + accum.w);
    }
    accum = accum.normalized;
    //float angle;  Vector3 axis;
    //accum.ToAngleAxis(out angle, out axis);
    //accum = Quaternion.AngleAxis(angle / 2, axis);
    accum = accum.Sqrt(); // Equivalent to halving the angle
    direction = new Vector3(accum.w, accum.x, accum.y).normalized;

    if (drawGizmos) {
      Gizmos.color = Color.green;
      Gizmos.DrawRay(origin, direction * 2f);
      Gizmos.DrawRay(origin, -direction * 2f);
    }
  }
}

// BELOW IS UNUSED EXCEPT IN BROKEN ANALYTIC LINE FUNCTION
public static class QuaternionExponentiation {
  /// <summary>Halves the angle of the quaternion</summary>
  public static Quaternion Sqrt(this Quaternion q) {
    float d = 1.0f + q.w;
    float s = 1f/Mathf.Sqrt(d + d);
    return new Quaternion(q.x * s, q.y * s, q.z * s, d * s);
  }

  ///<summary>
  ///Sets this quaternion to this^n (for a rotation quaternion, 
  ///this is equivalent to rotating this by itself n times).
  ///This should only work for unit quaternions.
  ///</summary>
  public static Quaternion Pow(this Quaternion q, float n) {
    return q.Ln().Scale(n).Exp();
  }

  public static Quaternion Exp(this Quaternion q) {
    float r = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z);
    float et = Mathf.Exp(q.w);
    float s = r >= 0.00001f ? et * Mathf.Sin(r) / r : 0f;
    return new Quaternion(q.x * s, q.y * s, q.z * s, et * Mathf.Cos(r));
  }


  public static Quaternion Ln(this Quaternion q) {
    float r = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z);
    float t = r > 0.00001f ? Mathf.Atan2(r, q.w) / r : 0f;
    return new Quaternion(q.x * t, q.y * t, q.z * t,
                          0.5f * Mathf.Log(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z));
  }

  public static Quaternion Scale(this Quaternion q, float scale) {
    return new Quaternion(q.x * scale, q.y * scale, q.z * scale, q.w * scale);
  }
}