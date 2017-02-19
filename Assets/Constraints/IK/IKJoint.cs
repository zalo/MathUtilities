using UnityEngine;

public class IKJoint : MonoBehaviour {
  public Vector3 axis = Vector3.right;
  public float maxAngle = 180;

  public void Evaluate(Transform ToolTip, Transform Target) {
    //Rotate the assembly so the tooltip better matches the target position
    transform.rotation = Quaternion.FromToRotation(ToolTip.position - transform.position, Target.position - transform.position) * transform.rotation;

    //Enforce only rotating with the hinge
    transform.rotation = Quaternion.FromToRotation(transform.rotation * axis, transform.parent.rotation * axis) * transform.rotation;

    //Enforce Joint Limits
    Vector3 perpendicular = (axis == Vector3.right ? Vector3.up : (axis == Vector3.up ? Vector3.right : axis == Vector3.forward ? Vector3.up : Vector3.Cross(axis, Vector3.one)));
    transform.rotation = Quaternion.FromToRotation(transform.rotation * perpendicular, (transform.rotation * perpendicular).ConstrainToNormal(transform.parent.rotation * perpendicular, maxAngle)) * transform.rotation;
  }
}
