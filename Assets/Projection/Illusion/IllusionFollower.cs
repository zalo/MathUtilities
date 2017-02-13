using UnityEngine;

[RequireComponent(typeof(Camera))]
public class IllusionFollower : MonoBehaviour {
  public Camera mainCamera;
  public AnimationCurve CameraFallOff;
  Camera illusionCam;
  void Start() {
    illusionCam = GetComponent<Camera>();
    illusionCam.fieldOfView = mainCamera.fieldOfView + 60f;
  }
	void LateUpdate () {
    transform.position = Vector3.Lerp(transform.position, mainCamera.transform.position, CameraFallOff.Evaluate(-mainCamera.transform.position.z));
    transform.rotation = Quaternion.Slerp(transform.rotation, mainCamera.transform.rotation, CameraFallOff.Evaluate(-mainCamera.transform.position.z));
  }
}
