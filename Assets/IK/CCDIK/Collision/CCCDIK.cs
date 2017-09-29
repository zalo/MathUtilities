using UnityEngine;

public class CCCDIK : MonoBehaviour {
  public Transform Tooltip;
  public Transform Target;
  public CCDIKJoint[] joints;

  public SphereCollider environment;
  Transform nearestPoint;
  Transform displacementTarget;

  private void Start() {
    nearestPoint = new GameObject().transform;
    displacementTarget = new GameObject().transform;
  }

  void Update() {
    for (int it = 0; it < 1; it++) {
      //Move toward main target
      for (int j = 0; j < joints.Length; j++) {
        joints[j].Evaluate(Tooltip, Target, j < 2);
      }

      //Find the closest point 
      for (int j = joints.Length - 1; j >= 0; j--) {
        Vector3 jointPoint = Constraints.ClosestPointOnCapsule(environment.transform.position, joints[j].GetComponent<Collider>() as CapsuleCollider);
        Vector3 offset = jointPoint - environment.transform.position;
        if (offset.magnitude < environment.radius * environment.transform.lossyScale.x) {
          nearestPoint.parent = joints[j].transform;
          nearestPoint.position = jointPoint;
          displacementTarget.position = nearestPoint.transform.position + (offset.normalized * ((environment.radius * environment.transform.lossyScale.x) - offset.magnitude));
          Debug.DrawLine(nearestPoint.position, displacementTarget.position);

          //Move away from environment
          for (int k = j; k < joints.Length; k++) {
            joints[k].Evaluate(nearestPoint, displacementTarget);
          }
        }
      }
    }
  }
}