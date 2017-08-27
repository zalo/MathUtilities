using UnityEngine;
using UnityEngine.Profiling;

public class StereoBundleAdjuster : MonoBehaviour {
  public Transform[] features;

  public Transform camera11;
  Vector3[] rayDirections11;
  public Transform camera12;
  Vector3[] rayDirections12;

  public Transform camera21;
  Vector3[] rayDirections21;
  public Transform camera22;
  Vector3[] rayDirections22;

  KabschSolver solver = new KabschSolver();
  Vector3[] inPoints, refPoints;

  void Start() {
    rayDirections11 = new Vector3[features.Length];
    rayDirections12 = new Vector3[features.Length];
    rayDirections21 = new Vector3[features.Length];
    rayDirections22 = new Vector3[features.Length];

    inPoints = new Vector3[features.Length*4];
    refPoints = new Vector3[features.Length*4];

    for (int i = 0; i < features.Length; i++) {
      rayDirections11[i] = camera11.InverseTransformPoint(features[i].position).normalized;
      rayDirections12[i] = camera12.InverseTransformPoint(features[i].position).normalized;
      rayDirections21[i] = camera21.InverseTransformPoint(features[i].position).normalized;
      rayDirections22[i] = camera22.InverseTransformPoint(features[i].position).normalized;
    }
  }

  void Update() {
    for (int i = 0; i < features.Length; i++) {
      Debug.DrawLine(camera11.position, camera11.TransformPoint(rayDirections11[i]));
      Debug.DrawLine(camera12.position, camera12.TransformPoint(rayDirections12[i]));
      Debug.DrawLine(camera21.position, camera21.TransformPoint(rayDirections21[i]));
      Debug.DrawLine(camera22.position, camera22.TransformPoint(rayDirections22[i]));
    }

    Profiler.BeginSample("Bundle Adjustment");
    for (int iteration = 0; iteration < 1; iteration++) {
      for (int i = 0; i < features.Length; i++) {
        Vector3 pointLineOne, pointLineTwo;
        Displacement(camera11.position, camera11.TransformPoint(rayDirections11[i]), camera21.position, camera21.TransformPoint(rayDirections21[i]), out pointLineOne, out pointLineTwo);
        inPoints[(i * 4)] = pointLineOne; refPoints[(i * 4)] = pointLineTwo;
        Debug.DrawLine(pointLineOne, pointLineTwo, Color.red);

        Displacement(camera11.position, camera11.TransformPoint(rayDirections11[i]), camera22.position, camera22.TransformPoint(rayDirections22[i]), out pointLineOne, out pointLineTwo);
        inPoints[(i * 4) + 1] = pointLineOne; refPoints[(i * 4) + 1] = pointLineTwo;
        Debug.DrawLine(pointLineOne, pointLineTwo, Color.red);

        Displacement(camera12.position, camera12.TransformPoint(rayDirections12[i]), camera21.position, camera21.TransformPoint(rayDirections21[i]), out pointLineOne, out pointLineTwo);
        inPoints[(i * 4) + 2] = pointLineOne; refPoints[(i * 4) + 2] = pointLineTwo;
        Debug.DrawLine(pointLineOne, pointLineTwo, Color.red);

        Displacement(camera12.position, camera12.TransformPoint(rayDirections12[i]), camera22.position, camera22.TransformPoint(rayDirections22[i]), out pointLineOne, out pointLineTwo);
        inPoints[(i * 4) + 3] = pointLineOne; refPoints[(i * 4) + 3] = pointLineTwo;
        Debug.DrawLine(pointLineOne, pointLineTwo, Color.red);
      }

      Matrix4x4 iterationStep = solver.SolveKabsch(refPoints, inPoints, true);
      camera21.parent.position += iterationStep.GetVector3();
      camera21.parent.rotation = iterationStep.GetQuaternion() * camera21.parent.rotation;
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