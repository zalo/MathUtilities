using UnityEngine;

public class FramerateControl : MonoBehaviour {
  public bool limitFrameRate = true;
	void Start () {
    if (limitFrameRate) { Application.targetFrameRate = 60; } else { Application.targetFrameRate = -1; }
	}
	void OnDestroy () {
    if (limitFrameRate) { Application.targetFrameRate = -1; }
  }
}
