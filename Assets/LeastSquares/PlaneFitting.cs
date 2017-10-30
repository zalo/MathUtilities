using UnityEngine;
public class PlaneFitting : MonoBehaviour {
  public Transform[] inPoints;
  Vector3[] points;

  //Set up the Input Points
  void Start() {
    points = new Vector3[inPoints.Length];
    for (int i = 0; i < inPoints.Length; i++) {
      points[i] = inPoints[i].position;
    }
  }

  //Calculate the Plane Fit for these Transforms
  void Update() {
    for (int i = 0; i < inPoints.Length; i++) {
      points[i] = inPoints[i].position;
    }

    PlaneFittingSolver.SolveApproximatePlaneFit(points);
  }
}

//Least Squares Implementation-----------------------------------------------------------
public static class PlaneFittingSolver {
  //Unity port of Emil Ernerfeldt's Approximate Least Squares Plane Fitting Code
  //http://www.ilikebigbits.com/blog/2017/9/24/fitting-a-plane-to-noisy-points-in-3d

  public static Plane SolveApproximatePlaneFit(Vector3[] inPoints) {
    //Calculate the centroid offset and construct the centroid-shifted point matrices
    Vector3 inCentroid = Vector3.zero;
    for (int i = 0; i < inPoints.Length; i++) {
      inCentroid += inPoints[i];
    }
    inCentroid /= inPoints.Length;

    // Calculate full 3x3 covariance matrix, excluding symmetries:
    float xx = 0.0f; float xy = 0.0f; float xz = 0.0f;
    float yy = 0.0f; float yz = 0.0f; float zz = 0.0f;

    foreach (Vector3 p in inPoints) {
      Vector3 r = p - inCentroid;
      xx += r.x * r.x;
      xy += r.x * r.y;
      xz += r.x * r.z;
      yy += r.y * r.y;
      yz += r.y * r.z;
      zz += r.z * r.z;
    }

    xx /= inPoints.Length;
    xy /= inPoints.Length;
    xz /= inPoints.Length;
    yy /= inPoints.Length;
    yz /= inPoints.Length;
    zz /= inPoints.Length;

    Vector3 weighted_dir = new Vector3();

    {
      float det_x = yy * zz - yz * yz;
      Vector3 axis_dir = new Vector3(
            det_x,
            xz * yz - xy * zz,
            xy * yz - xz * yy
        );
      float weight = det_x * det_x;
      if (Vector3.Dot(weighted_dir, axis_dir) < 0.0) { weight = -weight; }
      weighted_dir += axis_dir * weight;
    }

    {
      float det_y = xx * zz - xz * xz;
      Vector3 axis_dir = new Vector3(
            xz * yz - xy * zz,
            det_y,
            xy * xz - yz * xx
        );
      float weight = det_y * det_y;
      if (Vector3.Dot(weighted_dir, axis_dir) < 0.0) { weight = -weight; }
      weighted_dir += axis_dir * weight;
    }

    {
      float det_z = xx * yy - xy * xy;
      Vector3 axis_dir = new Vector3(
                xy * yz - xz * yy,
                xy * xz - yz * xx,
                det_z
            );
      float weight = det_z * det_z;
      if (Vector3.Dot(weighted_dir, axis_dir) < 0.0) { weight = -weight; }
      weighted_dir += axis_dir * weight;
    }

    if (weighted_dir.sqrMagnitude != 0f) {
      Plane toReturn = new Plane(weighted_dir.normalized, inCentroid);
      Debug.DrawRay(inCentroid, toReturn.normal*-0.5f);
      Debug.DrawRay(inCentroid, toReturn.normal*0.5f);
      return toReturn;
    } else {
      return new Plane();
    }
  }
}
