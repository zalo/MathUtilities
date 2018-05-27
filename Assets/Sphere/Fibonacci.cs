using UnityEngine;
public class Fibonacci : MonoBehaviour {
  public int numPoints = 1000;
  //Adapted from: https://stackoverflow.com/a/26127012
  public void OnDrawGizmos() {
    float rnd = 1f;
    float offset = 2f / numPoints;
    float increment = Mathf.PI * (3f - Mathf.Sqrt(5f));

    for (int i = 0; i < numPoints; i++) {
      float y = ((i * offset) - 1f) + (offset / 2f);
      float r = Mathf.Sqrt(1f - Mathf.Pow(y, 2f));
      float phi = ((i + rnd) % numPoints) * increment;

      Vector3 point = new Vector3(Mathf.Cos(phi) * r * transform.localScale.x, 
                                  y * transform.localScale.y, 
                                  Mathf.Sin(phi) * r * transform.localScale.z) * 0.5f;
      Gizmos.DrawSphere(point, 0.01f);
    }
  }
}
