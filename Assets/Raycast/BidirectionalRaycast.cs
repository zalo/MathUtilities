using System.Collections.Generic;
using UnityEngine;

public class BidirectionalRaycast : MonoBehaviour {
  [Range(0f,0.1f)]
  public float radius = 0f;
  public Transform rayDestination;

  RaycastHit[]     entries       = new RaycastHit[8],
                   exits         = new RaycastHit[8];
  List<RaycastHit> intersections = new List<RaycastHit>(16);

  void OnDrawGizmos() {
    if (rayDestination != null) {
      Vector3 direction = rayDestination.position - transform.position;
      BidirectionalRaycastNonAlloc(transform.position, radius, direction, Vector3.Distance(transform.position, rayDestination.position), ref entries, ref exits, ref intersections);

      Gizmos.DrawLine(transform.position, rayDestination.position);

      for (int i = 0; i < intersections.Count; i++) {
        Gizmos.color = Vector3.Dot(intersections[i].normal, direction) < 0f ? Color.green : Color.red;

        Gizmos.DrawSphere(intersections[i].point + (intersections[i].normal * radius), radius);
        Gizmos.DrawLine(intersections[i].point, intersections[i].point + (intersections[i].normal * 0.25f));
      }
    }
  }

  public void BidirectionalRaycastNonAlloc(Vector3 origin, float radius, Vector3 direction, float length, ref RaycastHit[] entries, ref RaycastHit[] exits, ref List<RaycastHit> hits) {
    hits.Clear();
    int hitNumber1, hitNumber2;
    direction.Normalize();
    if (radius <= 0f) {
      hitNumber1 = Physics.RaycastNonAlloc(origin, direction, entries, length);
      hitNumber2 = Physics.RaycastNonAlloc(origin + (direction * length), -direction, exits, length);
    } else {
      hitNumber1 = Physics.SphereCastNonAlloc(origin, radius, direction, entries, length);
      hitNumber2 = Physics.SphereCastNonAlloc(origin + (direction * length), radius, -direction, exits, length);
    }

    for (int i = 0; i < Mathf.Min(hitNumber1, entries.Length); i++) {
      hits.Add(entries[i]);
    }

    for (int i = 0; i < Mathf.Min(hitNumber2, exits.Length); i++) {
      exits[i].distance = length - exits[i].distance;
      hits.Add(exits[i]);
    }

    hits.Sort((x, y) => x.distance.CompareTo(y.distance));
  }
}
