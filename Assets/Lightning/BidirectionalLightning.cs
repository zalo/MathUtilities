using UnityEngine;

public class BidirectionalLightning : MonoBehaviour {
  public Transform destination;
  public bool recalculatePath;
  [Range(0.005f, 0.5f)]
  public float jumpRate = 0.1f;
  float lastJumpRate = 0.1f;

  [Range(1, 9999)]
  public int divergence = 1000;
  int lastDivergence = 1000;

  Vector3[] path1 = new Vector3[10000],
            path2 = new Vector3[10000];

  private void Start() { Debug.Log("Hey just look at it in the editor."); }

  public void OnDrawGizmos() {
    if (enabled) {

      if (recalculatePath || jumpRate != lastJumpRate) {
        path1[0] = transform.position;
        for (int i = 1; i < path1.Length; i++) {
          path1[i] = path1[i - 1] + (Random.onUnitSphere * jumpRate);
        }

        path2[0] = destination.position;
        for (int i = 1; i < path2.Length; i++) {
          path2[i] = path2[i - 1] + (Random.onUnitSphere * jumpRate);
        }

        lastJumpRate = jumpRate;
        lastDivergence = divergence;
      }

      for (int i = 0; i < divergence; i++) {
        Vector3 prev = Vector3.Lerp(path1[i], path2[divergence - i], (float)i / divergence);
        Vector3 next = Vector3.Lerp(path1[i + 1], path2[divergence - i - 1], (float)(i + 1) / divergence);

        Gizmos.DrawLine(prev, next);
      }
    }
  }
}
