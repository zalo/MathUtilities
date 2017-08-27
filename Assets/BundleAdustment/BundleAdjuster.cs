using UnityEngine;

public class BundleAdjuster : MonoBehaviour {
  public Transform[] features;

  public Transform[] cameras;
  Vector3[][] rayDirections;

  KabschSolver solver = new KabschSolver();
  Vector3[] inPoints, refPoints;
  void Start () {
    inPoints = new Vector3[features.Length];
    refPoints = new Vector3[features.Length];
    rayDirections = new Vector3[2][];

    for (int cameraIndex = 0; cameraIndex < 2; cameraIndex++) {
      rayDirections[cameraIndex] = new Vector3[features.Length];
      for (int i = 0; i < features.Length; i++) {
        rayDirections[cameraIndex][i] = cameras[cameraIndex].InverseTransformPoint(features[i].position).normalized;
      }
    }
  }

  void Update() {
    for (int iteration = 0; iteration < 1; iteration++) {
      for (int i = 0; i < features.Length; i++) {
        Debug.DrawLine(cameras[0].position, cameras[0].TransformPoint(rayDirections[0][i]));
        Debug.DrawLine(cameras[1].position, cameras[1].TransformPoint(rayDirections[1][i]));

        float timeLineOne, timeLineTwo;
        line2lineDisplacement(cameras[0].position, cameras[0].TransformPoint(rayDirections[0][i]), cameras[1].position, cameras[1].TransformPoint(rayDirections[1][i]), out timeLineOne, out timeLineTwo);

        //Take the abs of the times so they can't intersect behind the camera
        timeLineOne = Mathf.Abs(timeLineOne); timeLineTwo = Mathf.Abs(timeLineTwo);

        inPoints[i] = Vector3.LerpUnclamped(cameras[0].position, cameras[0].TransformPoint(rayDirections[0][i]), timeLineOne);
        refPoints[i] = Vector3.LerpUnclamped(cameras[1].position, cameras[1].TransformPoint(rayDirections[1][i]), timeLineTwo);
        Debug.DrawLine(inPoints[i], refPoints[i], Color.red);
      }

      Matrix4x4 iterationStep = solver.SolveKabsch(refPoints, inPoints, true);
      cameras[1].position += iterationStep.GetVector3();
      cameras[1].rotation = iterationStep.GetQuaternion() * cameras[1].rotation;
    }
  }

  //LINE <-> LINE ANALYSIS
  //-------------------------------------------------------------------
  public static Vector3 line2lineDisplacement(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2, out float timeToa2, out float timeTob2) {
    Vector3 u = a2 - a1; //from a1 to a2
    Vector3 v = b2 - b1; //from b1 to b2
    Vector3 w = a1 - b1;
    float a = Vector3.Dot(u, u);         // always >= 0
    float b = Vector3.Dot(u, v);
    float c = Vector3.Dot(v, v);         // always >= 0
    float d = Vector3.Dot(u, w);
    float e = Vector3.Dot(v, w);
    float D = a * c - b * b;        // always >= 0
    float sc, sN, sD = D;       // sc = sN / sD, default sD = D >= 0
    float tc, tN, tD = D;       // tc = tN / tD, default tD = D >= 0

    // compute the line parameters of the two closest points
    if (D < Mathf.Epsilon) { // the lines are almost parallel
      sN = 0.0f;         // force using point P0 on segment S1
      sD = 1.0f;         // to prevent possible division by 0.0 later
      tN = e;
      tD = c;
    } else {                 // get the closest points on the infinite lines
      sN = (b * e - c * d);
      tN = (a * e - b * d);
    }
    // finally do the division to get sc and tc
    sc = (Mathf.Abs(sN) < Mathf.Epsilon ? 0.0f : sN / sD);
    tc = (Mathf.Abs(tN) < Mathf.Epsilon ? 0.0f : tN / tD);

    // get the difference of the two closest points
    Vector3 dP = w + (sc * u) - (tc * v);  // =  S1(sc) - S2(tc)
    timeToa2 = sc; timeTob2 = tc;
    return dP;   // return the closest distance
  }
}
