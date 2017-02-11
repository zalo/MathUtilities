using UnityEngine;

public class BundleAdjuster : MonoBehaviour {
  public Transform[] features;

  public Transform cameraOne;
  Vector3[] rayDirectionsOne;

  public Transform cameraTwo;
  Vector3[] rayDirectionsTwo;

  Vector3 currentDeltaPos;
  void Start () {
    rayDirectionsOne = new Vector3[features.Length];
    rayDirectionsTwo = new Vector3[features.Length];

    for (int i = 0; i < features.Length; i++) {
      rayDirectionsOne[i] = cameraOne.InverseTransformPoint(features[i].position).normalized;
      rayDirectionsTwo[i] = cameraTwo.InverseTransformPoint(features[i].position).normalized;
    }
	}
	
	void Update () {
    currentDeltaPos = Vector3.zero;
    for (int i = 0; i < features.Length; i++) {
      Debug.DrawLine(cameraOne.position, cameraOne.TransformPoint(rayDirectionsOne[i]));
      Debug.DrawLine(cameraTwo.position, cameraTwo.TransformPoint(rayDirectionsTwo[i]));

      float timeLineOne, timeLineTwo;
      line2lineDisplacement(cameraOne.position, cameraOne.TransformPoint(rayDirectionsOne[i]), cameraTwo.position, cameraTwo.TransformPoint(rayDirectionsTwo[i]), out timeLineOne, out timeLineTwo);

      //Take the abs of the times so they can't intersect behind the camera
      timeLineOne = Mathf.Abs(timeLineOne); timeLineTwo = Mathf.Abs(timeLineTwo);

      Vector3 pointLineOne = Vector3.LerpUnclamped(cameraOne.position, cameraOne.TransformPoint(rayDirectionsOne[i]), timeLineOne);
      Vector3 pointLineTwo = Vector3.LerpUnclamped(cameraTwo.position, cameraTwo.TransformPoint(rayDirectionsTwo[i]), timeLineTwo);

      Debug.DrawLine(pointLineOne, pointLineTwo, Color.red);
      currentDeltaPos += (pointLineTwo - pointLineOne);
    }

    cameraTwo.position -= (currentDeltaPos / features.Length)*1f;
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