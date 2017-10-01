using UnityEngine;

public class DistanceField : MonoBehaviour {
  public Material volumeMaterial;
  public Transform configurationVolume;
  [Range(-0.1f, 0.15f)]
  public float inflation = 0f;

  const int resolution = 64;
  Texture3D distanceSpace;
  Color32[] distances = new Color32[resolution * resolution * resolution];

  void Start() {
    distanceSpace = new Texture3D(resolution, resolution, resolution, TextureFormat.ARGB32, false);
    distanceSpace.wrapMode = TextureWrapMode.Clamp;
    if (volumeMaterial.mainTexture == null) { volumeMaterial.mainTexture = distanceSpace; }

    for (int x = 0; x < resolution; x++) {
      for (int y = 0; y < resolution; y++) {
        for (int z = 0; z < resolution; z++) {

          //Write to the configuration space texture array
          //The 0-1 range here encodes the -50cm to +50cm range of distance values as a float
          distances[((z * resolution * resolution) + (y * resolution) + x)] = encodeFloatRGBA((new Vector3((float)x / resolution, (float)y / resolution, (float)z / resolution) - (Vector3.one * 0.5f)).magnitude);
        }
      }
    }

    //Fill the distance space texture and apply it
    distanceSpace.SetPixels32(distances);
    distanceSpace.Apply();
  }

  void Update() {
    volumeMaterial.SetFloat("_Inflation", inflation);
  }

  //Encode a float to four bytes (without generating garbage) for writing to the Configuration space volume texture
  Color32 encodeFloatRGBA(float input) {
    float r = input;
    float g = input * 255;
    float b = input * 255 * 255;
    float a = input * 255 * 255 * 255;

    r = r - Mathf.Floor(r);
    g = g - Mathf.Floor(g);
    b = b - Mathf.Floor(b);
    a = a - Mathf.Floor(a);

    return new Color32((byte)(r*255), (byte)(g*255), (byte)(b*255), (byte)(a*255));
  }
}
