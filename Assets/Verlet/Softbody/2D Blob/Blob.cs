using UnityEngine;

public class Blob : MonoBehaviour {
  [Range(2, 100)]
  public int segmentCount = 20;
  [Range(0.5f, 1f)]
  public float radius = 1f;
  public float floorHeight = -1.5f;
  [Range(1, 10)]
  public int solverIterations = 1;
  Vector2[] prevPoints, curPoints;
  Vector3[] accumDis;
  float segmentLength, area, circumfrence; //, segmentLengthSquared;

  // Use this for initialization
  void Start() {
    prevPoints = new Vector2[segmentCount]; curPoints = new Vector2[segmentCount]; accumDis = new Vector3[segmentCount];
    for (int i = 0; i < segmentCount; i++) {
      prevPoints[i] = Quaternion.Euler(0, 0, ((float)i / segmentCount) * 360f) * Vector2.up;
      curPoints[i] = prevPoints[i];
    }
  }

  // Update is called once per frame
  void Update() {
    //Calculate relevant circle intrinsics...
    area = Mathf.PI * radius * radius;
    circumfrence = 2 * Mathf.PI * radius;
    segmentLength = circumfrence / curPoints.Length;
    //segmentLengthSquared = segmentLength * segmentLength;

    //Integrate the points into the future
    for (int i = 0; i < curPoints.Length; i++) {
      Vector2 tempPos = curPoints[i];
      curPoints[i] += (curPoints[i] - prevPoints[i]) + ((Vector2)Physics.gravity * 0.001f);
      prevPoints[i] = tempPos;
    }
    //curPoints[0] = transform.position;

    //Accumulate displacements from constraints in accumulation buffer
    for (int k = 0; k < solverIterations; k++) {
      for (int i = 0; i < curPoints.Length; i++) {
        int j = (i == 0 ? curPoints.Length - 1 : i - 1);
        Vector3 offset = (curPoints[j] - curPoints[i]);
        //offset *= segmentLengthSquared / (Vector3.Dot(offset, offset) + segmentLengthSquared) - 0.5f; //Approximation; makes it squishier
        offset *= ((segmentLength - offset.magnitude) / offset.magnitude) * 0.5f; //Exact solution; more expensive, makes it more solid
        accumDis[i] += new Vector3(-offset.x, -offset.y, 1f);
        accumDis[j] += new Vector3(offset.x, offset.y, 1f);
      }

      //Calculate area of polygon and compare to desired area
      float sum = 0f;
      for (int i = 0; i < curPoints.Length; i++) {
        int j = (i == 0 ? curPoints.Length - 1 : i - 1);
        sum += (curPoints[i].x + curPoints[j].x) * (curPoints[i].y - curPoints[j].y);
      }
      float deltaArea = area - (sum * 0.5f);
      float dilationDistance = deltaArea / circumfrence;

      //Dilate the polygon by the distance required to acheieve the desired area
      for (int i = 0; i < curPoints.Length; i++) {
        Vector2 dilation = Vector3.Cross(Vector3.forward, curPoints[i == 0 ? curPoints.Length - 1 : i - 1] -
                                                          curPoints[i == curPoints.Length - 1 ? 0 : i + 1]).normalized * dilationDistance;
        accumDis[i] += new Vector3(dilation.x, dilation.y, 1f);
      }

      //Apply constraints
      for (int i = 0; i < curPoints.Length; i++) {
        if (accumDis[i] != Vector3.zero) { accumDis[i] /= accumDis[i][2]; }
        curPoints[i] += new Vector2(accumDis[i][0], accumDis[i][1]);
        accumDis[i] = Vector3.zero;
      }

      //Apply ground collision
      for (int i = 0; i < curPoints.Length; i++) {
        float displacement = Mathf.Max(curPoints[i].y, floorHeight) - curPoints[i].y;
        if (displacement > 0f) { curPoints[i].y += displacement; curPoints[i].x -= (curPoints[i].x - prevPoints[i].x) * displacement; }
      }
    }
  }

  void OnDrawGizmos() {
    if (curPoints != null && curPoints.Length > 0) {
      for (int i = 0; i < curPoints.Length; i++) {
        Gizmos.DrawSphere(curPoints[i], 0.1f);
      }

      for (int i = 0; i < curPoints.Length; i++) {
        Vector2 normal = Vector3.Cross(Vector3.forward, curPoints[i == 0 ? curPoints.Length - 1 : i - 1] -
                                                        curPoints[i == curPoints.Length - 1 ? 0 : i + 1]).normalized;
        Gizmos.DrawLine(curPoints[i], curPoints[i] + normal*0.5f); //Draw the normals of the polygon
      }
    }
  }
}
