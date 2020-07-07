using UnityEngine;

public class ThinPlateSpline : MonoBehaviour {
  public Transform[] controlPoints;

  // Based on the insightful article at
  // https://wordsandbuttons.online/biwhatever_transformations.html

  // Quadratic interpolation by the three values in 0, 0.5, and 1
  public static Vector3 Qerp(Vector3 y1, Vector3 y2, Vector3 y3, float x) {
    Vector3 a =  2.0f * y1 - 4.0f * y2 + 2.0f * y3;
    Vector3 b = -3.0f * y1 + 4.0f * y2 - y3;
    return a * x * x + b * x + y1;
  }

  // Interpolate a point on the thin-plate-spline
  public static Vector3 q_transform(Vector3[] points, float x, float y) {
    Vector3 x1 = Qerp(points[0], points[1], points[2], x);
    Vector3 x2 = Qerp(points[3], points[4], points[5], x);
    Vector3 x3 = Qerp(points[6], points[7], points[8], x);
    return       Qerp(x1       , x2       , x3       , y);
  }

  Vector3[] _internalPoints = new Vector3[9];
  public void OnDrawGizmos() {
    // Return if the transforms aren't set
    if (controlPoints == null || controlPoints.Length != 9) return;

    // Extract the positions from the transforms
    for(int i = 0; i < _internalPoints.Length; i++) {
      _internalPoints[i] = controlPoints[i].position;
    }

    // Draw Grid
    int numSegments = 20;
    for(int xi = 0; xi <= numSegments; xi += 1) {
      for (int yi = 0; yi <= numSegments; yi += 1) {
        float x = (float)xi / numSegments, y = (float)yi / numSegments, inc = 1f / numSegments;
          Vector3 curPos = q_transform(_internalPoints, x,       y      );
        if(xi != numSegments) {
          Vector3 nextX  = q_transform(_internalPoints, x + inc, y       );
          Gizmos.DrawLine(curPos, nextX);
        }
        if(yi != numSegments) {
          Vector3 nextY  = q_transform(_internalPoints, x,       y + inc);
          Gizmos.DrawLine(curPos, nextY);
        }
      }
    }
  }
}
