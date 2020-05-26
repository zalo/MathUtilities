using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Pulses : MonoBehaviour {
  public Mesh sphere;
  public Material wireMaterial;
  public Material fieldMaterial;

  public float speed = -0.2f;
  public bool constantCurrent = false;
  public float numberOfWinds = 20f;
  [Range(0f, 35f)]
  public float frequencyOverTotalLength = 100f;

  const int ELEMENTS = 256;

  Matrix4x4[] matrices    = new Matrix4x4[ELEMENTS];
  Vector4  [] wirePoints  = new Vector4  [ELEMENTS];
  float    [] wireCurrent = new float    [ELEMENTS];

  // Update is called once per frame
  void Update() {
    if(matrices.Length != ELEMENTS) {
      matrices    = new Matrix4x4[ELEMENTS];
      wirePoints  = new Vector4  [ELEMENTS];
      wireCurrent = new float    [ELEMENTS];
    }

    for(int i = 0; i < ELEMENTS; i++) {
      Vector4 torusPoint1 = calculateTorusPoint(i    );
      Vector4 torusPoint2 = calculateTorusPoint(i + 1);
      Quaternion rot = Quaternion.FromToRotation(Vector3.up, torusPoint2 - torusPoint1);
      float wireThickness = 0.025f * (1.35f + torusPoint1.w);
      matrices   [i] = Matrix4x4.TRS((torusPoint1+ torusPoint2)*0.5f, rot, new Vector3(wireThickness, Vector3.Distance(torusPoint2, torusPoint1)*0.5f, wireThickness));
      wirePoints [i] = torusPoint1 * 0.25f;
      wireCurrent[i] = torusPoint1.w;
    }

    Graphics.DrawMeshInstanced(sphere, 0, wireMaterial, matrices);

    fieldMaterial.SetVectorArray("_WirePoints" , wirePoints);
    fieldMaterial.SetFloatArray ("_WireCurrent", wireCurrent);
  }

  Vector4 calculateTorusPoint(int i) {
    float alpha = 6.28318530718f * (i / (float)ELEMENTS);
    float voltage = Mathf.Sin(((alpha + (Time.time * speed)) * (constantCurrent ? frequencyOverTotalLength : numberOfWinds)));
    //alpha += (Time.time * speed);
    //alpha %= 6.28f;
    float wind = alpha * numberOfWinds;

    Vector3 torusPoint = Torus.encodeToThreeTorus(alpha, wind);
    return new Vector4(torusPoint.x, torusPoint.z, torusPoint.y, voltage);
  }
}
