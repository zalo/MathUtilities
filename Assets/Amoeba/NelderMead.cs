using System;
using System.Collections.Generic;
using UnityEngine;

//Nelder-Mead Implementation-----------------------------------------------------------
public class NelderMead {
  public float[] centroidCoordinate;
  public List<Vertex> simplexVertices;
  Func<float[], float> costFunc;
  float alpha, beta, gamma, delta;

  //Initializes the solver with a starting coordinate, a simplex dimension, 
  //and coefficients (strengths) for the various transformations.
  public NelderMead(
    float[] initialVertex,
    Func<float[], float> costFunction,
    float initialSimplexSize = 1f) {

    //Assign the cost function; this function takes in a coordinate in parameter space
    //And outputs the "cost" associated with this parameter; the solver will try to minimize the cost
    costFunc = costFunction;

    //Below are the amounts the simplex moves at each step of the optimization process
    //These values are chosen according to: "Implementing the Nelder-Mead simplex algorithm with adaptive parameters" by Fuchang Gao and Lixing Han
    //https://pdfs.semanticscholar.org/15b4/c4aa7437df4d032c6ee6ce98d6030dd627be.pdf
    //This helps with high-dimensional (>10 dimensions) optimization

    //Reflection
    alpha = 1f;
    //Contraction
    beta = 0.75f - (1f / (2 * initialVertex.Length));
    //Expansion
    gamma = 1f + (2f / initialVertex.Length);
    //Shrink
    delta = 1f - (1f / initialVertex.Length);

    //Create Initial Simplex: Make the first vertex the initialVertex
    //And all subsequent vertices are translated "initialSimplexSize"
    //along just that dimension.  This is a "Right Angle" Simplex
    simplexVertices = new List<Vertex>(initialVertex.Length + 1);
    constructRightAngleSimplex(initialVertex, initialSimplexSize);
    centroidCoordinate = new float[simplexVertices[0].coordinates.Length];

    //Sort the list of vertices in our simplex by their costs (Smallest to Largest Cost)
    simplexVertices.Sort((x, y) => x.cost.CompareTo(y.cost));
  }

  public void stepSolver() {
    //First, calculate the centroid of all the vertices (except the largest cost vertex)
    //This point will lie on the center of the lowest-cost-face of the simplex
    centroidCoordinate = new float[simplexVertices[0].coordinates.Length];
    for (int i = 0; i < simplexVertices.Count - 1; i++) {
      centroidCoordinate = centroidCoordinate.Plus(simplexVertices[i].coordinates);
    }
    centroidCoordinate = centroidCoordinate.DivideBy(centroidCoordinate.Length);

    //Next, reflect the highest-cost vertex across this centroid and record it
    Vertex reflectedVertex = new Vertex(centroidCoordinate.Plus(centroidCoordinate.Minus(simplexVertices[simplexVertices.Count - 1].coordinates).Times(alpha)), costFunc, simplexVertices[simplexVertices.Count - 1].originalIndex);

    //Now that we've computed the necessary data, we can decide how the solver should proceed

    //First, check if our reflected point's cost is between our best cost and our second worst cost
    if (simplexVertices[0].cost <= reflectedVertex.cost && reflectedVertex.cost < simplexVertices[simplexVertices.Count - 2].cost) {
      simplexVertices[simplexVertices.Count - 1] = reflectedVertex;

      //Else, check if our reflected point has a lower cost than our best point, and exaggerate it if it does
    } else if (reflectedVertex.cost < simplexVertices[0].cost) {
      Vertex expandedVertex = new Vertex(centroidCoordinate.Plus(reflectedVertex.coordinates.Minus(centroidCoordinate).Times(gamma)), costFunc, simplexVertices[simplexVertices.Count - 1].originalIndex);

      //Take the lower cost of the two possible vertices
      simplexVertices[simplexVertices.Count - 1] = (expandedVertex.cost < reflectedVertex.cost) ? expandedVertex : reflectedVertex;

      //Else, check if our reflected point has a greater cost than our second worst point; contract it if it does
    } else if (reflectedVertex.cost >= simplexVertices[simplexVertices.Count - 2].cost) {
      Vertex contractedVertex;

      //Check if our reflected point is our new second-worst point, or...
      if (simplexVertices[simplexVertices.Count - 2].cost <= reflectedVertex.cost && reflectedVertex.cost < simplexVertices[simplexVertices.Count - 1].cost) {
        contractedVertex = new Vertex(centroidCoordinate.Plus(reflectedVertex.coordinates.Minus(centroidCoordinate).Times(beta)), costFunc, simplexVertices[simplexVertices.Count - 1].originalIndex);

        //If the contracted vertex is better than our reflected vertex...
        if (contractedVertex.cost < reflectedVertex.cost) {
          simplexVertices[simplexVertices.Count - 1] = contractedVertex;
        } else {
          //SHRINK - SUPER EXPENSIVE; RECALCULATES ENTIRE SIMPLEX
          for (int i = 1; i < simplexVertices.Count; i++) {
            simplexVertices[i] = new Vertex(simplexVertices[0].coordinates.Plus(simplexVertices[i].coordinates.Minus(simplexVertices[0].coordinates).Times(delta)), costFunc, simplexVertices[i].originalIndex);
          }
        }

        //if our reflected point is worse than the worst point
      } else if (reflectedVertex.cost >= simplexVertices[simplexVertices.Count - 1].cost) {
        contractedVertex = new Vertex(centroidCoordinate.Plus(simplexVertices[simplexVertices.Count - 1].coordinates.Minus(centroidCoordinate).Times(beta)), costFunc, simplexVertices[simplexVertices.Count - 1].originalIndex);

        //If the contracted vertex is better than our worst vertex...
        if (contractedVertex.cost < simplexVertices[simplexVertices.Count - 1].cost) {
          simplexVertices[simplexVertices.Count - 1] = contractedVertex;
        } else {
          //SHRINK - SUPER EXPENSIVE; RECALCULATES ENTIRE SIMPLEX
          for (int i = 1; i < simplexVertices.Count; i++) {
            simplexVertices[i] = new Vertex(simplexVertices[0].coordinates.Plus(simplexVertices[i].coordinates.Minus(simplexVertices[0].coordinates).Times(delta)), costFunc, simplexVertices[i].originalIndex);
          }
        }
      }
    }

    //Last, sort the list of vertices in our simplex by their costs (Smallest to Largest Cost)
    //TODO: Replace with an insertion sort of simplexVertices[simplexVertices.Count - 1] (except during Shrink)
    simplexVertices.Sort((x, y) => x.cost.CompareTo(y.cost));
  }

