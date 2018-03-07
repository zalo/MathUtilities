// ***********************************************************************
// Assembly         : StarMath
// Author           : MICampbell
// Created          : 05-14-2015
//
// Last Modified By : MICampbell
// Last Modified On : 07-07-2015
// ***********************************************************************
// <copyright file="make extract.cs" company="Design Engineering Lab -- MICampbell">
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
        #region Make simple Matrices functions       
        /// <summary>
        /// Makes a square matrix of size p by p of all zeros.
        /// </summary>
        /// <param name="p">The size (number of both rows and columns).</param>
        /// <returns>an empty (all zeros) square matrix (2D double).</returns>
        /// <exception cref="System.ArithmeticException">The size, p, must be a positive integer.</exception>
        public static double[,] makeZero(int p)
        {
            if (p <= 0) throw new ArithmeticException("The size, p, must be a positive integer.");
            return new double[p, p];
        }

        /// <summary>
        /// Makes a sqare matrix of size p by p of all zeros.
        /// </summary>
        /// <param name="p">The size (number of both rows and columns).</param>
        /// <returns>an empty (all zeros) square matrix (2D int).</returns>
        /// <exception cref="System.ArithmeticException">The size, p, must be a positive integer.</exception>
        public static int[,] makeZeroInt(int p)
        {
            if (p <= 0) throw new ArithmeticException("The size, p, must be a positive integer.");
            return new int[p, p];
        }

        /// <summary>
        /// Makes the zero vector.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">The size, p, must be a positive integer.</exception>
        public static double[] makeZeroVector(int p)
        {
            if (p <= 0) throw new ArithmeticException("The size, p, must be a positive integer.");
            return new double[p];
        }

        /// <summary>
        /// Makes the zero int vector.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>System.Int32[].</returns>
        /// <exception cref="System.ArithmeticException">The size, p, must be a positive integer.</exception>
        public static int[] makeZeroIntVector(int p)
        {
            if (p <= 0) throw new ArithmeticException("The size, p, must be a positive integer.");
            return new int[p];
        }

        /// <summary>
        /// Makes a matrix of size numRows by numCols of all zeros.
        /// </summary>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>an empty (all zeros) matrix.</returns>
        /// <exception cref="System.ArithmeticException">The number of rows, numRows, must be a positive integer.
        /// or
        /// The number of columns, numCols, must be a positive integer.</exception>
        public static double[,] makeZero(int numRows, int numCols)
        {
            if (numRows <= 0) throw new ArithmeticException("The number of rows, numRows, must be a positive integer.");
            if (numCols <= 0) throw new ArithmeticException("The number of columns, numCols, must be a positive integer.");
            return new double[numRows, numCols];
        }

        /// <summary>
        /// Makes a matrix of size numRows by numCols of all zeros.
        /// </summary>
        /// <param name="numRows">The number of rows.</param>
        /// <param name="numCols">The number of columns.</param>
        /// <returns>an empty (all zeros) matrix.</returns>
        /// <exception cref="System.ArithmeticException">The number of rows, numRows, must be a positive integer.
        /// or
        /// The number of columns, numCols, must be a positive integer.</exception>
        public static int[,] makeZeroInt(int numRows, int numCols)
        {
            if (numRows <= 0) throw new ArithmeticException("The number of rows, numRows, must be a positive integer.");
            if (numCols <= 0) throw new ArithmeticException("The number of columns, numCols, must be a positive integer.");
            return new int[numRows, numCols];
        }

        /// <summary>
        /// Makes an identity matrix of size p by p.
        /// </summary>
        /// <param name="p">The size (number of both rows and columns).</param>
        /// <returns>the identity matrix, I.</returns>
        /// <exception cref="System.ArithmeticException">The size, p, must be a positive integer.</exception>
        public static double[,] makeIdentity(int p)
        {
            if (p <= 0) throw new ArithmeticException("The size, p, must be a positive integer.");
            var I = new double[p, p];
            for (var i = 0; i < p; i++)
                I[i, i] = 1.0;
            return I;
        }

        /// <summary>
        /// Makes an identity matrix of size p by p.
        /// </summary>
        /// <param name="p">The size (number of both rows and columns).</param>
        /// <returns>the identity matrix, I.</returns>
        /// <exception cref="System.ArithmeticException">The size, p, must be a positive integer.</exception>
        public static int[,] makeIdentityInt(int p)
        {
            if (p <= 0) throw new ArithmeticException("The size, p, must be a positive integer.");
            var I = new int[p, p];
            for (var i = 0; i < p; i++)
                I[i, i] = 1;
            return I;
        }

        #endregion

        #region Make Progression

        /// <summary>
        /// Makes a linear progression from start to, but not including, the end.
        /// </summary>
        /// <param name="end">The end value (which will not be reached).</param>
        /// <param name="interval">The interval amount between values.</param>
        /// <param name="start">The starting value (the value of the first element).</param>
        /// <returns>Returns a double array with a series of numbers starting from start until the end
        /// with a distance of the interval between any pair of numbers.</returns>
        public static double[] makeLinearProgression(double end, double interval, double start = 0.0)
        {
            var NumOfElements = (int)((end - start) / interval);

            var prog = new double[NumOfElements];

            for (var i = 0; i < NumOfElements; i++)
                prog[i] = start + interval * i;

            return prog;
        }

        /// <summary>
        /// Makes a linear progression from start to, but not including, the end.
        /// </summary>
        /// <param name="end">The end value (which will not be reached).</param>
        /// <param name="interval">The interval amount between values.</param>
        /// <param name="start">The starting value (the value of the first element).</param>
        /// <returns>Returns an integer array with a series of numbers starting from start until the end
        /// with a distance of the interval between any pair of numbers.</returns>
        public static int[] makeLinearProgression(int end, int interval = 1, int start = 0)
        {
            var NumOfElements = (end - start) / interval;

            var prog = new int[NumOfElements];

            for (var i = 0; i < NumOfElements; i++)
                prog[i] = start + interval * i;

            return prog;
        }

        /// <summary>
        /// Makes a linear progression from start to, but not including, the end.
        /// </summary>
        /// <param name="end">The end value (which will not be reached).</param>
        /// <param name="numElements">The number of elements.</param>
        /// <param name="start">The starting value (the value of the first element).</param>
        /// <returns>Returns a double array with a series of numbers starting from start until the end
        /// with a distance of the interval between any pair of numbers.</returns>
        public static double[] makeLinearProgression(double end, int numElements, double start = 0.0)
        {
            var prog = new double[numElements];
            var interval = (end - start) / numElements;
            for (var i = 0; i < numElements; i++)
                prog[i] = start + interval * i;
            return prog;
        }

        #endregion


        /// <summary>
        /// Converts the 2D double array to a Sparse matrix.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="optionalTolerance">An optional tolerance.</param>
        /// <returns>SparseMatrix.</returns>
        public static SparseMatrix ConvertDenseToSparseMatrix(this double[,] A, double optionalTolerance = DefaultEqualityTolerance)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            List<int> rowIndices = new List<int>();
            List<int> colIndices = new List<int>();
            List<double> values = new List<double>();
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numCols; j++)
                {
                    if (!A[i, j].IsNegligible(optionalTolerance))
                    {
                        rowIndices.Add(i);
                        colIndices.Add(j);
                        values.Add(A[i, j]);
                    }
                }
            return new SparseMatrix(rowIndices, colIndices, values, numRows, numCols);
        }

        #region Get/Set/Remove parts of a matrix

        #region Get

        /// <summary>
        /// Gets the column of matrix, A.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>A double array that contains the requested column</returns>
        public static double[] GetColumn(this double[,] A, int colIndex)
        {
            return GetColumn(colIndex, A);
        }

        /// <summary>
        /// Gets the column of matrix, A.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>A double array that contains the requested column</returns>
        public static int[] GetColumn(this int[,] A, int colIndex)
        {
            return GetColumn(colIndex, A);
        }

        /// <summary>
        /// Gets a column of matrix, A.
        /// </summary>
        /// <param name="colIndex">The column index.</param>
        /// <param name="A">The matrix, A.</param>
        /// <returns>A double array that contains the requested column</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + colIndex
        ///                                     +  for getColumn is not in required range from 0 up to (but not including) 
        ///                                     + numRows + .</exception>
        public static double[] GetColumn(int colIndex, double[,] A)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if ((colIndex < 0) || (colIndex >= numCols))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + colIndex
                                    + " for getColumn is not in required range from 0 up to (but not including) "
                                    + numRows + ".");
            var v = new double[numRows];
            for (var i = 0; i < numRows; i++)
                v[i] = A[i, colIndex];
            return v;
        }

        /// <summary>
        /// Gets a column of matrix, A.
        /// </summary>
        /// <param name="colIndex">The column index.</param>
        /// <param name="A">The matrix, A.</param>
        /// <returns>A double array that contains the requested column</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + colIndex
        ///                                     +  for getColumn is not in required range from 0 up to (but not including) 
        ///                                     + numRows + .</exception>
        public static int[] GetColumn(int colIndex, int[,] A)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if ((colIndex < 0) || (colIndex >= numCols))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + colIndex
                                    + " for getColumn is not in required range from 0 up to (but not including) "
                                    + numRows + ".");
            var v = new int[numRows];
            for (var i = 0; i < numRows; i++)
                v[i] = A[i, colIndex];
            return v;
        }

        /// <summary>
        /// Get more than one column from a given 2D double array.
        /// </summary>
        /// <param name="A">2D double array from which columns need to be extracted</param>
        /// <param name="ColumnList">The column list indices.</param>
        /// <returns>A  2D double array that contains all the requested columns</returns>
        public static double[,] GetColumns(this double[,] A, IList<int> ColumnList)
        {
            return GetColumns(ColumnList, A);
        }

        /// <summary>
        /// Get more than one column from a given 2D int array.
        /// </summary>
        /// <param name="A">2D int array from which columns need to be extracted</param>
        /// <param name="ColumnList">The column list indices.</param>
        /// <returns>A  2D int array that contains all the requested columns</returns>
        public static int[,] GetColumns(this int[,] A, IList<int> ColumnList)
        {
            return GetColumns(ColumnList, A);
        }

        /// <summary>
        /// Get more than one column from a given 2D double array.
        /// </summary>
        /// <param name="ColumnList">The column list indices.</param>
        /// <param name="A">2D double array from which columns need to be extracted</param>
        /// <returns>A  2D double array that contains all the requested columns</returns>
        public static double[,] GetColumns(IList<int> ColumnList, double[,] A)
        {
            var columns = new double[A.GetLength(0), ColumnList.Count];
            var k = 0;
            foreach (var i in ColumnList)
                SetColumn(k++, columns, GetColumn(i, A));
            return columns;
        }

        /// <summary>
        /// Get more than one column from a given 2D double array.
        /// </summary>
        /// <param name="ColumnList">The column list indices.</param>
        /// <param name="A">2D int array from which columns need to be extracted</param>
        /// <returns>A  2D int array that contains all the requested columns</returns>
        public static int[,] GetColumns(IList<int> ColumnList, int[,] A)
        {
            var Columns = new int[A.GetLength(0), ColumnList.Count];
            var k = 0;
            foreach (var i in ColumnList)
                SetColumn(k++, Columns, GetColumn(i, A));
            return Columns;
        }


        /// <summary>
        /// Gets a row of matrix, A.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <returns>A double array that contains the requested row</returns>
        public static double[] GetRow(this double[,] A, int rowIndex)
        {
            return GetRow(rowIndex, A);
        }

        /// <summary>
        /// Gets a row of matrix, A.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <returns>A double array that contains the requested row</returns>
        public static int[] GetRow(this int[,] A, int rowIndex)
        {
            return GetRow(rowIndex, A);
        }

        /// <summary>
        /// Gets a row of matrix, A.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="A">The matrix, A.</param>
        /// <returns>A double array that contains the requested row</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + rowIndex
        ///                                     +  for getRow is not in required range from 0 up to (but not including) 
        ///                                     + numRows + .</exception>
        public static double[] GetRow(int rowIndex, double[,] A)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if ((rowIndex < 0) || (rowIndex >= numRows))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + rowIndex
                                    + " for getRow is not in required range from 0 up to (but not including) "
                                    + numRows + ".");
            var v = new double[numCols];
            for (var i = 0; i < numCols; i++)
                v[i] = A[rowIndex, i];
            return v;
        }

        /// <summary>
        /// Gets a row of matrix, A.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="A">The matrix, A.</param>
        /// <returns>A double array that contains the requested row</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + rowIndex
        ///                                     +  for getRow is not in required range from 0 up to (but not including) 
        ///                                     + numRows + .</exception>
        public static int[] GetRow(int rowIndex, int[,] A)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if ((rowIndex < 0) || (rowIndex >= numRows))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + rowIndex
                                    + " for getRow is not in required range from 0 up to (but not including) "
                                    + numRows + ".");
            var v = new int[numCols];
            for (var i = 0; i < numCols; i++)
                v[i] = A[rowIndex, i];
            return v;
        }

        /// <summary>
        /// Get more than one row from a given 2D double array.
        /// </summary>
        /// <param name="A">2D double array from which rows need to be extracted</param>
        /// <param name="RowList">The row list indices.</param>
        /// <returns>A  2D double array that contains all the requested rows</returns>
        public static double[,] GetRows(this double[,] A, IList<int> RowList)
        {
            return GetRows(RowList, A);
        }

        /// <summary>
        /// Get more than one row from a given 2D int array.
        /// </summary>
        /// <param name="A">2D int array from which rows need to be extracted</param>
        /// <param name="RowList">The row list indices.</param>
        /// <returns>A  2D int array that contains all the requested rows</returns>
        public static int[,] GetRows(this int[,] A, IList<int> RowList)
        {
            return GetRows(RowList, A);
        }

        /// <summary>
        /// Get more than one row from a given 2D double array.
        /// </summary>
        /// <param name="RowList">The row list indices.</param>
        /// <param name="A">2D double array from which rows need to be extracted</param>
        /// <returns>A  2D double array that contains all the requested rows</returns>
        public static double[,] GetRows(IList<int> RowList, double[,] A)
        {
            var Rows = new double[RowList.Count, A.GetLength(1)];
            var k = 0;
            foreach (var i in RowList)
                SetRow(k++, Rows, GetRow(i, A));
            return Rows;
        }

        /// <summary>
        /// Get more than one row from a given 2D double array.
        /// </summary>
        /// <param name="RowList">The row list indices.</param>
        /// <param name="A">2D int array from which rows need to be extracted</param>
        /// <returns>A  2D int array that contains all the requested rows</returns>
        public static int[,] GetRows(IList<int> RowList, int[,] A)
        {
            var Rows = new int[RowList.Count, A.GetLength(1)];
            var k = 0;
            foreach (var i in RowList)
                SetRow(k++, Rows, GetRow(i, A));
            return Rows;
        }

        #endregion

        #region Set

        /// <summary>
        /// Sets/Replaces the given row of matrix A with the vector v.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="rowIndex">The index of the row, rowIndex.</param>
        /// <param name="v">The vector, v.</param>
        public static void SetRow(this double[,] A, int rowIndex, IList<double> v)
        {
            SetRow(rowIndex, A, v);
        }

        /// <summary>
        /// Sets/Replaces the given row of matrix A with the vector v.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="rowIndex">The index of the row, rowIndex.</param>
        /// <param name="v">The vector, v.</param>
        public static void SetRow(this int[,] A, int rowIndex, IList<int> v)
        {
            SetRow(rowIndex, A, v);
        }

        /// <summary>
        /// Sets/Replaces the given row of matrix A with the vector v.
        /// </summary>
        /// <param name="rowIndex">The index of the row, rowIndex.</param>
        /// <param name="A">The matrix, A.</param>
        /// <param name="v">The vector, v.</param>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + rowIndex
        ///                                     +  for getRow is not in required range from 0 up to (but not including) 
        ///                                     + numRows + .</exception>
        public static void SetRow(int rowIndex, double[,] A, IList<double> v)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if ((rowIndex < 0) || (rowIndex >= numRows))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + rowIndex
                                    + " for getRow is not in required range from 0 up to (but not including) "
                                    + numRows + ".");
            for (var i = 0; i < numCols; i++)
                A[rowIndex, i] = v[i];
        }

        /// <summary>
        /// Sets/Replaces the given row of matrix A with the vector v.
        /// </summary>
        /// <param name="rowIndex">The index of the row, rowIndex.</param>
        /// <param name="A">The matrix, A.</param>
        /// <param name="v">The vector, v.</param>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + rowIndex
        ///                                     +  for getRow is not in required range from 0 up to (but not including) 
        ///                                     + numRows + .</exception>
        public static void SetRow(int rowIndex, int[,] A, IList<int> v)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if ((rowIndex < 0) || (rowIndex >= numRows))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + rowIndex
                                    + " for getRow is not in required range from 0 up to (but not including) "
                                    + numRows + ".");
            for (var i = 0; i < numCols; i++)
                A[rowIndex, i] = v[i];
        }

        /// <summary>
        /// Sets/Replaces the given column of matrix A with the vector v.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="colIndex">The index of the column, rowIndex.</param>
        /// <param name="v">The vector, v.</param>
        public static void SetColumn(this double[,] A, int colIndex, IList<double> v)
        {
            SetColumn(colIndex, A, v);
        }

        /// <summary>
        /// Sets/Replaces the given column of matrix A with the vector v.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="colIndex">The index of the column, rowIndex.</param>
        /// <param name="v">The vector, v.</param>
        public static void SetColumn(this int[,] A, int colIndex, IList<int> v)
        {
            SetColumn(colIndex, A, v);
        }

        /// <summary>
        /// Sets/Replaces the given column of matrix A with the vector v.
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <param name="A">The A.</param>
        /// <param name="v">The v.</param>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + colIndex
        ///                                     +  for getColumn is not in required range from 0 up to (but not including) 
        ///                                     + numCols + .</exception>
        public static void SetColumn(int colIndex, double[,] A, IList<double> v)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if ((colIndex < 0) || (colIndex >= numCols))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + colIndex
                                    + " for getColumn is not in required range from 0 up to (but not including) "
                                    + numCols + ".");
            for (var i = 0; i < numRows; i++)
                A[i, colIndex] = v[i];
        }

        /// <summary>
        /// Sets/Replaces the given column of matrix A with the vector v.
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <param name="A">The A.</param>
        /// <param name="v">The v.</param>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + colIndex
        ///                                     +  for getColumn is not in required range from 0 up to (but not including) 
        ///                                     + numCols + .</exception>
        public static void SetColumn(int colIndex, int[,] A, IList<int> v)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if ((colIndex < 0) || (colIndex >= numCols))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + colIndex
                                    + " for getColumn is not in required range from 0 up to (but not including) "
                                    + numCols + ".");
            for (var i = 0; i < numRows; i++)
                A[i, colIndex] = v[i];
        }

        #endregion

        #region Remove

        /// <summary>
        /// Removes the given row of matrix A with the vector v.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="rowIndex">The index of the row, rowIndex.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + rowIndex
        ///                                     +  for RemoveRow is not in required range from 0 up to (but not including) 
        ///                                     + numRows + .</exception>
        public static double[,] RemoveRow(this double[,] A, int rowIndex)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if ((rowIndex < 0) || (rowIndex >= numRows))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + rowIndex
                                    + " for RemoveRow is not in required range from 0 up to (but not including) "
                                    + numRows + ".");
            var B = new double[numRows - 1, numCols];
            for (var i = 0; i < rowIndex; i++)
                B.SetRow(i, A.GetRow(i));
            for (var i = rowIndex + 1; i < numRows; i++)
                B.SetRow(i - 1, A.GetRow(i));
            return B;
        }

        /// <summary>
        /// Removes the given row of matrix A with the vector v.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="rowIndex">The index of the row, rowIndex.</param>
        /// <returns>System.Int32[].</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + rowIndex
        ///                                     +  for RemoveRow is not in required range from 0 up to (but not including) 
        ///                                     + numRows + .</exception>
        public static int[,] RemoveRow(this int[,] A, int rowIndex)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if ((rowIndex < 0) || (rowIndex >= numRows))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + rowIndex
                                    + " for RemoveRow is not in required range from 0 up to (but not including) "
                                    + numRows + ".");
            var B = new int[numRows - 1, numCols];
            for (var i = 0; i < rowIndex; i++)
                B.SetRow(i, A.GetRow(i));
            for (var i = rowIndex + 1; i < numRows; i++)
                B.SetRow(i - 1, A.GetRow(i));
            return B;
        }


        /// <summary>
        /// Removes the given column of matrix A with the vector v.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="colIndex">The index of the column, rowIndex.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + colIndex
        ///                                     +  for RemoveColumn is not in required range from 0 up to (but not including) 
        ///                                     + numCols + .</exception>
        public static double[,] RemoveColumn(this double[,] A, int colIndex)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if ((colIndex < 0) || (colIndex >= numRows))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + colIndex
                                    + " for RemoveColumn is not in required range from 0 up to (but not including) "
                                    + numCols + ".");
            var B = new double[numRows, numCols - 1];
            for (var i = 0; i < colIndex; i++)
                B.SetColumn(i, A.GetColumn(i));
            for (var i = colIndex + 1; i < numCols; i++)
                B.SetColumn(i - 1, A.GetColumn(i));
            return B;
        }

        /// <summary>
        /// Removes the given column of matrix A with the vector v.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="colIndex">The index of the column, rowIndex.</param>
        /// <returns>System.Int32[].</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + colIndex
        ///                                     +  for RemoveColumn is not in required range from 0 up to (but not including) 
        ///                                     + numCols + .</exception>
        public static int[,] RemoveColumn(this int[,] A, int colIndex)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            if ((colIndex < 0) || (colIndex >= numRows))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + colIndex
                                    + " for RemoveColumn is not in required range from 0 up to (but not including) "
                                    + numCols + ".");
            var B = new int[numRows, numCols - 1];
            for (var i = 0; i < colIndex; i++)
                B.SetColumn(i, A.GetColumn(i));
            for (var i = colIndex + 1; i < numCols; i++)
                B.SetColumn(i - 1, A.GetColumn(i));
            return B;
        }

        /// <summary>
        /// Removes the given row of matrix A with the vector v.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="rowIndices">The row indices.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">
        /// StarMath Size Error: A row index, with value  + rowIndices.Max() +
        ///                                     , in the provided rowIndices for RemoveRows exceeds the number of rows (number of rows =  +
        ///                                     numRows +
        ///                                     ) in the provided matrix.
        /// or
        /// StarMath Size Error: An there are more rows to remove (rowIndices.Count =  +
        ///                                     rowIndices.Count +
        ///                                     ) than there are rows in the matrix provided to RemoveRows (number of rows =  +
        ///                                     numRows + ).
        /// </exception>
        public static double[,] RemoveRows(this double[,] A, IList<int> rowIndices)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            var numToRemove = rowIndices.Count;
            if (rowIndices.Max() >= numRows)
                throw new ArithmeticException("StarMath Size Error: A row index, with value " + rowIndices.Max() +
                                    ", in the provided rowIndices for RemoveRows exceeds the number of rows (number of rows = " +
                                    numRows +
                                    ") in the provided matrix.");
            if (rowIndices.Count >= numRows)
                throw new ArithmeticException("StarMath Size Error: An there are more rows to remove (rowIndices.Count = " +
                                    rowIndices.Count +
                                    ") than there are rows in the matrix provided to RemoveRows (number of rows = " +
                                    numRows + ").");
            var sortedRowIndices = rowIndices.OrderBy(x => x).ToArray();
            var B = new double[numRows - numToRemove, numCols];

            var k = 0; //rowIndices position
            for (var i = 0; i < numRows; i++)
                if (k < numToRemove && sortedRowIndices[k] == i) k++;
                else B.SetRow(i - k, A.GetRow(i));
            return B;
        }

        /// <summary>
        /// Removes the given rows of matrix A.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="rowIndices">The row indices.</param>
        /// <returns>System.Int32[].</returns>
        /// <exception cref="System.ArithmeticException">
        /// StarMath Size Error: A row index, with value  + rowIndices.Max() +
        ///                                     , in the provided rowIndices for RemoveRows exceeds the number of rows (number of rows =  +
        ///                                     numRows +
        ///                                     ) in the provided matrix.
        /// or
        /// StarMath Size Error: An there are more rows to remove (rowIndices.Count =  +
        ///                                     rowIndices.Count +
        ///                                     ) than there are rows in the matrix provided to RemoveRows (number of rows =  +
        ///                                     numRows + ).
        /// </exception>
        /// <exception cref="ArithmeticException">StarMath Size Error: A row index, with value  + rowIndices.Max() +
        /// , in the provided rowIndices for RemoveRows exceeds the number of rows (number of rows =  + numRows +
        /// ) in the provided matrix.
        /// or
        /// StarMath Size Error: An there are more rows to remove (rowIndices.Count =  + rowIndices.Count +
        /// ) than there are rows in the matrix provided to RemoveRows (number of rows =  + numRows + ).</exception>
        public static int[,] RemoveRows(this int[,] A, IList<int> rowIndices)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            var numToRemove = rowIndices.Count;
            if (rowIndices.Max() >= numRows)
                throw new ArithmeticException("StarMath Size Error: A row index, with value " + rowIndices.Max() +
                                    ", in the provided rowIndices for RemoveRows exceeds the number of rows (number of rows = " +
                                    numRows +
                                    ") in the provided matrix.");
            if (rowIndices.Count >= numRows)
                throw new ArithmeticException("StarMath Size Error: An there are more rows to remove (rowIndices.Count = " +
                                    rowIndices.Count +
                                    ") than there are rows in the matrix provided to RemoveRows (number of rows = " +
                                    numRows + ").");
            var sortedRowIndices = rowIndices.OrderBy(x => x).ToArray();
            var B = new int[numRows - numToRemove, numCols];

            var k = 0; //rowIndices position
            for (var i = 0; i < numRows; i++)
                if (k < numToRemove && sortedRowIndices[k] == i) k++;
                else B.SetRow(i - k, A.GetRow(i));
            return B;
        }


        /// <summary>
        /// Removes the given columns of matrix A.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="colIndices">The col indices.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">
        /// StarMath Size Error: A row index, with value  + colIndices.Max() +
        ///                                     , in the provided rowIndices for RemoveColumns exceeds the number of rows (number of rows =  +
        ///                                     numCols +
        ///                                     ) in the provided matrix.
        /// or
        /// StarMath Size Error: An there are more rows to remove (rowIndices.Count = 
        ///                                     + colIndices.Count +
        ///                                     ) than there are rows in the matrix provided to RemoveColumns (number of rows =  +
        ///                                     numCols + ).
        /// </exception>
        /// <exception cref="ArithmeticException">StarMath Size Error: A row index, with value  + colIndices.Max() +
        /// , in the provided rowIndices for RemoveColumns exceeds the number of rows (number of rows =  + numCols +
        /// ) in the provided matrix.
        /// or
        /// StarMath Size Error: An there are more rows to remove (rowIndices.Count =
        /// + colIndices.Count +
        /// ) than there are rows in the matrix provided to RemoveColumns (number of rows =  + numCols + ).</exception>
        public static double[,] RemoveColumns(this double[,] A, IList<int> colIndices)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            var numToRemove = colIndices.Count;
            if (colIndices.Max() >= numCols)
                throw new ArithmeticException("StarMath Size Error: A row index, with value " + colIndices.Max() +
                                    ", in the provided rowIndices for RemoveColumns exceeds the number of rows (number of rows = " +
                                    numCols +
                                    ") in the provided matrix.");
            if (colIndices.Count >= numCols)
                throw new ArithmeticException("StarMath Size Error: An there are more rows to remove (rowIndices.Count = "
                                    + colIndices.Count +
                                    ") than there are rows in the matrix provided to RemoveColumns (number of rows = " +
                                    numCols + ").");
            var B = new double[numRows, numCols - colIndices.Count];
         var sortedColIndices = colIndices.OrderBy(x => x).ToArray();
           var k = 0; //colIndices position
            for (var i = 0; i < numCols; i++)
                if (k < numToRemove && sortedColIndices[k] == i) k++;
                else B.SetColumn(i - k, A.GetColumn(i));
            return B;
        }

        /// <summary>
        /// Removes the given columns of matrix A.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="colIndices">The col indices.</param>
        /// <returns>System.Int32[].</returns>
        /// <exception cref="System.ArithmeticException">
        /// StarMath Size Error: A row index, with value  + colIndices.Max() +
        ///                                     , in the provided rowIndices for RemoveColumns exceeds the number of rows (number of rows =  +
        ///                                     numCols +
        ///                                     ) in the provided matrix.
        /// or
        /// StarMath Size Error: An there are more rows to remove (rowIndices.Count = 
        ///                                     + colIndices.Count +
        ///                                     ) than there are rows in the matrix provided to RemoveColumns (number of rows =  +
        ///                                     numCols + ).
        /// </exception>
        /// <exception cref="ArithmeticException">StarMath Size Error: A row index, with value  + colIndices.Max() +
        /// , in the provided rowIndices for RemoveColumns exceeds the number of rows (number of rows =  + numCols +
        /// ) in the provided matrix.
        /// or
        /// StarMath Size Error: An there are more rows to remove (rowIndices.Count =
        /// + colIndices.Count +
        /// ) than there are rows in the matrix provided to RemoveColumns (number of rows =  + numCols + ).</exception>
        public static int[,] RemoveColumns(this int[,] A, IList<int> colIndices)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            var numToRemove = colIndices.Count;
            if (colIndices.Max() >= numCols)
                throw new ArithmeticException("StarMath Size Error: A row index, with value " + colIndices.Max() +
                                    ", in the provided rowIndices for RemoveColumns exceeds the number of rows (number of rows = " +
                                    numCols +
                                    ") in the provided matrix.");
            if (colIndices.Count >= numCols)
                throw new ArithmeticException("StarMath Size Error: An there are more rows to remove (rowIndices.Count = "
                                    + colIndices.Count +
                                    ") than there are rows in the matrix provided to RemoveColumns (number of rows = " +
                                    numCols + ").");
            var B = new int[numRows, numCols - colIndices.Count];
            var sortedColIndices = colIndices.OrderBy(x => x).ToArray();
            var k = 0; //colIndices position
            for (var i = 0; i < numCols; i++)
                if (k < numToRemove && sortedColIndices[k] == i) k++;
                else B.SetColumn(i - k, A.GetColumn(i));
            return B;
        }

        #endregion

        /// <summary>
        /// Get some portion of a vector and put in a new vector.
        /// </summary>
        /// <param name="A">1D double array from which elements need to be extracted</param>
        /// <param name="indexList">The indices of the elements.</param>
        /// <returns>A single 1D double array that contains all the requested elements.</returns>
        public static double[] GetPartialVector(this IList<double> A, IList<int> indexList)
        {
            var result = new double[indexList.Count];
            for (var i = 0; i < indexList.Count; i++)
                result[i] = A[indexList[i]];
            return result;
        }

        /// <summary>
        /// Get some portion of a vector and put in a new vector.
        /// </summary>
        /// <param name="A">1D double array from which elements need to be extracted</param>
        /// <param name="indexList">The indices of the elements.</param>
        /// <returns>A single 1D double array that contains all the requested elements.</returns>
        public static int[] GetPartialVector(this IList<int> A, IList<int> indexList)
        {
            var result = new int[indexList.Count];
            for (var i = 0; i < indexList.Count; i++)
                result[i] = A[indexList[i]];
            return result;
        }

        /// <summary>
        /// Removes the given entry of vector A.
        /// </summary>
        /// <param name="A">The vector, A.</param>
        /// <param name="index">The index to remove.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + index
        ///                                     +  for RemoveVectorCell is not in required range from 0 up to (but not including) 
        ///                                     + length + .</exception>
        /// <exception cref="ArithmeticException">StarMath Size Error: An index value of
        /// + index
        /// +  for RemoveVectorCell is not in required range from 0 up to (but not including)
        /// + length + .</exception>
        public static double[] RemoveVectorCell(this IList<double> A, int index)
        {
            var length = A.Count;
            if ((index < 0) || (index >= length))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + index
                                    + " for RemoveVectorCell is not in required range from 0 up to (but not including) "
                                    + length + ".");
            var B = new List<double>(A);
            B.RemoveAt(index);
            return B.ToArray();
        }

        /// <summary>
        /// Removes the given entry of vector A.
        /// </summary>
        /// <param name="A">The vector, A.</param>
        /// <param name="index">The index to remove.</param>
        /// <returns>System.Int32[].</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: An index value of 
        ///                                     + index
        ///                                     +  for RemoveVectorCell is not in required range from 0 up to (but not including) 
        ///                                     + length + .</exception>
        /// <exception cref="ArithmeticException">StarMath Size Error: An index value of
        /// + index
        /// +  for RemoveVectorCell is not in required range from 0 up to (but not including)
        /// + length + .</exception>
        public static int[] RemoveVectorCell(this IList<int> A, int index)
        {
            var length = A.Count;
            if ((index < 0) || (index >= length))
                throw new ArithmeticException("StarMath Size Error: An index value of "
                                    + index
                                    + " for RemoveVectorCell is not in required range from 0 up to (but not including) "
                                    + length + ".");
            var B = new List<int>(A);
            B.RemoveAt(index);
            return B.ToArray();
        }


        /// <summary>
        /// Removes the given entry of vector A.
        /// </summary>
        /// <param name="A">The vector, A.</param>
        /// <param name="indices">The indices.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">
        /// StarMath Size Error: A cell index, with value  + indices.Max() +
        ///                                     , in the provided indices for RemoveVectorCells exceeds the number of entries (size =  +
        ///                                     length +
        ///                                     ) in the provided vector.
        /// or
        /// StarMath Size Error: There are more cells to remove (Count =  + indices.Count +
        ///                                     ) than there are cells in the vector provided to RemoveVectorCells (Count =  +
        ///                                     length + ).
        /// </exception>
        /// <exception cref="ArithmeticException">StarMath Size Error: An index value of
        /// + index
        /// +  for RemoveVectorCell is not in required range from 0 up to (but not including)
        /// + length + .</exception>
        public static double[] RemoveVectorCells(this IList<double> A, IList<int> indices)
        {
            var length = A.Count;
            var numToRemove = indices.Count;
            if (indices.Max() >= length)
                throw new ArithmeticException("StarMath Size Error: A cell index, with value " + indices.Max() +
                                    ", in the provided indices for RemoveVectorCells exceeds the number of entries (size = " +
                                    length +
                                    ") in the provided vector.");
            if (indices.Count >= length)
                throw new ArithmeticException("StarMath Size Error: There are more cells to remove (Count = " + indices.Count +
                                    ") than there are cells in the vector provided to RemoveVectorCells (Count = " +
                                    length + ").");
            var B = new double[length - numToRemove];
            var sortedRowIndices = indices.OrderBy(x => x).ToArray();
            var k = 0; //rowIndices position
            for (var i = 0; i < length; i++)
                if (k < numToRemove && sortedRowIndices[k] == i) k++;
                else B[i - k] = A[i];
            return B;
        }

        /// <summary>
        /// Removes the given entry of vector A.
        /// </summary>
        /// <param name="A">The vector, A.</param>
        /// <param name="indices">The indices.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">
        /// StarMath Size Error: A cell index, with value  + indices.Max() +
        ///                                     , in the provided indices for RemoveVectorCells exceeds the number of entries (size =  +
        ///                                     length +
        ///                                     ) in the provided vector.
        /// or
        /// StarMath Size Error: There are more cells to remove (Count =  + indices.Count +
        ///                                     ) than there are cells in the vector provided to RemoveVectorCells (Count =  +
        ///                                     length + ).
        /// </exception>
        /// <exception cref="ArithmeticException">StarMath Size Error: An index value of
        /// + index
        /// +  for RemoveVectorCell is not in required range from 0 up to (but not including)
        /// + length + .</exception>
        public static int[] RemoveVectorCells(this IList<int> A, IList<int> indices)
        {
            var length = A.Count;
            var numToRemove = indices.Count;
            if (indices.Max() >= length)
                throw new ArithmeticException("StarMath Size Error: A cell index, with value " + indices.Max() +
                                    ", in the provided indices for RemoveVectorCells exceeds the number of entries (size = " +
                                    length +
                                    ") in the provided vector.");
            if (indices.Count >= length)
                throw new ArithmeticException("StarMath Size Error: There are more cells to remove (Count = " + indices.Count +
                                    ") than there are cells in the vector provided to RemoveVectorCells (Count = " +
                                    length + ").");
            var B = new int[length - numToRemove];
            var sortedRowIndices = indices.OrderBy(x => x).ToArray();
            var k = 0; //rowIndices position
            for (var i = 0; i < length; i++)
                if (k < numToRemove && sortedRowIndices[k] == i) k++;
                else B[i - k] = A[i];
            return B;
        }

        #endregion

        #region Join Matrices into taller/fatter matrix

        /// <summary>
        /// Jions two 2D double arrays side by side and returns the results. The given variables remain unchanged
        /// </summary>
        /// <param name="Matrix1">The Matrix that comes on the left.</param>
        /// <param name="Matrix2">Matrix that is attached to the right</param>
        /// <returns>A 2D double array that has Matrix1 and Matrix2 side by side</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: Row dimensions do not match for matrix1 and matrix2</exception>
        public static double[,] JoinCol(this double[,] Matrix1, double[,] Matrix2)
        {
            if (Matrix1.GetLength(0) != Matrix2.GetLength(0))
                throw new ArithmeticException("StarMath Size Error: Row dimensions do not match for matrix1 and matrix2");
            var NumRows = Matrix1.GetLength(0);
            var NumCols = Matrix1.GetLength(1) + Matrix2.GetLength(1);
            var Mat1Cols = Matrix1.GetLength(1);
            var Mat2Cols = Matrix2.GetLength(1);

            var JointMatrix = new double[NumRows, NumCols];

            for (var j = 0; j < Mat1Cols; j++)
                for (var k = 0; k < NumRows; k++)
                    JointMatrix[k, j] = Matrix1[k, j];


            for (var j = 0; j < Mat2Cols; j++)
                for (var k = 0; k < NumRows; k++)
                    JointMatrix[k, j + Mat1Cols] = Matrix2[k, j];

            return JointMatrix;
        }

        /// <summary>
        /// Joins two 2D double arrays one under the other and returns the results. The given variables remain unchanged
        /// </summary>
        /// <param name="Matrix1">The Matrix that comes on the top.</param>
        /// <param name="Matrix2">Matrix that is attached to the bottom</param>
        /// <returns>A 2D double array that has Matrix1 and Matrix2 one below the other</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: Column dimensions do not match for matrix1 and matrix2</exception>
        public static double[,] JoinRow(this double[,] Matrix1, double[,] Matrix2)
        {
            if (Matrix1.GetLength(1) != Matrix2.GetLength(1))
                throw new ArithmeticException("StarMath Size Error: Column dimensions do not match for matrix1 and matrix2");
            var numRows = Matrix1.GetLength(0) + Matrix2.GetLength(0);
            var numCols = Matrix1.GetLength(1);
            var mat1Rows = Matrix1.GetLength(0);
            var mat2Rows = Matrix2.GetLength(0);
            var JointMatrix = new double[numRows, numCols];

            for (var j = 0; j < mat1Rows; j++)
                for (var k = 0; k < numCols; k++)
                    JointMatrix[j, k] = Matrix1[j, k];

            for (var j = 0; j < mat2Rows; j++)
                for (var k = 0; k < numCols; k++)
                    JointMatrix[j + mat1Rows, k] = Matrix2[j, k];

            return JointMatrix;
        }

        /// <summary>
        /// Jions two 2D int arrays side by side and returns the results. The given variables remain unchanged
        /// </summary>
        /// <param name="Matrix1">The Matrix that comes on the left.</param>
        /// <param name="Matrix2">Matrix that is attached to the right</param>
        /// <returns>A 2D int array that has Matrix1 and Matrix2 side by side</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: Row dimensions do not match for matrix1 and matrix2</exception>
        public static int[,] JoinCol(this int[,] Matrix1, int[,] Matrix2)
        {
            if (Matrix1.GetLength(0) != Matrix2.GetLength(0))
                throw new ArithmeticException("StarMath Size Error: Row dimensions do not match for matrix1 and matrix2");
            var NumRows = Matrix1.GetLength(0);
            var NumCols = Matrix1.GetLength(1) + Matrix2.GetLength(1);
            var Mat1Cols = Matrix1.GetLength(1);
            var Mat2Cols = Matrix2.GetLength(1);

            var JointMatrix = new int[NumRows, NumCols];

            for (var j = 0; j < Mat1Cols; j++)
                for (var k = 0; k < NumRows; k++)
                    JointMatrix[k, j] = Matrix1[k, j];


            for (var j = 0; j < Mat2Cols; j++)
                for (var k = 0; k < NumRows; k++)
                    JointMatrix[k, j + Mat1Cols] = Matrix2[k, j];

            return JointMatrix;
        }

        /// <summary>
        /// Joins two 2D int arrays one under the other and returns the results. The given variables remain unchanged
        /// </summary>
        /// <param name="Matrix1">The Matrix that comes on the top.</param>
        /// <param name="Matrix2">Matrix that is attached to the bottom</param>
        /// <returns>A 2D int array that has Matrix1 and Matrix2 one below the other</returns>
        /// <exception cref="System.ArithmeticException">StarMath Size Error: Column dimensions do not match for matrix1 and matrix2</exception>
        public static int[,] JoinRow(this int[,] Matrix1, int[,] Matrix2)
        {
            if (Matrix1.GetLength(1) != Matrix2.GetLength(1))
                throw new ArithmeticException("StarMath Size Error: Column dimensions do not match for matrix1 and matrix2");
            var numRows = Matrix1.GetLength(0) + Matrix2.GetLength(0);
            var numCols = Matrix1.GetLength(1);
            var mat1Rows = Matrix1.GetLength(0);
            var mat2Rows = Matrix2.GetLength(0);
            var JointMatrix = new int[numRows, numCols];

            for (var j = 0; j < mat1Rows; j++)
                for (var k = 0; k < numCols; k++)
                    JointMatrix[j, k] = Matrix1[j, k];

            for (var j = 0; j < mat2Rows; j++)
                for (var k = 0; k < numCols; k++)
                    JointMatrix[j + mat1Rows, k] = Matrix2[j, k];

            return JointMatrix;
        }

        #endregion

        #region Join Vectors into one long Vector

        /// <summary>
        /// Concatenates two 1D double arrays and returns the result. The given variables remain unchanged
        /// </summary>
        /// <param name="Array1">Array that comes first.</param>
        /// <param name="Array2">Array that is appended to the end of the first array</param>
        /// <returns>An double array that has Array1 and Array2 side by side</returns>
        public static double[] JoinVectors(this IList<double> Array1, IList<double> Array2)
        {
            var Array1Length = Array1.Count;
            var Array2Length = Array2.Count;
            var JointArray = new double[Array1Length + Array2Length];

            for (var j = 0; j < Array1Length; j++)
                JointArray[j] = Array1[j];

            for (var j = 0; j < Array2Length; j++)
                JointArray[j + Array1Length] = Array2[j];
            return JointArray;
        }

        /// <summary>
        /// Concatenates two 1D integer arrays and returns the result. The given variables remain unchanged
        /// </summary>
        /// <param name="Array1">Array that comes to the left.</param>
        /// <param name="Array2">Array that is appended to the end of the first array</param>
        /// <returns>An integer array that has Array1 and Array2 side by side</returns>
        public static int[] JoinVectors(this IList<int> Array1, IList<int> Array2)
        {
            var Array1Length = Array1.Count;
            var Array2Length = Array2.Count;
            var NumElements = Array1Length + Array2Length;
            var JointArray = new int[NumElements];

            for (var j = 0; j < Array1Length; j++)
                JointArray[j] = Array1[j];

            for (var j = 0; j < Array2Length; j++)
                JointArray[j + Array1Length] = Array2[j];

            return JointArray;
        }

        #endregion

        #region Join/Flatten Matrix into one long vector

        /// <summary>
        /// Joins the matrix columns into vector.
        /// </summary>
        /// <param name="A">The matrix of doubles, A.</param>
        /// <returns>System.Double[].</returns>
        public static double[] JoinMatrixColumnsIntoVector(this double[,] A)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            var B = new double[numRows * numCols];
            for (var i = 0; i < numCols; i++)
                GetColumn(i, A).CopyTo(B, i * numRows);
            return B;
        }

        /// <summary>
        /// Joins the matrix columns into vector.
        /// </summary>
        /// <param name="A">The matrix of integers, A.</param>
        /// <returns>System.Int32[].</returns>
        public static int[] JoinMatrixColumnsIntoVector(this int[,] A)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            var B = new int[numRows * numCols];
            for (var i = 0; i < numCols; i++)
                GetColumn(i, A).CopyTo(B, i * numRows);
            return B;
        }

        /// <summary>
        /// Joins the matrix rows into vector.
        /// </summary>
        /// <param name="A">The matrix of doubles, A.</param>
        /// <returns>System.Double[].</returns>
        public static double[] JoinMatrixRowsIntoVector(this double[,] A)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            var B = new double[numRows * numCols];
            for (var i = 0; i < numRows; i++)
                GetRow(i, A).CopyTo(B, i * numCols);
            return B;
        }

        /// <summary>
        /// Joins the matrix rows into vector.
        /// </summary>
        /// <param name="A">The matrix of integers, A.</param>
        /// <returns>System.Int32[].</returns>
        public static int[] JoinMatrixRowsIntoVector(this int[,] A)
        {
            var numRows = A.GetLength(0);
            var numCols = A.GetLength(1);
            var B = new int[numRows * numCols];
            for (var i = 0; i < numRows; i++)
                GetRow(i, A).CopyTo(B, i * numCols);
            return B;
        }

        #endregion

        #region Distinct

        /// <summary>
        /// Removes any duplicates in the List of vectors and returns just the distinct cases.
        /// The order is preserved with duplicates removed.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>List&lt;System.Double[]&gt;.</returns>
        public static List<double[]> DistinctVectors(this List<double[]> list)
        {
            var distinctList = new List<double[]>(list);
            var m = list.Count;
            var n = list[0].GetLength(0);
            var CarolNumbers = new double[n];
            var CarolSeed = StartingCarolSeed;
            for (var i = 0; i < n; i++)
            {
                var carolNumber = Math.Pow(2, CarolSeed) - 1;
                carolNumber *= carolNumber;
                carolNumber -= 2;
                CarolNumbers[i] = carolNumber;
                CarolSeed += 3;
            }
            var indices = makeLinearProgression(m);
            indices = indices.OrderBy(index => list[index].dotProduct(CarolNumbers)).ToArray();
            for (var i = m - 1; i > 0; i--)
            {
                if (IsPracticallySame(list[indices[i]], list[indices[i - 1]]))
                    distinctList[indices[i]] = null;
            }
            distinctList.RemoveAll(v => v == null);
            return distinctList;
        }

        /// <summary>
        /// Removes any duplicates in the List of vectors and returns just the distinct cases.
        /// The order is preserved with duplicates removed.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>List&lt;System.Int32[]&gt;.</returns>
        public static List<int[]> DistinctVectors(this List<int[]> list)
        {
            var distinctList = new List<int[]>(list);
            var m = list.Count;
            var n = list[0].GetLength(0);
            var CarolNumbers = new int[n];
            var CarolSeed = StartingCarolSeed;
            for (var i = 0; i < n; i++)
            {
                var carolNumber = (int)Math.Pow(2, CarolSeed) - 1;
                carolNumber *= carolNumber;
                carolNumber -= 2;
                CarolNumbers[i] = carolNumber;
                CarolSeed += 3;
            }
            var indices = makeLinearProgression(m);
            indices = indices.OrderBy(index => list[index].dotProduct(CarolNumbers)).ToArray();
            for (var i = m - 1; i > 0; i--)
            {
                if (list[indices[i]].subtract(list[indices[i - 1]]).Max() == 0)
                    distinctList[indices[i]] = null;
            }

            distinctList.RemoveAll(v => v == null);
            return distinctList;
        }

        #endregion
    }
}