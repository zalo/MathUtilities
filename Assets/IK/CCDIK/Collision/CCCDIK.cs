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
        Vector3 jointPoint = closestPointOnCapsule(environment.transform.position, joints[j].GetComponent<Collider>() as CapsuleCollider);
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

  Vector3 closestPointOnCapsule(Vector3 point, CapsuleCollider collider) {
    Vector3 offset = (collider.direction==0?Vector3.right:Vector3.up) * Mathf.Clamp01((collider.height*0.5f) - collider.radius);
    Vector3 onSegment = Constraints.ConstrainToSegment(point, collider.transform.TransformPoint(collider.center + offset), collider.transform.TransformPoint(collider.center - offset));
    return onSegment + ((point - onSegment).normalized * collider.radius);
  }
}