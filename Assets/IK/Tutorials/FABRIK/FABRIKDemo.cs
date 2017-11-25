using UnityEngine;

public class FABRIKDemo : MonoBehaviour {
  public Transform[] joints;
  public Transform target;
  public TextMesh status;
  public float speed = 1f;
  public bool instant;

  int iterationNumber = 0;
  bool forward;
  int currentJoint;
  float progress;
  Vector3 origin, destination, targetPrevPosition;

  // Update is called once per frame
  void Update() {
    if(target.position != targetPrevPosition) { Reset(); }

    if (instant) {
      for (int j = 0; j < 4; j++) {
        //Step back through all the joints and clamp their position to be within 2 units of the joint above it
        //The "Forward Reaching" pass
        for (int i = joints.Length - 1; i >= 0; i--) {
          if (i == joints.Length - 1) {
            joints[i].position = target.position;
          } else {
            joints[i].position = joints[i].position.ConstrainDistance(joints[i + 1].position, 2f);
          }
        }

        //Step forward through all the joints and clamp their position to be within 2 units of the joint below it
        //The "Backward Reaching" pass
        for (int i = 0; i < joints.Length; i++) {
          if (i == 0) {
            joints[i].position = Vector3.zero;
          } else {
            joints[i].position = joints[i].position.ConstrainDistance(joints[i - 1].position, 2f);
          }
        }
      }
    } else {
      if (progress == 1f) {
        if (forward) {
          if (currentJoint != 0) {
            //Step back one joint and clamp its destination position to be within 2 units of the joint above it
            currentJoint--;
            origin = joints[currentJoint].position;
            destination = joints[currentJoint].position.ConstrainDistance(joints[currentJoint + 1].position, 2f);
          } else {
            //Begin the backward reaching pass
            forward = false;
            origin = joints[currentJoint].position;
            destination = Vector3.zero;
            status.text = "Backward\nReaching Pass\nIteration: " + iterationNumber;
          }
        } else {
          if (currentJoint != joints.Length - 1) {
            //Step forward one joint and clamp its destination position to be within 2 units of the joint below it
            currentJoint++;
            origin = joints[currentJoint].position;
            destination = joints[currentJoint].position.ConstrainDistance(joints[currentJoint - 1].position, 2f);
          } else {
            //Begin the forward reaching pass
            forward = true;
            origin = joints[currentJoint].position;
            destination = target.position;
            iterationNumber++;
            status.text = "Forward\nReaching Pass\nIteration: " + iterationNumber;
          }
        }
        progress = 0f;
      }

      //Progress the animation ahead and update the joint position
      progress = Mathf.Clamp01(progress + (Time.deltaTime * speed));
      joints[currentJoint].position = Vector3.Lerp(origin, destination, easeInOutQuint(progress));
    }
  }

  private void Reset() {
    for(int i = 0; i<joints.Length; i++) {
      joints[i].position = new Vector3(0f, i * 2f, 0f);
    }
    forward = true;
    currentJoint = joints.Length - 1;
    origin = joints[currentJoint].position;
    destination = target.position;
    targetPrevPosition = target.position;
    progress = 0f;
    iterationNumber = 0;
    status.text = "Forward\nReaching Pass\nIteration: " + iterationNumber;
  }

  float easeInOutQuint(float t){ return t < .5 ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t; }

  private void OnDrawGizmos() {
    if (instant) {

    } else {
      //for (int i = 0; i < joints.Length; i++) {
      //  DrawCircle(joints[i].position, Vector3.forward, 2f);
      //}

      if (currentJoint == 0 && !forward) {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(joints[currentJoint].position, Vector3.zero);
      } else if (currentJoint == joints.Length - 1 && forward) {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(joints[currentJoint].position, target.position);
      } else {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(joints[currentJoint].position, joints[currentJoint + (forward ? 1 : -1)].position);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(joints[currentJoint].position, destination);
      }
    }
  }

  void DrawCircle(Vector3 position, Vector3 normal, float radius) {
    int numSegments = 45;
    Vector3 dir = Vector3.up * radius;
    for(int i = 0; i< numSegments; i++) {
      Vector3 newDir = Quaternion.AngleAxis(360f / numSegments, normal) * dir;
      Gizmos.DrawLine(dir + position, newDir + position);
      dir = newDir;
    }
  }
}
