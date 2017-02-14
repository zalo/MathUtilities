using UnityEngine;
public static class CameraExtensions {
  public static void SetFrustum(this Camera SimulatorCamera, Vector3[] corners, bool estimateViewFrustum = true) {
    Vector3 pa = corners[0];
    Vector3 pb = corners[3];
    Vector3 pc = corners[1];
    Vector3 pe = SimulatorCamera.transform.position;
    // eye position
    float n = SimulatorCamera.nearClipPlane;
    // distance of near clipping plane
    float f = SimulatorCamera.farClipPlane;
    // distance of far clipping plane

    Vector3 va; // from pe to pa
    Vector3 vb; // from pe to pb
    Vector3 vc; // from pe to pc
    Vector3 vr; // right axis of screen
    Vector3 vu; // up axis of screen
    Vector3 vn; // normal vector of screen

    float l; // distance to left screen edge
    float r; // distance to right screen edge
    float b; // distance to bottom screen edge
    float t; // distance to top screen edge
    float d; // distance from eye to screen

    vr = pb - pa;
    vu = pc - pa;
    vr.Normalize();
    vu.Normalize();
    vn = -Vector3.Cross(vr, vu);
    // we need the minus sign because Unity
    // uses a left-handed coordinate system
    vn.Normalize();

    va = pa - pe;
    vb = pb - pe;
    vc = pc - pe;

    d = -Vector3.Dot(va, vn);
    l = Vector3.Dot(vr, va) * n / d;
    r = Vector3.Dot(vr, vb) * n / d;
    b = Vector3.Dot(vu, va) * n / d;
    t = Vector3.Dot(vu, vc) * n / d;

    Matrix4x4 p = new Matrix4x4(); // projection matrix
    p[0, 0] = 2.0f * n / (r - l);
    p[0, 1] = 0.0f;
    p[0, 2] = (r + l) / (r - l);
    p[0, 3] = 0.0f;

    p[1, 0] = 0.0f;
    p[1, 1] = 2.0f * n / (t - b);
    p[1, 2] = (t + b) / (t - b);
    p[1, 3] = 0.0f;

    p[2, 0] = 0.0f;
    p[2, 1] = 0.0f;
    p[2, 2] = (f + n) / (n - f);
    p[2, 3] = 2.0f * f * n / (n - f);

    p[3, 0] = 0.0f;
    p[3, 1] = 0.0f;
    p[3, 2] = -1.0f;
    p[3, 3] = 0.0f;

    Matrix4x4 rm = new Matrix4x4(); // rotation matrix;
    rm[0, 0] = vr.x;
    rm[0, 1] = vr.y;
    rm[0, 2] = vr.z;
    rm[0, 3] = 0.0f;

    rm[1, 0] = vu.x;
    rm[1, 1] = vu.y;
    rm[1, 2] = vu.z;
    rm[1, 3] = 0.0f;

    rm[2, 0] = vn.x;
    rm[2, 1] = vn.y;
    rm[2, 2] = vn.z;
    rm[2, 3] = 0.0f;

    rm[3, 0] = 0.0f;
    rm[3, 1] = 0.0f;
    rm[3, 2] = 0.0f;
    rm[3, 3] = 1.0f;

    Matrix4x4 tm = new Matrix4x4(); // translation matrix;
    tm[0, 0] = 1.0f;
    tm[0, 1] = 0.0f;
    tm[0, 2] = 0.0f;
    tm[0, 3] = -pe.x;

    tm[1, 0] = 0.0f;
    tm[1, 1] = 1.0f;
    tm[1, 2] = 0.0f;
    tm[1, 3] = -pe.y;

    tm[2, 0] = 0.0f;
    tm[2, 1] = 0.0f;
    tm[2, 2] = 1.0f;
    tm[2, 3] = -pe.z;

    tm[3, 0] = 0.0f;
    tm[3, 1] = 0.0f;
    tm[3, 2] = 0.0f;
    tm[3, 3] = 1.0f;

    // set matrices
    SimulatorCamera.projectionMatrix = p;
    SimulatorCamera.worldToCameraMatrix = rm * tm;
    // The original paper puts everything into the projection
    // matrix (i.e. sets it to p * rm * tm and the other
    // matrix to the identity), but this doesn't appear to
    // work with Unity's shadow maps.

    if (estimateViewFrustum) {
      // rotate camera to screen for culling to work
      Quaternion q = new Quaternion();
      q.SetLookRotation((0.5f * (pb + pc) - pe), vu);
      // look at center of screen
      SimulatorCamera.transform.rotation = q;

      // set fieldOfView to a conservative estimate
      // to make frustum tall enough
      if (SimulatorCamera.aspect >= 1.0) {
        SimulatorCamera.fieldOfView = Mathf.Rad2Deg *
           Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude)
           / va.magnitude);
      } else {
        // take the camera aspect into account to
        // make the frustum wide enough
        SimulatorCamera.fieldOfView =
           Mathf.Rad2Deg / SimulatorCamera.aspect *
           Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude)
           / va.magnitude);
      }
    }
  }
}