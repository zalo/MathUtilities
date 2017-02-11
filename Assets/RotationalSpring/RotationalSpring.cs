using UnityEngine;
public class RotationalSpring : MonoBehaviour {
  public float StiffnessRatio = 0.1f;
  public float DampingRatio = 0.05f;
  Quaternion angularVelocity = Quaternion.identity;
  Quaternion targetRotation = Quaternion.identity;
  void Update() {
    angularVelocity = Quaternion.Slerp(angularVelocity, Quaternion.identity, DampingRatio);
    angularVelocity = Quaternion.Slerp(angularVelocity, Quaternion.Inverse(transform.rotation) * targetRotation, StiffnessRatio);
    transform.rotation *= angularVelocity;
  }
}