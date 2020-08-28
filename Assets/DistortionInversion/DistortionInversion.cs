using UnityEngine;
using System.Collections.Generic;

public class DistortionInversion : MonoBehaviour {
  public MeshFilter meshPreview, texturePreview;
  public Material   uvMaterial;

  protected DistortionInverter inverter;

  private void Start() {
    inverter = new DistortionInverter(uvMaterial);

    // Set up the previews
    meshPreview   .sharedMesh = inverter.distortionMesh;
    meshPreview   .GetComponent<MeshRenderer>().material = uvMaterial;
    texturePreview.GetComponent<MeshRenderer>().material.mainTexture = inverter.distortedTexture;
  }

  private void Update() {
    // An example distortion function that we'd like to bake inverted to a texture
    inverter.updateDistortion((Vector2 uv) => {
      Vector2 center    = Vector2.one * 0.5f;
      Vector2 centered  = uv - center;
      Vector2 distorted = centered * (0.5f + (centered.magnitude * centered.magnitude * Mathf.Sin(Time.time)));
      return  distorted + center;
    });
  }
}
