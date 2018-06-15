using UnityEngine;
namespace AutoDiff {
  public struct DualMatrix4x3 {
    public DualVector3 xBasis {
      get;
      private set;
    }
    public DualVector3 yBasis {
      get;
      private set;
    }
    public DualVector3 zBasis {
      get;
      private set;
    }
    public DualVector3 translation {
      get;
      private set;
    }

    #region Construction

    internal DualMatrix4x3(Vector3 translation, Quaternion rotation, Vector3 scale) : this() {
      this.translation = new DualVector3(translation, Vector3.zero);
      this.xBasis = new DualVector3(rotation * (DualVector3.right * scale.x), Vector3.zero);
      this.yBasis = new DualVector3(rotation * (DualVector3.up * scale.y), Vector3.zero);
      this.zBasis = new DualVector3(rotation * (DualVector3.forward * scale.z), Vector3.zero);
    }

    internal DualMatrix4x3(DualVector3 translation, Quaternion rotation, DualVector3 scale) : this() {
      this.translation = translation;
      xBasis = new DualVector3(rotation * (DualVector3.right * scale.x), Vector3.zero);
      yBasis = new DualVector3(rotation * (DualVector3.up * scale.y), Vector3.zero);
      zBasis = new DualVector3(rotation * (DualVector3.forward * scale.z), Vector3.zero);
    }

    internal DualMatrix4x3(DualVector3 xBasis, DualVector3 yBasis, DualVector3 zBasis, DualVector3 translation) : this() {
      this.translation = translation;
      this.xBasis = xBasis;
      this.yBasis = yBasis;
      this.zBasis = zBasis;
    }

    #endregion

    #region Properties

    public Matrix4x4 Value {
      get {
        Matrix4x4 matrix = new Matrix4x4(xBasis.Value, yBasis.Value, zBasis.Value, translation.Value); matrix.m33 = 1f;
        return matrix;
      }
      set {
        translation = new DualVector3(value.GetColumn(3), Vector4.zero);
        xBasis = new DualVector3(value.GetColumn(0), Vector4.zero);
        yBasis = new DualVector3(value.GetColumn(1), Vector4.zero);
        zBasis = new DualVector3(value.GetColumn(2), Vector4.zero);
      }
    }

    public Matrix4x4 Derivative {
      get {
        return new Matrix4x4(xBasis.Derivative, yBasis.Derivative, zBasis.Derivative, translation.Derivative);
      }
      set {
        translation = new DualVector3(translation.Value, value.GetColumn(3));
        xBasis = new DualVector3(xBasis.Value, value.GetColumn(0));
        yBasis = new DualVector3(yBasis.Value, value.GetColumn(1));
        zBasis = new DualVector3(zBasis.Value, value.GetColumn(2));
      }
    }

    public static DualMatrix4x3 identity {
      get {
        return new DualMatrix4x3(Vector3.zero, Quaternion.identity, Vector3.one);
      }
    }

    #endregion

    #region Operators 

    public static implicit operator DualMatrix4x3(Matrix4x4 value) {
      DualMatrix4x3 matrix = new DualMatrix4x3(); matrix.Value = value; return matrix;
    }

    public static implicit operator Matrix4x4(DualMatrix4x3 value) {
      return value.Value;
    }

    public static DualVector3 operator *(DualMatrix4x3 first, DualVector3 second) {
      return new DualVector3(
        (first.xBasis.x * second.x) + (first.yBasis.x * second.y) + (first.zBasis.x * second.z) + first.translation.x,
        (first.xBasis.y * second.x) + (first.yBasis.y * second.y) + (first.zBasis.y * second.z) + first.translation.y,
        (first.xBasis.z * second.x) + (first.yBasis.z * second.y) + (first.zBasis.z * second.z) + first.translation.z);
    }

    #endregion

    public override string ToString() {
      return "(" + this.Value + ")\n{" + this.Derivative + "}";
    }
  }
}
