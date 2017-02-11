using UnityEngine;

//To write a Kalman Filter, one must first invent Linear Algebra
public struct Matrix {
  public float[,] values;
  public int width;
  public int height;

  //Makes a square identity matrix with this dimension
  public Matrix(int inwidth) {
    values = new float[inwidth, inwidth];
    height = inwidth;
    width = inwidth;

    //Set the diagonals to 1
    for (int i = 0; i < width; i++) {
      values[i, i] = 1f;
    }
  }

  //Makes an Empty (all 0's) matrix with these dimensions
  public Matrix(int inwidth, int inheight, bool Identity = false) {
    values = new float[inheight, inwidth];
    height = inheight;
    width = inwidth;

    //If it should be an identity matrix, or an empty matrix
    if (Identity) {
      //Set the diagonals to 1
      for (int i = 0; i < (width >= height ? height : width); i++) {
        values[i, i] = 1f;
      }
    }
  }

  //Initializes a Matrix with this 2D Array (does a deep copy)
  public Matrix(float[,] invalues) {
    values = (float[,])invalues.Clone();
    width = invalues.GetLength(1);
    height = invalues.GetLength(0);
  }

  //Multiplies this Matrix by another Matrix
  public Matrix mult(Matrix inMatrix) {
    if (width == inMatrix.height) {
      Matrix toReturn = new Matrix(new float[height, inMatrix.width]);
      for (int i = 0; i < height; i++) { //i is the row in this matrix
        for (int j = 0; j < inMatrix.width; j++) {//j is the column in the other matrix
          for (int k = 0; k < width; k++) {//k is the column in this matrix
            toReturn.values[i, j] += values[i, k] * inMatrix.values[k, j];
          }
        }
      }
      return toReturn;
    } else {
      return this;
    }
  }

  //Returns a Vector3 column of a 3x3 matrix
  public Vector3 col(int i) {
    if (height == 3 && width == 3) {
      return new Vector3(values[0, i], values[1, i], values[2, i]);
    }else {
      Debug.LogError("NOT 3x3 MATRIX");
      return Vector3.one;
    }
  }

  //Returns a Vector3 row of a 3x3 matrix
  public Vector3 row(int i) {
    if (height == 3 && width == 3) {
      return new Vector3(values[i, 0], values[i, 1], values[i, 2]);
    } else {
      Debug.LogError("NOT 3x3 MATRIX");
      return Vector3.one;
    }
  }

  //Multiplies this Matrix by a Scalar
  public Matrix mult(float inScalar) {
    Matrix toReturn = new Matrix(values);
    for (int i = 0; i < height; i++) { //i is the row in this matrix
      for (int j = 0; j < width; j++) {//j is the column in the this matrix
        toReturn.values[i, j] *= inScalar;
      }
    }
    return toReturn;
  }

  //Multiplies this Matrix by a Scalar
  public Matrix sqrt() {
    Matrix toReturn = new Matrix(values);
    for (int i = 0; i < height; i++) { //i is the row in this matrix
      for (int j = 0; j < width; j++) {//j is the column in the this matrix
        toReturn.values[i, j] = Mathf.Sqrt(toReturn.values[i, j]);
      }
    }
    return toReturn;
  }

  //Adds this Matrix to another Matrix
  public Matrix add(Matrix inMatrix) {
    if (width == inMatrix.width && height == inMatrix.height) {
      Matrix toReturn = new Matrix(inMatrix.values);
      for (int i = 0; i < height; i++) { //i is the row in this matrix
        for (int j = 0; j < width; j++) {//j is the column in the other matrix
          toReturn.values[i, j] += values[i, j];
        }
      }
      return toReturn;
    } else {
      return this;
    }
  }

  //Subtracts another Matrix from this Matrix
  public Matrix sub(Matrix inMatrix) {
    if (width == inMatrix.width && height == inMatrix.height) {
      Matrix toReturn = new Matrix(values);
      for (int i = 0; i < height; i++) { //i is the row in this matrix
        for (int j = 0; j < width; j++) {//j is the column in the other matrix
          toReturn.values[i, j] -= inMatrix.values[i, j];
        }
      }
      return toReturn;
    } else {
      return this;
    }
  }

  //Transposes this Matrix
  public Matrix transpose() {
    Matrix toReturn = new Matrix(new float[width, height]);
    for (int i = 0; i < height; i++) { //i is the row in this matrix
      for (int j = 0; j < width; j++) {//j is the column in the other matrix
        toReturn.values[j, i] = values[i, j];
      }
    }
    return toReturn;
  }

