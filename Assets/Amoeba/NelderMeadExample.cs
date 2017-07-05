using UnityEngine;

public class NelderMeadExample : MonoBehaviour {
  const int dimensions = 3;
  Vector4[] debugPoints;
  NelderMead solver;

  //Initialize the solver with a starting state and a cost function
  void Start() {
    debugPoints = new Vector4[dimensions + 1];
    float[] initialCoord = new float[dimensions];
    for (int i = 0; i < dimensions; i++) { initialCoord[i] = Random.value * 7f; }
    solver = new NelderMead(initialCoord, distanceCost);

    for (int i = 0; i < solver.simplexVertices.Count; i++) {
      int index = solver.simplexVertices[i].originalIndex;
      debugPoints[index] = new Vector4(
        solver.simplexVertices[i].coordinates[0],
        solver.simplexVertices[i].coordinates[1],
        solver.simplexVertices[i].coordinates[2],
        solver.simplexVertices[i].cost);
    }
  }

  //Step the solver forward one iteration per second
  float updateTime = 0f;
  void Update() {
    if (Time.time - updateTime >= 1f) {
      solver.stepSolver();
      updateTime += 1f;
    }
  }

  //Draw the current state of the solver
  void OnDrawGizmos() {
    if (solver == null) { return; }

    //This is the tetrahedron that represents the state of the solver
    for (int i = 0; i < solver.simplexVertices.Count; i++) {
      int index = solver.simplexVertices[i].originalIndex;
      debugPoints[index] = Vector4.Lerp(debugPoints[index], new Vector4(
        solver.simplexVertices[i].coordinates[0],
        solver.simplexVertices[i].coordinates[1],
        solver.simplexVertices[i].coordinates[2],
        solver.simplexVertices[i].cost), 0.1f);
      //Gizmos.color = Color.HSVToRGB((debugPoints[i][3]/3f)%1f, 1f, 1f);
      Gizmos.DrawSphere(debugPoints[i], 0.1f);// * debugPoints[i][3]);
    }

    //This is the goal position that the solver is searching for
    Gizmos.color = Color.green;
    Gizmos.DrawSphere(Vector3.zero, 0.05f);

    //These lines connect the vertices of the tetrahedron
    Gizmos.color = Color.white;
    Gizmos.DrawLine(debugPoints[0], debugPoints[1]);
    Gizmos.DrawLine(debugPoints[1], debugPoints[2]);
    Gizmos.DrawLine(debugPoints[2], debugPoints[0]);
    Gizmos.DrawLine(debugPoints[0], debugPoints[3]);
    Gizmos.DrawLine(debugPoints[1], debugPoints[3]);
    Gizmos.DrawLine(debugPoints[2], debugPoints[3]);
  }

  //The evaluates the cost associated with a coordinate in parameter space
  //This particular function simply represents the distance that the coordinate is away from the origin in 3D space.
  public float distanceCost(float[] coordinate) {
    return Vector3.Distance(Vector3.zero, new Vector3(coordinate[0], coordinate[1], coordinate[2]));
  }
}
