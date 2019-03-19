using UnityEngine;

public class UnidirectionalLightning : MonoBehaviour {
  public Transform destination;
  public bool recalculatePath;
  [Range(0.005f, 0.5f)]
  public float jumpRate = 0.1f;
  float lastJumpRate = 0.1f;

  [Range(1, 9999)]
  public int divergence = 1000;

  Vector3[] path = new Vector3[10000];

  private void Start() { Debug.Log("Hey just look at it in the editor."); }

  public void OnDrawGizmos() {
    if(recalculatePath || jumpRate != lastJumpRate) {
      path[0] = transform.position;
      for (int i = 1; i < path.Length; i++) {
        path[i] = path[i - 1] + (Random.onUnitSphere * jumpRate);
      }
      lastJumpRate = jumpRate;
    }



    for (int i = 0; i < divergence; i++) {
      Vector3 prev = Vector3.Lerp(path[i], destination.position, (float)i / divergence);
      Vector3 next = Vector3.Lerp(path[i+1], destination.position, (float)(i+1) / divergence);

      Gizmos.DrawLine(prev, next);
    }
  }
}
