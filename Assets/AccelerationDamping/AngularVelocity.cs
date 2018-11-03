using UnityEngine;

public class AngularVelocity : MonoBehaviour {
  public Vector3 angVel = new Vector3(10f, 10f, 0f);
  Rigidbody body;

	// Use this for initialization
	void Start () {
    body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
    if (body != null) {
      body.angularVelocity = angVel;
    } else {
      transform.rotation = Quaternion.Euler(angVel*Time.fixedDeltaTime) * transform.rotation;
    }
	}
}