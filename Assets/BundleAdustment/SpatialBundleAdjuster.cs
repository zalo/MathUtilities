using UnityEngine;

public class SpatialBundleAdjuster : MonoBehaviour {
  public Transform[] features;
  public Transform aligningCamera;
  public bool useLinearEstimate = true;
  Vector3[] rayDirections;

  KabschSolver solver = new KabschSolver();
  Vector3[] inPoints, refPoints;
  float previousCost = 1000f;
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
    solver = new KabschSolver();

    for (int iteration = 0; iteration < 2; iteration++) {
      float currentCost = 0f;

      for (int i = 0; i < features.Length; i++) {
        refPoints[i] = features[i].position;
        inPoints[i] = Constraints.ConstrainToSegment(features[i].position, aligningCamera.position, aligningCamera.TransformPoint(rayDirections[i]*6f));
        currentCost += Vector3.Distance(refPoints[i], inPoints[i]);
        Debug.DrawLine(aligningCamera.position, aligningCamera.TransformPoint(rayDirections[i]));
        Debug.DrawLine(features[i].position, inPoints[i], Color.red);
      }

      Matrix4x4 iterationStep = solver.SolveKabsch(inPoints, refPoints, true);
      Matrix4x4 steppedTransform = iterationStep * aligningCamera.localToWorldMatrix;
      if (useLinearEstimate) {
        float stepScale = ((previousCost - currentCost) > 0.0000001f && iteration > 0) ? (previousCost / (previousCost - currentCost)) * 0.8f : 1f;
        aligningCamera.position += (steppedTransform.GetVector3() - aligningCamera.position) * stepScale;
        aligningCamera.rotation = Quaternion.SlerpUnclamped(Quaternion.identity, iterationStep.GetQuaternion(), stepScale) * aligningCamera.rotation;
      } else {
        aligningCamera.position = steppedTransform.GetVector3();
        aligningCamera.rotation = steppedTransform.GetQuaternion();
      }

      previousCost = currentCost;
    }
  }
}
