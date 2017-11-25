using UnityEngine;

public class CCDIKDemo : MonoBehaviour {
  public Transform[] joints;
  public Transform target;
  public TextMesh status;
  public float speed = 1f;
  public bool instant;

  int iterationNumber = 0;
  int currentJoint;
  float progress;
  Quaternion origin, destination;
  Vector3 targetPrevPosition;

  // Update is called once per frame
  void Update() {
    if (target.position != targetPrevPosition) { Reset(); }

    if (instant) {
      for (int j = 0; j < 4; j++) {
        for (int i = joints.Length - 2; i >= 0; i--) {
          joints[i].rotation = joints[i].rotation * Quaternion.FromToRotation(joints[joints.Length - 1].position - joints[i].position, target.position - joints[i].position);
        }
      }
    } else {
      if (progress == 1f) {
        //Step back one joint and rotate it so the tip joint is nearer to the target
        if (currentJoint == 0) { status.text = "Iteration: " + ++iterationNumber; }
        currentJoint = (currentJoint != 0) ? currentJoint - 1 : joints.Length - 2;
        origin = joints[currentJoint].rotation;
        destination = origin * Quaternion.FromToRotation(joints[joints.Length - 1].position - joints[currentJoint].position, target.position - joints[currentJoint].position);
        progress = 0f;
      }

      //Progress the animation ahead and update the joint position
      progress = Mathf.Clamp01(progress + (Time.deltaTime * speed));
      joints[currentJoint].rotation = Quaternion.Slerp(origin, destination, easeInOutQuint(progress));
    }
  }

  private void Reset() {
    for (int i = 0; i < joints.Length; i++) {
      joints[i].rotation = Quaternion.identity;
    }
    currentJoint = joints.Length - 2;
    origin = Quaternion.identity;
    destination = Quaternion.FromToRotation(joints[joints.Length - 1].position - joints[currentJoint].position, target.position - joints[currentJoint].position);
    targetPrevPosition = target.position;
    progress = 0f;
    iterationNumber = 0;
    status.text = "Iteration: " + iterationNumber;
  }

  float easeInOutQuint(float t) { return t < .5 ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t; }

  private void OnDrawGizmos() {
    if (instant) {
      for (int i = 0; i < joints.Length-1; i++) {
        Gizmos.DrawLine(joints[i].position, joints[i+1].position);
      }
    } else {
      Gizmos.color = Color.red;
      Gizmos.DrawLine(joints[currentJoint].position, target.position);
      Gizmos.color = Color.white;
      for (int i = 0; i < joints.Length - 1; i++) {
        Gizmos.DrawLine(joints[i].position, joints[i + 1].position);
      }
      Gizmos.color = Color.green;
      Gizmos.DrawLine(joints[currentJoint].position, joints[joints.Length - 1].position);
    }
  }
}
