using UnityEngine;

public class CCDIK : MonoBehaviour {
  public Transform Tooltip;
  public Transform Target;
  public CCDIKJoint[] joints;

  void Update() {
    for (int j = 0; j < joints.Length; j++) {
      joints[j].Evaluate(Tooltip, Target, j < 2);
    }
  }
}