using UnityEngine;
//Quick visualization of the solid angle calculation: https://en.wikipedia.org/wiki/Spherical_trigonometry#Area_and_spherical_excess
public class SphericalTriangles : MonoBehaviour {
  public Transform[] vertices = new Transform[3];
  void OnDrawGizmos() {
    if (vertices.Length == 3) {
      Debug.Log("Sum of Interior Angles: " + transform.position.SphericalTriangleInteriorAngleSum(vertices[0].position, vertices[1].position, vertices[2].position, true) + 
        ", Area of Spherical Triangle/Solid Angle: " + transform.position.SolidAngleOfTriangle(vertices[0].position, vertices[1].position, vertices[2].position));
    }
  }
}

public static class SphericalTriangleExtension {
  public static float SphericalTriangleInteriorAngleSum(this Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3, bool drawGizmos = false) {
    //Step 1: Project the Triangle Vertices onto a Unit Sphere around the point
    v1 = (v1 - point).normalized;
    v2 = (v2 - point).normalized;
    v3 = (v3 - point).normalized;

    //Step 2: Calculate the spherical tangents between the vertices of the triangle
    Vector3 tangent01 = Vector3.ProjectOnPlane(v2 - v1, v1),
            tangent02 = Vector3.ProjectOnPlane(v3 - v1, v1),
            tangent10 = Vector3.ProjectOnPlane(v1 - v2, v2),
            tangent12 = Vector3.ProjectOnPlane(v3 - v2, v2),
            tangent21 = Vector3.ProjectOnPlane(v2 - v3, v3),
            tangent20 = Vector3.ProjectOnPlane(v1 - v3, v3);

    //Step 3: Sum the angles between the tangents
    float interiorAngle  = Vector3.Angle(tangent01, tangent02);
          interiorAngle += Vector3.Angle(tangent10, tangent12);
          interiorAngle += Vector3.Angle(tangent21, tangent20);

    if (drawGizmos) {
      for (float i = 0; i < 1.0; i += 0.01f) {
        Gizmos.DrawLine(
          Vector3.Lerp(v1, v2, i).normalized + point,
          Vector3.Lerp(v1, v2, i + 0.01f).normalized + point);
        Gizmos.DrawLine(          
          Vector3.Lerp(v2, v3, i).normalized + point,
          Vector3.Lerp(v2, v3, i + 0.01f).normalized + point);
        Gizmos.DrawLine(          
          Vector3.Lerp(v3, v1, i).normalized + point,
          Vector3.Lerp(v3, v1, i + 0.01f).normalized + point);
      }
      Gizmos.color = Color.red;
      Gizmos.DrawRay(v1 + point, tangent01.normalized * 0.4f);
      Gizmos.DrawRay(v1 + point, tangent02.normalized * 0.4f);
      Gizmos.DrawRay(v2 + point, tangent10.normalized * 0.4f);
      Gizmos.DrawRay(v2 + point, tangent12.normalized * 0.4f);
      Gizmos.DrawRay(v3 + point, tangent21.normalized * 0.4f);
      Gizmos.DrawRay(v3 + point, tangent20.normalized * 0.4f);
      Gizmos.color = Color.gray;
      Gizmos.DrawWireSphere(point, 1f);
    }

    return interiorAngle;
  }
  public static float SolidAngleOfTriangle(this Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3, bool drawGizmos = false) {
    return (point.SphericalTriangleInteriorAngleSum(v1, v2, v3, drawGizmos) - 180f) * Mathf.Deg2Rad;
  }
}
