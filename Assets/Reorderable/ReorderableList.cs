using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReorderableList : MonoBehaviour {
  public Transform[] sockets;
  public GameObject nodePrefab;

  public List<Node> nodes = new List<Node>();

  void Start() {
    foreach(Transform socket in sockets) {
      createNode(socket);
    }
  }

  public void reevaluateNodeOrder(Node movingNode) {
    int closestSocket = -1;
    float minDist = 10000f;
    for(int i = 0; i < sockets.Length; i++) {
      float curDist = Vector3.Distance(movingNode.transform.position, sockets[i].position);
      if (curDist < minDist) {
        minDist = curDist;
        closestSocket = i;
      }
    }

    if(closestSocket != nodes.IndexOf(movingNode)) {
      nodes.Remove(movingNode);
      nodes.Insert(closestSocket, movingNode);
    }
  }

  Node createNode(Transform socket) {
    GameObject prefab = Instantiate(nodePrefab, socket.position, socket.rotation);
    Node node = prefab.GetComponent<Node>();
    node.tree = this;
    node.text.text = "Node: "+socket.name;
    nodes.Add(node);
    return node;
  }
}
