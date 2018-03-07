// ***********************************************************************
// Assembly         : StarMath
// Author           : MICampbell
// Created          : 05-14-2015
//
// Last Modified By : MICampbell
// Last Modified On : 07-16-2015
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
        #region Scalars multiplying vectors

        /// <summary>
        /// Multiplies all elements of a 1D double array with the double value.
        /// </summary>
        /// <param name="B">The double vector to be multiplied with</param>
        /// <param name="a">The double value to be multiplied</param>
        /// <returns>A 1D double array that contains the product</returns>
        public static double[] multiply(this IList<double> B, double a)
        {
            return multiply(a, B, B.Count());
        }

        /// <summary>
        /// Multiplies all elements of a 1D double array with the double value.
        /// </summary>
        /// <param name="B">The double vector to be multiplied with</param>
        /// <param name="a">The double value to be multiplied</param>
        /// <param name="length">The length of the vector.</param>
        /// <returns>A 1D double array that contains the product</returns>
        public static double[] multiply(this IList<double> B, double a, int length)
        {
            return multiply(a, B, length);
        }

        /// <summary>
        /// Multiplies all elements of a 1D double array with the double value.
        /// </summary>
        /// <param name="a">The double value to be multiplied</param>
        /// <param name="B">The double vector to be multiplied with</param>
        /// <returns>A 1D double array that contains the product</returns>
        public static double[] multiply(double a, IList<double> B)
        {
            return multiply(a, B, B.Count());
        }

        /// <summary>
        /// Multiplies all elements of a 1D double array with the double value.
        /// </summary>
        /// <param name="a">The double value to be multiplied</param>
        /// <param name="B">The double vector to be multiplied with</param>
        /// <param name="length">The length of the vector B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 1D double array that contains the product</returns>
        public static double[] multiply(double a, IList<double> B, int length)
        {
            // scale vector B by the amount of scalar a
            var c = new double[length];
            for (var i = 0; i != length; i++)
                c[i] = a*B[i];
            return c;
        }

        /* Note: We do not need need a scalar integer multiplied by a double vector
         * because the scalar integer can be automatically cast to a double and use 
         * the two functions above. */

        /// <summary>
        /// Multiplies all elements of a 1D integer array with the double value.
        /// </summary>
        /// <param name="B">The integer vector to be multiplied with</param>
        /// <param name="a">The double value to be multiplied</param>
        /// <returns>A 1D double array that contains the product</returns>
        public static double[] multiply(this IList<int> B, double a)
        {
            return multiply(a, B, B.Count());
        }

        /// <summary>
        /// Multiplies all elements of a 1D integer array with the double value.
        /// </summary>
        /// <param name="B">The integer vector to be multiplied with</param>
        /// <param name="a">The double value to be multiplied</param>
        /// <param name="length">The length of the vector.</param>
        /// <returns>A 1D double array that contains the product</returns>
        public static double[] multiply(this IList<int> B, double a, int length)
        {
            return multiply(a, B, length);
        }

        /// <summary>
        /// Multiplies all elements of a 1D integer array with the double value.
        /// </summary>
        /// <param name="a">The double value to be multiplied</param>
        /// <param name="B">The integer vector to be multiplied with</param>
        /// <returns>A 1D double array that contains the product</returns>
        public static double[] multiply(double a, IList<int> B)
        {
            return multiply(a, B, B.Count());
        }

        /// <summary>
        /// Multiplies all elements of a 1D integer array with the double value.
        /// </summary>
        /// <param name="a">The double value to be multiplied</param>
        /// <param name="B">The integer vector to be multiplied with</param>
        /// <param name="length">The length of the vector B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 1D double array that contains the product</returns>
        public static double[] multiply(double a, IList<int> B, int length)
        {
            // scale vector B by the amount of scalar a
            var c = new double[length];
            for (var i = 0; i != length; i++)
                c[i] = a*B[i];
            return c;
        }

        /// <summary>
        /// Multiplies all elements of a 1D integer array with the integer value.
        /// </summary>
        /// <param name="B">The integer vector to be multiplied with</param>
        /// <param name="a">The integer value to be multiplied</param>
        /// <returns>A 1D integer array that contains the product</returns>
        public static int[] multiply(this IList<int> B, int a)
        {
            return multiply(a, B, B.Count());
        }

        /// <summary>
        /// Multiplies all elements of a 1D integer array with the integer value.
        /// </summary>
        /// <param name="B">The integer vector to be multiplied with</param>
        /// <param name="a">The integer value to be multiplied</param>
        /// <param name="length">The length.</param>
        /// <returns>A 1D integer array that contains the product</returns>
        public static int[] multiply(this IList<int> B, int a, int length)
        {
            return multiply(a, B, length);
        }

        /// <summary>
        /// Multiplies all elements of a 1D integer array with the integer value.
        /// </summary>
        /// <param name="a">The integer value to be multiplied</param>
        /// <param name="B">The integer vector to be multiplied with</param>
        /// <returns>A 1D integer array that contains the product</returns>
        public static int[] multiply(int a, IList<int> B)
        {
            return multiply(a, B, B.Count());
        }

        /// <summary>
        /// Multiplies all elements of a 1D integer array with the integer value.
        /// </summary>
        /// <param name="a">The integer value to be multiplied</param>
        /// <param name="B">The integer vector to be multiplied with</param>
        /// <param name="length">The length of the vector B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 1D integer array that contains the product</returns>
        public static int[] multiply(int a, IList<int> B, int length)
        {
            // scale vector B by the amount of scalar a
            var c = new int[length];
            for (var i = 0; i != length; i++)
                c[i] = a*B[i];
            return c;
        }

        /// <summary>
        /// Divides all elements of a 1D double array by the double value.
        /// </summary>
        /// <param name="B">The vector to be divided</param>
        /// <param name="a">The double value to be divided by, the divisor.</param>
        /// <returns>A 1D double array that contains the product</returns>
        public static double[] divide(this IList<double> B, double a)
        {
            return multiply((1/a), B);
        }

        /// <summary>
        /// Divides all elements of a 1D double array by the double value.
        /// </summary>
        /// <param name="B">The vector to be divided</param>
        /// <param name="a">The double value to be divided by, the divisor.</param>
        /// <param name="length">The length of the vector B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 1D double array that contains the product</returns>
        public static double[] divide(this IList<double> B, double a, int length)
        {
            return multiply((1/a), B, length);
        }

        /// <summary>
        /// Divides all elements of a 1D integer array by the double value.
        /// </summary>
        /// <param name="B">The vector to be divided</param>
        /// <param name="a">The double value to be divided by, the divisor.</param>
        /// <returns>A 1D double array that contains the product</returns>
        public static double[] divide(this IList<int> B, double a)
        {
            return multiply((1/a), B);
        }

        /// <summary>
        /// Divides all elements of a 1D integer array by the double value.
        /// </summary>
        /// <param name="B">The vector to be divided</param>
        /// <param name="a">The double value to be divided by, the divisor.</param>
        /// <param name="length">The length of the vector B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 1D double array that contains the product</returns>
        public static double[] divide(this IList<int> B, double a, int length)
        {
            return multiply((1/a), B, length);
        }

        #endregion

        #region Scalars multiplying matrices

        /// <summary>
        /// Multiplies all elements of a 2D double array with a double value.
        /// </summary>
        /// <param name="B">The matrix to be multiplied with</param>
        /// <param name="a">The double value to be multiplied</param>
        /// <returns>A 2D double array that contains the product</returns>
        public static double[,] multiply(this double[,] B, double a)
        {
            return multiply(a, B, B.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Multiplies all elements of a 2D double array with a double value.
        /// </summary>
        /// <param name="a">The double value to be multiplied</param>
        /// <param name="B">The matrix to be multiplied with</param>
        /// <returns>A 2D double array that contains the product</returns>
        public static double[,] multiply(double a, double[,] B)
        {
            return multiply(a, B, B.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Multiplies all elements of a 2D double array with a double value.
        /// </summary>
        /// <param name="B">The matrix to be multiplied with</param>
        /// <param name="a">The double value to be multiplied</param>
        /// <param name="numRows">The number of rows. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <param name="numCols">The number of cols. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 2D double array that contains the product</returns>
        public static double[,] multiply(this double[,] B, double a, int numRows, int numCols)
        {
            return multiply(a, B, numRows, numCols);
        }

        /// <summary>
        /// Multiplies all elements of a 2D double array with a double value.
        /// </summary>
        /// <param name="a">The double value to be multiplied</param>
        /// <param name="B">The matrix to be multiplied with</param>
        /// <param name="numRows">The number of rows. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <param name="numCols">The number of cols. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 2D double array that contains the product</returns>
        public static double[,] multiply(double a, double[,] B, int numRows, int numCols)
        {
            var c = new double[numRows, numCols];
            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    c[i, j] = a*B[i, j];
            return c;
        }

        /// <summary>
        /// Multiplies all elements of a 2D int array with a double value.
        /// </summary>
        /// <param name="B">The matrix to be multiplied with</param>
        /// <param name="a">The double value to be multiplied</param>
        /// <returns>A 2D double array that contains the product</returns>
        public static double[,] multiply(this int[,] B, double a)
        {
            return multiply(a, B, B.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Multiplies all elements of a 2D int array with a double value.
        /// </summary>
        /// <param name="a">The double value to be multiplied</param>
        /// <param name="B">The matrix to be multiplied with</param>
        /// <returns>A 2D double array that contains the product</returns>
        public static double[,] multiply(double a, int[,] B)
        {
            return multiply(a, B, B.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Multiplies all elements of a 2D int array with a double value.
        /// </summary>
        /// <param name="B">The matrix to be multiplied with</param>
        /// <param name="a">The double value to be multiplied</param>
        /// <param name="numRows">The number of rows. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <param name="numCols">The number of cols. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 2D double array that contains the product</returns>
        public static double[,] multiply(this int[,] B, double a, int numRows, int numCols)
        {
            return multiply(a, B, numRows, numCols);
        }

        /// <summary>
        /// Multiplies all elements of a 2D int array with a double value.
        /// </summary>
        /// <param name="a">The double value to be multiplied</param>
        /// <param name="B">The matrix to be multiplied with</param>
        /// <param name="numRows">The number of rows. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <param name="numCols">The number of cols. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 2D double array that contains the product</returns>
        public static double[,] multiply(double a, int[,] B, int numRows, int numCols)
        {
            var c = new double[numRows, numCols];
            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    c[i, j] = a*B[i, j];
            return c;
        }

        /// <summary>
        /// Multiplies all elements of a 2D integer array with a integer value.
        /// </summary>
        /// <param name="B">The matrix to be multiplied with</param>
        /// <param name="a">The integer value to be multiplied</param>
        /// <returns>A 2D integer array that contains the product</returns>
        public static int[,] multiply(this int[,] B, int a)
        {
            return multiply(a, B, B.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Multiplies all elements of a 2D integer array with a integer value.
        /// </summary>
        /// <param name="a">The integer value to be multiplied</param>
        /// <param name="B">The matrix to be multiplied with</param>
        /// <returns>A 2D integer array that contains the product</returns>
        public static int[,] multiply(int a, int[,] B)
        {
            return multiply(a, B, B.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Multiplies all elements of a 2D integer array with a integer value.
        /// </summary>
        /// <param name="B">The matrix to be multiplied with</param>
        /// <param name="a">The integer value to be multiplied</param>
        /// <param name="numRows">The number of rows. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <param name="numCols">The number of cols. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 2D integer array that contains the product</returns>
        public static int[,] multiply(this int[,] B, int a, int numRows, int numCols)
        {
            return multiply(a, B, numRows, numCols);
        }

        /// <summary>
        /// Multiplies all elements of a 2D integer array with a integer value.
        /// </summary>
        /// <param name="a">The integer value to be multiplied</param>
        /// <param name="B">The matrix to be multiplied with</param>
        /// <param name="numRows">The number of rows. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <param name="numCols">The number of cols. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 2D integer array that contains the product</returns>
        public static int[,] multiply(int a, int[,] B, int numRows, int numCols)
        {
            var c = new int[numRows, numCols];
            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    c[i, j] = a*B[i, j];
            return c;
        }

        /// <summary>
        /// Divides all elements of a 2D double array by the double value.
        /// </summary>
        /// <param name="B">The matrix to be divided</param>
        /// <param name="a">The double value to be divided by, the divisor.</param>
        /// <returns>A 2D double array that contains the product</returns>
        public static double[,] divide(this double[,] B, double a)
        {
            return multiply((1/a), B);
        }

        /// <summary>
        /// Divides all elements of a 2D double array by the double value.
        /// </summary>
        /// <param name="B">The matrix to be divided</param>
        /// <param name="a">The double value to be divided by, the divisor.</param>
        /// <param name="numRows">The number of rows. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <param name="numCols">The number of cols. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 2D double array that contains the product</returns>
        public static double[,] divide(this double[,] B, double a, int numRows, int numCols)
        {
            return multiply((1/a), B, numRows, numCols);
        }

        /// <summary>
        /// Divides all elements of a 2D integer array by the double value.
        /// </summary>
        /// <param name="B">The matrix to be divided</param>
        /// <param name="a">The double value to be divided by, the divisor.</param>
        /// <returns>A 2D double array that contains the product</returns>
        public static double[,] divide(this int[,] B, double a)
        {
            return multiply((1/a), B);
        }

        /// <summary>
        /// Divides all elements of a 2D integer array by the double value.
        /// </summary>
        /// <param name="B">The matrix to be divided</param>
        /// <param name="a">The double value to be divided by, the divisor.</param>
        /// <param name="numRows">The number of rows. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <param name="numCols">The number of cols. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A 2D double array that contains the product</returns>
        public static double[,] divide(this int[,] B, double a, int numRows, int numCols)
        {
            return multiply((1/a), B, numRows, numCols);
        }

        #endregion

        #region Dot-product of vectors to vectors

        /// <summary>
        /// The dot product of the two 1D double vectors A and B
        /// </summary>
        /// <param name="A">1D double Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <returns>A double value that contains the dot product</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static double dotProduct(this IList<double> A, IList<double> B)
        {
            var length = A.Count();
            if (length != B.Count())
                throw new ArithmeticException("Matrix sizes do not match");
            return dotProduct(A, B, length);
        }

        /// <summary>
        /// The dot product of the two 1D double vectors A and B
        /// </summary>
        /// <param name="A">1D double Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <param name="length">The length of both vectors A and B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A double value that contains the dot product</returns>
        public static double dotProduct(this IList<double> A, IList<double> B, int length)
        {
            var c = 0.0;
            for (var i = 0; i != length; i++)
                c += A[i]*B[i];
            return c;
        }

        /// <summary>
        /// The dot product of the one 1D int vector and one 1D double vector
        /// </summary>
        /// <param name="A">1D int Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <returns>A double value that contains the dot product</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static double dotProduct(this IList<int> A, IList<double> B)
        {
            var length = A.Count();
            if (length != B.Count())
                throw new ArithmeticException("Matrix sizes do not match");
            return dotProduct(A, B, length);
        }

        /// <summary>
        /// The dot product of the one 1D int vector and one 1D double vector
        /// </summary>
        /// <param name="A">1D int Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <param name="length">The length of both vectors A and B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A double value that contains the dot product</returns>
        public static double dotProduct(this IList<int> A, IList<double> B, int length)
        {
            var c = 0.0;
            for (var i = 0; i != length; i++)
                c += A[i]*B[i];
            return c;
        }

        /// <summary>
        /// The dot product of the two 1D int vectors A and B
        /// </summary>
        /// <param name="A">1D int Array, A</param>
        /// <param name="B">1D int Array, B</param>
        /// <returns>A double value that contains the dot product</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static int dotProduct(this IList<int> A, IList<int> B)
        {
            // this is B dot term_i multiplication
            var length = A.Count();
            if (length != B.Count())
                throw new ArithmeticException("Matrix sizes do not match");
            return dotProduct(A, B, length);
        }

        /// <summary>
        /// The dot product of the two 1D int vectors A and B
        /// </summary>
        /// <param name="A">1D int Array, A</param>
        /// <param name="B">1D int Array, B</param>
        /// <param name="length">The length of both vectors A and B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A double value that contains the dot product</returns>
        public static int dotProduct(this IList<int> A, IList<int> B, int length)
        {
            var c = 0;
            for (var i = 0; i != length; i++)
                c += A[i]*B[i];
            return c;
        }

        #endregion

        #region Cross-product of vectors to vectors

        /// <summary>
        /// The cross product of two double-precision vectors, A and B,
        /// </summary>
        /// <param name="A">1D double Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <returns>A double value that contains the dot product</returns>
        /// <exception cref="System.ArithmeticException">Cross product is only possible for vectors of length: 1, 3, or 7</exception>
        public static double[] crossProduct(this IList<double> A, IList<double> B)
        {
            var Alength = A.Count();
            var Blength = B.Count();
            if ((Alength == 1) && (Blength == 1))
                return new[] {0.0};
            if ((Alength == 2) && (Blength == 2))
                return new[] {0.0, 0.0, crossProduct2(A, B)};
            if ((Alength == 3) && (Blength == 3))
                return crossProduct3(A, B);
            if ((Alength == 7) && (Blength == 7))
                return crossProduct7(A, B);
            throw new ArithmeticException("Cross product is only possible for vectors of length: 1, 3, or 7");
        }

        /// <summary>
        /// The cross product of an integer vector, A, and a double vector, B.
        /// </summary>
        /// <param name="A">1D integer Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <returns>A double value that contains the dot product</returns>
        /// <exception cref="System.ArithmeticException">Cross product is only possible for vectors of length: 1, 3, or 7</exception>
        public static double[] crossProduct(this IList<int> A, IList<double> B)
        {
            var Alength = A.Count();
            var Blength = B.Count();
            if ((Alength == 1) && (Blength == 1))
                return new[] {0.0};
            if ((Alength == 2) && (Blength == 2))
                return new[] {0.0, 0.0, crossProduct2(A, B)};
            if ((Alength == 3) && (Blength == 3))
                return crossProduct3(A, B);
            if ((Alength == 7) && (Blength == 7))
                return crossProduct7(A, B);
            throw new ArithmeticException("Cross product is only possible for vectors of length: 1, 3, or 7");
        }

        /// <summary>
        /// The cross product of an double vector, A, and a integer vector, B.
        /// </summary>
        /// <param name="A">1D double Array, A</param>
        /// <param name="B">1D integer Array, B</param>
        /// <returns>A double value that contains the dot product</returns>
        /// <exception cref="System.ArithmeticException">Cross product is only possible for vectors of length: 1, 3, or 7</exception>
        public static double[] crossProduct(this IList<double> A, IList<int> B)
        {
            var Alength = A.Count();
            var Blength = B.Count();
            if ((Alength == 1) && (Blength == 1))
                return new[] {0.0};
            if ((Alength == 2) && (Blength == 2))
                return new[] {0.0, 0.0, crossProduct2(A, B)};
            if ((Alength == 3) && (Blength == 3))
                return crossProduct3(A, B);
            if ((Alength == 7) && (Blength == 7))
                return crossProduct7(A, B);
            throw new ArithmeticException("Cross product is only possible for vectors of length: 1, 3, or 7");
        }

        /// <summary>
        /// The cross product of two integer vectors, A and B.
        /// </summary>
        /// <param name="A">1D integer Array, A</param>
        /// <param name="B">1D integer Array, B</param>
        /// <returns>A double value that contains the dot product</returns>
        /// <exception cref="System.ArithmeticException">Cross product is only possible for vectors of length: 1, 3, or 7</exception>
        public static int[] crossProduct(this IList<int> A, IList<int> B)
        {
            var Alength = A.Count();
            var Blength = B.Count();
            if ((Alength == 1) && (Blength == 1))
                return new[] {0};
            if ((Alength == 2) && (Blength == 2))
                return new[] {0, 0, crossProduct2(A, B)};
            if ((Alength == 3) && (Blength == 3))
                return crossProduct3(A, B);
            if ((Alength == 7) && (Blength == 7))
                return crossProduct7(A, B);
            throw new ArithmeticException("Cross product is only possible for vectors of length: 1, 3, or 7");
        }

        /// <summary>
        /// The cross product of two double vectors, A and B, which are of length, 2.
        /// In actuality, there is no cross-product for 2D. This is shorthand for 2D systems
        /// that are really simplifications of 3D. The returned scalar is actually the value in
        /// the third (read: z) direction.
        /// </summary>
        /// <param name="A">1D double Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArithmeticException">This cross product \shortcut\ is only used with 2D vectors to get the single value in the,
        ///                                 + would be, Z-direction.</exception>
        public static double crossProduct2(IList<double> A, IList<double> B)
        {
            if (((A.Count() == 2) && (B.Count() == 2))
                || ((A.Count() == 3) && (B.Count() == 3) && A[2] == 0.0 && B[2] == 0.0))
                return A[0]*B[1] - B[0]*A[1];
            throw new ArithmeticException("This cross product \"shortcut\" is only used with 2D vectors to get the single value in the,"
                                + "would be, Z-direction.");
        }

        /// <summary>
        /// The cross product of an integer vector, A, and a double vector, B, which are of length, 2.
        /// In actuality, there is no cross-product for 2D. This is shorthand for 2D systems
        /// that are really simplifications of 3D. The returned scalar is actually the value in
        /// the third (read: z) direction.
        /// </summary>
        /// <param name="A">1D integer Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArithmeticException">This cross product \shortcut\ is only used with 2D vectors to get the single value in the,
        ///                                 + would be, Z-direction.</exception>
        public static double crossProduct2(IList<int> A, IList<double> B)
        {
            if (((A.Count() == 2) && (B.Count() == 2))
                || ((A.Count() == 3) && (B.Count() == 3) && A[2] == 0.0 && B[2] == 0.0))
                return A[0]*B[1] - B[0]*A[1];
            throw new ArithmeticException("This cross product \"shortcut\" is only used with 2D vectors to get the single value in the,"
                                + "would be, Z-direction.");
        }

        /// <summary>
        /// The cross product of an double vector, A, and a integer vector, B, which are of length, 2.
        /// In actuality, there is no cross-product for 2D. This is shorthand for 2D systems
        /// that are really simplifications of 3D. The returned scalar is actually the value in
        /// the third (read: z) direction.
        /// </summary>
        /// <param name="A">1D double Array, A</param>
        /// <param name="B">1D integer Array, B</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArithmeticException">This cross product \shortcut\ is only used with 2D vectors to get the single value in the,
        ///                                 + would be, Z-direction.</exception>
        public static double crossProduct2(IList<double> A, IList<int> B)
        {
            if (((A.Count() == 2) && (B.Count() == 2))
                || ((A.Count() == 3) && (B.Count() == 3) && A[2] == 0.0 && B[2] == 0.0))
                return A[0]*B[1] - B[0]*A[1];
            throw new ArithmeticException("This cross product \"shortcut\" is only used with 2D vectors to get the single value in the,"
                                + "would be, Z-direction.");
        }

        /// <summary>
        /// The cross product of two integer vectors, A and B, which are of length, 2.
        /// In actuality, there is no cross-product for 2D. This is shorthand for 2D systems
        /// that are really simplifications of 3D. The returned scalar is actually the value in
        /// the third (read: z) direction.
        /// </summary>
        /// <param name="A">1D integer Array, A</param>
        /// <param name="B">1D integer Array, B</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.ArithmeticException">This cross product \shortcut\ is only used with 2D vectors to get the single value in the,
        ///                                 + would be, Z-direction.</exception>
        public static int crossProduct2(IList<int> A, IList<int> B)
        {
            if (((A.Count() == 2) && (B.Count() == 2))
                || ((A.Count() == 3) && (B.Count() == 3) && A[2] == 0.0 && B[2] == 0.0))
                return A[0]*B[1] - B[0]*A[1];
            throw new ArithmeticException("This cross product \"shortcut\" is only used with 2D vectors to get the single value in the,"
                                + "would be, Z-direction.");
        }

        /// <summary>
        /// Crosses the product7.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Double[].</returns>
        private static double[] crossProduct7(IList<double> A, IList<double> B)
        {
            return new[]
            {
                A[1]*B[3] - A[3]*B[1] + A[2]*B[6] - A[6]*B[2] + A[4]*B[5] - A[5]*B[4],
                A[2]*B[4] - A[4]*B[2] + A[3]*B[0] - A[0]*B[3] + A[5]*B[6] - A[6]*B[5],
                A[3]*B[5] - A[5]*B[3] + A[4]*B[1] - A[1]*B[4] + A[6]*B[0] - A[0]*B[6],
                A[4]*B[6] - A[6]*B[4] + A[5]*B[2] - A[2]*B[5] + A[0]*B[1] - A[1]*B[0],
                A[5]*B[0] - A[0]*B[5] + A[6]*B[3] - A[3]*B[6] + A[1]*B[2] - A[2]*B[1],
                A[6]*B[1] - A[1]*B[6] + A[0]*B[4] - A[4]*B[0] + A[2]*B[3] - A[3]*B[2],
                A[0]*B[2] - A[2]*B[0] + A[1]*B[5] - A[5]*B[1] + A[3]*B[4] - A[4]*B[3]
            };
        }

        /// <summary>
        /// Crosses the product7.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Double[].</returns>
        private static double[] crossProduct7(IList<int> A, IList<double> B)
        {
            return new[]
            {
                A[1]*B[3] - A[3]*B[1] + A[2]*B[6] - A[6]*B[2] + A[4]*B[5] - A[5]*B[4],
                A[2]*B[4] - A[4]*B[2] + A[3]*B[0] - A[0]*B[3] + A[5]*B[6] - A[6]*B[5],
                A[3]*B[5] - A[5]*B[3] + A[4]*B[1] - A[1]*B[4] + A[6]*B[0] - A[0]*B[6],
                A[4]*B[6] - A[6]*B[4] + A[5]*B[2] - A[2]*B[5] + A[0]*B[1] - A[1]*B[0],
                A[5]*B[0] - A[0]*B[5] + A[6]*B[3] - A[3]*B[6] + A[1]*B[2] - A[2]*B[1],
                A[6]*B[1] - A[1]*B[6] + A[0]*B[4] - A[4]*B[0] + A[2]*B[3] - A[3]*B[2],
                A[0]*B[2] - A[2]*B[0] + A[1]*B[5] - A[5]*B[1] + A[3]*B[4] - A[4]*B[3]
            };
        }

        /// <summary>
        /// Crosses the product7.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Double[].</returns>
        private static double[] crossProduct7(IList<double> A, IList<int> B)
        {
            return new[]
            {
                A[1]*B[3] - A[3]*B[1] + A[2]*B[6] - A[6]*B[2] + A[4]*B[5] - A[5]*B[4],
                A[2]*B[4] - A[4]*B[2] + A[3]*B[0] - A[0]*B[3] + A[5]*B[6] - A[6]*B[5],
                A[3]*B[5] - A[5]*B[3] + A[4]*B[1] - A[1]*B[4] + A[6]*B[0] - A[0]*B[6],
                A[4]*B[6] - A[6]*B[4] + A[5]*B[2] - A[2]*B[5] + A[0]*B[1] - A[1]*B[0],
                A[5]*B[0] - A[0]*B[5] + A[6]*B[3] - A[3]*B[6] + A[1]*B[2] - A[2]*B[1],
                A[6]*B[1] - A[1]*B[6] + A[0]*B[4] - A[4]*B[0] + A[2]*B[3] - A[3]*B[2],
                A[0]*B[2] - A[2]*B[0] + A[1]*B[5] - A[5]*B[1] + A[3]*B[4] - A[4]*B[3]
            };
        }

        /// <summary>
        /// Crosses the product7.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Int32[].</returns>
        private static int[] crossProduct7(IList<int> A, IList<int> B)
        {
            return new[]
            {
                A[1]*B[3] - A[3]*B[1] + A[2]*B[6] - A[6]*B[2] + A[4]*B[5] - A[5]*B[4],
                A[2]*B[4] - A[4]*B[2] + A[3]*B[0] - A[0]*B[3] + A[5]*B[6] - A[6]*B[5],
                A[3]*B[5] - A[5]*B[3] + A[4]*B[1] - A[1]*B[4] + A[6]*B[0] - A[0]*B[6],
                A[4]*B[6] - A[6]*B[4] + A[5]*B[2] - A[2]*B[5] + A[0]*B[1] - A[1]*B[0],
                A[5]*B[0] - A[0]*B[5] + A[6]*B[3] - A[3]*B[6] + A[1]*B[2] - A[2]*B[1],
                A[6]*B[1] - A[1]*B[6] + A[0]*B[4] - A[4]*B[0] + A[2]*B[3] - A[3]*B[2],
                A[0]*B[2] - A[2]*B[0] + A[1]*B[5] - A[5]*B[1] + A[3]*B[4] - A[4]*B[3]
            };
        }

        /// <summary>
        /// Crosses the product3.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Double[].</returns>
        private static double[] crossProduct3(IList<double> A, IList<double> B)
        {
            return new[]
            {
                A[1]*B[2] - B[1]*A[2],
                A[2]*B[0] - B[2]*A[0],
                A[0]*B[1] - B[0]*A[1]
            };
        }

        /// <summary>
        /// Crosses the product3.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Double[].</returns>
        private static double[] crossProduct3(IList<int> A, IList<double> B)
        {
            return new[]
            {
                A[1]*B[2] - B[1]*A[2],
                A[2]*B[0] - B[2]*A[0],
                A[0]*B[1] - B[0]*A[1]
            };
        }

        /// <summary>
        /// Crosses the product3.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Double[].</returns>
        private static double[] crossProduct3(IList<double> A, IList<int> B)
        {
            return new[]
            {
                A[1]*B[2] - B[1]*A[2],
                A[2]*B[0] - B[2]*A[0],
                A[0]*B[1] - B[0]*A[1]
            };
        }

        /// <summary>
        /// Crosses the product3.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Int32[].</returns>
        private static int[] crossProduct3(IList<int> A, IList<int> B)
        {
            return new[]
            {
                A[1]*B[2] - B[1]*A[2],
                A[2]*B[0] - B[2]*A[0],
                A[0]*B[1] - B[0]*A[1]
            };
        }

        #endregion

        #region Multiply vector by transpose of another vector.

        /// <summary>
        /// Multiply vector by transpose of another vector to create a matrix.
        /// Product of each element of array-1 (1D double) with each element of array-2 (1D double)
        /// C[i,j] = A[i] * B[j]
        /// </summary>
        /// <param name="A">1D double array - column vector (1 element per row)</param>
        /// <param name="B">1D double array - row vector (1 element column)</param>
        /// <returns>2D double array product matrix, value of element [i,j] = A[i] * B[j]</returns>
        public static double[,] multiplyVectorsIntoAMatrix(this IList<double> A, IList<double> B)
        {
            return multiplyVectorsIntoAMatrix(A, B, A.Count(), B.Count());
        }

        /// <summary>
        /// Multiply vector by transpose of another vector to create a matrix.
        /// Product of each element of array-1 (1D int) with each element of array-2 (1D double)
        /// C[i,j] = A[i] * B[j]
        /// </summary>
        /// <param name="A">1D integer array - column vector (1 element per row)</param>
        /// <param name="B">1D double array - row vector (1 element column)</param>
        /// <returns>2D double array product matrix, value of element [i,j] = A[i] * B[j]</returns>
        public static double[,] multiplyVectorsIntoAMatrix(this IList<int> A, IList<double> B)
        {
            return multiplyVectorsIntoAMatrix(A, B, A.Count(), B.Count());
        }

        /// <summary>
        /// Multiply vector by transpose of another vector to create a matrix.
        /// Product of each element of array-1 (1D int) with each element of array-2 (1D double)
        /// C[i,j] = A[i] * B[j]
        /// </summary>
        /// <param name="A">1D double array - column vector (1 element per row)</param>
        /// <param name="B">1D integer array - row vector (1 element column)</param>
        /// <returns>2D double array product matrix, value of element [i,j] = A[i] * B[j]</returns>
        public static double[,] multiplyVectorsIntoAMatrix(this IList<double> A, IList<int> B)
        {
            return multiplyVectorsIntoAMatrix(A, B, A.Count(), B.Count());
        }

        /// <summary>
        /// Multiply vector by transpose of another vector to create a matrix.
        /// Product of each element of array-1 (1D int) with each element of array-2 (1D int)
        /// C[i,j] = A[i] * B[j]
        /// </summary>
        /// <param name="A">1D integer array - column vector (1 element per row)</param>
        /// <param name="B">1D integer array - row vector (1 element column)</param>
        /// <returns>2D double array product matrix, value of element [i,j] = A[i] * B[j]</returns>
        public static int[,] multiplyVectorsIntoAMatrix(this IList<int> A, IList<int> B)
        {
            return multiplyVectorsIntoAMatrix(A, B, A.Count(), B.Count());
        }

        /// <summary>
        /// Multiply vector by transpose of another vector to create a matrix.
        /// Product of each element of array-1 (1D double) with each element of array-2 (1D double)
        /// C[i,j] = A[i] * B[j]
        /// </summary>
        /// <param name="A">1D double array - column vector (1 element per row)</param>
        /// <param name="B">1D double array - row vector (1 element column)</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of colimns.</param>
        /// <returns>2D double array product matrix, value of element [i,j] = A[i] * B[j]</returns>
        public static double[,] multiplyVectorsIntoAMatrix(this IList<double> A, IList<double> B,
            int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i]*B[j];
            return C;
        }

        /// <summary>
        /// Multiply vector by transpose of another vector to create a matrix.
        /// Product of each element of array-1 (1D int) with each element of array-2 (1D double)
        /// C[i,j] = A[i] * B[j]
        /// </summary>
        /// <param name="A">1D integer array - column vector (1 element per row)</param>
        /// <param name="B">1D double array - row vector (1 element column)</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of colimns.</param>
        /// <returns>2D double array product matrix, value of element [i,j] = A[i] * B[j]</returns>
        public static double[,] multiplyVectorsIntoAMatrix(this IList<int> A, IList<double> B,
            int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i]*B[j];
            return C;
        }

        /// <summary>
        /// Multiply vector by transpose of another vector to create a matrix.
        /// Product of each element of array-1 (1D int) with each element of array-2 (1D double)
        /// C[i,j] = A[i] * B[j]
        /// </summary>
        /// <param name="A">1D double array - column vector (1 element per row)</param>
        /// <param name="B">1D integer array - row vector (1 element column)</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of colimns.</param>
        /// <returns>2D double array product matrix, value of element [i,j] = A[i] * B[j]</returns>
        public static double[,] multiplyVectorsIntoAMatrix(this IList<double> A, IList<int> B,
            int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i]*B[j];
            return C;
        }

        /// <summary>
        /// Multiply vector by transpose of another vector to create a matrix.
        /// Product of each element of array-1 (1D int) with each element of array-2 (1D int)
        /// C[i,j] = A[i] * B[j]
        /// </summary>
        /// <param name="A">1D integer array - column vector (1 element per row)</param>
        /// <param name="B">1D integer array - row vector (1 element column)</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of colimns.</param>
        /// <returns>2D double array product matrix, value of element [i,j] = A[i] * B[j]</returns>
        public static int[,] multiplyVectorsIntoAMatrix(this IList<int> A, IList<int> B,
            int numRows, int numCols)
        {
            var C = new int[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i]*B[j];
            return C;
        }

        #endregion

        #region Matrix(2D) to matrix(2D) multiplication

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[,] multiply(this double[,] A, double[,] B)
        {
            if (A.GetLength(1) != B.GetLength(0))
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return multiply(A, B, A.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[,] multiply(this int[,] A, double[,] B)
        {
            if (A.GetLength(1) != B.GetLength(0))
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return multiply(A, B, A.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[,] multiply(this double[,] A, int[,] B)
        {
            if (A.GetLength(1) != B.GetLength(0))
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return multiply(A, B, A.GetLength(0), B.GetLength(1));
        }


        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <returns>A 2D int array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static int[,] multiply(this int[,] A, int[,] B)
        {
            if (A.GetLength(1) != B.GetLength(0))
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return multiply(A, B, A.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        public static double[,] multiply(this double[,] A, double[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                {
                    C[i, j] = 0.0;
                    for (var k = 0; k != A.GetLength(1); k++)
                        C[i, j] += A[i, k]*B[k, j];
                }
            return C;
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        public static double[,] multiply(this int[,] A, double[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                {
                    C[i, j] = 0.0;
                    for (var k = 0; k != A.GetLength(1); k++)
                        C[i, j] += A[i, k]*B[k, j];
                }
            return C;
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        public static double[,] multiply(this double[,] A, int[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                {
                    C[i, j] = 0.0;
                    for (var k = 0; k != A.GetLength(1); k++)
                        C[i, j] += A[i, k]*B[k, j];
                }
            return C;
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 2D int array that is the product of the two matrices A and B</returns>
        public static int[,] multiply(this int[,] A, int[,] B, int numRows, int numCols)
        {
            var C = new int[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                {
                    C[i, j] = 0;
                    for (var k = 0; k != A.GetLength(1); k++)
                        C[i, j] += A[i, k]*B[k, j];
                }
            return C;
        }

        #endregion

        #region Multiply matrix to a vector (and vice versa)

        /// <summary>
        /// Product of a matrix and a vector (2D double and 1D double)
        /// </summary>
        /// <param name="A">2D double Array</param>
        /// <param name="B">1D double array - column vector (1 element row)</param>
        /// <returns>A 1D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[] multiply(this double[,] A, IList<double> B)
        {
            // this is B dot term_i multiplication
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numCols != B.Count())
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return multiply(A, B, numRows, numCols);
        }

        /// <summary>
        /// Product of a matrix and a vector (2D double and 1D double)
        /// </summary>
        /// <param name="A">2D int Array</param>
        /// <param name="B">1D double array - column vector (1 element row)</param>
        /// <returns>A 1D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[] multiply(this int[,] A, IList<double> B)
        {
            // this is B dot term_i multiplication
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numCols != B.Count())
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return multiply(A, B, numRows, numCols);
        }

        /// <summary>
        /// Product of two matrices (2D double and 1D double)
        /// </summary>
        /// <param name="A">2D double Array</param>
        /// <param name="B">1D int array - column vector (1 element row)</param>
        /// <returns>A 1D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[] multiply(this double[,] A, IList<int> B)
        {
            // this is B dot term_i multiplication
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numCols != B.Count())
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return multiply(A, B, numRows, numCols);
        }

        /// <summary>
        /// Product of two matrices (2D double and 1D double)
        /// </summary>
        /// <param name="A">2D int Array</param>
        /// <param name="B">1D int array - column vector (1 element row)</param>
        /// <returns>A 1D int array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static int[] multiply(this int[,] A, IList<int> B)
        {
            // this is B dot term_i multiplication
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numCols != B.Count())
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return multiply(A, B, numRows, numCols);
        }

        /// <summary>
        /// Product of two matrices (1D double and 2D double)
        /// </summary>
        /// <param name="B">2D double Array</param>
        /// <param name="A">1D double array - row vector (1 element column)</param>
        /// <returns>A 1D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[] multiply(this IList<double> B, double[,] A)
        {
            // this is B dot term_i multiplication
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numRows != B.Count())
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return multiply(B, A, numRows, numCols);
        }

        /// <summary>
        /// Product of two matrices (1D double and 2D double)
        /// </summary>
        /// <param name="B">2D int Array</param>
        /// <param name="A">1D double array - row vector (1 element column)</param>
        /// <returns>A 1D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[] multiply(this IList<double> B, int[,] A)
        {
            // this is B dot term_i multiplication
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numRows != B.Count())
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return multiply(B, A, numRows, numCols);
        }

        /// <summary>
        /// Product of two matrices (1D double and 2D double)
        /// </summary>
        /// <param name="B">2D double Array</param>
        /// <param name="A">1D int array - row vector (1 element column)</param>
        /// <returns>A 1D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[] multiply(this IList<int> B, double[,] A)
        {
            // this is B dot term_i multiplication
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numRows != B.Count())
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return multiply(B, A, numRows, numCols);
        }

        /// <summary>
        /// Product of two matrices (1D double and 2D double)
        /// </summary>
        /// <param name="B">2D int Array</param>
        /// <param name="A">1D int array - row vector (1 element column)</param>
        /// <returns>A 1D int array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static int[] multiply(this IList<int> B, int[,] A)
        {
            // this is B dot term_i multiplication
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if (numRows != B.Count())
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return multiply(B, A, numRows, numCols);
        }

        /// <summary>
        /// Product of a matrix and a vector (2D double and 1D double)
        /// </summary>
        /// <param name="A">2D double Array</param>
        /// <param name="B">1D double array - column vector (1 element row)</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 1D double array that is the product of the two matrices A and B</returns>
        public static double[] multiply(this double[,] A, IList<double> B, int numRows, int numCols)
        {
            var C = new double[numRows];

            for (var i = 0; i != numRows; i++)
            {
                C[i] = 0.0;
                for (var j = 0; j != numCols; j++)
                    C[i] += A[i, j]*B[j];
            }
            return C;
        }

        /// <summary>
        /// Product of a matrix and a vector (2D double and 1D double)
        /// </summary>
        /// <param name="A">2D int Array</param>
        /// <param name="B">1D double array - column vector (1 element row)</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 1D double array that is the product of the two matrices A and B</returns>
        public static double[] multiply(this int[,] A, IList<double> B, int numRows, int numCols)
        {
            var C = new double[numRows];

            for (var i = 0; i != numRows; i++)
            {
                C[i] = 0.0;
                for (var j = 0; j != numCols; j++)
                    C[i] += A[i, j]*B[j];
            }
            return C;
        }

        /// <summary>
        /// Product of two matrices (2D double and 1D double)
        /// </summary>
        /// <param name="A">2D double Array</param>
        /// <param name="B">1D int array - column vector (1 element row)</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 1D double array that is the product of the two matrices A and B</returns>
        public static double[] multiply(this double[,] A, IList<int> B, int numRows, int numCols)
        {
            var C = new double[numRows];

            for (var i = 0; i != numRows; i++)
            {
                C[i] = 0.0;
                for (var j = 0; j != numCols; j++)
                    C[i] += A[i, j]*B[j];
            }
            return C;
        }

        /// <summary>
        /// Product of two matrices (2D double and 1D double)
        /// </summary>
        /// <param name="A">2D int Array</param>
        /// <param name="B">1D int array - column vector (1 element row)</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 1D int array that is the product of the two matrices A and B</returns>
        public static int[] multiply(this int[,] A, IList<int> B, int numRows, int numCols)
        {
            var C = new int[numRows];

            for (var i = 0; i != numRows; i++)
            {
                C[i] = 0;
                for (var j = 0; j != numCols; j++)
                    C[i] += A[i, j]*B[j];
            }
            return C;
        }

        /// <summary>
        /// Product of two matrices (1D double and 2D double)
        /// </summary>
        /// <param name="B">2D double Array</param>
        /// <param name="A">1D double array - row vector (1 element column)</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 1D double array that is the product of the two matrices A and B</returns>
        public static double[] multiply(this IList<double> B, double[,] A, int numRows, int numCols)
        {
            var C = new double[numCols];

            for (var i = 0; i != numCols; i++)
            {
                C[i] = 0.0;
                for (var j = 0; j != numRows; j++)
                    C[i] += B[j]*A[j, i];
            }
            return C;
        }

        /// <summary>
        /// Product of two matrices (1D double and 2D double)
        /// </summary>
        /// <param name="B">2D int Array</param>
        /// <param name="A">1D double array - row vector (1 element column)</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 1D double array that is the product of the two matrices A and B</returns>
        public static double[] multiply(this IList<double> B, int[,] A, int numRows, int numCols)
        {
            var C = new double[numCols];

            for (var i = 0; i != numCols; i++)
            {
                C[i] = 0.0;
                for (var j = 0; j != numRows; j++)
                    C[i] += B[j]*A[j, i];
            }
            return C;
        }

        /// <summary>
        /// Product of two matrices (1D double and 2D double)
        /// </summary>
        /// <param name="B">2D double Array</param>
        /// <param name="A">1D int array - row vector (1 element column)</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 1D double array that is the product of the two matrices A and B</returns>
        public static double[] multiply(this IList<int> B, double[,] A, int numRows, int numCols)
        {
            var C = new double[numCols];

            for (var i = 0; i != numCols; i++)
            {
                C[i] = 0.0;
                for (var j = 0; j != numRows; j++)
                    C[i] += B[j]*A[j, i];
            }
            return C;
        }

        /// <summary>
        /// Product of two matrices (1D double and 2D double)
        /// </summary>
        /// <param name="B">2D int Array</param>
        /// <param name="A">1D int array - row vector (1 element column)</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 1D int array that is the product of the two matrices A and B</returns>
        public static int[] multiply(this IList<int> B, int[,] A, int numRows, int numCols)
        {
            var C = new int[numCols];

            for (var i = 0; i != numCols; i++)
            {
                C[i] = 0;
                for (var j = 0; j != numRows; j++)
                    C[i] += B[j]*A[j, i];
            }
            return C;
        }

        #endregion

        #region Hadamard Multiplication

        /* http://en.wikipedia.org/wiki/Hadamard_product_(matrices) */

        /// <summary>
        /// The element-by-element of the two 1D double vectors A and B
        /// </summary>
        /// <param name="A">1D double Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <returns>A double value that contains the element-by-element</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static double[] EltMultiply(this IList<double> A, IList<double> B)
        {
            var length = A.Count();
            if (length != B.Count())
                throw new ArithmeticException("Matrix sizes do not match");
            return EltMultiply(A, B, length);
        }

        /// <summary>
        /// The element-by-element of the two 1D double vectors A and B
        /// </summary>
        /// <param name="A">1D double Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <param name="length">The length of both vectors A and B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A double value that contains the element-by-element</returns>
        public static double[] EltMultiply(this IList<double> A, IList<double> B, int length)
        {
            var C = new double[length];
            for (var i = 0; i != length; i++)
                C[i] = A[i] * B[i];
            return C;
        }

        /// <summary>
        /// The element-by-element of the one 1D int vector and one 1D double vector
        /// </summary>
        /// <param name="A">1D int Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <returns>A double value that contains the element-by-element</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static double[] EltMultiply(this IList<int> A, IList<double> B)
        {
            var length = A.Count();
            if (length != B.Count())
                throw new ArithmeticException("Matrix sizes do not match");
            return EltMultiply(A, B, length);
        }

        /// <summary>
        /// The element-by-element of the one 1D int vector and one 1D double vector
        /// </summary>
        /// <param name="A">1D int Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <param name="length">The length of both vectors A and B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A double value that contains the element-by-element</returns>
        public static double[] EltMultiply(this IList<int> A, IList<double> B, int length)
        {
            var C = new double[length];
            for (var i = 0; i != length; i++)
                C[i] = A[i] * B[i];
            return C;
        }

        /// <summary>
        /// The element-by-element of the two 1D int vectors A and B
        /// </summary>
        /// <param name="A">1D int Array, A</param>
        /// <param name="B">1D int Array, B</param>
        /// <returns>A double value that contains the element-by-element</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static int[] EltMultiply(this IList<int> A, IList<int> B)
        {
            // this is B dot term_i multiplication
            var length = A.Count();
            if (length != B.Count())
                throw new ArithmeticException("Matrix sizes do not match");
            return EltMultiply(A, B, length);
        }

        /// <summary>
        /// The element-by-element of the two 1D int vectors A and B
        /// </summary>
        /// <param name="A">1D int Array, A</param>
        /// <param name="B">1D int Array, B</param>
        /// <param name="length">The length of both vectors A and B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A double value that contains the element-by-element</returns>
        public static int[] EltMultiply(this IList<int> A, IList<int> B, int length)
        {
            var C = new int[length];
            for (var i = 0; i != length; i++)
                C[i] = A[i] * B[i];
            return C;
        }


        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[,] EltMultiply(this double[,] A, double[,] B)
        {
            if (A.GetLength(0) != B.GetLength(0) && A.GetLength(1) != B.GetLength(1))
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return EltMultiply(A, B, A.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[,] EltMultiply(this int[,] A, double[,] B)
        {
            if (A.GetLength(0) != B.GetLength(0) && A.GetLength(1) != B.GetLength(1))
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return EltMultiply(A, B, A.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[,] EltMultiply(this double[,] A, int[,] B)
        {
            if (A.GetLength(0) != B.GetLength(0) && A.GetLength(1) != B.GetLength(1))
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return EltMultiply(A, B, A.GetLength(0), B.GetLength(1));
        }


        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <returns>A 2D int array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static int[,] EltMultiply(this int[,] A, int[,] B)
        {
            if (A.GetLength(0) != B.GetLength(0) && A.GetLength(1) != B.GetLength(1))
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return EltMultiply(A, B, A.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        public static double[,] EltMultiply(this double[,] A, double[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] * B[i, j];

            return C;
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        public static double[,] EltMultiply(this int[,] A, double[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] * B[i, j];

            return C;
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        public static double[,] EltMultiply(this double[,] A, int[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] * B[i, j];

            return C;
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 2D int array that is the product of the two matrices A and B</returns>
        public static int[,] EltMultiply(this int[,] A, int[,] B, int numRows, int numCols)
        {
            var C = new int[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] * B[i, j];

            return C;
        }

        #endregion
        #region Hadamard Division

        /// <summary>
        /// The element-by-element of the two 1D double vectors A and B
        /// </summary>
        /// <param name="A">1D double Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <returns>A double value that contains the element-by-element</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static double[] EltDivide(this IList<double> A, IList<double> B)
        {
            var length = A.Count();
            if (length != B.Count())
                throw new ArithmeticException("Matrix sizes do not match");
            return EltDivide(A, B, length);
        }

        /// <summary>
        /// The element-by-element of the two 1D double vectors A and B
        /// </summary>
        /// <param name="A">1D double Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <param name="length">The length of both vectors A and B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A double value that contains the element-by-element</returns>
        public static double[] EltDivide(this IList<double> A, IList<double> B, int length)
        {
            var C = new double[length];
            for (var i = 0; i != length; i++)
                C[i] = A[i] / B[i];
            return C;
        }

        /// <summary>
        /// The element-by-element of the one 1D int vector and one 1D double vector
        /// </summary>
        /// <param name="A">1D int Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <returns>A double value that contains the element-by-element</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static double[] EltDivide(this IList<int> A, IList<double> B)
        {
            var length = A.Count();
            if (length != B.Count())
                throw new ArithmeticException("Matrix sizes do not match");
            return EltDivide(A, B, length);
        }

        /// <summary>
        /// The element-by-element of the one 1D int vector and one 1D double vector
        /// </summary>
        /// <param name="A">1D int Array, A</param>
        /// <param name="B">1D double Array, B</param>
        /// <param name="length">The length of both vectors A and B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A double value that contains the element-by-element</returns>
        public static double[] EltDivide(this IList<int> A, IList<double> B, int length)
        {
            var C = new double[length];
            for (var i = 0; i != length; i++)
                C[i] = A[i] / B[i];
            return C;
        }

        /// <summary>
        /// The element-by-element of the two 1D int vectors A and B
        /// </summary>
        /// <param name="A">1D int Array, A</param>
        /// <param name="B">1D int Array, B</param>
        /// <returns>A double value that contains the element-by-element</returns>
        /// <exception cref="System.ArithmeticException">Matrix sizes do not match</exception>
        public static double[] EltDivide(this IList<int> A, IList<int> B)
        {
            // this is B dot term_i multiplication
            var length = A.Count();
            if (length != B.Count())
                throw new ArithmeticException("Matrix sizes do not match");
            return EltDivide(A, B, length);
        }

        /// <summary>
        /// The element-by-element of the two 1D int vectors A and B
        /// </summary>
        /// <param name="A">1D int Array, A</param>
        /// <param name="B">1D int Array, B</param>
        /// <param name="length">The length of both vectors A and B. This is an optional argument, but if it is already known
        /// - there is a slight speed advantage to providing it.</param>
        /// <returns>A double value that contains the element-by-element</returns>
        public static double[] EltDivide(this IList<int> A, IList<int> B, int length)
        {
            var C = new double[length];
            for (var i = 0; i != length; i++)
                C[i] = A[i] / (double)B[i];
            return C;
        }


        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[,] EltDivide(this double[,] A, double[,] B)
        {
            if (A.GetLength(0) != B.GetLength(0) && A.GetLength(1) != B.GetLength(1))
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return EltDivide(A, B, A.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[,] EltDivide(this int[,] A, double[,] B)
        {
            if (A.GetLength(0) != B.GetLength(0) && A.GetLength(1) != B.GetLength(1))
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return EltDivide(A, B, A.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[,] EltDivide(this double[,] A, int[,] B)
        {
            if (A.GetLength(0) != B.GetLength(0) && A.GetLength(1) != B.GetLength(1))
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return EltDivide(A, B, A.GetLength(0), B.GetLength(1));
        }


        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <returns>A 2D int array that is the product of the two matrices A and B</returns>
        /// <exception cref="System.ArithmeticException">Column count in first matrix must be equal to row count in second matrix</exception>
        public static double[,] EltDivide(this int[,] A, int[,] B)
        {
            if (A.GetLength(0) != B.GetLength(0) && A.GetLength(1) != B.GetLength(1))
                throw new ArithmeticException("Column count in first matrix must be equal to row count in second matrix");
            return EltDivide(A, B, A.GetLength(0), B.GetLength(1));
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        public static double[,] EltDivide(this double[,] A, double[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] / B[i, j];

            return C;
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        public static double[,] EltDivide(this int[,] A, double[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] / B[i, j];

            return C;
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 2D double array that is the product of the two matrices A and B</returns>
        public static double[,] EltDivide(this double[,] A, int[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] / B[i, j];

            return C;
        }

        /// <summary>
        /// Product of two matrices (2D double)
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>A 2D int array that is the product of the two matrices A and B</returns>
        public static double[,] EltDivide(this int[,] A, int[,] B, int numRows, int numCols)
        {
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[i, j] / (double)B[i, j];

            return C;
        }

        #endregion

        #region Kronecker Product
        /*  https://en.wikipedia.org/wiki/Kronecker_product    */


        /// <summary>
        /// Returns the Kronecker product of the two matrices.
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <returns>System.Double[].</returns>
        public static double[,] KronProduct(this double[,] A, double[,] B)
        {
            var m = A.GetLength(0);
            var n = A.GetLength(1);
            var p = B.GetLength(0);
            var q = B.GetLength(1);
            var C = new double[m * p, n * q];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < p; k++)
                    {
                        for (int l = 0; l < q; l++)
                        {
                            C[p * i + k, q * j + l] = A[i, j] * B[k, l];
                        }
                    }
                }
            }
            return C;
        }
        /// <summary>
        /// Returns the Kronecker product of the two matrices.
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <returns>System.Int32[].</returns>
        public static int[,] KronProduct(this int[,] A, int[,] B)
        {
            var m = A.GetLength(0);
            var n = A.GetLength(1);
            var p = B.GetLength(0);
            var q = B.GetLength(1);
            var C = new int[m * p, n * q];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < p; k++)
                    {
                        for (int l = 0; l < q; l++)
                        {
                            C[p * i + k, q * j + l] = A[i, j] * B[k, l];
                        }
                    }
                }
            }
            return C;
        }
        /// <summary>
        /// Returns the Kronecker product of the two matrices.
        /// </summary>
        /// <param name="A">2D double Array, A</param>
        /// <param name="B">2D int Array, A</param>
        /// <returns>System.Double[].</returns>
        public static double[,] KronProduct(this double[,] A, int[,] B)
        {
            var m = A.GetLength(0);
            var n = A.GetLength(1);
            var p = B.GetLength(0);
            var q = B.GetLength(1);
            var C = new double[m * p, n * q];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < p; k++)
                    {
                        for (int l = 0; l < q; l++)
                        {
                            C[p * i + k, q * j + l] = A[i, j] * B[k, l];
                        }
                    }
                }
            }
            return C;
        }
        /// <summary>
        /// Returns the Kronecker product of the two matrices.
        /// </summary>
        /// <param name="A">2D int Array, A</param>
        /// <param name="B">2D double Array, A</param>
        /// <returns>System.Double[].</returns>
        public static double[,] KronProduct(this int[,] A, double[,] B)
        {
            var m = A.GetLength(0);
            var n = A.GetLength(1);
            var p = B.GetLength(0);
            var q = B.GetLength(1);
            var C = new double[m * p, n * q];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < p; k++)
                    {
                        for (int l = 0; l < q; l++)
                        {
                            C[p * i + k, q * j + l] = A[i, j] * B[k, l];
                        }
                    }
                }
            }
            return C;
        }
        #endregion

    }
}