using UnityEngine;

public class StereographicCamera : MonoBehaviour {
  public Camera TopCamera, BottomCamera, LeftCamera, RightCamera;

  //Only need to set this once since they do not change relative to each other as the camera moves around
	void Start () {
    Shader.SetGlobalVector("_CameraPos", transform.position);
    Shader.SetGlobalMatrix("_CameraRot", transform.localToWorldMatrix);
    Shader.SetGlobalMatrix("_TopProjection", TopCamera.projectionMatrix * TopCamera.worldToCameraMatrix);
    Shader.SetGlobalMatrix("_BottomProjection", BottomCamera.projectionMatrix * BottomCamera.worldToCameraMatrix);
    Shader.SetGlobalMatrix("_LeftProjection", LeftCamera.projectionMatrix * LeftCamera.worldToCameraMatrix);
    Shader.SetGlobalMatrix("_RightProjection", RightCamera.projectionMatrix * RightCamera.worldToCameraMatrix);
  }

  void OnValidate() {
    Start();
  }

  void OnDrawGizmos() {
    for (float v = 0f; v < 1f; v += 0.05f) {
      for (float u = 0f; u < 1f; u += 0.99f) {
        Debug.DrawLine(transform.position, transform.TransformPoint(UVToStereographicRay(u, v)));
      }
    }
    for (float v = 0f; v < 1f; v += 0.99f) {
      for (float u = 0f; u < 1f; u += 0.05f) {
        Debug.DrawLine(transform.position, transform.TransformPoint(UVToStereographicRay(u, v)));
      }
    }
  }

  //Note that the Z-Axis here is negated from the standard formula
  //because we want to project forward along the z-axis (not backward)
  Vector3 UVToStereographicRay(float u, float v) {
    float x = (u * 2f) - 1f;
    float y = (v * 2f) - 1f;
    return new Vector3(
      (2f * x) / (1f + (x * x) + (y * y)),
      (2f * y) / (1f + (x * x) + (y * y)),
      -((-1f + (x * x) + (y * y)) / (1 + (x * x) + (y * y))));
  }
}