using UnityEngine;

public class Player : MonoBehaviour {
  public SpringJoint2D LFoot;
  public SpringJoint2D RFoot;

  float rotationAngle = 0f;
  float jumpTimer = 0f;

  public void Update() {
    //Force feet to settle on a level plane
    rotationAngle = (rotationAngle < 0f) ? rotationAngle + 360f : (rotationAngle > 360f ? rotationAngle - 360f : rotationAngle);

    //Control Jumping
    Vector2 jump = Vector2.zero;
    if (Input.GetKeyDown(KeyCode.W)) {
      jumpTimer = Time.time;
    }
    if (Input.GetKey(KeyCode.W) && jumpTimer + 0.1f > Time.time) {
      jump = Vector2.up * 0.4f;
    }

    //Control Walking
    if (Input.GetKey(KeyCode.A)) {
      rotationAngle += 10f;
    } else if (Input.GetKey(KeyCode.D)) {
      rotationAngle -= 10f;
    } else {
      rotationAngle = (rotationAngle < 90f || rotationAngle > 270f) ? 0f : 180f;
    }
   
    //Take into account all of the above for foot placement (relative to the body)
    Vector2 anchor = Quaternion.Euler(0f, 0f, rotationAngle) * Vector2.left * 0.25f;
    LFoot.anchor = new Vector2(1f * anchor.x, 0.4f * anchor.y) + jump;
    anchor = -anchor;
    RFoot.anchor = new Vector2(1f * anchor.x, 0.4f * anchor.y) + jump;
  }
}