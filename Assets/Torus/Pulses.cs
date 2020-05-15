using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Pulses : MonoBehaviour {
  public Mesh sphere;
  public Material material;

  public float speed = -0.2f;
  public float windingNumber = 20f;

  Matrix4x4[] matrices = new Matrix4x4[1023];

  // Update is called once per frame
  void Update() {
    for(int i = 0; i < 1023; i++) {
      Vector4 torusPoint1 = calculateTorusPoint(i);
      Vector4 torusPoint2 = calculateTorusPoint(i + 1);
      Quaternion rot = Quaternion.FromToRotation(Vector3.up, torusPoint2 - torusPoint1);
      matrices[i] = Matrix4x4.TRS(torusPoint1, rot, Vector3.one * 0.025f * (1.35f+ torusPoint1.w));
    }

    Graphics.DrawMeshInstanced(sphere, 0, material, matrices);
  }

  Vector4 calculateTorusPoint(int i) {
    float alpha = 6.28318530718f * (i / 1024f);
    float voltage = Mathf.Sin((alpha * windingNumber));
    alpha += (Time.time * speed);
    //alpha %= 6.28f;
    float wind = alpha * windingNumber;

    Vector3 torusPoint = Torus.encodeToThreeTorus(alpha, wind);
    return new Vector4(torusPoint.x, torusPoint.z, torusPoint.y, voltage);
  }
}
