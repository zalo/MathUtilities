using UnityEngine;

public class CubicBezier : MonoBehaviour {
  [Range(0f, 10f)]
  public float visualTime = 0f;
  [Range(0f, 10f)]
  public float duration = 1f;
  public Transform start;
  public Transform end;
  [Range(0f, 10f)]
  public float startSpeed = 1f, endSpeed = 1f;

  private void OnDrawGizmos() {
    Gizmos.color = Color.red;
    float increment = duration * 0.01f;
    Vector3 startingVelocity = start.rotation * Vector3.forward * startSpeed;
    Vector3 endingVelocity = end.rotation * Vector3.forward * endSpeed;
    for (float t = 0f; t < duration; t += increment) {
      Vector3 lineStart = cubicBezier(start.position, startingVelocity, end.position, endingVelocity, duration, t);
      Vector3 lineEnd = cubicBezier(start.position, startingVelocity, end.position, endingVelocity, duration, t+increment);
      Gizmos.DrawLine(lineStart, lineEnd);
    }
    //Draw the visual gizmos
    cubicBezier(start.position, startingVelocity, end.position, endingVelocity, duration, visualTime, true);
  }

  Vector3 cubicBezier(Vector3 aStart, Vector3 aVel, Vector3 bEnd, Vector3 bVel, float duration, float time, bool drawGizmos = false) {
    float alpha = time / duration; //Normalize the time
    Vector3 aEnd = aStart + (aVel * duration); //Where it would be if it were only using A
    Vector3 bStart = bEnd - (bVel * duration); //Where it would start if it were just B

    Vector3 curA = Vector3.Lerp(aStart, aEnd, alpha); //Pure A's Progress
    Vector3 curB = Vector3.Lerp(bStart, bEnd, alpha); //Pure B's Progress
    Vector3 curAB = Vector3.Lerp(aEnd, bStart, alpha); //Blending the destinations

    Vector3 curAAB = Vector3.Lerp(curA, curAB, alpha); //Blend pure A towards blended destination
    Vector3 curBAB = Vector3.Lerp(curAB, curB, alpha); //Blend pure B towards blended destination

    Vector3 curAABBAB = Vector3.Lerp(curAAB, curBAB, alpha); //Blend destination blended A and B with eachother

    if (drawGizmos) {
      Gizmos.color = Color.grey;
      Gizmos.DrawLine(aStart, aEnd);
      Gizmos.DrawLine(bStart, bEnd);
      Gizmos.DrawLine(aEnd, bStart);

      Gizmos.color = Color.green;
      Gizmos.DrawSphere(curA, 0.02f);
      Gizmos.DrawSphere(curB, 0.02f);
      Gizmos.DrawSphere(curAB, 0.02f);
      Gizmos.DrawLine(curA, curAB);
      Gizmos.DrawLine(curB, curAB);

      Gizmos.color = Color.blue;
      Gizmos.DrawLine(curAAB, curBAB);
      Gizmos.DrawSphere(curAAB, 0.02f);
      Gizmos.DrawSphere(curBAB, 0.02f);

      Gizmos.color = Color.black;
      Gizmos.DrawSphere(curAABBAB, 0.02f);
    }

    return curAABBAB;
  }
}
