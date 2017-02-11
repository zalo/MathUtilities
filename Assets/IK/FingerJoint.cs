using UnityEngine;

public class FingerJoint : ConstrainedJoint {
  public Transform target;
  public bool isBase = false;
  public float maximumAngle = 30;

  public override void Evaluate() {
    //Initialize Point to Constrain
    Vector3 targetPosition = target != null ? target.position : transform.GetChild(0).TransformPoint(Vector3.down / 2f);
    Vector3 distalConnection = transform.parent.InverseTransformPoint(targetPosition);

    //Constrain To Plane
    if (!isBase) {
      distalConnection = Vector3.ProjectOnPlane(distalConnection, Vector3.right);
    }
    //Constrain Distance
    distalConnection = distalConnection.ConstrainDistance(Vector3.up/2f, 1f);

    //Constrain Angle
    distalConnection -= Vector3.up / 2f;
    distalConnection = distalConnection.ConstrainToNormal(Vector3.up-(isBase?Vector3.forward *0.5f: Vector3.forward), maximumAngle);
    distalConnection += Vector3.up / 2f;

    //Set this Link's orientation
    transform.localPosition = Vector3.Lerp(Vector3.up / 2f, distalConnection, 0.5f);
    transform.localRotation = Quaternion.FromToRotation(transform.parent.InverseTransformDirection(transform.up / 2f), distalConnection - (Vector3.up / 2f)) * transform.localRotation;

    //Constrain Twisting
    transform.localRotation = Quaternion.LookRotation(-Vector3.Cross(transform.localRotation * Vector3.up, Vector3.right), transform.localRotation * Vector3.up);

    if (!isBase) {
      transform.parent.ConstrainToPoint(transform.TransformPoint(Vector3.up / 2), targetPosition);
    }
  }
}