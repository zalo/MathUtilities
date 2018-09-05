using UnityEngine;
using System.Collections.Generic;

public class LeastSquaresFitting : MonoBehaviour {
  public Transform parentTransform;

  [Tooltip("Performs a plane fit when checked and a line fit when unchecked")]
  public bool doPlaneFit = false;

  Vector3 position, direction;
  void OnDrawGizmos() {
    List<Vector3> points = new List<Vector3>(parentTransform.childCount);
    for (int i = 0; i < parentTransform.childCount; i++) points.Add(parentTransform.GetChild(i).position);

    if (doPlaneFit) {
      Fit.Plane(points, out position, out direction, 100, true);
    } else {
      Fit.Line(points, out position, ref direction, 100, true);
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
}