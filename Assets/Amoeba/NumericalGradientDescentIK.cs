using UnityEngine;

public class NumericalGradientDescentIK : MonoBehaviour {
  public Transform Tooltip;
  public Transform Target;
  public Transform[] joints;
  public float stepSize = 50f;
  public float epsilon = 0.001f;
  public int iterationsPerFrame = 5;

  NumericalGradientDescent solver;

  //Initialize the solver with a neutral starting state and a cost function
  void Start() {
    float[] initialCoord = new float[joints.Length];
    solver = new NumericalGradientDescent(initialCoord, distanceCost, stepSize, epsilon);
  }

  void Update() {
    //Allow for the modification of the solver parameters after initialization
    solver.alpha = stepSize;
    solver.epsilon = epsilon;

    //Step the solver forward
    for (int i = 0; i < iterationsPerFrame; i++) {
      solver.stepSolver();
    }
  }

  //This evaluates the cost associated with a coordinate in parameter space
  //This particular function simply represents the distance that the tooltip is away from the target.
  public float distanceCost(float[] coordinate) {
    joints[0].localRotation = Quaternion.Euler(0f, coordinate[0], 0f);
    joints[1].localRotation = Quaternion.Euler(coordinate[1], 0f, 0f);
    joints[2].localRotation = Quaternion.Euler(coordinate[2], 0f, 0f);
    joints[3].localRotation = Quaternion.Euler(coordinate[3], 0f, 0f);
    joints[4].localRotation = Quaternion.Euler(0f, coordinate[4], 0f);

    return (Tooltip.position - Target.position).magnitude;
  }
}
