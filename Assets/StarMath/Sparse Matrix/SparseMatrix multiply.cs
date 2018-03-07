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
        /// <summary>
        /// Product of two matrices (2D double and a SparseMatrix)
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static double[,] multiply(this double[,] A, SparseMatrix B)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Product of two matrices (2D double and a SparseMatrix), which results in a 
        /// new sparse matrix.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>SparseMatrix.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static SparseMatrix multiplyToSparse(this double[,] A, SparseMatrix B)
        {
            throw new NotImplementedException();
        }
    }

    public partial class SparseMatrix
    {
        #region Scalars multiplying matrices

        /// <summary>
        /// Multiplies all elements of this sparse matrix with a double value.
        /// </summary>
        /// <param name="a">The double value to be multiplied</param>
        /// <returns>A 2D double array that contains the product</returns>
        public void multiply(double a)
        {
            foreach (var sparseCell in cellsRowbyRow)
                sparseCell.Value *= a;
        }

        /// <summary>
        /// Divides all elements of this sparse matrix with a double value.
        /// </summary>
        /// <param name="a">The double value to be divided by.</param>
        /// <returns>A 2D double array that contains the product</returns>
        public void Divide(double a)
        {
             multiply(1/a);
        }

        #endregion

        #region Matrix(2D) to matrix(2D) multiplication

        /// <summary>
        /// Multiplies this sparse matrix by a 2D double array. This sparse matrix is
        /// altered to reflect the result.
        /// </summary>
        /// <param name="A">a.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void multiplyInPlace(double[,] A)
        {
            throw new NotImplementedException();
            //var C = new double[numRows, numCols];

            //for (var i = 0; i  NumRows; i++)
            //    for (var j = 0; j != numCols; j++)
            //    {
            //        C[i, j] = 0.0;
            //        for (var k = 0; k != A.GetLength(1); k++)
            //            C[i, j] += A[i, k] * B[k, j];
            //    }
            //return C;
        }

        /// <summary>
        /// Multiplies this sparse matrix by a 2D double array, and returns a new double array.
        /// </summary>
        /// <param name="A">a.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="NotImplementedException"></exception>
        public double[,] multiply(double[,] A)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Multiply matrix to a vector (and vice versa)

        /// <summary>
        /// Multiplies the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="ArithmeticException">Matrix number of columns does not match length of vector.</exception>
        public double[] multiply(IList<double> x)
        {
            var length = x.Count;
            if (length!= NumCols)
                throw new ArithmeticException("Matrix number of columns does not match length of vector.");
            var b = new double[length];
            for (int i = 0; i < NumRows; i++)
            {
                var sum = 0.0;
                var cell = RowFirsts[i];
                while (cell != null)
                {
                    sum += cell.Value*x[cell.ColIndex];
                    cell = cell.Right;
                }
                b[i] = sum;
            }
            return b;
        }
        #endregion
    }
}
