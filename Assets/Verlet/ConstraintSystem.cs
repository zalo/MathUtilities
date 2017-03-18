using UnityEngine;

public class ConstraintSystem : MonoBehaviour {
  DistanceConstraint[] constraints;

	void Start () {
    constraints = GetComponentsInChildren<DistanceConstraint>();
  }

	void Update () {
    for (int i = 0; i < 10; i++) {

      //Knuth Shuffle; randomize the order of the constraint resolution
      for (int t = 0; t < constraints.Length; t++) {
        DistanceConstraint tmp = constraints[t];
        int r = Random.Range(t, constraints.Length);
        constraints[t] = constraints[r];
        constraints[r] = tmp;
      }

      foreach (DistanceConstraint constraint in constraints) {
        constraint.ResolveConstraint();
      }
    }
  }
}
