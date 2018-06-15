using UnityEngine;
using AutoDiff;

public class AutoDiffTransform : MonoBehaviour {
  public Transform baseTransform;
  public Mesh lowPolyIcoSphere;
  public bool fitPosition = true;
  public bool fitRotation = true;
  public bool fitScale = false;
  [Range(0f, 1f)]
  public float alpha = 0.25f;

  DualMatrix4x3 baseTransformMatrix;

  Vector3[] icoVerts;
  void Start() {
    icoVerts = lowPolyIcoSphere.vertices;
  }

  void Update() {
    baseTransformMatrix = new DualMatrix4x3(baseTransform.position, baseTransform.rotation, baseTransform.localScale);

    Vector3 positionGradient = Vector3.zero;
    if (fitPosition) {
      //Calculate derivative of the error wrt x, y, and z
      positionGradient.x = SphereFittingError(0, true).Derivative;
      positionGradient.y = SphereFittingError(1, false).Derivative;
      positionGradient.z = SphereFittingError(2, false).Derivative;
    }

    Vector3 yBasisGradient = Vector3.zero;
    Vector3 zBasisGradient = Vector3.zero;
    if (fitRotation) {
      //Calculate derivative of the error wrt yx, yy, and yz
      yBasisGradient.x = SphereFittingError(3, false).Derivative;
      yBasisGradient.y = SphereFittingError(4, false).Derivative;
      yBasisGradient.z = SphereFittingError(5, false).Derivative;

      //Calculate derivative of the error wrt zx, zy, and zz
      zBasisGradient.x = SphereFittingError(6, false).Derivative;
      zBasisGradient.y = SphereFittingError(7, false).Derivative;
      zBasisGradient.z = SphereFittingError(8, false).Derivative;
    }

    Vector3 scaleGradient = Vector3.zero;
    if (fitScale) {
      //Calculate derivative of the error wrt sx, sy, and sz
      scaleGradient.x = SphereFittingError(9, false).Derivative;
      scaleGradient.y = SphereFittingError(10, false).Derivative;
      scaleGradient.z = SphereFittingError(11, false).Derivative;
    }

    //Apply the System
    transform.position -= positionGradient * alpha;

    Vector3 forwardBasis = transform.forward - (zBasisGradient * alpha);
    Vector3 upBasis = transform.up - (yBasisGradient * alpha);
    transform.rotation = Quaternion.LookRotation(forwardBasis, upBasis);

    transform.localScale -= scaleGradient * alpha;
  }

  /// <summary>
  /// Calculates value and derivative of the position of the sphere with respect to whichDeriv
  /// </summary>
  DualNumber SphereFittingError(int whichDeriv, bool drawDebug) {
    DualVector3 thisPosition = new DualVector3(
      whichDeriv == 0 ? DualNumber.Variable(transform.position.x) : DualNumber.Constant(transform.position.x),
      whichDeriv == 1 ? DualNumber.Variable(transform.position.y) : DualNumber.Constant(transform.position.y),
      whichDeriv == 2 ? DualNumber.Variable(transform.position.z) : DualNumber.Constant(transform.position.z));

    DualVector3 thisYBasis = new DualVector3(
      whichDeriv == 3 ? DualNumber.Variable(transform.up.x) : DualNumber.Constant(transform.up.x),
      whichDeriv == 4 ? DualNumber.Variable(transform.up.y) : DualNumber.Constant(transform.up.y),
      whichDeriv == 5 ? DualNumber.Variable(transform.up.z) : DualNumber.Constant(transform.up.z));

    DualVector3 thisZBasis = new DualVector3(
      whichDeriv == 6 ? DualNumber.Variable(transform.forward.x) : DualNumber.Constant(transform.forward.x),
      whichDeriv == 7 ? DualNumber.Variable(transform.forward.y) : DualNumber.Constant(transform.forward.y),
      whichDeriv == 8 ? DualNumber.Variable(transform.forward.z) : DualNumber.Constant(transform.forward.z));

    DualVector3 thisScale = new DualVector3(
      whichDeriv == 9 ? DualNumber.Variable(transform.localScale.x) : DualNumber.Constant(transform.localScale.x),
      whichDeriv == 10 ? DualNumber.Variable(transform.localScale.y) : DualNumber.Constant(transform.localScale.y),
      whichDeriv == 11 ? DualNumber.Variable(transform.localScale.z) : DualNumber.Constant(transform.localScale.z));

    thisYBasis = thisYBasis.Normalize(); thisZBasis = thisZBasis.Normalize();
    DualVector3 thisXBasis = thisYBasis.Cross(thisZBasis);
    thisYBasis = thisZBasis.Cross(thisXBasis);

    //THIS IS A FULLY DIFFERENTIABLE RIGID TRANSFORM - NO SINGULARITIES
    DualMatrix4x3 thisTransform = new DualMatrix4x3(thisXBasis * thisScale.x, thisYBasis * thisScale.y, thisZBasis * thisScale.z, thisPosition);

    //Sum the Squared Errors
    DualNumber error = new DualNumber();
    for (int i = 0; i < icoVerts.Length; i++) {
      DualVector3 thisPoint = thisTransform * icoVerts[i]; DualVector3 basePoint = baseTransformMatrix * icoVerts[i];
      if (drawDebug) { Debug.DrawLine(thisPoint, basePoint); }
      error += (thisPoint - basePoint).SqrMagnitude();
    }
    //Divide by Number of Squared Errors
    error /= icoVerts.Length;

    return error;
  }
}
