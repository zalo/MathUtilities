using System;
using System.Collections.Generic;

//Numerical Gradient Descent Implementation-----------------------------------------------------------
public class NumericalGradientDescent {
  public List<NelderMead.Vertex> simplexVertices;
  public float alpha, epsilon;
  Func<float[], float> costFunc;

  //Initializes the solver with a starting coordinate, a simplex dimension, 
  //and coefficients (strengths) for the various transformations.
  public NumericalGradientDescent(
    float[] initialVertex,
    Func<float[], float> costFunction,
    float stepSize = 1f,
    float differentiationEpsilon = 0.0001f) {

    //Assign the cost function; this function takes in a coordinate in parameter space
    //And outputs the "cost" associated with this parameter; the solver will try to minimize the cost
    costFunc = costFunction;

    //These hyperparameters control how the solver converges.
    //Increasing the alpha will make it more aggressive in how it converges
    alpha = stepSize;
    //Increasing the epsilon will make the gradient consider a wider region of parameter space.
    epsilon = differentiationEpsilon;

    //Create Initial Simplex: Make the first vertex the initialVertex
    //And all subsequent vertices are translated "differentiationEpsilon"
    //along just that dimension.  This is a "Right Angle" Simplex
    simplexVertices = new List<NelderMead.Vertex>(initialVertex.Length + 1);
    constructRightAngleSimplex(initialVertex, differentiationEpsilon);
  }

  //Calculate the gradient according to epsilon and step along it according to alpha.
  public void stepSolver() {
    for(int i = 0; i < simplexVertices[0].coordinates.Length; i++) {
      simplexVertices[0].coordinates[i] += (-(simplexVertices[i + 1].cost - simplexVertices[0].cost) / epsilon) * alpha;
    }

    updateRightAngleSimplex(epsilon);
  }

  //This creates a right-angle simplex around the "initial vertex" position
  public void constructRightAngleSimplex(float[] initialVertex, float simplexSize) {
    simplexVertices.Clear();
    simplexVertices.Add(new NelderMead.Vertex(initialVertex, costFunc, 0));
    for (int i = 0; i < initialVertex.Length; i++) {
      float[] vertexCoordinate = new float[initialVertex.Length];
      Array.Copy(initialVertex, vertexCoordinate, initialVertex.Length);
      vertexCoordinate[i] += simplexSize;

      simplexVertices.Add(new NelderMead.Vertex(vertexCoordinate, costFunc, i + 1));
    }
  }

  //This reconstructs the right-angle simplex around the first vertex
  public void updateRightAngleSimplex(float simplexSize) {
    simplexVertices[0] = new NelderMead.Vertex(simplexVertices[0].coordinates, costFunc, 0);
    for (int i = 0; i < simplexVertices[0].coordinates.Length; i++) {
      Array.Copy(simplexVertices[0].coordinates, simplexVertices[i + 1].coordinates, simplexVertices[0].coordinates.Length);
      simplexVertices[i + 1].coordinates[i] += simplexSize;

      simplexVertices[i + 1] = new NelderMead.Vertex(simplexVertices[i + 1].coordinates, costFunc, i + 1);
    }
  }
}