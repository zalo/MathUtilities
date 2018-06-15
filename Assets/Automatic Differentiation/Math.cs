//Copyright (c) 2014 Ivan Gusiev <ivan.gusiev@gmail.com>
//https://github.com/ivan-gusiev/autodiff/
using System;
using SMath = System.Math;

namespace AutoDiff {
  public static class Math {
    /// <summary>
    /// Calculates a sine of dual number
    /// </summary>
    public static DualNumber Sin(DualNumber value) {
      return new DualNumber((float)SMath.Sin(value.Value), value.Derivative * (float)SMath.Cos(value.Value));
    }

    /// <summary>
    /// Calculates a cosine of dual number
    /// </summary>
    public static DualNumber Cos(DualNumber value) {
      return new DualNumber((float)SMath.Cos(value.Value), -value.Derivative * (float)SMath.Sin(value.Value));
    }

    /// <summary>
    /// Calculates an exponent of dual number
    /// </summary>
    public static DualNumber Exp(DualNumber value) {
      return new DualNumber((float)SMath.Exp(value.Value), value.Derivative * (float)SMath.Exp(value.Value));
    }

    /// <summary>
    /// Calculates a natural logarithm of dual number
    /// </summary>
    public static DualNumber Log(DualNumber value) {
      return new DualNumber((float)SMath.Log(value.Value), value.Derivative / value.Value);
    }

    /// <summary>
    /// Calculates a power of dual number
    /// </summary>
    public static DualNumber Pow(DualNumber value, float power) {
      return new DualNumber((float)SMath.Pow(value.Value, power), value.Derivative * power * (float)SMath.Pow(value.Value, power - 1f));
    }

    /// <summary>
    /// Calculates a square of dual number
    /// </summary>
    public static DualNumber Squared(this DualNumber value) {
      return new DualNumber((float)SMath.Pow(value.Value, 2f), value.Derivative * 2f * value.Value);
    }

    /// <summary>
    /// Calculates an absolute value of dual number
    /// </summary>
    public static DualNumber Abs(DualNumber value) {
      return new DualNumber(SMath.Abs(value.Value), value.Derivative * SMath.Sign(value.Value));
    }

    #region Functional

    /// <summary>
    /// Creates an autodiff function from two regular functions, one for actual value, one for derivative
    /// </summary>
    /// <param name="getValue">Value transform, e.g. for x^2 it is x => x * x</param>
    /// <param name="getDerivative">Derivative transform, e.g. for x^2 it is x => 2 * x</param>
    /// <returns>A function from DualNumber to DualNumber</returns>
    public static Func<DualNumber, DualNumber> ToDual(Func<float, float> getValue, Func<float, float> getDerivative) {
      return d => new DualNumber(getValue(d.Value), getDerivative(d.Value) * d.Derivative);
    }

    /// <summary>
    /// Turns an autodiff function to a normal function, assuming that the argument is actually variable
    /// </summary>
    /// <param name="fn">Autodiff function, such as AutoDiff.Math.Log</param>
    /// <returns>A function that takes a float and calculates f(x) and f'(x)</returns>
    public static Func<float, DualNumber> Parameterize(Func<DualNumber, DualNumber> fn) {
      return value => fn(DualNumber.Variable(value));
    }

    public static DualVector3 Cross(this DualVector3 first, DualVector3 second) {
      return new DualVector3((first.y * second.z) - (first.z * second.y),
                             (first.z * second.x) - (first.x * second.z),
                             (first.x * second.y) - (first.y * second.x));
    }

    public static DualNumber Dot(this DualVector3 first, DualVector3 second) {
      return ((first.x * second.x) + (first.y * second.y) + (first.z * second.z));
    }

    public static DualNumber SqrMagnitude(this DualVector3 vector) {
      return vector.x.Squared() + vector.y.Squared() + vector.z.Squared();
    }

    public static DualNumber Magnitude(this DualVector3 vector) {
      return Pow(SqrMagnitude(vector), 0.5f);
    }

    public static DualVector3 Normalize(this DualVector3 vector) {
      return vector/vector.Magnitude();
    }

    /// <summary> Orthogonalizes the rotation; preserves forward then y </summary>
    public static DualMatrix4x3 Orthogonalize(this DualMatrix4x3 matrix) {
      DualVector3 newXBasis = matrix.yBasis.Cross(matrix.zBasis);
      return new DualMatrix4x3(newXBasis, newXBasis.Cross(matrix.zBasis), matrix.zBasis, matrix.translation);
    }

    /// <summary> Normalizes the rotation; preserves forward </summary>
    public static DualMatrix4x3 Normalize(this DualMatrix4x3 matrix) {
      return new DualMatrix4x3(matrix.xBasis.Normalize(), matrix.yBasis.Normalize(), matrix.zBasis.Normalize(), matrix.translation);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Helper for extracting the values from autodiff functions
    /// </summary>
    public static float[] Values(this DualNumber[] numbers) {
      float[] values = new float[numbers.Length];
      for (int i = 0; i < numbers.Length; i++) {
        values[i] = numbers[i].Value;
      }
      return values;
    }
    /// <summary>
    /// Helper for extracting the derivatives from autodiff functions
    /// </summary>
    public static float[] Derivatives(this DualNumber[] numbers) {
      float[] derivatives = new float[numbers.Length];
      for (int i = 0; i < numbers.Length; i++) {
        derivatives[i] = numbers[i].Derivative;
      }
      return derivatives;
    }
    #endregion


  }
}
