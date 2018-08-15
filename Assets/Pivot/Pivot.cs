using UnityEngine;
public class Pivot : MonoBehaviour {
  public Transform animatedTransform;

  float alpha;
  Vector3 pivotPoint;

	void Update () {
    //Calculate the Pivot Point
    if (transform.position != Vector3.zero) {
      pivotPoint = calculatePivotPoint(transform.position, transform.rotation);

      //Move the animated visualization
      alpha = (1f - Mathf.Cos(Time.time * Mathf.PI % Mathf.PI)) * 0.5f;
      animatedTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
      animatedTransform.RotateAroundPivot(pivotPoint, Quaternion.Slerp(Quaternion.identity, transform.rotation, alpha), alpha);
    }
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.white * 0.75f;
    Gizmos.DrawLine(pivotPoint, Vector3.zero);
    Gizmos.DrawLine(pivotPoint, animatedTransform.position);
    Gizmos.DrawLine(pivotPoint, transform.position);
    Gizmos.color = Color.grey;
    Gizmos.DrawSphere(pivotPoint, 0.025f);
  }

  //For all rotations in 2D (and a subset in 3D) calculate the pivot of this transformation
  public static Vector3 calculatePivotPoint(Vector3 deltaPosition, Quaternion deltaRotation) {
    Vector3 axis; float angle; deltaRotation.ToAngleAxis(out angle, out axis);
    Vector3 halfAngleDirection = Vector3.Cross(axis, deltaPosition);
    Quaternion halfRotation = Quaternion.Slerp(Quaternion.identity, deltaRotation, 0.5f);
    Ray preRay = new Ray(Vector3.zero, Quaternion.Inverse(halfRotation) * halfAngleDirection);
    Ray postRay = new Ray(deltaPosition, halfRotation * halfAngleDirection);
    return Constraints.ClosestPointOnRayToRay(preRay, postRay);
  }
}
