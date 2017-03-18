using UnityEngine;

public class DistanceConstraint : MonoBehaviour {
  public Transform anchorObject;
  public float Distance = 0.5f;
  public bool anchorIsKinematic = false;

  void Update() {
    Debug.DrawLine(transform.position, anchorObject.position);
  }

  public void ResolveConstraint() {
    if (anchorIsKinematic) {
      transform.position = transform.position.ConstrainDistance(anchorObject.transform.position, Distance);
    } else {
      Vector3 tempPos = transform.position;
      transform.position += (transform.position.ConstrainDistance(anchorObject.transform.position, Distance) - transform.position) / 2f;
      anchorObject.position += (anchorObject.position.ConstrainDistance(tempPos, Distance) - anchorObject.position) / 2f;
    }
  }
}