  //Gloms another Matrix onto the side of this one
  public Matrix concatenate(Matrix inMatrix) {
    if (height == inMatrix.height) {
      Matrix toReturn = new Matrix(width + inMatrix.width, height);
      for (int i = 0; i < height; i++) { //i is the row in this matrix
        for (int j = 0; j < width; j++) {//j is the column in the this matrix
          toReturn.values[i, j] = values[i, j];
        }
        for (int j = inMatrix.width; j < inMatrix.width + width; j++) {
          toReturn.values[i, j] = inMatrix.values[i, j - width];
        }
      }
      return toReturn;
    } else {
      return this;
    }
  }

  //Trims away the excess identity Matrix once you're done inverting it
  public Matrix deconcatenate(int startColumn) {
    Matrix toReturn = new Matrix(width - startColumn, height);
    for (int i = 0; i < height; i++) { //i is the row in this matrix
      for (int j = startColumn; j < width; j++) {//j is the column in the this matrix
        toReturn.values[i, j - startColumn] = values[i, j];
      }
    }
    return toReturn;
  }

  //Swaps two rows in this Matrix
  public Matrix swapRow(int row1, int row2) {
    Matrix toReturn = new Matrix(values);
    float[] tempRow = new float[width];
    for (int i = 0; i < width; i++) {
      tempRow[i] = toReturn.values[row2, i];
      toReturn.values[row2, i] = toReturn.values[row1, i];
      toReturn.values[row1, i] = tempRow[i];
    }
    return toReturn;
  }

  //Code ported from Vik's Blog on Matrix Inversion ----
  //http://www.vikparuchuri.com/blog/inverting-your-very-own-matrix/

  //Checks Matrix to see if only zeros exist at or below row in column
  public bool checkforAllZeros(int row, int column, out float sum, out int firstnonzeroindex) {
    sum = 0;
    firstnonzeroindex = -1;
    for (int i = row; i < height; i++) {
      sum += values[i, column];
      if (firstnonzeroindex == -1 && sum != 0) {
        firstnonzeroindex = i;
      }
    }
    return sum == 0 ? true : false;
  }

  //Inverts this Matrix using Gauss-Jordan Elimination
  public Matrix invert() {
    //Add an Identity Matrix on to the side of this Matrix
    Matrix Inversion = concatenate(new Matrix(width, height, true));

    //Begin Converting left Matrix to Row-Echelon form
    int i = 0;
    for (int j = 0; j < width; j++) {
      //Debug.Log ("On col: "+j+" and row: "+i);

      float sum;
      int firstnonzeroindex;
      if (checkforAllZeros(i, j, out sum, out firstnonzeroindex)) {
        if (j == width) {
          Debug.Log("wtf is this");
          return Inversion;
        }
        Debug.LogError("Matrix isn't invertible.");
      } else {
        if (firstnonzeroindex != i) {
          Inversion = Inversion.swapRow(i, firstnonzeroindex);
        }

        for (int k = 0; k < Inversion.width; k++) {
          float Normalizer = Inversion.values[i, j];
          Inversion.values[i, k] /= Normalizer;
        }

        for (int q = 0; q < Inversion.height; q++) {
          if (q != i) {
            float scale = Inversion.values[q, j];
            for (int k = 0; k < Inversion.width; k++) {
              Inversion.values[q, k] -= scale * Inversion.values[i, k];
            }
          }
        }

        if (i == height || j == width) {
          break;
        }
        i += 1;
      }
    }

    //Trim off the husk Identity left over from the Inversion
    Inversion = Inversion.deconcatenate(width);

    return Inversion;
  }
  //------------------------------------------------------

  //Copied from Rosetta Code
  //https://rosettacode.org/wiki/Cholesky_decomposition#C.23
  //
  public Matrix cholesky() {
    int n = (int)Mathf.Sqrt(values.Length);

    float[,] ret = new float[n, n];
    for (int r = 0; r < n; r++)
      for (int c = 0; c <= r; c++) {
        if (c == r) {
          float sum = 0;
          for (int j = 0; j < c; j++) {
            sum += ret[c, j] * ret[c, j];
          }
          ret[c, c] = Mathf.Sqrt(values[c, c] - sum);
        } else {
          float sum = 0;
          for (int j = 0; j < c; j++)
            sum += ret[r, j] * ret[c, j];
          ret[r, c] = 1.0f / ret[c, c] * (values[r, c] - sum);
        }
      }
    return new Matrix(ret);
  }

  //Prints this Matrix out all pretty-like
  public override string ToString() {
    string MatrixString = "[ ";
    for (int i = 0; i < height; i++) {
      for (int j = 0; j < width; j++) {
        MatrixString += values[i, j];
        if (i == height - 1 && j == width - 1) {
          MatrixString += " ]";
        } else if (j == width - 1) {
          MatrixString += ",\n  ";
        } else {
          MatrixString += ", ";
        }
      }
    }
    return MatrixString;
  }
}