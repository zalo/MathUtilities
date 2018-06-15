using UnityEngine;
using AutoDiff;

public class AutoDiffTransformOptimizer : MonoBehaviour {
  public Transform baseTransform;
  public Mesh lowPolyIcoSphere;
  public bool fitPosition = true;
  public bool fitRotation = true;
  public bool fitScale = false;
  [Range(0f, 1f)]
  public float alpha = 0.25f;
  public bool drawDebug = true;

  DualMatrix4x3 baseTransformMatrix;
  Optimizer optimizer = new Optimizer();

  Vector3[] icoVerts;
  void Start() {
    icoVerts = lowPolyIcoSphere.vertices;
  }

  void Update() {
    baseTransformMatrix = new DualMatrix4x3(baseTransform.position, baseTransform.rotation, baseTransform.localScale);

    //Set up the variables to optimize
    optimizer.Add(transform.position, "position");
    optimizer.Add(transform.up, "yBasis");
    optimizer.Add(transform.forward, "zBasis");
    optimizer.Add(transform.localScale, "scale");
    optimizer.CalculateErrorDerivative = CalculateErrorDerivative;

    //Calculate and Apply the Instantaneous Gradients at this point in time
    transform.position -= optimizer.CalcErrorGradient("position") * alpha;

    Vector3 forwardBasis = transform.forward - (optimizer.CalcErrorGradient("zBasis") * alpha);
    Vector3 upBasis = transform.up - (optimizer.CalcErrorGradient("yBasis") * alpha);
    transform.rotation = Quaternion.LookRotation(forwardBasis, upBasis);

    transform.localScale -= optimizer.CalcErrorGradient("scale") * alpha;
  }

  /// <summary>
  /// Calculates value and derivative of the position of the sphere with respect to whichDeriv
  /// </summary>
  float CalculateErrorDerivative() {
    DualVector3 thisYBasis = optimizer.vec("yBasis"), thisZBasis = optimizer.vec("zBasis");
    thisYBasis = thisYBasis.Normalize(); thisZBasis = thisZBasis.Normalize();
    DualVector3 thisXBasis = thisYBasis.Cross(thisZBasis);
    thisYBasis = thisZBasis.Cross(thisXBasis);
    
    //THIS IS A FULLY DIFFERENTIABLE RIGID TRANSFORM - NO SINGULARITIES
    //Also Single Covers the space (when non-uniform scaling is disabled)
    DualVector3 scale = optimizer.vec("scale");
    DualMatrix4x3 thisTransform = new DualMatrix4x3(thisXBasis * scale.x, thisYBasis * scale.y, thisZBasis * scale.z, optimizer.vec("position"));
    
    //Sum the Squared Errors
    DualNumber error = new DualNumber();
    for (int i = 0; i < icoVerts.Length; i++) {
      DualVector3 thisPoint = thisTransform * icoVerts[i]; DualVector3 basePoint = baseTransformMatrix * icoVerts[i];
      if (drawDebug) { Debug.DrawLine(thisPoint, basePoint); }
      error += (thisPoint - basePoint).SqrMagnitude();
    }
    //Divide by Number of Squared Errors
    error /= icoVerts.Length;

    return error.Derivative;
  }
}
