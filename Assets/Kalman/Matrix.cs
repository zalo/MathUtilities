using System;
using UnityEngine;

//To write a Kalman Filter, one must first invent Linear Algebra
namespace SimpleMatrices {
  public static class MatrixExtensions {

    //Makes an identity matrix with these dimensions
    public static double[,] makeIdentity(int inwidth, int inheight) {
      double[,] toReturn = new double[inheight, inwidth];

      //Set the diagonals to 1
      for (int i = 0; i < Mathf.Min(inwidth, inheight); i++) {
        toReturn[i, i] = 1f;
      }

      return toReturn;
    }

    public static double[,] makeIdentity(int indimension) {
      return makeIdentity(indimension, indimension);
    }

    //Multiplies this Matrix by another Matrix
    public static double[,] multiply(this double[,] thisMatrix, double[,] inMatrix) {
      if (thisMatrix.GetLength(1) == inMatrix.GetLength(0)) {
        double[,] toReturn = new double[thisMatrix.GetLength(0), inMatrix.GetLength(1)];
        for (int i = 0; i < thisMatrix.GetLength(0); i++) { //i is the row in this matrix
          for (int j = 0; j < inMatrix.GetLength(1); j++) { //j is the column in the other matrix
            for (int k = 0; k < thisMatrix.GetLength(1); k++) { //k is the column in this matrix
              toReturn[i, j] += thisMatrix[i, k] * inMatrix[k, j];
            }
          }
        }
        return toReturn;
      } else {
        return thisMatrix;
      }
    }

    //Returns a Vector3 column of a 3x3 matrix
    public static Vector3 col(this double[,] thisMatrix, int i) {
      if (thisMatrix.GetLength(0) == 3 && thisMatrix.GetLength(1) == 3) {
        return new Vector3((float)thisMatrix[0, i], (float)thisMatrix[1, i], (float)thisMatrix[2, i]);
      } else {
        Debug.LogError("Not a 3x3 Matrix!");
        return Vector3.one;
      }
    }

    //Returns a Vector3 row of a 3x3 matrix
    public static Vector3 row(this double[,] thisMatrix, int i) {
      if (thisMatrix.GetLength(0) == 3 && thisMatrix.GetLength(1) == 3) {
        return new Vector3((float)thisMatrix[i, 0], (float)thisMatrix[i, 1], (float)thisMatrix[i, 2]);
      } else {
        Debug.LogError("Not a 3x3 Matrix!");
        return Vector3.one;
      }
    }

    //Multiplies this Matrix by a Scalar
    public static double[,] multiply(this double[,] thisMatrix, double inScalar) {
      double[,] toReturn = new double[thisMatrix.GetLength(0), thisMatrix.GetLength(1)];
      for (int i = 0; i < thisMatrix.GetLength(0); i++) { //i is the row in this matrix
        for (int j = 0; j < thisMatrix.GetLength(1); j++) { //j is the column in the this matrix
          toReturn[i, j] = thisMatrix[i, j] * inScalar;
        }
      }
      return toReturn;
    }

    //Performs a component-wise sqrt on this matrix
    public static double[,] sqrt(this double[,] thisMatrix) {
      double[,] toReturn = new double[thisMatrix.GetLength(0), thisMatrix.GetLength(1)];
      for (int i = 0; i < thisMatrix.GetLength(0); i++) { //i is the row in this matrix
        for (int j = 0; j < thisMatrix.GetLength(1); j++) { //j is the column in the this matrix
          toReturn[i, j] = Math.Sqrt(toReturn[i, j]);
        }
      }
      return toReturn;
    }

    //Performs a component-wise addition between two matrices
    public static double[,] add(this double[,] thisMatrix, double[,] inMatrix) {
      if (thisMatrix.GetLength(0) == inMatrix.GetLength(0) && thisMatrix.GetLength(1) == inMatrix.GetLength(1)) {
        double[,] toReturn = new double[thisMatrix.GetLength(0), thisMatrix.GetLength(1)];
        for (int i = 0; i < thisMatrix.GetLength(0); i++) { //i is the row in this matrix
          for (int j = 0; j < thisMatrix.GetLength(1); j++) { //j is the column in this matrix
            toReturn[i, j] = thisMatrix[i, j] + inMatrix[i, j];
          }
        }
        return toReturn;
      } else {
        Debug.LogWarning("Matrices are not of equal dimensions!\nThis Matrix is a " +
          thisMatrix.GetLength(0) + "x" + thisMatrix.GetLength(1) + " while the other is a " +
          inMatrix.GetLength(0) + "x" + inMatrix.GetLength(1) + "!");
        return thisMatrix;
      }
    }

