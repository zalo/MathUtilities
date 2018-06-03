using UnityEngine;
public class DistanceField : MonoBehaviour {
  public CustomRenderTexture blittingTexture;
  public Material volumeMaterial;
  public Material sphereBlittingMaterial;
  [Range(-0.1f, 0.15f)]
  public float inflation = 0f;
  [Range(0f, 6.28f)]
  public float rotation = 0f;
  public bool subtraction = false;
  [Range(0.0f, 0.3f)]
  public float radius = 0.05f;
  public bool repairDistanceField = false;
  public bool usePlanarRepair = false;

  bool parametersUpdatedThisFrame = false;
  private void Start() {
    blittingTexture.Initialize();
  }

  void OnValidate() {
    parametersUpdatedThisFrame = true;
  }

  void Update() {
    if (parametersUpdatedThisFrame) {
      blittingTexture.shaderPass = 0;
      Vector3 center = (Mathf.Sin(rotation) * Vector3.right * 0.25f) +
                       (Mathf.Cos(rotation) * Vector3.up * 0.25f) +
                       (Vector3.one * 0.5f);
      sphereBlittingMaterial.SetVector("_Center", center);
      sphereBlittingMaterial.SetInt("_Subtraction", subtraction ? 1 : 0);
      sphereBlittingMaterial.SetFloat("_Radius", radius);
      volumeMaterial.SetFloat("_Inflation", inflation);
      blittingTexture.Update();
    } else {
      //Else just run the distance field healing shader passively
      blittingTexture.shaderPass = usePlanarRepair ? 3 : 2;
      if (repairDistanceField) { blittingTexture.Update(); }
    }
    parametersUpdatedThisFrame = false;
  }
}