  //This should be done when you know that the optimum of a cost function has changed!
  public void recalculateSimplexCosts() {
    for (int i = 0; i < simplexVertices.Count; i++) {
      simplexVertices[i] = new Vertex(simplexVertices[i].coordinates, costFunc, simplexVertices[i].originalIndex);
    }

    //Sort the list of vertices in our simplex by their costs (Smallest to Largest Cost)
    simplexVertices.Sort((x, y) => x.cost.CompareTo(y.cost));
  }

  //This creates a right-angle simplex around the "initial vertex" position
  //This can be called to "reinitialize" the solver while it is running to
  //fix: it getting stuck, pre-converged simplices, or degenerate simplices.
  public void constructRightAngleSimplex(float[] initialVertex, float initialSimplexSize) {
    simplexVertices.Clear();
    simplexVertices.Add(new Vertex(initialVertex, costFunc, 0));
    for (int i = 0; i < initialVertex.Length; i++) {
      float[] vertexCoordinate = new float[initialVertex.Length];
      Array.Copy(initialVertex, vertexCoordinate, initialVertex.Length);
      vertexCoordinate[i] += initialSimplexSize;

      simplexVertices.Add(new Vertex(vertexCoordinate, costFunc, i + 1));
    }

    //Sort the list of vertices in our simplex by their costs (Smallest to Largest Cost)
    simplexVertices.Sort((x, y) => x.cost.CompareTo(y.cost));
  }

  //Simple Container Struct that stores coordinates and an associated cost
  public struct Vertex {
    public float[] coordinates;
    public float cost;
    public int originalIndex; // This is for visualization only!

    public Vertex(float[] Coordinates, Func<float[], float> costFunction, int index = 0) {
      coordinates = Coordinates;
      cost = costFunction(coordinates);
      originalIndex = index;
    }

    public float recalculateCost(Func<float[], float> costFunction) {
      return costFunction(coordinates);
    }
  }
}

//These are basic mathematical operations that can be performed on n-vectors
public static class FloatArrayExtension {
  public static float[] Plus(this float[] a1, float[] a2) {
    if (a1.Length == a2.Length) {
      float[] result = new float[a1.Length];
      for (int i = 0; i < a1.Length; i++) {
        result[i] = a1[i] + a2[i];
      }
      return result;
    } else {
      Debug.LogError("Vectors are not the same length!");
      return null;
    }
  }

  public static float[] Minus(this float[] a1, float[] a2) {
    if (a1.Length == a2.Length) {
      float[] result = new float[a1.Length];
      for (int i = 0; i < a1.Length; i++) {
        result[i] = a1[i] - a2[i];
      }
      return result;
    } else {
      Debug.LogError("Vectors are not the same length!");
      return null;
    }
  }

  public static float[] Times(this float[] a1, float a2) {
    float[] result = new float[a1.Length];
    for (int i = 0; i < a1.Length; i++) {
      result[i] = a1[i] * a2;
    }
    return result;
  }

  public static float[] DivideBy(this float[] a1, float a2) {
    float[] result = new float[a1.Length];
    for (int i = 0; i < a1.Length; i++) {
      result[i] = a1[i] / a2;
    }
    return result;
  }
}