    //Performs a component-wise subtraction between two matrices
    public static double[,] subtract(this double[,] thisMatrix, double[,] inMatrix) {
      if (thisMatrix.GetLength(0) == inMatrix.GetLength(0) && thisMatrix.GetLength(1) == inMatrix.GetLength(1)) {
        double[,] toReturn = new double[thisMatrix.GetLength(0), thisMatrix.GetLength(1)];
        for (int i = 0; i < thisMatrix.GetLength(0); i++) { //i is the row in this matrix
          for (int j = 0; j < thisMatrix.GetLength(1); j++) { //j is the column in this matrix
            toReturn[i, j] = thisMatrix[i, j] - inMatrix[i, j];
          }
        }
        return toReturn;
      } else {
        Debug.LogWarning("Matrices are not of equal dimensions!\nThis Matrix is a " +
          thisMatrix.GetLength(0) + "x" + thisMatrix.GetLength(1) + " while the other is a " +
          inMatrix.GetLength(0) + "x" + inMatrix.GetLength(1) + "!");
        return thisMatrix;
      }
    }

    //Transposes this Matrix
    public static double[,] transpose(this double[,] thisMatrix) {
      double[,] toReturn = new double[thisMatrix.GetLength(1), thisMatrix.GetLength(0)];
      for (int i = 0; i < thisMatrix.GetLength(0); i++) { //i is the row in this matrix
        for (int j = 0; j < thisMatrix.GetLength(1); j++) { //j is the column in this matrix
          toReturn[j, i] = thisMatrix[i, j];
        }
      }
      return toReturn;
    }

    //Gloms another Matrix onto the right side of this one
    public static double[,] concatenate(this double[,] thisMatrix, double[,] inMatrix) {
      if (thisMatrix.GetLength(0) == inMatrix.GetLength(1)) {
        double[,] toReturn = new double[thisMatrix.GetLength(0), thisMatrix.GetLength(1) + inMatrix.GetLength(1)];
        for (int i = 0; i < thisMatrix.GetLength(0); i++) { //i is the row in this matrix
          for (int j = 0; j < thisMatrix.GetLength(1); j++) { //j is the column in this matrix
            toReturn[i, j] = thisMatrix[i, j];
          }
          for (int j = inMatrix.GetLength(1); j < inMatrix.GetLength(1) + thisMatrix.GetLength(1); j++) {
            toReturn[i, j] = inMatrix[i, j - thisMatrix.GetLength(1)];
          }
        }
        return toReturn;
      } else {
        Debug.LogWarning("Matrices are not of equal height!");
        return thisMatrix;
      }
    }

    //Trims away the excess identity Matrix once you're done inverting it
    public static double[,] deconcatenate(this double[,] thisMatrix, int startColumn) {
      double[,] toReturn = new double[thisMatrix.GetLength(0), thisMatrix.GetLength(1) - startColumn];
      for (int i = 0; i < thisMatrix.GetLength(0); i++) { //i is the row in this matrix
        for (int j = startColumn; j < thisMatrix.GetLength(1); j++) { //j is the column in the this matrix
          toReturn[i, j - startColumn] = thisMatrix[i, j];
        }
      }
      return toReturn;
    }

    //Swaps two rows in this Matrix
    public static double[,] swapRow(this double[,] thisMatrix, int row1, int row2) {
      double[,] toReturn = (double[,])thisMatrix.Clone();
      double[] tempRow = new double[thisMatrix.GetLength(1)];
      for (int i = 0; i < thisMatrix.GetLength(1); i++) {
        tempRow[i] = toReturn[row2, i];
        toReturn[row2, i] = toReturn[row1, i];
        toReturn[row1, i] = tempRow[i];
      }
      return toReturn;
    }

