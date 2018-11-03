using UnityEngine;
using System.Collections.Generic;

public class LeastSquaresFitting : MonoBehaviour {
  public Transform parentTransform;

  public enum FitType { Line, Plane, OrdinaryLine, OrdinaryParabola, OrdinaryCubic };
  [Tooltip("Performs a plane fit when checked and a line fit when unchecked")]
  public FitType fitType = FitType.Line;

  public Vector3 position, direction;
  void OnDrawGizmos() {
    List<Vector3> points = new List<Vector3>(parentTransform.childCount);
    foreach(Transform child in parentTransform) {
      if (child.gameObject.activeInHierarchy) {
        points.Add(child.position);
      }
    }

    if (fitType == FitType.Line) {
      Fit.Line(points, out position, ref direction, 100, true);
    } else if (fitType == FitType.Plane) {
      Fit.Plane(points, out position, out direction, 100, true);
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

  public static void Line(List<Vector3> points, out Vector3 position,
  ref Vector3 direction, int iters = 100, bool drawGizmos = false) {
    if (
    direction == Vector3.zero ||
    float.IsNaN(direction.x) ||
    float.IsInfinity(direction.x)) direction = Vector3.right;

    //Calculate Average
    position = Vector3.zero;
    for (int i = 0; i < points.Count; i++) position += points[i];
    position /= points.Count;

    //Step the optimal fitting line approximation
    for (int iter = 0; iter < iters; iter++) {
      Vector3 accumulatedOffset = Vector3.zero; float sum = 0f;
      for (int i = 0; i < points.Count; i++) {
        float alpha = TimeAlongSegment(points[i], position, position + direction);
        Vector3 lineToPointOffset = points[i] - Vector3.LerpUnclamped(position, position + direction, alpha);
        accumulatedOffset += lineToPointOffset * alpha;
        sum += alpha * alpha;

        if (drawGizmos) {
          Gizmos.color = Color.red;
          Gizmos.DrawRay(points[i], -lineToPointOffset);
        }
      }
      direction += accumulatedOffset / sum;
      direction = direction.normalized;
    }
    if (drawGizmos) {
      Gizmos.color = Color.white;
      Gizmos.DrawRay(position, direction * 2f);
      Gizmos.DrawRay(position, -direction * 2f);
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
}