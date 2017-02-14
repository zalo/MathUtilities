using UnityEngine;

[RequireComponent(typeof(Camera))]
public class IllusionFollower : MonoBehaviour {
  public Camera mainCamera;
  public RectTransform illusionRect;
  public AnimationCurve CameraFallOff;
  Camera illusionCam;
  Vector3[] corners = new Vector3[4];
  void Start() {
    illusionCam = GetComponent<Camera>();
    illusionRect.GetWorldCorners(corners);
  }
	void LateUpdate () {
    transform.position = Vector3.Lerp(transform.position, mainCamera.transform.position, CameraFallOff.Evaluate(-mainCamera.transform.position.z));
    transform.rotation = Quaternion.Slerp(transform.rotation, mainCamera.transform.rotation, CameraFallOff.Evaluate(-mainCamera.transform.position.z));
    illusionRect.GetWorldCorners(corners);
    illusionCam.SetFrustum(corners, true);
  }
}
