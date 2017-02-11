using UnityEngine;

public class Evaluator : MonoBehaviour {
  public ConstrainedJoint[] joints;

	// Update is called once per frame
	void Update () {
    for (int i = 0; i < joints.Length; i++) {
      joints[i].Evaluate();
    }
	}
}