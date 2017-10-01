using UnityEngine;
public class DistanceField : MonoBehaviour {
  public CustomRenderTexture blittingTexture;
  public Material volumeMaterial;
  public Material sphereBlittingMaterial;
  [Range(-0.1f, 0.15f)]
  public float inflation = 0f;

  private void Start() {
    blittingTexture.Initialize();
  }

  void Update() {
    Vector3 center = (Mathf.Sin(Time.time) * Vector3.right * 0.25f) + 
                     (Mathf.Cos(Time.time) * Vector3.up * 0.25f) + 
                     (Vector3.one * 0.5f);
    sphereBlittingMaterial.SetVector("_Center", center);

    volumeMaterial.SetFloat("_Inflation", inflation);
  }
}
