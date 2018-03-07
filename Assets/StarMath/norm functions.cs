// ***********************************************************************
// Assembly         : StarMath
// Author           : MICampbell
// Created          : 05-14-2015
//
// Last Modified By : MICampbell
// Last Modified On : 07-07-2015
// ***********************************************************************
// <copyright file="norm functions.cs" company="Design Engineering Lab -- MICampbell">
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
        #region Norm Functions

        #region 1-norm (Manhattan Distance)

        /// <summary>
        /// Returns to 1-norm (sum of absolute values of all terms)
        /// of the vector, x.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <returns>Scalar value of 1-norm.</returns>
        /// <exception cref="System.ArithmeticException">The vector, x, is null.</exception>
        public static double norm1(this IEnumerable<double> x)
        {
            if (x == null) throw new ArithmeticException("The vector, x, is null.");
            return x.Sum(a => Math.Abs(a));
        }

        /// <summary>
        /// Returns to 1-norm (sum of absolute values of all terms)
        /// of the vector, x.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <returns>Scalar value of 1-norm.</returns>
        /// <exception cref="System.ArithmeticException">The vector, x, is null.</exception>
        public static int norm1(this IEnumerable<int> x)
        {
            if (x == null) throw new ArithmeticException("The vector, x, is null.");
            return x.Sum(a => Math.Abs(a));
        }

        /// <summary>
        /// Returns to 1-norm (sum of absolute values of all terms)
        /// of the difference between x and y.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="y">The vector, y.</param>
        /// <returns>Scalar value of 1-norm.</returns>
        /// <exception cref="System.ArithmeticException">The vector, x, is null.
        /// or
        /// The vector, y, is null.
        /// or
        /// The vectors are not the same size.</exception>
        public static double norm1(this IList<double> x, IList<double> y)
        {
            var xlength = x.Count();
            if (x == null) throw new ArithmeticException("The vector, x, is null.");
            if (y == null) throw new ArithmeticException("The vector, y, is null.");
            if (xlength != y.Count()) throw new ArithmeticException("The vectors are not the same size.");
            return norm1(x, y, xlength);
        }

        /// <summary>
        /// Returns to 1-norm (sum of absolute values of all terms)
        /// of the difference between x and y.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="y">The vector, y.</param>
        /// <param name="length">The length of the vector.</param>
        /// <returns>Scalar value of 1-norm.</returns>
        public static double norm1(this IList<double> x, IList<double> y, int length)
        {
            var norm = 0.0;
            for (var i = 0; i != length; i++)
                norm += Math.Abs(x[i] - y[i]);
            return norm;
        }

        /// <summary>
        /// Returns to 1-norm (sum of absolute values of all terms)
        /// of the difference between x and y.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="y">The vector, y.</param>
        /// <returns>Scalar value of 1-norm.</returns>
        /// <exception cref="System.ArithmeticException">The vector, x, is null.
        /// or
        /// The vector, y, is null.
        /// or
        /// The vectors are not the same size.</exception>
        public static int norm1(this IList<int> x, IList<int> y)
        {
            var xlength = x.Count();
            if (x == null) throw new ArithmeticException("The vector, x, is null.");
            if (y == null) throw new ArithmeticException("The vector, y, is null.");
            if (xlength != y.Count()) throw new ArithmeticException("The vectors are not the same size.");
            return norm1(x, y, xlength);
        }

        /// <summary>
        /// Returns to 1-norm (sum of absolute values of all terms)
        /// of the difference between x and y.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="y">The vector, y.</param>
        /// <param name="length">The length of the vector.</param>
        /// <returns>Scalar value of 1-norm.</returns>
        public static int norm1(this IList<int> x, IList<int> y, int length)
        {
            var norm = 0;
            for (var i = 0; i != length; i++)
                norm += Math.Abs(x[i] - y[i]);
            return norm;
        }

        /// <summary>
        /// Returns to 1-norm (sum of absolute values of all terms)
        /// of the matrix, A.
        /// </summary>
        /// <param name="a">The matrix, A.</param>
        /// <returns>Scalar value of 1-norm.</returns>
        /// <exception cref="System.ArithmeticException">The matrix, A, is null.</exception>
        public static double norm1(this double[,] a)
        {
            if (a == null) throw new ArithmeticException("The matrix, A, is null.");
            return norm1(a, a.GetLength(0), a.GetLength(1));
        }

        /// <summary>
        /// Returns to 1-norm (sum of absolute values of all terms)
        /// of the matrix, A.
        /// </summary>
        /// <param name="a">The matrix, A.</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>Scalar value of 1-norm.</returns>
        public static double norm1(this double[,] a, int numRows, int numCols)
        {
            var norm = 0.0;
            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    norm += Math.Abs(a[i, j]);
            return norm;
        }


        /// <summary>
        /// Returns to 1-norm (sum of absolute values of all terms)
        /// of the matrix, A.
        /// </summary>
        /// <param name="a">The matrix, A.</param>
        /// <returns>Scalar value of 1-norm.</returns>
        /// <exception cref="System.ArithmeticException">The matrix, A, is null.</exception>
        public static int norm1(this int[,] a)
        {
            if (a == null) throw new ArithmeticException("The matrix, A, is null.");
            return norm1(a, a.GetLength(0), a.GetLength(1));
        }

        /// <summary>
        /// Returns to 1-norm (sum of absolute values of all terms)
        /// of the matrix, A.
        /// </summary>
        /// <param name="a">The matrix, A.</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>Scalar value of 1-norm.</returns>
        public static int norm1(this int[,] a, int numRows, int numCols)
        {
            var norm = 0;
            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    norm += Math.Abs(a[i, j]);
            return norm;
        }

        #endregion

        #region 2-norm (Euclidian Distance)

        /// <summary>
        /// Returns to 2-norm (square root of the sum of squares of all terms)
        /// of the difference between x and y.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="y">The vector, y.</param>
        /// <param name="dontDoSqrt">if set to <c>true</c> [don't take the square root].</param>
        /// <returns>Scalar value of 2-norm.</returns>
        /// <exception cref="System.ArithmeticException">The vector, x, is null.
        /// or
        /// The vector, y, is null.
        /// or
        /// The vectors are not the same size.</exception>
        public static double norm2(this IList<double> x, IList<double> y, Boolean dontDoSqrt = false)
        {
            var xlength = x.Count();
            if (x == null) throw new ArithmeticException("The vector, x, is null.");
            if (y == null) throw new ArithmeticException("The vector, y, is null.");
            if (xlength != y.Count()) throw new ArithmeticException("The vectors are not the same size.");
            return norm2(x, y, xlength, dontDoSqrt);
        }

        /// <summary>
        /// Returns to 2-norm (square root of the sum of squares of all terms)
        /// of the difference between x and y.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="y">The vector, y.</param>
        /// <param name="length">The length of the vectors.</param>
        /// <param name="dontDoSqrt">if set to <c>true</c> [don't take the square root].</param>
        /// <returns>Scalar value of 2-norm.</returns>
        public static double norm2(this IList<double> x, IList<double> y, int length, Boolean dontDoSqrt = false)
        {
            var norm = 0.0;
            for (var i = 0; i != length; i++)
                norm += (x[i] - y[i])*(x[i] - y[i]);
            return dontDoSqrt ? norm : Math.Sqrt(norm);
        }


        /// <summary>
        /// Returns to 2-norm (square root of the sum of squares of all terms)
        /// of the difference between x and y.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="y">The vector, y.</param>
        /// <param name="dontDoSqrt">if set to <c>true</c> [don't take the square root].</param>
        /// <returns>Scalar value of 2-norm.</returns>
        /// <exception cref="System.ArithmeticException">The vector, x, is null.
        /// or
        /// The vector, y, is null.
        /// or
        /// The vectors are not the same size.</exception>
        public static double norm2(this IList<int> x, IList<int> y, Boolean dontDoSqrt = false)
        {
            var xlength = x.Count();
            if (x == null) throw new ArithmeticException("The vector, x, is null.");
            if (y == null) throw new ArithmeticException("The vector, y, is null.");
            if (xlength != y.Count()) throw new ArithmeticException("The vectors are not the same size.");
            return norm2(x, y, xlength, dontDoSqrt);
        }

        /// <summary>
        /// Returns to 2-norm (square root of the sum of squares of all terms)
        /// of the difference between x and y.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="y">The vector, y.</param>
        /// <param name="length">The length of the vectors.</param>
        /// <param name="dontDoSqrt">if set to <c>true</c> [don't take the square root].</param>
        /// <returns>Scalar value of 2-norm.</returns>
        public static double norm2(this IList<int> x, IList<int> y, int length, Boolean dontDoSqrt = false)
        {
            var norm = 0.0;
            for (var i = 0; i != length; i++)
                norm += (x[i] - y[i])*(x[i] - y[i]);
            return dontDoSqrt ? norm : Math.Sqrt(norm);
        }

        /// <summary>
        /// Returns to 2-norm (square root of the sum of squares of all terms)
        /// of the vector, x.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="dontDoSqrt">if set to <c>true</c> [don't take the square root].</param>
        /// <returns>Scalar value of 2-norm.</returns>
        /// <exception cref="System.ArithmeticException">The vector, x, is null.</exception>
        public static double norm2(this IEnumerable<double> x, Boolean dontDoSqrt = false)
        {
            if (x == null) throw new ArithmeticException("The vector, x, is null.");
            return dontDoSqrt ? x.Sum(a => a*a) : Math.Sqrt(x.Sum(a => a*a));
        }


        /// <summary>
        /// Returns to 2-norm (square root of the sum of squares of all terms)
        /// of the vector, x.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="dontDoSqrt">if set to <c>true</c> [don't take the square root].</param>
        /// <returns>Scalar value of 2-norm.</returns>
        /// <exception cref="System.ArithmeticException">The vector, x, is null.</exception>
        public static double norm2(this IEnumerable<int> x, Boolean dontDoSqrt = false)
        {
            if (x == null) throw new ArithmeticException("The vector, x, is null.");
            return dontDoSqrt ? x.Sum(a => a*a) : Math.Sqrt(x.Sum(a => a*a));
        }


        /// <summary>
        /// Returns to 2-norm (square root of the sum of squares of all terms)
        /// of the matrix, A.
        /// </summary>
        /// <param name="a">The matrix, A.</param>
        /// <param name="dontDoSqrt">if set to <c>true</c> [don't take the square root].</param>
        /// <returns>Scalar value of 2-norm.</returns>
        /// <exception cref="System.ArithmeticException">The matrix, A, is null.</exception>
        public static double norm2(this double[,] a, Boolean dontDoSqrt = false)
        {
            if (a == null) throw new ArithmeticException("The matrix, A, is null.");
            return norm2(a, a.GetLength(0), a.GetLength(1), dontDoSqrt);
        }

        /// <summary>
        /// Returns to 2-norm (square root of the sum of squares of all terms)
        /// of the matrix, A.
        /// </summary>
        /// <param name="a">The matrix, A.</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <param name="dontDoSqrt">if set to <c>true</c> [don't take the square root].</param>
        /// <returns>Scalar value of 2-norm.</returns>
        public static double norm2(this double[,] a, int numRows, int numCols, Boolean dontDoSqrt = false)
        {
            var norm = 0.0;
            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    norm += (a[i, j]*a[i, j]);
            return dontDoSqrt ? norm : Math.Sqrt(norm);
        }

        /// <summary>
        /// Returns to 2-norm (square root of the sum of squares of all terms)
        /// of the matrix, A.
        /// </summary>
        /// <param name="a">The matrix, A.</param>
        /// <param name="dontDoSqrt">if set to <c>true</c> [don't take the square root].</param>
        /// <returns>Scalar value of 2-norm.</returns>
        /// <exception cref="System.ArithmeticException">The matrix, A, is null.</exception>
        public static double norm2(this int[,] a, Boolean dontDoSqrt = false)
        {
            if (a == null) throw new ArithmeticException("The matrix, A, is null.");
            return norm2(a, a.GetLength(0), a.GetLength(1), dontDoSqrt);
        }

        /// <summary>
        /// Returns to 2-norm (square root of the sum of squares of all terms)
        /// of the matrix, A.
        /// </summary>
        /// <param name="a">The matrix, A.</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <param name="dontDoSqrt">if set to <c>true</c> [don't take the square root].</param>
        /// <returns>Scalar value of 2-norm.</returns>
        public static double norm2(this int[,] a, int numRows, int numCols, Boolean dontDoSqrt = false)
        {
            var norm = 0.0;
            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    norm += (a[i, j]*a[i, j]);
            return dontDoSqrt ? norm : Math.Sqrt(norm);
        }

        #endregion

        #region P-norm

        /// <summary>
        /// Returns to p-norm (p-root of the sum of each term raised to the p power)
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="p">The power, p.</param>
        /// <param name="dontDoPRoot">if set to <c>true</c> [don't take the P-root].</param>
        /// <returns>Scalar value of P-norm.</returns>
        /// <exception cref="System.ArithmeticException">The vector, x, is null.</exception>
        public static double normP(this IEnumerable<double> x, double p, Boolean dontDoPRoot = false)
        {
            if (x == null) throw new ArithmeticException("The vector, x, is null.");
            return dontDoPRoot ? x.Sum(a => Math.Pow(a, p)) : Math.Pow(x.Sum(a => a*a), 1/p);
        }

        /// <summary>
        /// Returns to p-norm (p-root of the sum of each term raised to the p power)
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="p">The power, p.</param>
        /// <param name="dontDoPRoot">if set to <c>true</c> [don't take the P-root].</param>
        /// <returns>Scalar value of P-norm.</returns>
        /// <exception cref="System.ArithmeticException">The vector, x, is null.</exception>
        public static double normP(this IEnumerable<int> x, double p, Boolean dontDoPRoot = false)
        {
            if (x == null) throw new ArithmeticException("The vector, x, is null.");
            return dontDoPRoot ? x.Sum(a => Math.Pow(a, p)) : Math.Pow(x.Sum(a => a*a), 1/p);
        }


        /// <summary>
        /// Returns to p-norm (p-root of the sum of each term raised to the p power)
        /// </summary>
        /// <param name="a">The matrix, A.</param>
        /// <param name="p">The power, p.</param>
        /// <param name="dontDoPRoot">if set to <c>true</c> [don't take the P-root].</param>
        /// <returns>Scalar value of P-norm.</returns>
        /// <exception cref="System.ArithmeticException">The matrix, A, is null.</exception>
        public static double normP(this double[,] a, double p, Boolean dontDoPRoot = false)
        {
            if (a == null) throw new ArithmeticException("The matrix, A, is null.");
            return normP(a, p, a.GetLength(0), a.GetLength(1), dontDoPRoot);
        }

        /// <summary>
        /// Returns to p-norm (p-root of the sum of each term raised to the p power)
        /// </summary>
        /// <param name="a">The matrix, A.</param>
        /// <param name="p">The power, p.</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <param name="dontDoPRoot">if set to <c>true</c> [don't take the P-root].</param>
        /// <returns>Scalar value of P-norm.</returns>
        public static double normP(this double[,] a, double p, int numRows, int numCols, Boolean dontDoPRoot = false)
        {
            var norm = 0.0;
            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    norm += Math.Pow(a[i, j], p);
            return dontDoPRoot ? norm : Math.Pow(norm, 1/p);
        }

        /// <summary>
        /// Returns to p-norm (p-root of the sum of each term raised to the p power)
        /// </summary>
        /// <param name="a">The matrix, A.</param>
        /// <param name="p">The power, p.</param>
        /// <param name="dontDoPRoot">if set to <c>true</c> [don't take the P-root].</param>
        /// <returns>Scalar value of P-norm.</returns>
        /// <exception cref="System.ArithmeticException">The matrix, A, is null.</exception>
        public static double normP(this int[,] a, double p, Boolean dontDoPRoot = false)
        {
            if (a == null) throw new ArithmeticException("The matrix, A, is null.");
            return normP(a, p, a.GetLength(0), a.GetLength(1), dontDoPRoot);
        }

        /// <summary>
        /// Returns to p-norm (p-root of the sum of each term raised to the p power)
        /// </summary>
        /// <param name="a">The matrix, A.</param>
        /// <param name="p">The power, p.</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <param name="dontDoPRoot">if set to <c>true</c> [don't take the P-root].</param>
        /// <returns>Scalar value of 2-norm.</returns>
        public static double normP(this int[,] a, double p, int numRows, int numCols, Boolean dontDoPRoot = false)
        {
            var norm = 0.0;
            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    norm += Math.Pow(a[i, j], p);
            return dontDoPRoot ? norm : Math.Pow(norm, 1/p);
        }

        #endregion

        #endregion

        #region Normalize

        /// <summary>
        /// Returns the normalized vector (has length (or 2-norm) of 1)
        /// for the vector, x.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <returns>unit vector.</returns>
        public static double[] normalize(this IList<double> x)
        {
            return normalize(x, x.Count());
        }

        /// <summary>
        /// Returns the normalized vector (has length (or 2-norm) of 1)
        /// for the vector, x.
        /// </summary>
        /// <param name="x">The vector, x.</param>
        /// <param name="length">The length of the vector.</param>
        /// <returns>unit vector.</returns>
        public static double[] normalize(this IList<double> x, int length)
        {
            return divide(x, norm2(x), length);
        }


        /// <summary>
        /// Destructively normalizes the vector x.
        /// </summary>
        /// <param name="x">The vector x.</param>
        /// <returns>IList&lt;System.Double&gt;.</returns>
        public static IList<double> normalizeInPlace(this double[] x)
        {
            return normalizeInPlace(x, x.GetLength(0));
        }

        /// <summary>
        /// Destructively normalizes the vector x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="length">The length.</param>
        /// <returns>IList&lt;System.Double&gt;.</returns>
        public static IList<double> normalizeInPlace(this IList<double> x, int length)
        {
            var magnitude = norm2(x);
            for (var i = 0; i < length; i++) x[i] /= magnitude;
            return x;
        }

        #endregion

        #region Sum

        /// <summary>
        /// Sum up all the elements of a given matrix
        /// </summary>
        /// <param name="b">Matrix (1D double) whose parameters need to be summed up</param>
        /// <returns>Returns the total (double)</returns>
        public static double SumAllElements(this IEnumerable<double> b)
        {
            return b.Sum();
        }

        /// <summary>
        /// Sum up all the elements of a given matrix
        /// </summary>
        /// <param name="b">Matrix (1D int) whose parameters need to be summed up</param>
        /// <returns>Returns the total (int)</returns>
        public static double SumAllElements(this IEnumerable<int> b)
        {
            return b.Sum();
        }

        /// <summary>
        /// Sum up all the elements of a given matrix
        /// </summary>
        /// <param name="b">Matrix (2D double) whose parameters need to be summed up</param>
        /// <returns>Returns the total (double)</returns>
        public static double SumAllElements(this double[,] b)
        {
            return SumAllElements(b, b.GetLength(0), b.GetLength(1));
        }

        /// <summary>
        /// Sum up all the elements of a given matrix
        /// </summary>
        /// <param name="b">Matrix (2D double) whose parameters need to be summed up</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of cols.</param>
        /// <returns>Returns the total (double)</returns>
        private static double SumAllElements(this double[,] b, int numRows, int numCols)
        {
            var sumValue = 0.0;
            for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    sumValue += b[i, j];
            return sumValue;
        }

        /// <summary>
        /// Sum up all the elements of a given matrix
        /// </summary>
        /// <param name="b">Matrix (2D double) whose parameters need to be summed up</param>
        /// <returns>Returns the total (int)</returns>
        public static double SumAllElements(this int[,] b)
        {
            return SumAllElements(b, b.GetLength(0), b.GetLength(1));
        }

        /// <summary>
        /// Sum up all the elements of a given matrix
        /// </summary>
        /// <param name="b">Matrix (2D double) whose parameters need to be summed up</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of cols.</param>
        /// <returns>Returns the total (double)</returns>
        private static int SumAllElements(this int[,] b, int numRows, int numCols)
        {
            var sumValue = 0;
            for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    sumValue += b[i, j];
            return sumValue;
        }

        /// <summary>
        /// Sums all of the rows into a 1D array.
        /// </summary>
        /// <returns>System.Double[].</returns>
        public static double[] SumAllRows(this double[,] A)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            var rowSums = new double[numRows];
            for (int i = 0; i < numRows; i++)
                rowSums[i] = A.SumRow(i,numCols);
            return rowSums;
        }
        /// <summary>
        /// Sums the row of the matrix.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Double.</returns>
        public static double SumRow(this double[,] A, int index)
        {
           return SumRow(A,index,A.GetLength(1));
        }
        /// <summary>
        /// Sums the values of a specified row.
        /// </summary>
        /// <returns>System.Double[].</returns>
        public static double SumRow(this double[,] A, int index, int numCols)
        {
            var sum = 0.0;
            for (var j = 0; j < numCols; j++)
                sum += A[index, j];
            return sum;
        }
        /// <summary>
        /// Sums all of the columns into a 1D array.
        /// </summary>
        /// <param name="A">a.</param>
        /// <returns>System.Double[].</returns>
        public static double[] SumAllColumns(this double[,] A)
        {
            var numCols = A.GetLength(1);
            var colSums = new double[numCols];
            for (int i = 0; i < numCols; i++)
                colSums[i] = SumColumn(A,i);
            return colSums;
        }
        /// <summary>
        /// Sums the values of a specified column.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Double[].</returns>
        public static double SumColumn(this double[,] A, int index)
        {
            return SumColumn(A, index, A.GetLength(0));
        }
        /// <summary>
        /// Sums the values of a specified column.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="index">The index.</param>
        /// <param name="numRows">The number rows.</param>
        /// <returns>System.Double[].</returns>
        public static double SumColumn(this double[,] A, int index, int numRows)
        {
            var sum = 0.0;
            for (var i = 0; i < numRows; i++)
                sum += A[i, index];
            return sum;
        }
        #endregion

        #region Standard Deviation

        /// <summary>
        /// Calculates the standard deviation assuming the whole population is provided (not sample st. dev.).
        /// </summary>
        /// <param name="A">An vector of integers, A.</param>
        /// <returns>System.Double.</returns>
        public static double standardDeviation(this IList<int> A)
        {
            var mean = A.Average();
            var variance = A.Sum(a => (a - mean)*(a - mean));
            return Math.Sqrt(variance/A.Count);
        }

        /// <summary>
        /// Calculates the standard deviation assuming the whole population is provided (not sample st. dev.).
        /// </summary>
        /// <param name="A">An vector of doubles, A.</param>
        /// <returns>System.Double.</returns>
        public static double standardDeviation(this IList<double> A)
        {
            var mean = A.Average();
            var variance = A.Sum(a => (a - mean)*(a - mean));
            return Math.Sqrt(variance/A.Count);
        }

        /// <summary>
        /// Calculates the standard deviation assuming the whole population is provided (not sample st. dev.).
        /// </summary>
        /// <param name="a">A matrix in integers, A.</param>
        /// <returns>System.Double.</returns>
        public static double standardDeviation(this int[,] a)
        {
            return standardDeviation(JoinMatrixColumnsIntoVector(a));
        }

        /// <summary>
        /// Calculates the standard deviation assuming the whole population is provided (not sample st. dev.).
        /// </summary>
        /// <param name="a">A matrix in doubles, A.</param>
        /// <returns>System.Double.</returns>
        public static double standardDeviation(this double[,] a)
        {
            return standardDeviation(JoinMatrixColumnsIntoVector(a));
        }

        #endregion
    }
}