using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
  public int nodeIndex;
  public ReorderableList tree;
  public TextMesh text;

  public bool dragging = false;
  Vector3 offset = Vector3.zero;

  void Start() {}

  void OnMouseDown() {
    offset = transform.position - Camera.main.ScreenPointToRay(Input.mousePosition).origin;
  }

  void OnMouseDrag() {
    Vector3 curOffset = transform.position - Camera.main.ScreenPointToRay(Input.mousePosition).origin;
    if (!dragging && offset != curOffset) dragging = true;
    if (dragging) {
      transform.position = Camera.main.ScreenPointToRay(Input.mousePosition).origin + offset;
      tree.reevaluateNodeOrder(this);

      //Transform socket = tree.sockets[tree.nodes.IndexOf(this)];
      //if (socket != null) {
      //  transform.position += (socket.position - transform.position) * 0.7f;
      //}
    }
  }

  void OnMouseUp() {
    if (dragging) {
      dragging = false;
    } else {
      //Activate button
    }
  }

  // Update is called once per frame
  void Update() {
    Transform socket = tree.sockets[tree.nodes.IndexOf(this)];
    if (socket != null) {
      if (!dragging) {
        transform.position += (socket.position - transform.position) * 0.1f;
      }
      transform.rotation = Quaternion.Slerp(transform.rotation, socket.rotation, 0.1f);
    }
  }
}
