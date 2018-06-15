using UnityEngine;
using AutoDiff;

public class AutoDiffIK : MonoBehaviour {
  public Transform joint1, joint2;
  [Range(0f, 6.28f)]
  public float theta = 0f;
  [Range(0f, 6.28f)]
  public float phi = 0f;

  DualVector3 baseJoint, endJoint;

  void Update() {
    Vector2 mousePos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
    Vector2 clampedPos = mousePos.normalized * Mathf.Clamp(mousePos.magnitude, 0f, 2f);

    //Calculate derivative of the error wrt theta
    float thetaDeriv = ArmFKError(clampedPos, 0).Derivative * 0.15f;

    //Calculate derivative of the error wrt phi
    float phiDeriv = ArmFKError(clampedPos, 1).Derivative * 0.15f;

    //Apply the derivative only after all of them are calculated
    theta -= thetaDeriv;
    phi -= phiDeriv;

    //Loop the range of the angles for the inspector
    while (theta < 0f || theta > 6.28f) { theta -= Mathf.Sign(theta) * 6.28f; }
    while (phi < 0f || phi > 6.28f) { phi -= Mathf.Sign(phi) * 6.28f; }

    //Evaluate the Forward Kinematics and Draw the System
    Debug.DrawLine(mousePos, joint2.position);
    joint1.position = baseJoint; joint2.position = endJoint;
  }

  /// <summary>
  /// Calculates value and derivative of the position of the arm with respect to whichDeriv
  /// </summary>
  DualNumber ArmFKError(Vector3 clampedPos, int whichDeriv) {
    DualNumber thetaVar = whichDeriv == 0 ? DualNumber.Variable(theta) : DualNumber.Constant(theta);
    DualNumber phiVar = whichDeriv == 1 ? DualNumber.Variable(phi) : DualNumber.Constant(phi);

    baseJoint = new DualVector3(Math.Sin(thetaVar), Math.Cos(thetaVar), new DualNumber());
    endJoint = baseJoint + new DualVector3(Math.Sin(thetaVar + phiVar), Math.Cos(thetaVar + phiVar), new DualNumber());
    return (((DualVector3)clampedPos - endJoint).SqrMagnitude());
  }
}
