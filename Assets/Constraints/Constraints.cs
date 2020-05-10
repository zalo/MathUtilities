 using UnityEngine;

public static class Constraints {
  public static void ConstrainToPoint(this Transform transform, Vector3 oldPoint, Vector3 newPoint, bool fastApproximate = true) {
    Quaternion rotation = Quaternion.FromToRotation(transform.position - oldPoint, transform.position - newPoint);
    transform.position = fastApproximate ? transform.position.FastConstrainDistance(newPoint, (transform.position - oldPoint).sqrMagnitude) :
                                           transform.position.ConstrainDistance(newPoint, (transform.position - oldPoint).magnitude);
    transform.rotation = rotation * transform.rotation;
  }

  public static void ConstrainToPoint(this Transform transform, Vector3 oldPoint, Vector3 newPoint, Vector3 anchor) {
    Quaternion rotation = Quaternion.FromToRotation(oldPoint - anchor, newPoint - anchor);
    transform.RotateAroundPivot(anchor, rotation);
    transform.position += newPoint - oldPoint.RotateAroundPivot(anchor, rotation);
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
    Vector3 ba = b - a;
    return Vector3.Lerp(a, b, Vector3.Dot(position - a, ba) / ba.sqrMagnitude);
  }

  public static Vector3 ConstrainToPlane(this Vector3 position, Vector3 planePoint, Vector3 planeNormal) {
    return position - ((Vector3.Dot(position - planePoint, planeNormal) / Vector3.Dot(planeNormal, planeNormal)) * planeNormal);
  }

  public static float ClosestTimeOnSegmentToLine(Vector3 segA, Vector3 segB, Vector3 lineA, Vector3 lineB) {
    Vector3 lineBA = lineB - lineA; float lineDirSqrMag = Vector3.Dot(lineBA, lineBA);
    Vector3 inPlaneA = segA - ((Vector3.Dot(segA - lineA, lineBA) / lineDirSqrMag) * lineBA),
            inPlaneB = segB - ((Vector3.Dot(segB - lineA, lineBA) / lineDirSqrMag) * lineBA);
    Vector3 inPlaneBA = inPlaneB - inPlaneA;
    return (inPlaneA != inPlaneB) ? Vector3.Dot(lineA - inPlaneA, inPlaneBA) / Vector3.Dot(inPlaneBA, inPlaneBA) : 0f;
  }

  public static float ClosestTimeOnRayToRay(Ray a, Ray b) {
    return ClosestTimeOnSegmentToLine(a.origin, a.origin + a.direction, b.origin, b.origin + b.direction);
  }

  public static Vector3 ClosestPointOnRayToRay(Ray a, Ray b) {
    return Vector3.LerpUnclamped(b.origin, b.origin + b.direction, ClosestTimeOnSegmentToLine(b.origin, b.origin + b.direction, a.origin, a.origin + a.direction));
  }

  public static Vector3 ClosestPointOnSegmentToLine(Vector3 segA, Vector3 segB, Vector3 lineA, Vector3 lineB) {
    return Vector3.Lerp(segA, segB, ClosestTimeOnSegmentToLine(segA, segB, lineA, lineB));
  }

  public static void ClosestPointSegmentToSegment(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Vector3 point1, out Vector3 point2) {
    Vector3 rayPoint = ClosestPointOnSegmentToLine(a, b, c, d);
    point2 = rayPoint.ConstrainToSegment(c, d); point1 = point2.ConstrainToSegment(a, b);
  }

  public static Vector3 ConstrainToCapsule(this Vector3 position, Vector3 a, Vector3 b, float radius, bool toSurface = false) {
    Vector3 onSegment = ConstrainToSegment(position, a, b);
    Vector3 displacement = position - onSegment;
    float magnitude = displacement.magnitude;
    return magnitude > radius ? onSegment + (displacement.normalized * radius) : position;
  }

  public static Vector3 ClosestPointOnCapsule(this Vector3 point, CapsuleCollider collider) {
    Vector3 offset = (collider.direction == 0 ? Vector3.right : Vector3.up) * Mathf.Clamp01((collider.height * 0.5f) - collider.radius);
    Vector3 onSegment = ConstrainToSegment(point, collider.transform.TransformPoint(collider.center + offset), collider.transform.TransformPoint(collider.center - offset));
    return onSegment + ((point - onSegment).normalized * collider.radius);
  }

  //Implemented in real-time here:
  //https://www.shadertoy.com/view/4l2cRW
  public static Vector3 closestPointToBiCapsule(this Vector3 pos, Vector3 a, Vector3 b, float r1, float r2, out float capsuleTime) {
    //Standard line segment closest point
    Vector3 ba = b - a; float baMagnitude = ba.magnitude;
    float alpha = (Vector3.Dot(pos - a, ba) / Vector3.Dot(ba, ba));
    Vector3 capsuleSegmentPos = Vector3.Lerp(a, b, alpha);

    //Calculate the offset along segment according to the slope of the bicapsule
    float pointSphereRadius = r1 - r2; //This collapses the problem into finding the tangent angle for a point/sphere
    float exsecantLength = ((baMagnitude / Mathf.Abs(pointSphereRadius)) - 1f) * baMagnitude;
    float tangentAngle = Mathf.Acos(1.0f / (exsecantLength + 1.0f)); //This is also known as the "arcexsecant" function
    float tangentOffset = (capsuleSegmentPos - pos).magnitude / Mathf.Tan(tangentAngle); //This is adjacent / tan(theta) = opposite
    tangentOffset *= sign(pointSphereRadius); //Allows it to handle r2 > r1 as well

    //And back to classic capsule closest point (with lerped radii)
    capsuleTime = alpha - tangentOffset;
    Vector3 bicapsuleSegmentPos = Vector3.Lerp(a, b, alpha - tangentOffset); float bicapsuleRadius = Mathf.Lerp(r1, r2, alpha - tangentOffset);
    return bicapsuleSegmentPos + ((pos - bicapsuleSegmentPos).normalized * bicapsuleRadius);
  }

