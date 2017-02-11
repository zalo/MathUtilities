using UnityEngine;

public class HandJoint : ConstrainedJoint {
  public Transform target;
  public bool isBase = false;
  public float minDotProduct;
  public Vector3 Normal = Vector3.up;
  public Vector3 anchor;
  Vector3 localAnchor;
  Quaternion startingOrientation;

  void Start() {
    localAnchor = isBase ? anchor : Vector3.up / 2f;
    startingOrientation = transform.localRotation;
  }

  public override void Evaluate() {
    //Initialize Point to Constrain
    Vector3 targetPosition = target != null ? target.position : transform.childCount>0?transform.GetChild(0).TransformPoint(Vector3.down / 2f):transform.TransformPoint(Vector3.up/2f);
    Vector3 distalConnection = transform.parent.InverseTransformPoint(targetPosition);

    //Constrain To Plane
    if (!isBase) { distalConnection = Vector3.ProjectOnPlane(distalConnection, startingOrientation * Vector3.right); }

    //Constrain Distance
    distalConnection = distalConnection.ConstrainDistance(localAnchor, 1f);

    //Constrain Angle
    distalConnection = distalConnection.ConstrainToCone(localAnchor, startingOrientation * Normal, minDotProduct);

    //Set this Link's orientation
    transform.localPosition = Vector3.Lerp(localAnchor, distalConnection, 0.5f);
    transform.localRotation = Quaternion.FromToRotation(transform.parent.InverseTransformDirection(transform.up), distalConnection - localAnchor) * transform.localRotation;

    //Constrain Twisting
    transform.localRotation = Quaternion.LookRotation(-Vector3.Cross(transform.localRotation * Vector3.up, (isBase?startingOrientation * Vector3.right:Vector3.right)), transform.localRotation * Vector3.up);

    if (!isBase) {
      //Propagate un-accounted for displacement up the chain
      transform.parent.ConstrainToPoint(transform.TransformPoint(Vector3.up / 2f), targetPosition);
    }else {
      //Move the palm in a Dampened Fashion
      Quaternion oldRot = transform.parent.rotation; Vector3 oldPos = transform.parent.position;
      transform.parent.ConstrainToPoint(transform.TransformPoint(Vector3.up / 2f), targetPosition);
      transform.parent.position = Vector3.Lerp(oldPos, transform.parent.position, 0.1f);
      transform.parent.rotation = Quaternion.Lerp(oldRot, transform.parent.rotation, 0.1f);
    }
  }
}