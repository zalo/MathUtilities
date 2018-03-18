using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AverageRot : MonoBehaviour {
  public Transform[] otherPoints;
  Vector3[] displacements;


	// Use this for initialization
	void Start () {
    displacements = new Vector3[otherPoints.Length];
    for(int i = 0; i < otherPoints.Length; i++) {
      displacements[i] = otherPoints[i].position - transform.position;
    }
  }
	
	// Update is called once per frame
	void Update () {
    Vector3 rotation = Vector3.zero; int activePoints = 0;
    for (int i = 0; i < otherPoints.Length; i++) {
      if (otherPoints[i].gameObject.activeSelf) {
        Quaternion rot = Quaternion.FromToRotation(displacements[i], otherPoints[i].position - transform.position);
        Vector3 axis; float angle;
        rot.ToAngleAxis(out angle, out axis);
        rotation += axis.normalized * angle;
        activePoints++;
      }
    }
    rotation /= activePoints;

    transform.rotation = Quaternion.AngleAxis(rotation.magnitude, rotation.normalized);
  }
}
