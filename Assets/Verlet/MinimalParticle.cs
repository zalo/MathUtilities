using UnityEngine;

[DefaultExecutionOrder(-1)]
public class MinimalParticle : MonoBehaviour {
  public bool useGravity = true;
  Vector3 prevPosition; float prevDeltaTime = 1f;
  void Update() {
    //Grab State from Previous Frame
    Vector3 tempPos = transform.position;
    float tempDeltaTime = Time.deltaTime;

    //Integrate Position
    transform.position += (transform.position - prevPosition) * (Time.deltaTime / prevDeltaTime) + (useGravity ? (Physics.gravity * Time.deltaTime * Time.deltaTime) : Vector3.zero);

    //Store State from Previous Frame
    prevPosition = tempPos;
    prevDeltaTime = tempDeltaTime;
  }
}
