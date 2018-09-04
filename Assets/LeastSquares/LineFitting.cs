using UnityEngine;

public class LineFitting : MonoBehaviour {
  public Transform parentTransform;
  Vector3 currentDirection = Vector3.right;

  void OnDrawGizmos() {
    if (currentDirection == Vector3.zero ||
        float.IsNaN(currentDirection.x)  ||
        float.IsInfinity(currentDirection.x)) {
      currentDirection = Vector3.right;
    }

    //Calculate Average
    Vector3 average = Vector3.zero;
    for (int i = 0; i < parentTransform.childCount; i++) {
      average += parentTransform.GetChild(i).position;
    }
    average /= parentTransform.childCount;

    //Step the optimal fitting line approximation
    Vector3 accumulatedOffset = Vector3.zero; float sum = 0f;
    for (int i = 0; i < parentTransform.childCount; i++) {
      float alpha = TimeAlongSegment(parentTransform.GetChild(i).position, average, average + currentDirection);
      Vector3 lineToPointOffset = parentTransform.GetChild(i).position - Vector3.LerpUnclamped(average, average + currentDirection, alpha);

      //Error?
      //accumulatedOffset += lineToPointOffset * Mathf.Sign(alpha);
      //sum += Mathf.Abs(alpha);
      //Squared Error?
      accumulatedOffset += lineToPointOffset * alpha;
      sum += alpha * alpha;
      //Cubed Error?
      //accumulatedOffset += lineToPointOffset * alpha * alpha * Mathf.Sign(alpha);
      //sum += Mathf.Abs(alpha * alpha * alpha);

      Gizmos.color = Color.red;
      Gizmos.DrawRay(parentTransform.GetChild(i).position, -lineToPointOffset);
    }
    currentDirection += accumulatedOffset / sum;
    currentDirection = currentDirection.normalized;

    Gizmos.color = Color.white;
    Gizmos.DrawRay(average, currentDirection * 2f);
    Gizmos.DrawRay(average, -currentDirection * 2f);
  }


  public static float TimeAlongSegment(Vector3 position, Vector3 a, Vector3 b) {
    Vector3 ba = b - a;
    return Vector3.Dot(position - a, ba) / ba.sqrMagnitude;
  }
}
