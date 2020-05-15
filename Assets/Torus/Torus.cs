using UnityEngine;

public class Torus : MonoBehaviour {

  [Range(0f, 6.28f)]
  public float slice;

  void OnDrawGizmos() {
    Vector3 prevPos = Vector3.zero;
    for (float i = 0f; i < slice; i += 0.1f) {
      for (float j = 0f; j <= 6.28f; j += 0.2f) {
        float[] input = { i, j };
        float[] curPosArr = encodeToNTorus(input, 0.5f);
        Vector3 curPos = new Vector3(curPosArr[0], curPosArr[1], curPosArr[2]);
        Gizmos.DrawLine(prevPos, curPos);
        prevPos = curPos;
      }
    }
  }

  //UNTESTED ABOVE 3 DIMENSIONS - CURSE YOU, DIMENSIONALITY!!
  public static float[] encodeToNTorus(float[] value, float radiusReductionFactor = 1f) {
    float[] torusMapping = new float[value.Length + 1];

    //Sweeping creates a unit circle on the xy-plane
    torusMapping[0] = Mathf.Sin(value[0]); torusMapping[1] = Mathf.Cos(value[0]);

    //The key bit to extend to n-dimensions!
    //Sweeping creates a unit circle on the yz plane, but rotated by the first one
    //and scaled down by radiusReductionFactor!
    Vector3 curRotatedDirection = new Vector3(0f, torusMapping[0], torusMapping[1]);
    for (int i = 1; i < value.Length; i++) {
      curRotatedDirection = new Vector3(curRotatedDirection.y * Mathf.Sin(value[i]),
                                        curRotatedDirection.z * Mathf.Sin(value[i]),
                                                                Mathf.Cos(value[i])) * radiusReductionFactor;

      torusMapping[i - 1] += curRotatedDirection.x;
      torusMapping[i]     += curRotatedDirection.y;
      torusMapping[i + 1] += curRotatedDirection.z;
    }

    return torusMapping;
  }

  public static Vector3 encodeToThreeTorus(float value1, float value2) {
    //Sweeping creates a unit circle on the xy-plane
    Vector3 torusPoint1 = new Vector3(Mathf.Sin(value1), Mathf.Cos(value1), 0f);

    //The key bit to extend to n-dimensions!
    //Sweeping creates a unit circle on the yz plane, but rotated by the first one
    //and scaled down by half!
    Vector3 rotatedTorusPoint2 = new Vector3(torusPoint1.x * Mathf.Sin(value2),
                                             torusPoint1.y * Mathf.Sin(value2),
                                             Mathf.Cos(value2)) * 0.5f;

    return torusPoint1 + rotatedTorusPoint2;
  }
}