    //Code ported from Vik's Blog on Matrix Inversion ----
    //http://www.vikparuchuri.com/blog/inverting-your-very-own-matrix/

    //Checks Matrix to see if only zeros exist at or below row in column
    public static bool checkforAllZeros(this double[,] thisMatrix, int row, int column, out double sum, out int firstnonzeroindex) {
      sum = 0;
      firstnonzeroindex = -1;
      for (int i = row; i < thisMatrix.GetLength(0); i++) {
        sum += thisMatrix[i, column];
        if (firstnonzeroindex == -1 && sum != 0) {
          firstnonzeroindex = i;
        }
      }
      return sum == 0 ? true : false;
    }

    //Inverts this Matrix using Gauss-Jordan Elimination
    //THIS FUNCTION IS BROKEN
    public static double[,] inverse(this double[,] thisMatrix) {
      //Add an Identity Matrix on to the side of this Matrix
      double[,] Inversion = thisMatrix.concatenate(makeIdentity(thisMatrix.GetLength(0), thisMatrix.GetLength(1)));

      //Begin Converting left Matrix to Row-Echelon form
      int i = 0;
      for (int j = 0; j < thisMatrix.GetLength(1); j++) {
        //Debug.Log ("On col: "+j+" and row: "+i);

        double sum;
        int firstnonzeroindex;
        if (Inversion.checkforAllZeros(i, j, out sum, out firstnonzeroindex)) {
          if (j == thisMatrix.GetLength(1)) {
            Debug.Log("wtf is this");
            return Inversion;
          }
          Debug.LogError("Matrix isn't invertible.");
        } else {
          if (firstnonzeroindex != i) {
            Inversion = Inversion.swapRow(i, firstnonzeroindex);
          }

          for (int k = 0; k < Inversion.GetLength(1); k++) {
            double Normalizer = Inversion[i, j];
            Inversion[i, k] /= Normalizer;
          }

          for (int q = 0; q < Inversion.GetLength(0); q++) {
            if (q != i) {
              double scale = Inversion[q, j];
              for (int k = 0; k < Inversion.GetLength(1); k++) {
                Inversion[q, k] -= scale * Inversion[i, k];
              }
            }
          }

          if (i == thisMatrix.GetLength(0) || j == thisMatrix.GetLength(1)) {
            break;
          }
          i += 1;
        }
      }

      //Trim off the husk Identity left over from the Inversion
      Inversion = Inversion.deconcatenate(thisMatrix.GetLength(1));

      return Inversion;
    }
    //------------------------------------------------------

    //Copied from Rosetta Code
    //https://rosettacode.org/wiki/Cholesky_decomposition#C.23
    //
    public static double[,] cholesky(this double[,] thisMatrix) {
      int n = (int)Mathf.Sqrt(thisMatrix.Length);

      double[,] ret = new double[n, n];
      for (int r = 0; r < n; r++)
        for (int c = 0; c <= r; c++) {
          if (c == r) {
            double sum = 0;
            for (int j = 0; j < c; j++) {
              sum += ret[c, j] * ret[c, j];
            }
            ret[c, c] = Math.Sqrt(thisMatrix[c, c] - sum);
          } else {
            double sum = 0;
            for (int j = 0; j < c; j++)
              sum += ret[r, j] * ret[c, j];
            ret[r, c] = 1.0f / ret[c, c] * (thisMatrix[r, c] - sum);
          }
        }
      return ret;
    }

    //Prints this Matrix out all pretty-like
    public static string MatrixToString(this double[,] thisMatrix) {
      string MatrixString = "[ ";
      for (int i = 0; i < thisMatrix.GetLength(0); i++) {
        for (int j = 0; j < thisMatrix.GetLength(1); j++) {
          MatrixString += thisMatrix[i, j];
          if (i == thisMatrix.GetLength(0) - 1 && j == thisMatrix.GetLength(1) - 1) {
            MatrixString += " ]";
          } else if (j == thisMatrix.GetLength(1) - 1) {
            MatrixString += ",\n  ";
          } else {
            MatrixString += ", ";
          }
        }
      }
      return MatrixString;
    }
  }
}