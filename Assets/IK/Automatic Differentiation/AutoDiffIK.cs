using UnityEngine;
using AutoDiff;

public class AutoDiffIK : MonoBehaviour {
  public Transform joint1, joint2;
  public bool useIK = false;
  [Range(0f, 6.28f)]
  public float theta = 0f;
  [Range(0f, 6.28f)]
  public float phi = 0f;

  void Update() {
    Vector2 mousePos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
    Vector2 clampedPos = mousePos.normalized * Mathf.Clamp(mousePos.magnitude, 0f, 2f);
    Vector2 error = clampedPos - (Vector2)joint2.position;

    float[] derivativeTheta = ArmFK(theta, phi, 0).Derivatives();
    float[] derivativePhi = ArmFK(theta, phi, 1).Derivatives();

    if (useIK) {
      theta +=
        ((error.x * derivativeTheta[2]) +
         (error.y * derivativeTheta[3])) * 0.25f;
      phi +=
        ((error.x * derivativePhi[2]) +
         (error.y * derivativePhi[3])) * 0.25f;

      //Loop the range of the angles for the inspector
      while (theta < 0f || theta > 6.28f) { theta -= Mathf.Sign(theta) * 6.28f; }
      while (phi < 0f || phi > 6.28f) { phi -= Mathf.Sign(phi) * 6.28f; }
    }

    //Evaluate the Forward Kinematics and Draw the System
    Debug.DrawLine(mousePos, joint2.position);
    float[] FKValues = ArmFK(theta, phi, -1).Values();
    joint1.position = new Vector3(FKValues[0], FKValues[1]);
    joint2.position = new Vector3(FKValues[2], FKValues[3]);
  }

  /// <summary>
  /// Calculates value and derivative of the position of the arm with respect to whichDeriv
  /// </summary>
  static DualNumber[] ArmFK(float theta, float phi, int whichDeriv) {
    DualNumber thetaVar = whichDeriv == 0 ? DualNumber.Variable(theta) : DualNumber.Constant(theta);
    DualNumber phiVar = whichDeriv == 1 ? DualNumber.Variable(phi) : DualNumber.Constant(phi);

    DualNumber[] outputValues = {
      Math.Sin(thetaVar), //X Position of First Joint
      Math.Cos(thetaVar), //Y Position of First Joint
      Math.Sin(thetaVar) + Math.Sin(thetaVar + phiVar), //X Position of Second Joint
      Math.Cos(thetaVar) + Math.Cos(thetaVar + phiVar), //Y Position of Second Joint
     };
    return outputValues;
  }
}
