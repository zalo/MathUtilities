using UnityEngine;

public class StereoBundleAdjuster : MonoBehaviour {
  public bool adjustPosition = true;
  public bool adjustRotation = false;

  public Transform[] features;

  public Transform camera11;
  Vector3[] rayDirections11;
  public Transform camera12;
  Vector3[] rayDirections12;

  public Transform camera21;
  Vector3[] rayDirections21;
  public Transform camera22;
  Vector3[] rayDirections22;

  Vector3 currentDeltaPos;
  Vector4 currentDeltaRot;
  void Start() {
    rayDirections11 = new Vector3[features.Length];
    rayDirections12 = new Vector3[features.Length];
    rayDirections21 = new Vector3[features.Length];
    rayDirections22 = new Vector3[features.Length];

    for (int i = 0; i < features.Length; i++) {
      rayDirections11[i] = camera11.InverseTransformPoint(features[i].position).normalized;
      rayDirections12[i] = camera12.InverseTransformPoint(features[i].position).normalized;
      rayDirections21[i] = camera21.InverseTransformPoint(features[i].position).normalized;
      rayDirections22[i] = camera22.InverseTransformPoint(features[i].position).normalized;
    }
  }

  void Update() {
    currentDeltaPos = Vector3.zero;
    currentDeltaRot = Vector4.zero;
    for (int i = 0; i < features.Length; i++) {
      Debug.DrawLine(camera11.position, camera11.TransformPoint(rayDirections11[i]));
      Debug.DrawLine(camera12.position, camera12.TransformPoint(rayDirections12[i]));
      Debug.DrawLine(camera21.position, camera21.TransformPoint(rayDirections21[i]));
      Debug.DrawLine(camera22.position, camera22.TransformPoint(rayDirections22[i]));

      Displacement(camera21.parent.position, camera11.position, camera11.TransformPoint(rayDirections11[i]), camera21.position, camera21.TransformPoint(rayDirections21[i]), ref currentDeltaPos, ref currentDeltaRot);
      Displacement(camera21.parent.position, camera11.position, camera11.TransformPoint(rayDirections11[i]), camera22.position, camera22.TransformPoint(rayDirections22[i]), ref currentDeltaPos, ref currentDeltaRot);
      Displacement(camera21.parent.position, camera12.position, camera12.TransformPoint(rayDirections12[i]), camera21.position, camera21.TransformPoint(rayDirections21[i]), ref currentDeltaPos, ref currentDeltaRot);
      Displacement(camera21.parent.position, camera12.position, camera12.TransformPoint(rayDirections12[i]), camera22.position, camera22.TransformPoint(rayDirections22[i]), ref currentDeltaPos, ref currentDeltaRot);
    }

    camera21.parent.position -= (currentDeltaPos / (features.Length*4f));

    currentDeltaRot = (currentDeltaRot / (features.Length * 4f));
    Quaternion rot = new Quaternion(currentDeltaRot.x, currentDeltaRot.y, currentDeltaRot.z, currentDeltaRot.w);

    camera21.parent.rotation = rot * camera21.parent.rotation;
  }

  public void Displacement(Vector3 parentOrigin, Vector3 camOneOrigin, Vector3 camOneDirection, Vector3 camTwoOrigin, Vector3 camTwoDirection, ref Vector3 DisplacementSum, ref Vector4 QuaternionSum) {
    float timeLineOne, timeLineTwo;
    BundleAdjuster.line2lineDisplacement(camOneOrigin, camOneDirection, camTwoOrigin, camTwoDirection, out timeLineOne, out timeLineTwo);

    //Take the abs of the times so they can't intersect behind the camera
    timeLineOne = Mathf.Abs(timeLineOne); timeLineTwo = Mathf.Abs(timeLineTwo);

    Vector3 pointLineOne = Vector3.LerpUnclamped(camOneOrigin, camOneDirection, timeLineOne);
    Vector3 pointLineTwo = Vector3.LerpUnclamped(camTwoOrigin, camTwoDirection, timeLineTwo);

    Debug.DrawLine(pointLineOne, pointLineTwo, Color.red);
    if (adjustPosition) {
      DisplacementSum += (pointLineTwo - pointLineOne) / 2f;
    }

    Vector3 oldDisplacement = parentOrigin - pointLineTwo;
    Vector3 newDisplacement = parentOrigin - pointLineOne;
    Quaternion rotDis = Quaternion.FromToRotation(oldDisplacement, newDisplacement);
    if (adjustRotation) {
      QuaternionSum += new Vector4(rotDis.x, rotDis.y, rotDis.z, rotDis.w);
    }
  }
}