using UnityEngine;

public class Tangent : MonoBehaviour {
  public Transform sphere;
  public float radius = 0.5f;

  //This led to the eventual construction of a closest point function for cone spheres
  //https://www.shadertoy.com/view/4l2cRW

	void OnDrawGizmos () {
    float distance = (Vector3.Distance(transform.position, sphere.position) / radius)-1.0f;
    Gizmos.DrawLine(transform.position, sphere.position);

    //Find tangent point by rotating a quaternion
    float tangentAngle = Mathf.Acos(1f / (1f + Mathf.Max(0f, distance)));
    Vector3 sphereSurface = ((transform.position - sphere.position).normalized * radius);

    Vector3 tangentPos = Quaternion.Euler(0f, 0f, tangentAngle * Mathf.Rad2Deg) * sphereSurface;
    Vector3 negativeTangentPos = Quaternion.Euler(0f, 0f, -tangentAngle * Mathf.Rad2Deg) * sphereSurface;

    Gizmos.DrawSphere(tangentPos, 0.05f);
    Gizmos.DrawLine(transform.position, tangentPos);
    Gizmos.DrawSphere(negativeTangentPos, 0.05f);
    Gizmos.DrawLine(transform.position, negativeTangentPos);
  }
}
