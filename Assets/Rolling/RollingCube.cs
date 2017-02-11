using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RollingCube : MonoBehaviour {
  public Transform target;
  Rigidbody body;
  void Start() {
    body = GetComponent<Rigidbody>();
  }

  void FixedUpdate() {
    Vector3 direction = (transform.position - target.position).normalized;
    body.rotation = Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up), transform.up);
    body.angularVelocity = body.rotation * new Vector3(0f, 0f, 100f);
  }
}