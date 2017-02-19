using UnityEngine;

public class RobotIK : MonoBehaviour {
  public Transform Tooltip;
  public Transform Target;
  public IKJoint[] joints;

  void Update() {
    for (int j = 0; j < joints.Length; j++) {
      joints[j].Evaluate(Tooltip, Target);
    }
  }
}