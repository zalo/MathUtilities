using UnityEngine;
//Quick visualization of the solid angle calculation: https://en.wikipedia.org/wiki/Spherical_trigonometry#Area_and_spherical_excess
public class SphericalTriangles : MonoBehaviour {
  public Transform[] vertices = new Transform[3];
  void OnDrawGizmos() {
    if (vertices.Length == 3) {
      Vector3[] triangleVertices = { vertices[0].position, vertices[1].position, vertices[2].position };
      float interiorAngle = transform.position.SphericalTriangleInteriorAngleSum(triangleVertices, true);
      Debug.Log("Sum of Interior Angles: " + transform.position.SphericalTriangleInteriorAngleSum(triangleVertices, true) + 
        ", Area of Spherical Triangle/Solid Angle: " + transform.position.SolidAngleOfTriangle(triangleVertices));
    }
  }
}

public static class SphericalTriangleExtension {
  public static float SphericalTriangleInteriorAngleSum(this Vector3 point, Vector3[] vertices, bool drawGizmos = false) {
    //Step 1: Project the Triangle Vertices onto a Unit Sphere around the point
    for (int i = 0; i < 3; i++) {
      vertices[i] = vertices[i].ConstrainDistance(point, 1f);
    }

    //Step 2: Calculate the spherical tangents between the vertices of the triangle
    Vector3 tangent01 = Vector3.ProjectOnPlane(vertices[1] - vertices[0], vertices[0] - point),
            tangent02 = Vector3.ProjectOnPlane(vertices[2] - vertices[0], vertices[0] - point),
            tangent10 = Vector3.ProjectOnPlane(vertices[0] - vertices[1], vertices[1] - point),
            tangent12 = Vector3.ProjectOnPlane(vertices[2] - vertices[1], vertices[1] - point),
            tangent21 = Vector3.ProjectOnPlane(vertices[1] - vertices[2], vertices[2] - point),
            tangent20 = Vector3.ProjectOnPlane(vertices[0] - vertices[2], vertices[2] - point);

    //Step 3: Sum the angles between the tangents
    float interiorAngle  = Vector3.Angle(tangent01, tangent02);
          interiorAngle += Vector3.Angle(tangent10, tangent12);
          interiorAngle += Vector3.Angle(tangent21, tangent20);

    if (drawGizmos) {
      for (float i = 0; i < 1.0; i += 0.01f) {
        Gizmos.DrawLine(
          Vector3.Lerp(vertices[0], vertices[1], i).ConstrainDistance(point, 1f),
          Vector3.Lerp(vertices[0], vertices[1], i + 0.01f).ConstrainDistance(point, 1f));
        Gizmos.DrawLine(
          Vector3.Lerp(vertices[1], vertices[2], i).ConstrainDistance(point, 1f),
          Vector3.Lerp(vertices[1], vertices[2], i + 0.01f).ConstrainDistance(point, 1f));
        Gizmos.DrawLine(
          Vector3.Lerp(vertices[2], vertices[0], i).ConstrainDistance(point, 1f),
          Vector3.Lerp(vertices[2], vertices[0], i + 0.01f).ConstrainDistance(point, 1f));
      }
      Gizmos.color = Color.red;
      Gizmos.DrawRay(vertices[0], tangent01.normalized * 0.4f);
      Gizmos.DrawRay(vertices[0], tangent02.normalized * 0.4f);
      Gizmos.DrawRay(vertices[1], tangent10.normalized * 0.4f);
      Gizmos.DrawRay(vertices[1], tangent12.normalized * 0.4f);
      Gizmos.DrawRay(vertices[2], tangent21.normalized * 0.4f);
      Gizmos.DrawRay(vertices[2], tangent20.normalized * 0.4f);
      Gizmos.color = Color.gray;
      Gizmos.DrawWireSphere(point, 1f);
    }

    return interiorAngle;
  }
  public static float SolidAngleOfTriangle(this Vector3 point, Vector3[] vertices, bool drawGizmos = false) {
    return (point.SphericalTriangleInteriorAngleSum(vertices, drawGizmos) - 180f) * Mathf.Deg2Rad;
  }
}
