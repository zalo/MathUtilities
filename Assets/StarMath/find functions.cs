// ***********************************************************************
// Assembly         : StarMath
// Author           : MICampbell
// Created          : 05-14-2015
//
// Last Modified By : MICampbell
// Last Modified On : 07-07-2015
// ***********************************************************************
// <copyright file="find functions.cs" company="Design Engineering Lab -- MICampbell">
//     2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;

namespace StarMathLib
{
    public static partial class StarMath
    {
        #region Max matrix functions.

        /// <summary>
        /// Finds the maximum value in the given 2D double array
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <returns>A double value that is the maximum of A</returns>
        public static double Max(this double[,] A)
        {
            var max = double.NegativeInfinity;
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    if (max < A[i, j]) max = A[i, j];
            return max;
        }

        /// <summary>
        /// Finds the maximum value in the given 2D double array and returns the row and column indices along with it.
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns>the maximum value</returns>
        public static double Max(this double[,] A, out int rowIndex, out int colIndex)
        {
            var max = double.NegativeInfinity;
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            rowIndex = colIndex = -1;
            for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    if (max < A[i, j])
                    {
                        max = A[i, j];
                        rowIndex = i;
                        colIndex = j;
                    }
            return max;
        }

        /// <summary>
        /// Finds the maximum value in the given 2D integer array
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <returns>An integer value that is the maximum of A</returns>
        public static int Max(this int[,] A)
        {
            var max = int.MinValue;
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    if (max < A[i, j]) max = A[i, j];
            return max;
        }

        /// <summary>
        /// Finds the maximum value in the given 2D double array and returns the row and column indices along with it.
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns>the maximum value</returns>
        public static int Max(this int[,] A, out int rowIndex, out int colIndex)
        {
            var max = int.MinValue;
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            rowIndex = colIndex = -1;
            for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    if (max < A[i, j])
                    {
                        max = A[i, j];
                        rowIndex = i;
                        colIndex = j;
                    }
            return max;
        }

        #endregion

        #region Min matrix functions.

        /// <summary>
        /// Finds the minimum value in the given 2D double array
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <returns>A double value that is the minimum of A</returns>
        public static double Min(this double[,] A)
        {
            var min = double.PositiveInfinity;
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    if (min > A[i, j]) min = A[i, j];
            return min;
        }

        /// <summary>
        /// Finds the minimum value in the given 2D double array and returns the row and column indices along with it.
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns>the minimum value</returns>
        public static double Min(this double[,] A, out int rowIndex, out int colIndex)
        {
            var min = double.PositiveInfinity;
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            rowIndex = colIndex = -1;
            for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    if (min > A[i, j])
                    {
                        min = A[i, j];
                        rowIndex = i;
                        colIndex = j;
                    }
            return min;
        }

        /// <summary>
        /// Finds the minimum value in the given 2D integer array
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <returns>An integer value that is the minimum of A</returns>
        public static int Min(this int[,] A)
        {
            var min = int.MaxValue;
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    if (min > A[i, j]) min = A[i, j];
            return min;
        }

        /// <summary>
        /// Finds the minimum value in the given 2D double array and returns the row and column indices along with it.
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns>the minimum value</returns>
        public static int Min(this int[,] A, out int rowIndex, out int colIndex)
        {
            var min = int.MaxValue;
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            rowIndex = colIndex = -1;
            for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    if (min > A[i, j])
                    {
                        min = A[i, j];
                        rowIndex = i;
                        colIndex = j;
                    }
            return min;
        }

        #endregion

        #region Min and max vector functions.

        /// <summary>
        /// Finds the maximum value in the given 1D integer array
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <returns>An integer value that is the maximum of A</returns>
        public static int Max(this IList<int> A)
        {
            var max = int.MinValue;
            var numElts = A.Count;
            for (var i = 0; i < numElts; i++)
                if (max < A[i]) max = A[i];
            return max;
        }

        /// <summary>
        /// Finds the minimum value in the given 1D integer array
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <returns>An integer value that is the minimum of A</returns>
        public static int Min(this IList<int> A)
        {
            var min = int.MaxValue;
            var numElts = A.Count;
            for (var i = 0; i < numElts; i++)
                if (min > A[i]) min = A[i];
            return min;
        }

        /// <summary>
        /// Finds the maximum value in the given 1D double array
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <returns>An double value that is the maximum of A</returns>
        public static double Max(this IList<double> A)
        {
            var max = double.NegativeInfinity;
            var numElts = A.Count;
            for (var i = 0; i < numElts; i++)
                if (max < A[i]) max = A[i];
            return max;
        }

        /// <summary>
        /// Finds the minimum value in the given 1D double array
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <returns>An double value that is the minimum of A</returns>
        public static double Min(this IList<double> A)
        {
            var min = double.PositiveInfinity;
            var numElts = A.Count;
            for (var i = 0; i < numElts; i++)
                if (min > A[i]) min = A[i];
            return min;
        }

