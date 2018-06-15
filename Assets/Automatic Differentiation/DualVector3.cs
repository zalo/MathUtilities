using UnityEngine;
namespace AutoDiff {
  public struct DualVector3 {
    public DualNumber x {
      get;
      private set;
    }
    public DualNumber y {
      get;
      private set;
    }
    public DualNumber z {
      get;
      private set;
    }

    #region Construction

    internal DualVector3(Vector3 value, Vector3 derivative) : this() {
      x = new DualNumber(value.x, derivative.x);
      y = new DualNumber(value.y, derivative.y);
      z = new DualNumber(value.z, derivative.z);
    }

    internal DualVector3(Vector3 value, int whichIndexIsVariable = -1) : this() {
      x = new DualNumber(value.x, whichIndexIsVariable == 0 ? 1f : 0f);
      y = new DualNumber(value.y, whichIndexIsVariable == 1 ? 1f : 0f);
      z = new DualNumber(value.z, whichIndexIsVariable == 2 ? 1f : 0f);
    }

    internal DualVector3(DualNumber x, DualNumber y, DualNumber z) : this() {
      this.x = x; this.y = y; this.z = z;
    }

    #endregion

    #region Properties

    public Vector3 Value {
      get {
        return new Vector3(x.Value, y.Value, z.Value);
      }
      set {
        x = new DualNumber(value.x, x.Derivative);
        y = new DualNumber(value.y, y.Derivative);
        z = new DualNumber(value.z, z.Derivative);
      }
    }

    public Vector3 Derivative {
      get {
        return new Vector3(x.Derivative, y.Derivative, z.Derivative);
      }
      set {
        x = new DualNumber(x.Value, value.x);
        y = new DualNumber(y.Value, value.y);
        z = new DualNumber(z.Value, value.z);
      }
    }

    public static readonly DualVector3 right = new DualVector3(Vector3.right);
    public static readonly DualVector3 up = new DualVector3(Vector3.up);
    public static readonly DualVector3 forward = new DualVector3(Vector3.forward);

    #endregion

    #region Operators 

    public static implicit operator DualVector3(Vector3 value) {
      return new DualVector3(value);
    }

    public static implicit operator Vector3(DualVector3 value) {
      return new Vector3(value.x.Value, value.y.Value, value.z.Value);
    }

    public static DualVector3 operator +(DualVector3 first, DualVector3 second) {
      return new DualVector3(first.x + second.x, first.y + second.y, first.z + second.z);
    }

    public static DualVector3 operator +(DualVector3 first, Vector3 second) {
      return new DualVector3(first.Value + second, first.Derivative);
    }

    public static DualVector3 operator +(Vector3 first, DualVector3 second) {
      return second + first;
    }

    public static DualVector3 operator -(DualVector3 vector) {
      return new DualVector3(-vector.x, -vector.y, -vector.z);
    }

    public static DualVector3 operator -(DualVector3 first, DualVector3 second) {
      return first + (-second);
    }

    public static DualVector3 operator *(DualVector3 first, DualNumber second) {
      return new DualVector3(first.x * second, first.y * second, first.z * second);
    }

    public static DualVector3 operator *(Quaternion first, DualVector3 second) {
      return new DualVector3(first * second.Value, first * second.Derivative);
    }

    public static DualVector3 operator /(DualVector3 first, DualNumber second) {
      return new DualVector3(first.x / second, first.y / second, first.z / second);
    }

    #endregion

    public override string ToString() {
      return string.Format("("+x.Value+", "+ y.Value + ", " + z.Value + "){"+ x.Derivative + ", " + y.Derivative + ", " + z.Derivative + "}");
    }
  }
}
