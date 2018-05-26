using UnityEngine;

public class CCDIKJoint : MonoBehaviour {
  public Vector3 axis = Vector3.right;
  public float maxAngle = 180;
  Vector3 perpendicular; void Start() { perpendicular = axis.Perpendicular(); }

  public void Evaluate(Transform ToolTip, Transform Target, bool rotateToDirection = false) {
    //Rotate the assembly so the tooltip better matches the target position/direction
    transform.rotation = (rotateToDirection ? Quaternion.FromToRotation(ToolTip.up, Target.forward) : Quaternion.FromToRotation(ToolTip.position - transform.position, Target.position - transform.position)) * transform.rotation;

    //Enforce only rotating with the hinge
    transform.rotation = Quaternion.FromToRotation(transform.rotation * axis, transform.parent.rotation * axis) * transform.rotation;

    //Enforce Joint Limits
    transform.rotation = Quaternion.FromToRotation(transform.rotation * perpendicular, (transform.rotation * perpendicular).ConstrainToNormal(transform.parent.rotation * perpendicular, maxAngle)) * transform.rotation;
  }
}
