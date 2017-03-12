 using UnityEngine;

public static class Constraints {
  public static void ConstrainToPoint(this Transform transform, Vector3 oldPoint, Vector3 newPoint, float alpha = 1f) {
    Vector3 oldDisplacement = transform.position - oldPoint;
    Vector3 newCenterPosition = newPoint + (transform.position - newPoint).normalized * oldDisplacement.magnitude;
    Vector3 newDisplacement = newCenterPosition - newPoint;

    transform.position = Vector3.Lerp(transform.position, newCenterPosition, alpha);
    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(oldDisplacement, newDisplacement) * transform.rotation, alpha);
  }

  public static void RotateToPoint(this Transform transform, Vector3 oldPoint, Vector3 newPoint, float alpha = 1f) {
    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(transform.position - oldPoint, transform.position - newPoint) * transform.rotation, alpha);
  }

  public static Vector3 ConstrainToCone(this Vector3 point, Vector3 origin, Vector3 normalDirection, float minDot) {
    return (point - origin).ConstrainToNormal(normalDirection, minDot) + origin;
  }

  public static Vector3 ConstrainToNormal(this Vector3 direction, Vector3 normalDirection, float maxAngle) {
    if (maxAngle <= 0f) return normalDirection.normalized * direction.magnitude; if (maxAngle >= 180f) return direction;
    float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(direction.normalized, normalDirection.normalized), -1f, 1f)) * Mathf.Rad2Deg;
    return Vector3.Slerp(direction.normalized, normalDirection.normalized, (angle - maxAngle) / angle) * direction.magnitude;
  }

  public static Vector3 ConstrainToSegment(this Vector3 position, Vector3 a, Vector3 b) {
    return Vector3.Lerp(a, b, Vector3.Dot(position - a, (b - a).normalized) / (b - a).magnitude);
  }

  public static Vector3 ConstrainDistance(this Vector3 position, Vector3 anchor, float distance) {
    return anchor + ((position - anchor).normalized * distance);
  }

  public static Quaternion ConstrainRotationToCone(Quaternion rotation, Vector3 constraintAxis, Vector3 objectLocalAxis, float maxAngle) {
    return Quaternion.FromToRotation(rotation * objectLocalAxis, ConstrainToNormal(rotation * objectLocalAxis, constraintAxis, maxAngle)) * rotation;
  }

  public static Quaternion ConstrainRotationToConeWithTwist(Quaternion rotation, Vector3 constraintAxis, Vector3 objectLocalAxis, float maxAngle, float maxTwistAngle) {
    Quaternion coneRotation = ConstrainRotationToCone(rotation, constraintAxis, objectLocalAxis, maxAngle);
    Vector3 perpendicularAxis = Vector3.Cross(constraintAxis, Quaternion.Euler(10f, 0f, 0f) * constraintAxis).normalized;
    Quaternion coneConstraint = Quaternion.FromToRotation(objectLocalAxis, coneRotation * objectLocalAxis);
    return ConstrainRotationToCone(coneRotation, coneConstraint * perpendicularAxis, perpendicularAxis, maxTwistAngle);
  }
}