using UnityEngine;

public class Intersection : MonoBehaviour {
  [Range(0, 100)]
  public int iterations = 15;
  public Collider object1, object2;
  [Tooltip("Different algorithms to find an intersecting point")]
  public IntersectionMethod method;

  //https://en.wikipedia.org/wiki/Projections_onto_convex_sets
  //https://en.wikipedia.org/wiki/Dykstra%27s_projection_algorithm
  public enum IntersectionMethod {
    AlternatingProjections, //Fast
    AverageProjections,     //Robust
    DykstrasProjections     //Correct
  };

  private void OnDrawGizmos() {
    Vector3 point = transform.position;
    switch (method) {
      case IntersectionMethod.AlternatingProjections:
        for(int i = 0; i < iterations; i++) {
          Gizmos.DrawLine(point, point = object1.ClosestPoint(point));
          Gizmos.DrawLine(point, point = object2.ClosestPoint(point));
        }
        break;
      case IntersectionMethod.AverageProjections:
        for (int i = 0; i < iterations; i++) {
          Vector3 obj1Point = object1.ClosestPoint(point),
                  obj2Point = object2.ClosestPoint(point);
          Gizmos.DrawLine(point, point = (obj1Point + obj2Point) / 2f);
        }
        break;
      case IntersectionMethod.DykstrasProjections:
        Vector3 P = Vector3.zero, Q = Vector3.zero;
        for (int i = 0; i < iterations; i++) {
          Vector3 Y = object1.ClosestPoint(point + P);
          Gizmos.DrawLine(point, Y);
          P = point + P - Y;
          point = object2.ClosestPoint(Y + Q);
          Gizmos.DrawLine(Y, point);
          Q = Y + Q - point;
        }
        break;
    }
  }
}
