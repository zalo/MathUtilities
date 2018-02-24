using UnityEngine;

public class MotorTorque : MonoBehaviour {
  HingeJoint joint;
	void Start () {
    joint = GetComponent<HingeJoint>();
	}

	void FixedUpdate () {
    Vector3 jointPos = transform.TransformPoint(joint.anchor);
    Vector2 jointXZ = new Vector2(jointPos.x, jointPos.z);

    Vector3 massPos = GetComponent<Rigidbody>().worldCenterOfMass;
    Vector2 massXZ = new Vector2(massPos.x, massPos.z);

    float displacement = Vector2.Distance(jointXZ,massXZ);
    joint.AddTorque(-Physics.gravity.y * displacement * GetComponent<Rigidbody>().mass * 1.001f);
	}
}

//Torque Extension class, adds the ability to apply torque to Rigidbodies and HingeJoints
public static class TorqueExtension {
  public static void AddTorque(this HingeJoint joint, float torque) {
    Rigidbody body = joint.GetComponent<Rigidbody>();
    Vector3 worldAxis = body.transform.TransformDirection(joint.axis).normalized;
    body.AddTorqueAtPosition(joint.anchor, worldAxis, torque);
    if (joint.connectedBody) { joint.connectedBody.AddTorqueAtPosition(joint.connectedAnchor, worldAxis, -torque); }
  }

  public static void AddTorqueAtPosition(this Rigidbody body, Vector3 localPosition, Vector3 globalTorqueAxis, float torque) {
    Vector3 worldPosition = body.transform.TransformPoint(localPosition);
    Vector3 forceOffset = globalTorqueAxis.perpendicularVector();
    Vector3 forceDirection = Vector3.Cross(globalTorqueAxis, forceOffset);

    body.AddForceAtPosition(forceDirection * torque * 0.5f, worldPosition + forceOffset, ForceMode.Force);
    body.AddForceAtPosition(-forceDirection* torque * 0.5f, worldPosition - forceOffset, ForceMode.Force);
  }

  public static Matrix4x4 inverseWorldInertiaTensor(this Rigidbody body) {
    Matrix4x4 tensorRotation = Matrix4x4.TRS(Vector3.zero, body.rotation * body.inertiaTensorRotation, Vector3.one);
    Matrix4x4 worldInertiaTensor = (tensorRotation * Matrix4x4.Scale(body.inertiaTensor) * tensorRotation.inverse);
    return worldInertiaTensor.inverse;
  }

  public static Matrix4x4 inverseInertiaTensor(this Rigidbody body) {
    return Matrix4x4.TRS(Vector3.zero, body.inertiaTensorRotation, new Vector3(1f / body.inertiaTensor.x, 1f / body.inertiaTensor.y, 1f / body.inertiaTensor.z));
  }

  public static Matrix4x4 inertiaTensor(this Rigidbody body) {
    return Matrix4x4.TRS(Vector3.zero, body.inertiaTensorRotation, body.inertiaTensor);
  }

  public static Vector3 perpendicularVector(this Vector3 vector) {
    if (Mathf.Abs(vector.x) < 0.000001f  && Mathf.Abs(vector.y) < 0.000001f) {
      if (Equals(vector,Vector3.zero)) { return vector; }
      return new Vector3(0f, 1f, 0f);
    }
    return new Vector3(-vector.y, vector.x, 0f).normalized;
  }
}