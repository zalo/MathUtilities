using UnityEngine;
using AutoDiff;

public class AutoDiffFitting : MonoBehaviour {
  public Transform baseSphere;
  public Mesh lowPolyIcoSphere;

  Vector3[] icoVerts;
  void Start() {
    icoVerts = lowPolyIcoSphere.vertices;

    Debug.Log(Matrix4x4.identity);
    Debug.Log(DualMatrix4x3.identity);
  }

  void Update() {
    Vector3 positionGradient = Vector3.zero;

    //Calculate derivative of the error wrt x, y, and z
    positionGradient.x = SphereFittingError(0, true).Derivative * 0.25f;
    positionGradient.y = SphereFittingError(1, false).Derivative * 0.25f;
    positionGradient.z = SphereFittingError(2, false).Derivative * 0.25f;
    float scaleGradient = SphereFittingError(3, false).Derivative * 0.25f;

    //Draw the System
    transform.position -= positionGradient;
    transform.localScale = Vector3.one * (transform.localScale.x - scaleGradient);
  }

  /// <summary>
  /// Calculates value and derivative of the position of the sphere with respect to whichDeriv
  /// </summary>
  DualNumber SphereFittingError(int whichDeriv, bool drawDebug) {
    DualVector3 spherePosition = new DualVector3(
      whichDeriv == 0 ? DualNumber.Variable(transform.position.x) : DualNumber.Constant(transform.position.x), 
      whichDeriv == 1 ? DualNumber.Variable(transform.position.y) : DualNumber.Constant(transform.position.y), 
      whichDeriv == 2 ? DualNumber.Variable(transform.position.z) : DualNumber.Constant(transform.position.z));
    DualNumber radius = whichDeriv == 3 ? DualNumber.Variable(transform.localScale.x) : DualNumber.Constant(transform.localScale.x);

    //Sum the Squared Errors
    DualNumber error = new DualNumber();
    for(int i = 0; i < icoVerts.Length; i++) {
      error += DistFromSurface(baseSphere.position, baseSphere.localScale.x * 0.5f, spherePosition + (radius * icoVerts[i]), drawDebug).Squared();
    }
    //Divide by Number of Squared Errors
    error /= icoVerts.Length;

    return error;
  }

  /// <summary>
  /// Calculates squared distance of a point from the surface of a sphere
  /// </summary>
  DualNumber DistFromSurface(Vector3 spherePosition, float radius, DualVector3 point, bool draw) {
    if (draw) { Debug.DrawLine(point, spherePosition + ((point.Value - spherePosition).normalized * radius)); }
    return ((DualVector3)spherePosition - point).Magnitude() - radius;
  }
}
