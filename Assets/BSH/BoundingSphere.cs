using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoundingSphere : MonoBehaviour {
  public Transform lastPoint;
  List<Vector3> randomPoints;
  Matrix4x4[] pointMatrices;
  public Material pointMaterial, sphereMaterial;
  public Mesh sphereMesh;
  Vector4 boundingSphere;

  // Start is called before the first frame update
  void Start() {
    if (randomPoints == null) randomPoints = new List<Vector3>(1023);
    pointMatrices = new Matrix4x4[1023];
    for (int i = 0; i < randomPoints.Capacity; i++) {
      randomPoints.Add(Vector3.Scale(new Vector3(0.25f, 0.5f, 1f), Random.insideUnitSphere));
      pointMatrices[i] = Matrix4x4.TRS(randomPoints[i], Quaternion.identity, Vector3.one * 0.01f);
    }

    boundingSphere = findMaximumSphere(randomPoints);
  }

  // Update is called once per frame
  void Update() {
    if (pointMatrices == null) Start();

    randomPoints [1022] = lastPoint.position;
    pointMatrices[1022] = Matrix4x4.TRS(randomPoints[1022], Quaternion.identity, Vector3.one * 0.01f);
    boundingSphere = findMaximumSphere(randomPoints);

    Graphics.DrawMeshInstanced(sphereMesh, 0, pointMaterial, pointMatrices);
    Graphics.DrawMesh         (sphereMesh, Matrix4x4.TRS(boundingSphere, Quaternion.identity, boundingSphere.w * Vector3.one), sphereMaterial, 0);

    // This doesn't seem to get triggered!
    foreach(Vector3 point in randomPoints) {
      if(Vector3.Distance(point, (Vector3)boundingSphere) > boundingSphere.w) {
        Debug.Log("SPHERE DOESN'T BOUND, POINT IS OUT BY: "+ (Vector3.Distance(point, (Vector3)boundingSphere) - boundingSphere.w));
      }
    }
  }

  public static Vector4 findMaximumSphere(List<Vector3> points) {
    // First find maximum bounding box for points and get an approximate sphere
    Bounds boundingBox = new Bounds(points[0], Vector3.zero);
    foreach (Vector3 point in points) boundingBox.Encapsulate(point);
    Vector4 curSphere = new Vector4(
      boundingBox.center.x, boundingBox.center.y, boundingBox.center.z, 
      Mathf.Max(boundingBox.extents.x, 
        Mathf.Max(boundingBox.extents.y, 
                    boundingBox.extents.z)));

    // Implement https://www.researchgate.net/publication/242453691_An_Efficient_Bounding_Sphere
    // Make Radius into Squared Radius
    float radiusSq = curSphere.w * curSphere.w;

    foreach (Vector3 point in points) {
      // Given x, y, z, cenx, ceny, cenz, rad, and rad_sq
      float old_to_p_sq = Vector3.SqrMagnitude(point - (Vector3)curSphere);

      // Do economical r**2 test before calc sqrt
      if (old_to_p_sq > radiusSq) {

        // Point is outside current sphere update
        float old_to_p = Mathf.Sqrt(old_to_p_sq);
        curSphere.w = (curSphere.w + old_to_p) / 2.0f;

        // Update square of radius for next compare
        radiusSq = curSphere.w * curSphere.w;

        float old_to_new = old_to_p - curSphere.w;
        curSphere.x = (curSphere.w * curSphere.x + old_to_new * point.x) / old_to_p;
        curSphere.y = (curSphere.w * curSphere.y + old_to_new * point.y) / old_to_p;
        curSphere.z = (curSphere.w * curSphere.z + old_to_new * point.z) / old_to_p;
      }
    }

    // A bit of floating-point leighway
    curSphere.w *= 1.0000001f;

    return curSphere;
  }
}
