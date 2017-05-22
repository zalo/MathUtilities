using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probe : MonoBehaviour {
  public Transform Target;
  Rigidbody body;

	void Start () {
    body = GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
    body.velocity = (Target.position - body.position) / Time.fixedDeltaTime;
    body.velocity = body.velocity.normalized * Mathf.Clamp(body.velocity.magnitude, 0f, 20f);

    body.maxAngularVelocity = 20;
    Quaternion rotation = Target.rotation * Quaternion.Inverse(body.rotation);
    Vector3 angularVelocity = (new Vector3(
      Mathf.DeltaAngle(0, rotation.eulerAngles.x), 
      Mathf.DeltaAngle(0, rotation.eulerAngles.y), 
      Mathf.DeltaAngle(0, rotation.eulerAngles.z)) 
      * Mathf.Deg2Rad) / Time.fixedDeltaTime;

    body.angularVelocity = angularVelocity;
  }
}
