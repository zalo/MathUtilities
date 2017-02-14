using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class MinimalFPSController : MonoBehaviour {
  CharacterController controller;
  void Start() {
    controller = GetComponent<CharacterController>();
  }
  void Update() {
    Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    transform.Rotate(0, Input.GetAxis("Mouse X") * 3f, 0);
    Camera.main.transform.localRotation *= Quaternion.Euler(-Input.GetAxis("Mouse Y") * 3f, 0, 0);
    controller.Move(transform.rotation * movement * Time.deltaTime * 3f);
  }
}