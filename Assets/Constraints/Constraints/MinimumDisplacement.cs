using UnityEngine;

public class MinimumDisplacement : MonoBehaviour {
  public Transform anchorObject;
  public Vector3 localAnchor;
  public Vector3 connectedAnchor;
  public bool autoConfigureConnectedAnchor = true;

  void Start() {
    if (autoConfigureConnectedAnchor) {
      connectedAnchor = anchorObject.InverseTransformPoint(transform.TransformPoint(localAnchor));
    }
  }

  void Update () {
    transform.ConstrainToPoint(transform.TransformPoint(localAnchor), anchorObject.TransformPoint(connectedAnchor));
  }
}