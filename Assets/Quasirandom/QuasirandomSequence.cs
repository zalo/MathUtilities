using System.Collections.Generic;
using UnityEngine;

public class QuasirandomSequence : MonoBehaviour {
  public void OnDrawGizmos() {
    Quasirandom.currentSeed = 0;
    int numPoints = Application.isPlaying ? Time.frameCount : 1024;
    for (int i = 0; i < numPoints; i++) {
      Vector3 curPoint = Vector3.zero; float[] pointArr = Quasirandom.value(3);
      for (int j = 0; j < 3; j++) curPoint[j] = pointArr[j];
      Gizmos.DrawSphere(curPoint, 0.008f);
    }
  }
}

//Generic Quasirandom Number Generating Class
//http://extremelearning.com.au/unreasonable-effectiveness-of-quasirandom-sequences/
public static class Quasirandom {
  public static long currentSeed;
  static Dictionary<int, float[]> alphas = new Dictionary<int, float[]>();

  static float phi(int dimension) {
    float x = 1f;
    for (int i = 0; i < 20; i++)
      x = x - (Mathf.Pow(x, dimension + 1) - x - 1) / ((dimension + 1) * Mathf.Pow(x, dimension) - 1);
    return x;
  }

  static float[] bakeAlphas(int dimensions) {
    float gamma = phi(dimensions);
    float[] newAlphas = new float[dimensions];
    for (int i = 0; i < dimensions; i++) newAlphas[i] = Mathf.Pow(1f / gamma, i + 1) % 1f;
    alphas.Add(dimensions, newAlphas);
    return newAlphas;
  }

  public static float[] value(int dimensions=3, long seed = 0) {
    float[] currentAlphas;
    if (!alphas.TryGetValue(dimensions, out currentAlphas)) currentAlphas = bakeAlphas(dimensions);
    if (seed != 0) currentSeed = seed;
    return value(currentAlphas, dimensions);
  }

  public static float[] value(float[] currentAlphas, int dimensions = 3) {
    float[] value = new float[dimensions];
    for (int i = 0; i < dimensions; i++) value[i] = (currentSeed * currentAlphas[i]) % 1f;
    currentSeed++;
    return value;
  }
}
