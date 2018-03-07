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
        /// Adds the specified SparseMatrix, B, to this 2D double array to create a new
        /// 2D double array.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static double[,] add(this double[,] A, SparseMatrix B)
        {
            var numRows = A.GetLength(0);
            if (B.NumRows != numRows) throw new ArithmeticException("Cannot add matrices of different sizes.");
            var numCols = A.GetLength(1);
            if (B.NumCols != numCols) throw new ArithmeticException("Cannot add matrices of different sizes.");

            var C = (double[,])A.Clone();

            for (var i = 0; i < numRows; i++)
            {
                var cell = B.RowFirsts[i];
                while (cell != null)
                {
                    C[i, cell.ColIndex] += cell.Value;
                    cell = cell.Right;
                }
            }
            return C;
        }
        /// <summary>
        /// Subtracts the specified SparseMatrix, B, to this 2D double array to create a new
        /// 2D double array.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static double[,] subtract(this double[,] A, SparseMatrix B)
        {
            var numRows = A.GetLength(0);
            if (B.NumRows != numRows) throw new ArithmeticException("Cannot add matrices of different sizes.");
            var numCols = A.GetLength(1);
            if (B.NumCols != numCols) throw new ArithmeticException("Cannot add matrices of different sizes.");

            var C = (double[,])A.Clone();

            for (var i = 0; i < numRows; i++)
            {
                var cell = B.RowFirsts[i];
                while (cell != null)
                {
                    C[i, cell.ColIndex] -= cell.Value;
                    cell = cell.Right;
                }
            }
            return C;
        }
    }

    /// <summary>
    /// Class SparseMatrix.
    /// </summary>
    public partial class SparseMatrix
    {
        /// <summary>
        /// Adds the specified 2D double array, A to this sparse matrix to create a new
        /// 2D double array.
        /// </summary>
        /// <param name="A">a.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="NotImplementedException"></exception>
        public double[,] add(double[,] A)
        {
            var numRows = A.GetLength(0);
            if (NumRows != numRows) throw new ArithmeticException("Cannot add matrices of different sizes.");
            var numCols = A.GetLength(1);
            if (NumCols != numCols) throw new ArithmeticException("Cannot add matrices of different sizes.");

            var C = (double[,])A.Clone();

            for (var i = 0; i < numRows; i++)
            {
                var cell = RowFirsts[i];
                while (cell != null)
                {
                    C[i, cell.ColIndex] += cell.Value;
                    cell = cell.Right;
                }
            }
            return C;
        }

        /// <summary>
        /// Subtracts the specified 2D double array, A to this sparse matrix to create a new
        /// 2D double array.
        /// </summary>
        /// <param name="A">a.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">
        /// Cannot subtract matrices of different sizes.
        ///  </exception>
        public double[,] subtract(double[,] A)
        {
            var numRows = A.GetLength(0);
            if (NumRows != numRows) throw new ArithmeticException("Cannot subtract matrices of different sizes.");
            var numCols = A.GetLength(1);
            if (NumCols != numCols) throw new ArithmeticException("Cannot subtract matrices of different sizes.");

            var C = (double[,])A.Clone();

            for (var i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                    C[i, j] = -C[i, j];
                var cell = RowFirsts[i];
                while (cell != null)
                {
                    C[i, cell.ColIndex] += cell.Value;
                    cell = cell.Right;
                }
            }
            return C;
        }

        /// <summary>
        /// Adds the specified 2D double array, A to this sparse matrix and writes over
        /// this sparse matrix with the result.
        /// </summary>
        /// <param name="A">a.</param>
        /// <exception cref="System.ArithmeticException">Adding Sparse Matrices can only be accomplished if both are the same size.</exception>
        public void addInPlace(SparseMatrix A)
        {
            if (NumRows != A.NumRows || NumCols != A.NumCols)
                throw new ArithmeticException(
                    "Adding Sparse Matrices can only be accomplished if both are the same size.");

            for (var i = 0; i < NumRows; i++)
            {
                var thisCell = RowFirsts[i];
                var ACell = A.RowFirsts[i];
                while (thisCell != null || ACell != null)
                {
                    if (thisCell == null || (ACell != null && ACell.ColIndex < thisCell.ColIndex))
                    {
                        AddCell(i, ACell.ColIndex, ACell.Value);
                        ACell = ACell.Right;
                    }
                    else if (ACell == null || thisCell.ColIndex < ACell.ColIndex)
                    {
                        thisCell = thisCell.Right;
                    }
                    else //then the two values must be at the same cell
                    {
                        thisCell.Value += ACell.Value;
                        thisCell = thisCell.Right;
                        ACell = ACell.Right;
                    }
                }
            }
        }
        /// <summary>
        /// Subtracts the specified 2D double array, A from this sparse matrix and writes over
        /// this sparse matrix with the result.
        /// </summary>
        /// <param name="A">a.</param>
        /// <exception cref="System.ArithmeticException">Adding Sparse Matrices can only be accomplished if both are the same size.</exception>
        public void subtractInPlace(SparseMatrix A)
        {
            if (NumRows != A.NumRows || NumCols != A.NumCols)
                throw new ArithmeticException(
                    "Adding Sparse Matrices can only be accomplished if both are the same size.");

            for (var i = 0; i < NumRows; i++)
            {
                var thisCell = RowFirsts[i];
                var ACell = A.RowFirsts[i];
                while (thisCell != null || ACell != null)
                {
                    if (thisCell == null || (ACell != null && ACell.ColIndex < thisCell.ColIndex))
                    {
                        AddCell(i, ACell.ColIndex, -ACell.Value);
                        ACell = ACell.Right;
                    }
                    else if (ACell == null || thisCell.ColIndex < ACell.ColIndex)
                    {
                        thisCell = thisCell.Right;
                    }
                    else //then the two values must be at the same cell
                    {
                        thisCell.Value -= ACell.Value;
                        thisCell = thisCell.Right;
                        ACell = ACell.Right;
                    }
                }
            }
        }


        /// <summary>
        /// Adds the specified 2D double array, A to this sparse matrix to create a new
        /// 2D double array.
        /// </summary>
        /// <param name="A">a.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="NotImplementedException"></exception>
        public SparseMatrix add(SparseMatrix A)
        {
            var copy = this.Copy();
            copy.addInPlace(A);
            return copy;
        }

        /// <summary>
        /// Subtracts the specified 2D double array, A from this sparse matrix to create a new
        /// 2D double array.
        /// </summary>
        /// <param name="A">a.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="NotImplementedException"></exception>
        public SparseMatrix subtract(SparseMatrix A)
        {
            var copy = this.Copy();
            copy.subtractInPlace(A);
            return copy;
        }

        /// <summary>
        /// Sums all elements.
        /// </summary>
        /// <returns>System.Double.</returns>
        public double SumAllElements()
        {
            return SumAllRows().Sum();
        }
        /// <summary>
        /// Sums all of the rows.
        /// </summary>
        /// <returns>System.Double[].</returns>
        public double[] SumAllRows()
        {
            var rowSums = new double[NumRows];
            for (int i = 0; i < NumRows; i++)
                rowSums[i] = SumRow(i);
            return rowSums;
        }
        /// <summary>
        /// Sums the values of a specified row.
        /// </summary>
        /// <returns>System.Double[].</returns>
        public double SumRow(int index)
        {
            var sum = 0.0;
            var cell = RowFirsts[index];
            while (cell != null)
            {
                sum += cell.Value;
                cell = cell.Right;
            }
            return sum;
        }
        /// <summary>
        /// Sums the columns.
        /// </summary>
        /// <returns>System.Double[].</returns>
        public double[] SumAllColumns()
        {
            var colSums = new double[NumCols];
            for (int i = 0; i < NumCols; i++)
                colSums[i] = SumColumn(i);
            return colSums;
        }
        /// <summary>
        /// Sums the values of a specified column.
        /// </summary>
        /// <returns>System.Double[].</returns>
        public double SumColumn(int index)
        {
            var sum = 0.0;
            var cell = ColFirsts[index];
            while (cell != null)
            {
                sum += cell.Value;
                cell = cell.Down;
            }
            return sum;
        }
    }
}
