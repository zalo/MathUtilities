// ***********************************************************************
// Assembly         : StarMath
// Author           : Matthew I. Campbell
// Created          : 02-28-2016
// Originally Modified from    : Timothy A. Davis's CSparse code, 2006-2014
// Last Modified By : Matt Campbell
// Last Modified On : 03-17-2016
// ***********************************************************************
// <copyright file="CSparseClasses.cs" company="Design Engineering Lab -- MICampbell">
//     2014
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace StarMathLib
{
    /// <summary>
    /// Class SymbolicFactorization.
    /// </summary>
    internal class SymbolicFactorization
    {
        /// <summary>
        /// The column pointers
        /// </summary>
        internal int[] ColumnPointers;

        /// <summary>
        /// The parent indices used in the elimination tree for Cholesky
        /// </summary>
        internal int[] ParentIndices;
        /// <summary>
        /// The inverse permute
        /// </summary>
        internal int[] InversePermute;
        /// <summary>
        /// The permutation vector
        /// </summary>
        internal int[] PermutationVector;
    }

    /// <summary>
    /// Class CompressedColumnStorage.
    /// </summary>
    internal class CompressedColumnStorage
    {
        /// <summary>
        /// The ncols
        /// </summary>
        internal readonly int ncols;
        /// <summary>
        /// The nrows
        /// </summary>
        internal readonly int nrows;
        /// <summary>
        /// The column pointers
        /// </summary>
        internal int[] ColumnPointers;
        /// <summary>
        /// The row indices
        /// </summary>
        internal int[] RowIndices;
        /// <summary>
        /// The values
        /// </summary>
        internal double[] Values;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressedColumnStorage"/> class.
        /// </summary>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        /// <param name="numNonZero">The number non zero.</param>
        internal CompressedColumnStorage(int rowCount, int columnCount, int numNonZero)
        {
            ColumnPointers = new int[columnCount + 1];
            RowIndices = new int[numNonZero];
            nrows = rowCount;
            ncols = columnCount;
            if (numNonZero > 0)
            {
                Values = new double[numNonZero];
            }
        }

        /// <summary>
        /// Change the max # of entries sparse matrix
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool Resize(int size)
        {
            if (size <= 0)
            {
                size = ColumnPointers[ncols];
            }
            Array.Resize(ref RowIndices, size);
            Array.Resize(ref Values, size);

            return true;
        }
    }

    /// <summary>
    /// Represents the nonzero pattern of a column-compressed matrix.
    /// </summary>
    /// <remarks>Used for ordering and symbolic factorization.</remarks>
    internal class SymbolicColumnStorage
    {
        /// <summary>
        /// The ncols
        /// </summary>
        private readonly int ncols;
        /// <summary>
        /// The nrows
        /// </summary>
        private readonly int nrows;
        /// <summary>
        /// The column pointers
        /// </summary>
        internal int[] ColumnPointers;
        /// <summary>
        /// The row indices
        /// </summary>
        internal int[] RowIndices;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicColumnStorage"/> class.
        /// </summary>
        /// <param name="mat">The mat.</param>
        public SymbolicColumnStorage(CompressedColumnStorage mat)
        {
            nrows = mat.nrows;
            ncols = mat.ncols;
            ColumnPointers = mat.ColumnPointers;
            RowIndices = mat.RowIndices;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicColumnStorage"/> class.
        /// </summary>
        /// <param name="ncols">The ncols.</param>
        /// <param name="nrows">The nrows.</param>
        /// <param name="colPointers">The col pointers.</param>
        /// <param name="rowIndices">The row indices.</param>
        public SymbolicColumnStorage(int ncols, int nrows, int[] colPointers, int[] rowIndices)
        {
            this.ncols = ncols;
            this.nrows = nrows;
            ColumnPointers = colPointers;
            RowIndices = rowIndices;
        }

        /// <summary>
        /// Change the max # of entries sparse matrix
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool Resize(int size)
        {
            if (size <= 0)
            {
                size = ColumnPointers[ncols];
            }

            Array.Resize(ref RowIndices, size);

            return true;
        }

        /// <summary>
        /// Computes the transpose of a sparse matrix, C = A';
        /// </summary>
        /// <returns>Transposed matrix, C = A'</returns>
        internal SymbolicColumnStorage Transpose()
        {
            var colPointers = new int[nrows + 1];
            var rowIndices = new int[RowIndices.Length];
            var workspace = new int[nrows];

            for (int p = 0; p < ColumnPointers[ncols]; p++)
            {
                // Row counts.
                workspace[RowIndices[p]]++;
            }
            // Column pointers.
            CumulativeSum(colPointers, workspace, nrows);

            for (int j = 0; j < ncols; j++)
            {
                for (int p = ColumnPointers[j]; p < ColumnPointers[j + 1]; p++)
                {
                  int  k = workspace[RowIndices[p]]++;
                    // Place A(i,j) as entry C(j,i)
                    rowIndices[k] = j;
                }
            }
            return new SymbolicColumnStorage(ncols, nrows, colPointers, rowIndices);
        }

        /// <summary>
        /// Cumulative sum of given array.
        /// </summary>
        /// <param name="sum">Output: cumulative sum of counts</param>
        /// <param name="counts">input array, overwritten with sum</param>
        /// <param name="size">length of counts</param>
        /// <returns>sum[size] (non-zeros)</returns>
        private static int CumulativeSum(int[] sum, int[] counts, int size)
        {
            int i, nz = 0;

            for (i = 0; i < size; i++)
            {
                sum[i] = nz;
                nz += counts[i];
                counts[i] = sum[i]; // also copy p[0..n-1] back into c[0..n-1]
            }

            sum[size] = nz;

            return nz;
        }

        #region for AMD Generate (ConstructMatrix uses subsequent 3 functions)

        /// <summary>
        /// Constructs the matrix.
        /// </summary>
        /// <param name="a">a.</param>
        /// <returns>SymbolicColumnStorage.</returns>
        internal static SymbolicColumnStorage ConstructMatrix(CompressedColumnStorage a)
        {
            var A = new SymbolicColumnStorage(a);
            // Compute A'
            var AT = A.Transpose();
            // Return A+A'
            var result = A.Add(AT);
            // Drop diagonal entries.
            result.Keep();

            return result;
        }

        /// <summary>
        /// Symbolic sum C = A + B
        /// </summary>
        /// <param name="other">column-compressed matrix</param>
        /// <returns>Sum C = A + B</returns>
        private SymbolicColumnStorage Add(SymbolicColumnStorage other)
        {
            var workspace = new int[this.nrows];
            // Allocate result: (anz + bnz) is an upper bound
            var columnPointers = new int[other.ColumnPointers.Length];
            var rowIndices = new int[ColumnPointers[ncols] + other.ColumnPointers[other.ncols]];
            int nz = 0;
            for (int j = 0; j < other.ncols; j++)
            {
                // Column j of result starts here
                columnPointers[j] = nz;
                nz = Scatter(j, workspace, j + 1, rowIndices, nz); // A(:,j)
                nz = other.Scatter(j, workspace, j + 1, rowIndices, nz); // B(:,j)
            }
            // Finalize the last column
            columnPointers[other.ncols] = nz;

            // Remove extra space
            Array.Resize(ref rowIndices, nz);

            return new SymbolicColumnStorage(this.nrows, other.ncols, columnPointers,
                rowIndices);
        }


        /// <summary>
        /// Keeps this instance.
        /// </summary>
        /// <returns>System.Int32.</returns>
        private int Keep()
        {
            int i, j, nz = 0;

            for (j = 0; j < ncols; j++)
            {
                i = ColumnPointers[j];

                // Record new location of col j.
                ColumnPointers[j] = nz;

                for (; i < ColumnPointers[j + 1]; i++)
                {
                    if (RowIndices[i] != j) //keep off diag
                    {
                        // Keep A(i,j).
                        RowIndices[nz] = RowIndices[i];
                        nz++;
                    }
                }
            }

            // Record new nonzero count.
            ColumnPointers[ncols] = nz;

            // Remove extra space.
            Array.Resize(ref RowIndices, nz);

            return nz;
        }

        /// <summary>
        /// Scatters and sums a sparse vector A(:,j) into a dense vector, x = x + beta * A(:,j).
        /// </summary>
        /// <param name="j">the column of A to use</param>
        /// <param name="work">size m, node i is marked if w[i] = mark</param>
        /// <param name="mark">mark value of w</param>
        /// <param name="ci">pattern of x accumulated in ci</param>
        /// <param name="nz">pattern of x placed in C starting at C.i[nz]</param>
        /// <returns>new value of nz</returns>
        private int Scatter(int j, int[] work, int mark, int[] ci, int nz)
        {
             for (int p = ColumnPointers[j]; p < ColumnPointers[j + 1]; p++)
            {
              int  i = RowIndices[p]; // A(i,j) is nonzero
                if (work[i] < mark)
                {
                    work[i] = mark; // i is new entry in column j
                    ci[nz++] = i; // add i to pattern of C(:,j)
                }
            }

            return nz;
        }

        #endregion
    }
}