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
public class Bezier : MonoBehaviour {
  List<Vector3> deltaPoseHistory = new List<Vector3>();
  Vector3 curFinalPos;
  Vector3 correction;

  public Transform newFinalPos;

	// Use this for initialization
	void Start () {
    deltaPoseHistory.Add(transform.position);
	}
	
	// Update is called once per frame
	void Update () {
    Vector3 curDelta = transform.position - curFinalPos;

    if (Vector3.Distance(transform.position, curFinalPos) > 0.1f) {
      deltaPoseHistory.Add(curDelta);
      curFinalPos += curDelta;
    }

    correction = newFinalPos.position - curFinalPos;
  }

  public void OnDrawGizmos() {
    if (deltaPoseHistory.Count > 0) {
      Vector3 curPos = deltaPoseHistory[0];
      for (int i = 1; i < deltaPoseHistory.Count; i++) {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(curPos + (correction * ((float)i / deltaPoseHistory.Count)), 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(curPos, 0.1f);

        curPos += deltaPoseHistory[i];
      }
    }
  }
}
