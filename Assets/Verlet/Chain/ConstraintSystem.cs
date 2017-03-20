using UnityEngine;

public class ConstraintSystem : MonoBehaviour {
  DistanceConstraint[] constraints;

	void Start () {
    constraints = GetComponentsInChildren<DistanceConstraint>();
  }

	void Update () {
    for (int i = 0; i < 10; i++) {
      foreach (DistanceConstraint constraint in constraints) {
        constraint.ResolveConstraint();
      }
    }
  }
}
