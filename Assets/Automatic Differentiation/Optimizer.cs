using System.Collections.Generic;
using UnityEngine;
using AutoDiff;

public class Optimizer {
  public Optimizer() { }

  protected Dictionary<string, DualVector3> vectors = new Dictionary<string, DualVector3>();
  public void Add(Vector3 vector, string key) {
    if (vectors.ContainsKey(key)) {
      vectors[key] = vector;
    } else { vectors.Add(key, vector); }
  }
  public DualVector3 vec(string key) {
    DualVector3 toReturn;
    if(vectors.TryGetValue(key, out toReturn)) {
      return toReturn;
    }
    return toReturn;
  }
  public Vector3 CalcErrorGradient(string vectorKey) {
    DualVector3 vector = new DualVector3();
    if (CalculateErrorDerivative != null && vectors.TryGetValue(vectorKey, out vector)) {
      Vector3 errorGradient = Vector3.zero;
      for(int i = 0; i < 3; i++) {
        vectors[vectorKey] = new DualVector3(vector.Value, i);
        errorGradient[i] = CalculateErrorDerivative();
      }
      vectors[vectorKey] = new DualVector3(vector.Value);
      return errorGradient;
    }
    return Vector3.zero;
  }

  protected Dictionary<string, DualNumber> scalars = new Dictionary<string, DualNumber>();
  public void Add(float scalar, string key) {
    if (scalars.ContainsKey(key)) {
      scalars[key] = scalar;
    } else { scalars.Add(key, scalar); }
  }
  public DualNumber num(string key) {
    DualNumber toReturn;
    if (scalars.TryGetValue(key, out toReturn)) {
      return toReturn;
    }
    return toReturn;
  }
  public float CalcErrorDerivative(string scalarKey) {
    DualNumber scalar = new DualNumber();
    if (CalculateErrorDerivative != null && scalars.TryGetValue(scalarKey, out scalar)) {
      scalars[scalarKey] = new DualNumber(scalar.Value, 1f);
      float errorDerivative = CalculateErrorDerivative();
      scalars[scalarKey] = new DualNumber(scalar.Value, 0f);
      return errorDerivative;
    }
    return 0f;
  }

  public System.Func<float> CalculateErrorDerivative;

}
