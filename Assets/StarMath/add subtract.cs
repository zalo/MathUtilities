// ***********************************************************************
// Assembly         : StarMath
// Author           : MICampbell
// Created          : 05-14-2015
//
// Last Modified By : MICampbell
// Last Modified On : 07-07-2015
// ***********************************************************************
// <copyright file="add subtract multiply.cs" company="Design Engineering Lab -- MICampbell">
//     2014
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace StarMathLib
{
    public static partial class StarMath
    {
        #region Add Vector-to-Vector and Matrix-to-Matrix

        /// <summary>
        ///     Adds arrays A and B
        /// </summary>
        /// <param name="A">1D double array 1</param>
        /// <param name="B">1D double array 2</param>
        /// <returns>1D double array that contains sum of vectros A and B</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static double[] add(this IList<double> A, IList<double> B)
        {
            var length = A.Count;
            if (length != B.Count) throw new ArithmeticException("Matrix sizes do not match");
            return add(A, B, length);
        }

        /// <summary>
        ///     Adds arrays A and B
        /// </summary>
        /// <param name="A">1D int array 1</param>
        /// <param name="B">1D double array 2</param>
        /// <returns>1D double array that contains sum of vectros A and B</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static double[] add(this IList<int> A, IList<double> B)
        {
            var length = A.Count();
            if (length != B.Count()) throw new ArithmeticException("Matrix sizes do not match");
            return add(A, B, length);
        }

        /// <summary>
        ///     Adds arrays A and B
        /// </summary>
        /// <param name="A">1D int array 1</param>
        /// <param name="B">1D int array 2</param>
        /// <returns>1D integer array that contains sum of vectros A and B</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static int[] add(this IList<int> A, IList<int> B)
        {
            var length = A.Count();
            if (length != B.Count()) throw new ArithmeticException("Matrix sizes do not match");
            return add(A, B, length);
        }

        /// <summary>
        ///     Adds arrays A and B
        /// </summary>
        /// <param name="A">2D double array 1</param>
        /// <param name="B">2D double array 2</param>
        /// <returns>2D double array that contains sum of vectros A and B</returns>
        /// <exception cref="System.ArithmeticException">
        ///     Matrix row count do not match
        ///     or
        ///     Matrix column count do not match
        /// </exception>
        public static double[,] add(this double[,] A, double[,] B)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numRows != B.GetLength(0))
                throw new ArithmeticException("Matrix row count do not match");
            if (numCols != B.GetLength(1))
                throw new ArithmeticException("Matrix column count do not match");

            return add(A, B, numRows, numCols);
        }

        /// <summary>
        ///     Adds arrays A and B
        /// </summary>
        /// <param name="A">2D double array 1</param>
        /// <param name="B">2D int array 2</param>
        /// <returns>2D double array that contains sum of vectros A and B</returns>
        /// <exception cref="System.ArithmeticException">
        ///     Matrix row count do not match
        ///     or
        ///     Matrix column count do not match
        /// </exception>
        public static double[,] add(this int[,] A, double[,] B)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numRows != B.GetLength(0))
                throw new ArithmeticException("Matrix row count do not match");
            if (numCols != B.GetLength(1))
                throw new ArithmeticException("Matrix column count do not match");

            return add(A, B, numRows, numCols);
        }

        /// <summary>
        ///     Adds arrays A and B
        /// </summary>
        /// <param name="A">2D integer array 1</param>
        /// <param name="B">2D integer array 2</param>
        /// <returns>2D integer array that contains sum of vectros A and B</returns>
        /// <exception cref="System.ArithmeticException">
        ///     Matrix row count do not match
        ///     or
        ///     Matrix column count do not match
        /// </exception>
        public static int[,] add(this int[,] A, int[,] B)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numRows != B.GetLength(0))
                throw new ArithmeticException("Matrix row count do not match");
            if (numCols != B.GetLength(1))
                throw new ArithmeticException("Matrix column count do not match");

            return add(A, B, numRows, numCols);
        }


        /// <summary>
        ///     Adds arrays A and B
        /// </summary>
        /// <param name="A">1D double array 1</param>
        /// <param name="B">1D double array 2</param>
        /// <param name="length">The length of the array.</param>
        /// <returns>1D double array that contains sum of vectros A and B</returns>
        public static double[] add(this IList<double> A, IList<double> B, int length)
        {
            var c = new double[length];
            for (var i = 0; i != length; i++)
                c[i] = A[i] + B[i];
            return c;
        }

        /// <summary>
        ///     Adds arrays A and B
        /// </summary>
        /// <param name="A">1D int array 1</param>
        /// <param name="B">1D double array 2</param>
        /// <param name="length">The length of the array.</param>
        /// <returns>1D double array that contains sum of vectros A and B</returns>
        public static double[] add(this IList<int> A, IList<double> B, int length)
        {
            var c = new double[length];
            for (var i = 0; i != length; i++)
                c[i] = A[i] + B[i];
            return c;
        }

        /// <summary>
        ///     Adds arrays A and B
        /// </summary>
        /// <param name="A">1D int array 1</param>
        /// <param name="B">1D int array 2</param>
        /// <param name="length">The length of the array.</param>
        /// <returns>1D integer array that contains sum of vectros A and B</returns>
        public static int[] add(this IList<int> A, IList<int> B, int length)
        {
            var c = new int[length];
            for (var i = 0; i != length; i++)
                c[i] = A[i] + B[i];
            return c;
        }

        /// <summary>
        ///     Adds arrays A and B
        /// </summary>
        /// <param name="A">2D double array 1</param>
        /// <param name="B">2D double array 2</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>2D double array that contains sum of vectros A and B</returns>
        public static double[,] add(this double[,] A, double[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] + B[i, j];
            return C;
        }

        /// <summary>
        ///     Adds arrays A and B
        /// </summary>
        /// <param name="A">2D double array 1</param>
        /// <param name="B">2D int array 2</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>2D double array that contains sum of vectros A and B</returns>
        public static double[,] add(this int[,] A, double[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] + B[i, j];
            return C;
        }

        /// <summary>
        ///     Adds arrays A and B
        /// </summary>
        /// <param name="A">2D integer array 1</param>
        /// <param name="B">2D integer array 2</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>2D integer array that contains sum of vectros A and B</returns>
        public static int[,] add(this int[,] A, int[,] B, int numRows, int numCols)
        {
            var C = new int[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] + B[i, j];
            return C;
        }

        #endregion

        #region Subtract Vector-to-Vector and Matrix-to-Matrix

        /// <summary>
        ///     Subtracts one vector (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend vector, A (1D double)</param>
        /// <param name="B">The subtrahend vector, B (1D double)</param>
        /// <returns>Returns the difference vector, C (1D double)</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static double[] subtract(this IList<double> A, IList<double> B)
        {
            var length = A.Count;
            if (length != B.Count)
                throw new ArithmeticException("Matrix sizes do not match");
            return subtract(A, B, length);
        }

        /// <summary>
        ///     Subtracts one vector (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend vector, A (1D int)</param>
        /// <param name="B">The subtrahend vector, B (1D double)</param>
        /// <returns>Returns the difference vector, C (1D double)</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static double[] subtract(this IList<int> A, IList<double> B)
        {
            var length = A.Count;
            if (length != B.Count)
                throw new ArithmeticException("Matrix sizes do not match");
            return subtract(A, B, length);
        }

        /// <summary>
        ///     Subtracts one vector (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend vector, A (1D double)</param>
        /// <param name="B">The subtrahend vector, B (1D int)</param>
        /// <returns>Returns the difference vector, C (1D double)</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static double[] subtract(this IList<double> A, IList<int> B)
        {
            var length = A.Count;
            if (length != B.Count)
                throw new ArithmeticException("Matrix sizes do not match");
            return subtract(A, B, length);
        }

        /// <summary>
        ///     Subtracts one vector (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend vector, A (1D int)</param>
        /// <param name="B">The subtrahend vector, B (1D int)</param>
        /// <returns>Returns the difference vector, C (1D int)</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static int[] subtract(this IList<int> A, IList<int> B)
        {
            var length = A.Count;
            if (length != B.Count)
                throw new ArithmeticException("Matrix sizes do not match");
            return subtract(A, B, length);
        }

        /// <summary>
        ///     Subtracts one matrix (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend matrix, A (2D double).</param>
        /// <param name="B">The subtrahend matrix, B (2D double).</param>
        /// <returns>Returns the difference matrix, C (2D double)</returns>
        /// <exception cref="System.ArithmeticException">
        ///     Matrix row count do not match
        ///     or
        ///     Matrix column count do not match
        /// </exception>
        public static double[,] subtract(this double[,] A, double[,] B)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numRows != B.GetLength(0))
                throw new ArithmeticException("Matrix row count do not match");
            if (numCols != B.GetLength(1))
                throw new ArithmeticException("Matrix column count do not match");

            return subtract(A, B, numRows, numCols);
        }

        /// <summary>
        ///     Subtracts one matrix (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend matrix, A (2D int).</param>
        /// <param name="B">The subtrahend matrix, B (2D double).</param>
        /// <returns>Returns the difference matrix, C (2D double)</returns>
        /// <exception cref="System.ArithmeticException">
        ///     Matrix row count do not match
        ///     or
        ///     Matrix column count do not match
        /// </exception>
        public static double[,] subtract(this int[,] A, double[,] B)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numRows != B.GetLength(0))
                throw new ArithmeticException("Matrix row count do not match");
            if (numCols != B.GetLength(1))
                throw new ArithmeticException("Matrix column count do not match");

            return subtract(A, B, numRows, numCols);
        }

        /// <summary>
        ///     Subtracts one matrix (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend matrix, A (2D double).</param>
        /// <param name="B">The subtrahend matrix, B (2D int).</param>
        /// <returns>Returns the difference matrix, C (2D double)</returns>
        /// <exception cref="System.ArithmeticException">
        ///     Matrix row count do not match
        ///     or
        ///     Matrix column count do not match
        /// </exception>
        public static double[,] subtract(this double[,] A, int[,] B)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numRows != B.GetLength(0))
                throw new ArithmeticException("Matrix row count do not match");
            if (numCols != B.GetLength(1))
                throw new ArithmeticException("Matrix column count do not match");

            return subtract(A, B, numRows, numCols);
        }

        /// <summary>
        ///     Subtracts one matrix (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend matrix, A (2D integer).</param>
        /// <param name="B">The subtrahend matrix, B (2D integer).</param>
        /// <returns>Returns the difference matrix, C (2D integer)</returns>
        /// <exception cref="System.ArithmeticException">
        ///     Matrix row count do not match
        ///     or
        ///     Matrix column count do not match
        /// </exception>
        public static int[,] subtract(this int[,] A, int[,] B)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numRows != B.GetLength(0))
                throw new ArithmeticException("Matrix row count do not match");
            if (numCols != B.GetLength(1))
                throw new ArithmeticException("Matrix column count do not match");

            return subtract(A, B, numRows, numCols);
        }


        /// <summary>
        ///     Subtracts one vector (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend vector, A (1D double)</param>
        /// <param name="B">The subtrahend vector, B (1D double)</param>
        /// <param name="length">The length of the vectors.</param>
        /// <returns>Returns the difference vector, C (1D double)</returns>
        public static double[] subtract(this IList<double> A, IList<double> B, int length)
        {
            var c = new double[length];
            for (var i = 0; i != length; i++)
                c[i] = A[i] - B[i];
            return c;
        }

        /// <summary>
        ///     Subtracts one vector (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend vector, A (1D int)</param>
        /// <param name="B">The subtrahend vector, B (1D double)</param>
        /// <param name="length">The length of the vectors.</param>
        /// <returns>Returns the difference vector, C (1D double)</returns>
        public static double[] subtract(this IList<int> A, IList<double> B, int length)
        {
            var c = new double[length];
            for (var i = 0; i != length; i++)
                c[i] = A[i] - B[i];
            return c;
        }

        /// <summary>
        ///     Subtracts one vector (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend vector, A (1D double)</param>
        /// <param name="B">The subtrahend vector, B (1D int)</param>
        /// <param name="length">The length of the vectors.</param>
        /// <returns>Returns the difference vector, C (1D double)</returns>
        public static double[] subtract(this IList<double> A, IList<int> B, int length)
        {
            var c = new double[length];
            for (var i = 0; i != length; i++)
                c[i] = A[i] - B[i];
            return c;
        }

        /// <summary>
        ///     Subtracts one vector (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend vector, A (1D int)</param>
        /// <param name="B">The subtrahend vector, B (1D int)</param>
        /// <param name="length">The length of the vectors.</param>
        /// <returns>Returns the difference vector, C (1D int)</returns>
        public static int[] subtract(this IList<int> A, IList<int> B, int length)
        {
            var c = new int[length];
            for (var i = 0; i != length; i++)
                c[i] = A[i] - B[i];
            return c;
        }

        /// <summary>
        ///     Subtracts one matrix (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend matrix, A (2D double).</param>
        /// <param name="B">The subtrahend matrix, B (2D double).</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>Returns the difference matrix, C (2D double)</returns>
        public static double[,] subtract(this double[,] A, double[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] - B[i, j];
            return C;
        }

        /// <summary>
        ///     Subtracts one matrix (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend matrix, A (2D int).</param>
        /// <param name="B">The subtrahend matrix, B (2D double).</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>Returns the difference matrix, C (2D double)</returns>
        public static double[,] subtract(this int[,] A, double[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] - B[i, j];
            return C;
        }

        /// <summary>
        ///     Subtracts one matrix (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend matrix, A (2D double).</param>
        /// <param name="B">The subtrahend matrix, B (2D int).</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>Returns the difference matrix, C (2D double)</returns>
        public static double[,] subtract(this double[,] A, int[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] - B[i, j];
            return C;
        }

        /// <summary>
        ///     Subtracts one matrix (B) from the other (A). C = A - B.
        /// </summary>
        /// <param name="A">The minuend matrix, A (2D integer).</param>
        /// <param name="B">The subtrahend matrix, B (2D integer).</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>Returns the difference matrix, C (2D integer)</returns>
        public static int[,] subtract(this int[,] A, int[,] B, int numRows, int numCols)
        {
            var C = new int[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] - B[i, j];
            return C;
        }

        #endregion
    }
}