        /// <summary>
        /// Finds the minimum value in the given 1D double array and returns its index along with it.
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <param name="index">The index.</param>
        /// <returns>the minimum value</returns>
        public static double Min(this IList<double> A, out int index)
        {
            index = -1;
            var min = double.PositiveInfinity;
            var numElts = A.Count;
            for (var i = 0; i < numElts; i++)
                if (min > A[i])
                {
                    min = A[i];
                    index = i;
                }
            return min;
        }

        /// <summary>
        /// Finds the minimum value in the given 1D double array and returns its index along with it.
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <param name="index">The index.</param>
        /// <returns>the maximum value</returns>
        public static double Max(this IList<double> A, out int index)
        {
            index = -1;
            var max = double.NegativeInfinity;
            var numElts = A.Count;
            for (var i = 0; i < numElts; i++)
                if (max < A[i])
                {
                    max = A[i];
                    index = i;
                }
            return max;
        }

        /// <summary>
        /// Finds the minimum value in the given 1D double array and returns its index along with it.
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <param name="index">The index.</param>
        /// <returns>the minimum value</returns>
        public static int Min(this IList<int> A, out int index)
        {
            index = -1;
            var min = int.MaxValue;
            var numElts = A.Count;
            for (var i = 0; i < numElts; i++)
                if (min > A[i])
                {
                    min = A[i];
                    index = i;
                }
            return min;
        }

        /// <summary>
        /// Finds the minimum value in the given 1D double array and returns its index along with it.
        /// </summary>
        /// <param name="A">The array to be searched for</param>
        /// <param name="index">The index.</param>
        /// <returns>the maximum value</returns>
        public static int Max(this IList<int> A, out int index)
        {
            index = -1;
            var max = int.MinValue;
            var numElts = A.Count;
            for (var i = 0; i < numElts; i++)
                if (max < A[i])
                {
                    max = A[i];
                    index = i;
                }
            return max;
        }

        #endregion

        #region Find where a particular value is in a vector or matrix.

        /// <summary>
        /// Finds all the indices for the specified find value.
        /// </summary>
        /// <param name="A">The A.</param>
        /// <param name="FindVal">The find value.</param>
        /// <returns>IList&lt;System.Int32&gt;.</returns>
        public static IList<int> find(this IList<double> A, double FindVal)
        {
            return find(FindVal, A);
        }

        /// <summary>
        /// Finds all the indices for the specified find value.
        /// </summary>
        /// <param name="FindVal">The find value.</param>
        /// <param name="A">The A.</param>
        /// <returns>IList&lt;System.Int32&gt;.</returns>
        public static IList<int> find(double FindVal, IList<double> A)
        {
            return A.Select((value, index) => new {Item = value, Position = index})
                .Where(x => x.Item == FindVal).Select(a => a.Position).ToList();
        }

        /// <summary>
        /// Finds all the indices for the specified find value.
        /// </summary>
        /// <param name="A">The A.</param>
        /// <param name="FindVal">The find value.</param>
        /// <returns>IList&lt;System.Int32&gt;.</returns>
        public static IList<int> find(this IList<int> A, int FindVal)
        {
            return find(FindVal, A);
        }

        /// <summary>
        /// Finds all the indices for the specified find value.
        /// </summary>
        /// <param name="FindVal">The find value.</param>
        /// <param name="A">The A.</param>
        /// <returns>IList&lt;System.Int32&gt;.</returns>
        public static IList<int> find(int FindVal, IList<int> A)
        {
            return A.Select((value, index) => new {Item = value, Position = index})
                .Where(x => x.Item == FindVal).Select(a => a.Position).ToList();
        }

        /// <summary>
        /// Finds the [rowIndex, colIndex] for the specified find value.
        /// </summary>
        /// <param name="A">The A.</param>
        /// <param name="FindVal">The find value.</param>
        /// <returns>System.Int32[].</returns>
        public static int[] find(this double[,] A, double FindVal)
        {
            return find(FindVal, A);
        }

        /// <summary>
        /// Finds the [rowIndex, colIndex] for the specified find value.
        /// </summary>
        /// <param name="FindVal">The find value.</param>
        /// <param name="A">The A.</param>
        /// <returns>System.Int32[].</returns>
        public static int[] find(double FindVal, double[,] A)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    if (FindVal == A[i, j])
                        return new[] {i, j};
            return null;
        }

        /// <summary>
        /// Finds the [rowIndex, colIndex] for the specified find value.
        /// </summary>
        /// <param name="A">The A.</param>
        /// <param name="FindVal">The find value.</param>
        /// <returns>System.Int32[].</returns>
        public static int[] find(this int[,] A, int FindVal)
        {
            return find(FindVal, A);
        }

        /// <summary>
        /// Finds the [rowIndex, colIndex] for the specified find value.
        /// </summary>
        /// <param name="FindVal">The find value.</param>
        /// <param name="A">The A.</param>
        /// <returns>System.Int32[].</returns>
        public static int[] find(int FindVal, int[,] A)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    if (FindVal == A[i, j])
                        return new[] {i, j};
            return null;
        }

        #endregion
    }
}