using UnityEngine;
using UnityEngine.Profiling;

public class StereoBundleAdjuster : MonoBehaviour {
  public Transform[] features;

  public Transform[] cameras;
  Vector3[][] rayDirections;

  KabschSolver solver = new KabschSolver();
  Vector3[] inPoints, refPoints;

  void Start() {
    inPoints = new Vector3[features.Length * 4];
    refPoints = new Vector3[features.Length * 4];
    rayDirections = new Vector3[4][];

    for (int cameraIndex = 0; cameraIndex < 4; cameraIndex++) {
      rayDirections[cameraIndex] = new Vector3[features.Length];

      for (int i = 0; i < features.Length; i++) {
        rayDirections[cameraIndex][i] = cameras[cameraIndex].InverseTransformPoint(features[i].position).normalized;
      }
    }
  }

  void Update() {
    for (int cameraIndex = 0; cameraIndex < 4; cameraIndex++) {
      for (int i = 0; i < features.Length; i++) {
        Debug.DrawLine(cameras[cameraIndex].position, cameras[cameraIndex].TransformPoint(rayDirections[cameraIndex][i]));
      }
    }

    Profiler.BeginSample("Bundle Adjustment");
    for (int iteration = 0; iteration < 1; iteration++) {
      for (int i = 0; i < features.Length; i++) {
        for (int cameraIndex = 0; cameraIndex < 4; cameraIndex++) {
          Vector3 pointLineOne, pointLineTwo;
          Displacement(cameras[(cameraIndex / 2)    ].position, cameras[(cameraIndex / 2)    ].TransformPoint(rayDirections[(cameraIndex / 2)    ][i]),
                       cameras[(cameraIndex % 2) + 2].position, cameras[(cameraIndex % 2) + 2].TransformPoint(rayDirections[(cameraIndex % 2) + 2][i]), out pointLineOne, out pointLineTwo);

          inPoints [(i * cameras.Length) + cameraIndex] = pointLineOne;
          refPoints[(i * cameras.Length) + cameraIndex] = pointLineTwo;

          Debug.DrawLine(pointLineOne, pointLineTwo, Color.red);
        }
      }

      Matrix4x4 iterationStep = solver.SolveKabsch(refPoints, inPoints, true);
      cameras[2].parent.position += iterationStep.GetVector3();
      cameras[2].parent.rotation = iterationStep.GetQuaternion() * cameras[2].parent.rotation;
    }
    Profiler.EndSample();
  }

  public void Displacement(Vector3 camOneOrigin, Vector3 camOneDirection, Vector3 camTwoOrigin, Vector3 camTwoDirection, out Vector3 pointLineOne, out Vector3 pointLineTwo) {
    float timeLineOne, timeLineTwo;
    BundleAdjuster.line2lineDisplacement(camOneOrigin, camOneDirection, camTwoOrigin, camTwoDirection, out timeLineOne, out timeLineTwo);

    //Take the abs of the times so they can't intersect behind the camera
    timeLineOne = Mathf.Abs(timeLineOne); timeLineTwo = Mathf.Abs(timeLineTwo);

    pointLineOne = Vector3.LerpUnclamped(camOneOrigin, camOneDirection, timeLineOne);
    pointLineTwo = Vector3.LerpUnclamped(camTwoOrigin, camTwoDirection, timeLineTwo);
  }
}