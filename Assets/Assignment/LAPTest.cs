using System.Collections.Generic;
using UnityEngine;

public class LAPTest : MonoBehaviour {
  public int greenPointCount = 14;
  public int redPointCount = 14;
  List<Vector3> pointsA = new List<Vector3>(14);
  List<Vector3> pointsB = new List<Vector3>(14);
  int[] matches;

  void OnValidate() {
    pointsA.Clear();
    pointsB.Clear();

    for (int i = 0; i < greenPointCount; i++) {
      pointsA.Add(Random.insideUnitSphere * 2f);
    }

    for (int i = 0; i < redPointCount; i++) {
      pointsB.Add(Random.insideUnitSphere * 2f);
    }

    float[,] distanceMatrix = new float[pointsA.Count, pointsB.Count];

    for (int i = 0; i < pointsA.Count; i++) {
      for (int j = 0; j < pointsB.Count; j++) {
        distanceMatrix[i, j] = i != j ? Vector3.Distance(pointsA[i], pointsB[j]) : float.PositiveInfinity;
      }
    }

    //Debug.Log("Total Distance: " + 
    distanceMatrix.lap(out matches)
    //)
    ;
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.green;
    for (int i = 0; i < pointsA.Count; i++) {
      Gizmos.DrawSphere(pointsA[i], 0.05f);
    }

    Gizmos.color = Color.red;
    for (int i = 0; i < pointsB.Count; i++) {
      Gizmos.DrawSphere(pointsB[i], 0.05f);
    }

    Gizmos.color = Color.white;
    if (pointsA.Count <= pointsB.Count) {
      for (int i = 0; i < pointsA.Count; i++) {
        Gizmos.DrawLine(pointsA[i], pointsB[matches[i]]);
      }
    } else {
      for (int i = 0; i < pointsB.Count; i++) {
        Gizmos.DrawLine(pointsB[i], pointsA[matches[i]]);
      }
    }
  }
}
