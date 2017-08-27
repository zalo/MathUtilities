using UnityEngine;

public class SpatialBundleAdjuster : MonoBehaviour {
  public Transform[] features;
  public Transform aligningCamera;
  Vector3[] rayDirections;

  KabschSolver solver = new KabschSolver();
  Vector3[] inPoints, refPoints;
  void Start() {
    rayDirections = new Vector3[features.Length];
    inPoints = new Vector3[features.Length];
    refPoints = new Vector3[features.Length];

    for (int i = 0; i < features.Length; i++) {
      refPoints[i] = features[i].position;
      rayDirections[i] = aligningCamera.InverseTransformPoint(features[i].position).normalized;
    }
  }

  void Update() {
    for (int iteration = 0; iteration < 1; iteration++) {
      for (int i = 0; i < features.Length; i++) {
        refPoints[i] = features[i].position;
        inPoints[i] = Constraints.ConstrainToSegment(features[i].position, aligningCamera.position, aligningCamera.TransformPoint(rayDirections[i] * 6f));
        Debug.DrawLine(aligningCamera.position, aligningCamera.TransformPoint(rayDirections[i] * 6f));
        Debug.DrawLine(features[i].position, inPoints[i], Color.red);
      }

      Matrix4x4 iterationStep = solver.SolveKabsch(inPoints, refPoints, true);
      aligningCamera.position += iterationStep.GetVector3();
      aligningCamera.rotation = iterationStep.GetQuaternion() * aligningCamera.rotation;
    }
  }
}