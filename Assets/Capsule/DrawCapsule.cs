using UnityEngine;

public class DrawCapsule : MonoBehaviour {
  public Transform CapsuleStart;
  public Transform CapsuleEnd;
  public float Radius = 1f;
  public Mesh SphereMesh;
  public Material CapsuleMaterial;

  private Mesh capsuleMesh;
  private Vector3[] sphereVerts;
  private Vector3[] capsuleVerts;
  private Vector3[] capsuleNormals;
  private Vector3 midPoint;
  private float scale;
  private float prevRadius = 1f;

  void Start() {
    capsuleMesh = Instantiate(SphereMesh);
    capsuleMesh.MarkDynamic();
    sphereVerts = SphereMesh.vertices;
    capsuleVerts = new Vector3[sphereVerts.Length];
    capsuleNormals = new Vector3[sphereVerts.Length];
  }

  void Update() {
    if (CapsuleStart.hasChanged || CapsuleEnd.hasChanged || Radius != prevRadius) {
      UpdateCapsuleMesh();
      prevRadius = Radius;
      CapsuleStart.hasChanged = false;
      CapsuleEnd.hasChanged = false;
    }

    Graphics.DrawMesh(capsuleMesh, Matrix4x4.TRS(midPoint, Quaternion.identity, Vector3.one * scale), CapsuleMaterial, 0);
  }

  void UpdateCapsuleMesh() {
    scale = ((CapsuleStart.position - CapsuleEnd.position).magnitude + (Radius * 10f)); //2f is minimum, but 10f gets better vertex distribution on the Caps
    midPoint = (CapsuleStart.position + CapsuleEnd.position) * 0.5f;

    float invScale = 1f / scale;
    Vector3 localStart = (CapsuleStart.position - midPoint) * invScale;
    Vector3 localEnd = (CapsuleEnd.position - midPoint) * invScale;
    float inverseLength = 1f / ((localStart - localEnd).magnitude);

    for (int i = 0; i < sphereVerts.Length; i++) {
      Vector3 nearestPointOnSegment = sphereVerts[i].ConstrainToSegment(localStart, localEnd, inverseLength);
      capsuleVerts[i] = sphereVerts[i].ConstrainDistance(nearestPointOnSegment, Radius * invScale);
      capsuleNormals[i] = sphereVerts[i] - nearestPointOnSegment;
    }

    capsuleMesh.vertices = capsuleVerts;
    capsuleMesh.normals = capsuleNormals;
    capsuleMesh.UploadMeshData(false);
  }
}
