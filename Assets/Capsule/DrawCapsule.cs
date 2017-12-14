using UnityEngine;

public class DrawCapsule : MonoBehaviour {
  public Transform CapsuleStart;
  public Transform CapsuleEnd;
  public float RadiusStart = 1f, RadiusEnd = 1f; //Differing starting/ending radii is experimental
  public Mesh SphereMesh;
  public Material CapsuleMaterial;

  private Mesh capsuleMesh;
  private Vector3[] sphereVerts;
  private Vector3[] capsuleVerts;
  private Vector3[] capsuleNormals;
  private Vector3 midPoint;
  private float scale;
  private float prevRadiusStart = 1f, prevRadiusEnd = 1f;

  void Start() {
    capsuleMesh = Instantiate(SphereMesh);
    capsuleMesh.MarkDynamic();
    sphereVerts = SphereMesh.vertices;
    capsuleVerts = new Vector3[sphereVerts.Length];
    capsuleNormals = new Vector3[sphereVerts.Length];
  }

  void Update() {
    if (CapsuleStart.hasChanged || CapsuleEnd.hasChanged || RadiusStart != prevRadiusStart || RadiusEnd != prevRadiusEnd) {
      UpdateCapsuleMesh();
      prevRadiusStart = RadiusStart;
      prevRadiusEnd = RadiusEnd;
      CapsuleStart.hasChanged = false;
      CapsuleEnd.hasChanged = false;
    }

    Graphics.DrawMesh(capsuleMesh, Matrix4x4.TRS(midPoint, Quaternion.identity, Vector3.one * scale), CapsuleMaterial, 0);
  }

  void UpdateCapsuleMesh() {
    scale = ((CapsuleStart.position - CapsuleEnd.position).magnitude + (Mathf.Max(RadiusStart, RadiusEnd) * 10f)); //2f is minimum, but 10f gets better vertex distribution on the Caps
    midPoint = (CapsuleStart.position + CapsuleEnd.position) * 0.5f;

    float invScale = 1f / scale;
    Vector3 localStart = (CapsuleStart.position - midPoint) * invScale;
    Vector3 localEnd = (CapsuleEnd.position - midPoint) * invScale;

    for (int i = 0; i < sphereVerts.Length; i++) {
      float bicapsuleTime = 0f;
      capsuleVerts[i] = sphereVerts[i].closestPointToBiCapsule(localStart, localEnd, RadiusStart * invScale, RadiusEnd * invScale, out bicapsuleTime);
      capsuleNormals[i] = sphereVerts[i] - Vector3.Lerp(localStart, localEnd, bicapsuleTime);
    }

    capsuleMesh.vertices = capsuleVerts;
    capsuleMesh.normals = capsuleNormals;
    capsuleMesh.UploadMeshData(false);
  }
}
