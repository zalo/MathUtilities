using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class DragObject : MonoBehaviour {
  private Rigidbody2D body;
  bool dragging = false;
  Vector3 offset = Vector3.zero;

  void Start() {
    body = GetComponent<Rigidbody2D>();
  }

	void Update () {
    Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Input.GetMouseButtonDown(0)) {
      if (body.GetComponent<Collider2D>().OverlapPoint(mouseRay.origin)) {
        dragging = true;
        offset = transform.InverseTransformPoint(mouseRay.origin);
      }
    }

    if (!Input.GetMouseButton(0)){
      dragging = false;
    }

    if (dragging) {
      Vector3 worldPivot = transform.TransformPoint(offset);
      body.AddForceAtPosition(new Vector3(mouseRay.origin.x - worldPivot.x, mouseRay.origin.y - worldPivot.y,0f) / Time.fixedDeltaTime, worldPivot);
    }
  }
}