  public static Vector3 closestPointToBiCapsule(this Vector3 pos, Vector3 a, Vector3 b, float r1, float r2) {
    float time;
    return pos.closestPointToBiCapsule(a, b, r1, r2, out time);
  }

  //Ack late night function; horrible horrible code
  public static Vector4 ClosestPointCapsuleOnPlane(CapsuleCollider collider, Vector3 point, Vector3 normal) {
    Vector3 offset = (collider.direction == 0 ? Vector3.right : Vector3.up) * Mathf.Clamp01((collider.height * 0.5f) - collider.radius);

    Vector3 capsuleEnd = collider.transform.TransformPoint(collider.center + offset);
    Vector4 planeCandidate1 = Vector3.ProjectOnPlane(capsuleEnd - point, normal) + point;
    planeCandidate1 = new Vector4(planeCandidate1.x, planeCandidate1.y, planeCandidate1.z, (-Mathf.Sign(Vector3.Dot(capsuleEnd - point, normal)) * Vector3.Distance(planeCandidate1, capsuleEnd)) - collider.radius);

    capsuleEnd = collider.transform.TransformPoint(collider.center - offset);
    Vector4 planeCandidate2 = Vector3.ProjectOnPlane(capsuleEnd - point, normal) + point;
    planeCandidate2 = new Vector4(planeCandidate2.x, planeCandidate2.y, planeCandidate2.z, (-Mathf.Sign(Vector3.Dot(capsuleEnd - point, normal)) * Vector3.Distance(planeCandidate2, capsuleEnd)) - collider.radius);

    if(Vector3.Dot(normal, collider.transform.rotation * offset) > 0f) {
      return planeCandidate2;
    } else {
      return planeCandidate1;
    }
  }

  public static Vector3 ConstrainDistance(this Vector3 position, Vector3 anchor, float distance) {
    return anchor + ((position - anchor).normalized * distance);
  }

  public static Vector3 FastConstrainDistance(this Vector3 position, Vector3 anchor, float sqrDistance) {
    Vector3 offset = (position - anchor);
    offset *= (sqrDistance / (Vector3.Dot(offset, offset) + sqrDistance) - 0.5f) * 2f;
    return position + offset;
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

  private static int sign(float num) {
    return num == 0 ? 0 : (num >= 0 ? 1 : -1);
  }

  public static Vector3 ConstrainToTriangle(this Vector3 position, Vector3 a, Vector3 b, Vector3 c) {
    Vector3 normal = Vector3.Cross(b - a, a - c);
    bool outsidePlaneBounds =
    (sign(Vector3.Dot(Vector3.Cross(b - a, normal), position - a)) +
     sign(Vector3.Dot(Vector3.Cross(c - b, normal), position - b)) +
     sign(Vector3.Dot(Vector3.Cross(a - c, normal), position - c)) < 2);
    if (!outsidePlaneBounds) {
      //Project onto plane
      return Vector3.ProjectOnPlane(position - a, normal) + a;
    } else {
      //Constrain to edges
      Vector3 edge1 = position.ConstrainToSegment(a, b);
      Vector3 edge2 = position.ConstrainToSegment(b, c);
      Vector3 edge3 = position.ConstrainToSegment(c, a);
      float sm1 = Vector3.SqrMagnitude(position - edge1);
      float sm2 = Vector3.SqrMagnitude(position - edge2);
      float sm3 = Vector3.SqrMagnitude(position - edge3);
      if (sm1 < sm2) {
        if (sm1 < sm3) {
          return edge1;
        }
      } else {
        if (sm2 < sm3) {
          return edge2;
        }
      }
      return edge3;
    }
  }

  public static void RotateAroundPivot(this Transform transform, Vector3 pivot, Quaternion rotation, float alpha = 1f) {
    Quaternion toRotate = Quaternion.Slerp(Quaternion.identity, rotation, alpha);
    transform.position = transform.position.RotateAroundPivot(pivot, rotation);
    transform.rotation = toRotate * transform.rotation;
  }

  public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, Quaternion rotation) {
    return (rotation * (point - pivot)) + pivot;
  }

  //Credit to Sam Hocevar of LolEngine
  //lolengine.net/blog/2013/09/21/picking-orthogonal-vector-combing-coconuts
  public static Vector3 Perpendicular(this Vector3 vec) {
      return Mathf.Abs(vec.x) > Mathf.Abs(vec.z) ? new Vector3(-vec.y, vec.x, 0f)
                                                 : new Vector3(0f, -vec.z, vec.y);
  }
}
