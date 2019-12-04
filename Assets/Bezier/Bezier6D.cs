using System.Collections.Generic;
using UnityEngine;

//-A simple technique for directly manipulating trajectories
//Multiple of these operations can be applied to a trajectory
//in sequence (with different starting/ending points); given an
//arbitrary number of sequentially satisfied constraints
//-Very similar to the concept of the quadratic bezier curve
//-Useful for loop closure and map relinearization (where many 
//spatial collocation constraints need to be directly satisfied).
//https://april.eecs.umich.edu/media/pdfs/olson2006icra.pdf
public class Bezier6D : MonoBehaviour {
  List<Matrix4x4> poseHistory = new List<Matrix4x4>();
  Matrix4x4 curFinalPose;
  Matrix4x4 correction;

  public Transform newFinalPose;

  // Use this for initialization
  void Start() {
    poseHistory.Add(transform.localToWorldMatrix);
    curFinalPose = poseHistory[0];
  }

  // Update is called once per frame
  void Update() {
    Matrix4x4 curDelta = curFinalPose.inverse * transform.localToWorldMatrix;

    if ((curDelta.MultiplyPoint3x4(Vector3.zero)).magnitude > 0.1f) {
      poseHistory.Add(transform.localToWorldMatrix);
      curFinalPose = transform.localToWorldMatrix;
    }

    correction =  newFinalPose.localToWorldMatrix * curFinalPose.inverse;
  }

  public void OnDrawGizmos() {
    if (poseHistory.Count > 0) {
      for (int i = 1; i < poseHistory.Count; i++) {
        Gizmos.color = Color.white;
        Matrix4x4 warpedPose = FromMatrixExtension.Lerp(Matrix4x4.identity, correction, ((float)i / poseHistory.Count)) * poseHistory[i];
        Gizmos.DrawSphere(poseHistory[i].MultiplyPoint3x4(Vector3.zero), 0.1f);
        
        Vector3 origin  = warpedPose.MultiplyPoint3x4(Vector3.zero);
        Vector3 right   = warpedPose.MultiplyPoint3x4(Vector3.right);
        Vector3 up      = warpedPose.MultiplyPoint3x4(Vector3.up);
        Vector3 forward = warpedPose.MultiplyPoint3x4(Vector3.forward);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, right);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, up);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin, forward);
      }
    }
  }
}
