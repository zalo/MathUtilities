// ***********************************************************************
// Assembly         : StarMath
// Author           : Matthew I. Campbell
// Created          : 09-03-2015
//
// Last Modified By : Matt
// Last Modified On : 09-03-2015
// ***********************************************************************
// <copyright file="SparseMatrix.cs" company="Design Engineering Lab -- MICampbell">
//     2014
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace StarMathLib
{
    /// <summary>
    /// Class SparseMatrix.
    /// </summary>
    public partial class SparseMatrix
    {
        #region Fields and Properties

        public bool TopologyChanged { get; private set; } = true;
        public bool ValuesChanged { get; private set; } = true;
        private readonly List<SparseCell> cellsRowbyRow;

        /// <summary>
        /// The first non-zero cell in each row.
        /// </summary>
        internal SparseCell[] Diagonals { get; private set; }

        /// <summary>
        /// The first non-zero cell in each row.
        /// </summary>
        internal SparseCell[] RowFirsts { get; private set; }

        /// <summary>
        /// The last non-zero cell in each row.
        /// </summary>
        internal SparseCell[] RowLasts { get; private set; }

        /// <summary>
        /// The first non-zero cell in each column.
        /// </summary>
        internal SparseCell[] ColFirsts { get; private set; }

        /// <summary>
        /// The last non-zero cell in each column.
        /// </summary>
        internal SparseCell[] ColLasts { get; private set; }

        /// <summary>
        /// The number non zero
        /// </summary>
        public int NumNonZero { get; private set; }

        /// <summary>
        /// The number cols
        /// </summary>
        public int NumCols { get; private set; }

        /// <summary>
        /// The number rows
        /// </summary>
        public int NumRows { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix" /> class.
        /// </summary>
        /// <param name="rowIndices">The row indices.</param>
        /// <param name="colIndices">The col indices.</param>
        /// <param name="values">The values.</param>
        /// <param name="numRows">The number rows.</param>
        /// <param name="numCols">The number cols.</param>
        public SparseMatrix(IList<int> rowIndices, IList<int> colIndices, IList<double> values, int numRows, int numCols)
            : this(numRows, numCols)
        {
            var count = values.Count;
            for (int i = 0; i < count; i++)
                this[rowIndices[i], colIndices[i]] += values[i];
            NumNonZero = cellsRowbyRow.Count;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix"/> class.
        /// </summary>
        /// <param name="indices">The row by row indices.</param>
        /// <param name="values">The values.</param>
        /// <param name="numRows">The number rows.</param>
        /// <param name="numCols">The number cols.</param>
        /// <param name="InRowOrder">The in row order.</param>
        public SparseMatrix(IList<int> indices, IList<double> values, int numRows, int numCols,
            bool InRowOrder = true) : this(numRows, numCols)
        {
            if (InRowOrder)
            {
                #region Fill-in In Order

                /* this is an elaborate method to speed up the stitching together of new cells */
                var rowI = 0;
                var rowLowerLimit = 0;
                var rowUpperLimit = NumCols;
                var newRow = true;
                for (var i = 0; i < values.Count; i++)
                {
                    var index = indices[i];
                    var value = values[i];
                    while (i < values.Count - 1 && indices[i] == indices[i + 1])
                    {
                        i++;
                        value += values[i];
                    }
                    while (index >= rowUpperLimit)
                    {
                        newRow = true;
                        rowI++;
                        rowLowerLimit += NumCols;
                        rowUpperLimit += NumCols;
                    }
                    var colI = index - rowLowerLimit;
                    var cell = new SparseCell(rowI, colI, value);
                    if (rowI == colI) Diagonals[rowI] = cell;
                    cellsRowbyRow.Add(cell);
                    if (newRow)
                    {
                        RowFirsts[rowI] = cell;
                        RowLasts[rowI] = cell;
                        newRow = false;
                    }
                    else
                    {
                        cell.Left = RowLasts[rowI];
                        RowLasts[rowI].Right = cell;
                        RowLasts[rowI] = cell;
                    }
                    if (ColFirsts[colI] == null)
                    {
                        ColFirsts[colI] = cell;
                        ColLasts[colI] = cell;
                    }
                    else
                    {
                        cell.Up = ColLasts[colI];
                        ColLasts[colI].Down = cell;
                        ColLasts[colI] = cell;
                    }
                }

                #endregion
            }
            else
            {
                var count = values.Count;
                for (int i = 0; i < count; i++)
                {
                    var index = indices[i];
                    var rowI = index / NumCols;
                    var colI = index % NumCols;
                    this[rowI, colI] += values[i];
                }
            }
            NumNonZero = cellsRowbyRow.Count;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix"/> class.
        /// </summary>
        /// <param name="cellDictionary">The cell dictionary with keys as [i,j] pairs.</param>
        /// <param name="numRows">The number rows.</param>
        /// <param name="numCols">The number cols.</param>
        public SparseMatrix(Dictionary<int[], double> cellDictionary, int numRows, int numCols) : this(numRows, numCols)
        {
            foreach (var keyValuePair in cellDictionary)
                this[keyValuePair.Key[0], keyValuePair.Key[1]] += keyValuePair.Value;
            NumNonZero = cellsRowbyRow.Count;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix"/> class.
        /// </summary>
        /// <param name="numRows">The number rows.</param>
        /// <param name="numCols">The number cols.</param>
        public SparseMatrix(int numRows, int numCols)
        {
            cellsRowbyRow = new List<SparseCell>();
            NumRows = numRows;
            NumCols = numCols;
            Diagonals = new SparseCell[numRows];
            RowFirsts = new SparseCell[numRows];
            RowLasts = new SparseCell[numRows];
            ColFirsts = new SparseCell[numCols];
            ColLasts = new SparseCell[numCols];
        }

        /// <summary>
        /// Updates the values.
        /// </summary>
        /// <param name="rowIndices">The row indices.</param>
        /// <param name="colIndices">The col indices.</param>
        /// <param name="values">The values.</param>
        public void UpdateValues(IList<int> rowIndices, IList<int> colIndices, IList<double> values)
        {
            ValuesChanged = true;
            foreach (var sparseCell in cellsRowbyRow)
                sparseCell.Value = 0;
            var count = values.Count;
            for (int i = 0; i < count; i++)
                this[rowIndices[i], colIndices[i]] += values[i];
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix" /> class.
        /// </summary>
        /// <param name="rowByRowIndices">The row by row indices.</param>
        /// <param name="values">The values.</param>
        /// <param name="InRowOrder">The in row order.</param>
        public void UpdateValues(IList<int> rowByRowIndices, IList<double> values, bool InRowOrder)
        {
            ValuesChanged = true;
            foreach (var sparseCell in cellsRowbyRow)
                sparseCell.Value = 0;
            var count = values.Count;
            if (InRowOrder)
            {
                var i = 0;
                foreach (var sparseCell in cellsRowbyRow)
                {
                    var cellIndex = rowByRowIndices[i];
                    do
                    {
                        sparseCell.Value += values[i++];
                    } while (i < count && rowByRowIndices[i] == cellIndex);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    var index = rowByRowIndices[i];
                    var rowI = index / NumCols;
                    var colI = index % NumCols;
                    this[rowI, colI] += values[i];
                }
            }
        }


        #endregion

        /// <summary>
        /// Converts the sparse matrix to a dense matrix.
        /// </summary>
        /// <returns>System.Double[].</returns>
        public double[,] ConvertSparseToDenseMatrix()
        {
            var A = new double[NumRows, NumCols];
            for (int i = 0; i < NumRows; i++)
            {
                var cell = RowFirsts[i];
                while (cell != null)
                {
                    A[i, cell.ColIndex] = cell.Value;
                    cell = cell.Right;
                }
            }
            return A;
        }

        #region Finding Cell(s) Methods

        /// <summary>
        /// Gets or sets the <see cref="System.Double" /> with the specified row i.
        /// </summary>
        /// <param name="rowI">The row i.</param>
        /// <param name="colI">The col i.</param>
        /// <returns>System.Double.</returns>
        public double this[int rowI, int colI]
        {
            get
            {
                var c = CellAt(rowI, colI);
                if (c == null) return 0.0;
                else return c.Value;
            }
            set
            {
                var c = CellAt(rowI, colI);
                if (c == null) AddCell(rowI, colI, value);
                else c.Value = value;
                ValuesChanged = true;
            }
        }

        private SparseCell CellAt(int rowI, int colI)
        {
            if (rowI == colI) return Diagonals[rowI];
            if (rowI >= colI) return SearchRightToCell(colI, RowFirsts[rowI]);
            return SearchDownToCell(rowI, ColFirsts[colI]);
        }

        /// <summary>
        /// Searches the left to.
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <param name="startCell">The start cell.</param>
        /// <returns>SparseCell.</returns>
        private Boolean TrySearchRightToCell(int colIndex, ref SparseCell startCell)
        {
            do
            {
                if (startCell == null || startCell.ColIndex > colIndex)
                    return false;
                if (startCell.ColIndex == colIndex) return true;
                startCell = startCell.Right;
            } while (true);
        }

        /// <summary>
        /// Searches the left to.
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <param name="startCell">The start cell.</param>
        /// <returns>SparseCell.</returns>
        private SparseCell SearchRightToCell(int colIndex, SparseCell startCell)
        {
            do
            {
                if (startCell == null || startCell.ColIndex > colIndex)
                    return null;
                if (startCell.ColIndex == colIndex) return startCell;
                startCell = startCell.Right;
            } while (true);
        }

        /// <summary>
        /// Searches down to.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="startCell">The start cell.</param>
        /// <returns>SparseCell.</returns>
        /// <exception cref="ArithmeticException">No non-zero sparse matrix cell found at the location.</exception>
        private SparseCell SearchDownToCell(int rowIndex, SparseCell startCell)
        {
            do
            {
                if (startCell == null || startCell.RowIndex > rowIndex)
                    return null;
                if (startCell.RowIndex == rowIndex) return startCell;
                startCell = startCell.Down;
            } while (true);
        }

        #endregion

        public SparseMatrix Copy()
        {
            return new SparseMatrix(cellsRowbyRow.Select(x => x.RowIndex * NumCols + x.ColIndex).ToArray(),
                cellsRowbyRow.Select(c => c.Value).ToArray(), NumRows, NumCols);
        }

        private void RemoveCell(SparseCell cell)
        {
            if (cell.Left == null)
                RowFirsts[cell.RowIndex] = cell.Right;
            else cell.Left.Right = cell.Right;
            if (cell.Right == null)
                RowLasts[cell.RowIndex] = cell.Left;
            else cell.Right.Left = cell.Left;

            if (cell.Up == null)
                ColFirsts[cell.ColIndex] = cell.Down;
            else cell.Up.Down = cell.Down;
            if (cell.Down == null)
                ColLasts[cell.ColIndex] = cell.Up;
            else cell.Down.Up = cell.Up;
            cellsRowbyRow.Remove(cell);
            NumNonZero--;
            TopologyChanged = true;
        }

        private SparseCell AddCell(int rowI, int colI, double value = Double.NaN)
        {
            var cell = new SparseCell(rowI, colI, value);
            // stitch it into the rows
            if (RowFirsts[rowI] == null && RowLasts[rowI] == null)
                RowFirsts[rowI] = RowLasts[rowI] = cell;
            else if (RowFirsts[rowI] == null || RowFirsts[rowI].ColIndex > colI)
            {
                cell.Right = RowFirsts[rowI];
                RowFirsts[rowI].Left = cell;
                RowFirsts[rowI] = cell;
            }
            else if (RowLasts[rowI].ColIndex < colI)
            {
                cell.Left = RowLasts[rowI];
                RowLasts[rowI].Right = cell;
                RowLasts[rowI] = cell;
            }
            else
            {
                var startCell = RowFirsts[rowI];
                while (startCell.ColIndex < colI)
                    startCell = startCell.Right;
                cell.Right = startCell;
                cell.Left = startCell.Left;
                cell.Left.Right = cell;
                startCell.Left = cell;
            }
            // stitch it into the colums
            if (ColFirsts[colI] == null && ColLasts[colI] == null)
                ColFirsts[colI] = ColLasts[colI] = cell;
            else if (ColFirsts[colI].RowIndex > rowI)
            {
                cell.Down = ColFirsts[colI];
                ColFirsts[colI].Up = cell;
                ColFirsts[colI] = cell;
            }
            else if (ColLasts[colI].RowIndex < rowI)
            {
                cell.Up = ColLasts[colI];
                ColLasts[colI].Down = cell;
                ColLasts[colI] = cell;
            }
            else
            {
                var startCell = ColFirsts[colI];
                while (startCell.RowIndex < rowI)
                    startCell = startCell.Down;
                cell.Down = startCell;
                cell.Up = startCell.Up;
                cell.Up.Down = cell;
                startCell.Up = cell;
            }
            if (rowI == colI) Diagonals[rowI] = cell;
            cellsRowbyRow.Add(cell);
            NumNonZero++;
            TopologyChanged = true;

            return cell;
        }

        /// <summary>
        /// Finds the index of the insertion within the cellsRowbyRow. Of course, there are built-in functions to do this,
        /// but the idea with writing our own is that we can make a faster search given information that is known about
        /// this .
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns>System.Int32.</returns>
        //private int FindInsertionIndex(SparseCell cell)
        //{
        //int i= cellsRowbyRow.IndexOf(cell);
        // if (i >= 0) return i;
        //var averageCellPerRow = NumNonZero / NumRows;
        //var index = Math.Min(averageCellPerRow * cell.RowIndex + cell.ColIndex, NumNonZero - 1);
        //int step = averageCellPerRow;
        //do
        //{
        //    if (cell.RowIndex < cellsRowbyRow[index].RowIndex
        //        || (cell.RowIndex == cellsRowbyRow[index].RowIndex
        //        && cell.ColIndex < cellsRowbyRow[index].ColIndex))
        //    {
        //        if (index == 0 || step == 1) step = 0;
        //        else if (step > 0) step = -step / 2;
        //    }
        //    else if (cell.RowIndex > cellsRowbyRow[index].RowIndex
        //        || (cell.RowIndex == cellsRowbyRow[index].RowIndex
        //        && cell.ColIndex > cellsRowbyRow[index].ColIndex))
        //    {
        //        if (index == NumNonZero - 1 || step == -1) step = 0;
        //        else if (step < 0) step = -step / 2;
        //    }
        //    else step = 0;
        //    index += step;
        //    if (index < 0)
        //    {
        //        step -= index;
        //        index = 0;
        //    }
        //    else if (index >= NumNonZero)
        //    {
        //        step = index - (NumNonZero - 1);
        //        index = NumNonZero - 1;
        //    }
        //} while (step != 0);
        //return index;
        //}


        /// <summary>
        /// Removes the row.
        /// </summary>
        /// <param name="rowIndexToRemove">The row index to remove.</param>
        public void RemoveRow(int rowIndexToRemove)
        {
            TopologyChanged = true;
            var thisCell = RowFirsts[rowIndexToRemove];
            while (thisCell != null)
            {
                var nextCell = thisCell.Right;
                RemoveCell(thisCell);
                if (thisCell.ColIndex == rowIndexToRemove) Diagonals[rowIndexToRemove] = null;
                thisCell = nextCell;
            }

            NumRows--;
            var newRowFirsts = new SparseCell[NumRows];
            var newRowLasts = new SparseCell[NumRows];

            for (int i = 0; i < rowIndexToRemove; i++)
            {
                newRowFirsts[i] = RowFirsts[i];
                newRowLasts[i] = RowLasts[i];
            }
            for (int i = rowIndexToRemove; i < NumRows; i++)
            {
                newRowFirsts[i] = RowFirsts[i + 1];
                newRowLasts[i] = RowLasts[i + 1];
                var cell = RowFirsts[i + 1];
                while (cell != null)
                {
                    cell.RowIndex = i;
                    if (cell.ColIndex == i) Diagonals[i] = cell;
                    cell = cell.Right;
                }
            }
            RowFirsts = newRowFirsts;
            RowLasts = newRowLasts;
        }

        /// <summary>
        /// Removes the column.
        /// </summary>
        /// <param name="colIndexToRemove">The col index to remove.</param>
        public void RemoveColumn(int colIndexToRemove)
        {
            TopologyChanged = true;
            var thisCell = ColFirsts[colIndexToRemove];
            while (thisCell != null)
            {
                var nextCell = thisCell.Down;
                RemoveCell(thisCell);
                if (thisCell.RowIndex == colIndexToRemove) Diagonals[colIndexToRemove] = null;
                thisCell = nextCell;
            }

            NumCols--;
            var newColFirsts = new SparseCell[NumCols];
            var newColLasts = new SparseCell[NumCols];

            for (int i = 0; i < colIndexToRemove; i++)
            {
                newColFirsts[i] = ColFirsts[i];
                newColLasts[i] = ColLasts[i];
            }
            for (int i = colIndexToRemove; i < NumCols; i++)
            {
                newColFirsts[i] = ColFirsts[i + 1];
                newColLasts[i] = ColLasts[i + 1];
                var cell = ColFirsts[i + 1];
                while (cell != null)
                {
                    cell.ColIndex = i;
                    if (cell.RowIndex == i) Diagonals[i] = cell;
                    cell = cell.Down;
                }
            }
            ColFirsts = newColFirsts;
            ColLasts = newColLasts;
        }

        /// <summary>
        /// Removes the rows.
        /// </summary>
        /// <param name="rowIndicesToRemove">The row indices to remove.</param>
        public void RemoveRows(IList<int> rowIndicesToRemove)
        {
            TopologyChanged = true;
            var numToRemove = rowIndicesToRemove.Count;
            var removeIndices = rowIndicesToRemove.OrderBy(i => i).ToArray();
            for (int i = 0; i < numToRemove; i++)
            {
                var cell = RowFirsts[removeIndices[i]];
                while (cell != null)
                {
                    var nextCell = cell.Right;
                    RemoveCell(cell);
                    if (cell.ColIndex == cell.RowIndex) Diagonals[cell.RowIndex] = null;
                    cell = nextCell;
                }
            }
            NumRows -= numToRemove;
            var newRowFirsts = new SparseCell[NumRows];
            var newRowLasts = new SparseCell[NumRows];
            var offset = 0;
            for (int i = 0; i < NumRows; i++)
            {
                while (offset < numToRemove && (i + offset) == removeIndices[offset])
                    offset++;
                newRowFirsts[i] = RowFirsts[i + offset];
                newRowLasts[i] = RowLasts[i + offset];
                var cell = RowFirsts[i + offset];
                while (cell != null)
                {
                    cell.RowIndex = i;
                    if (cell.ColIndex == i) Diagonals[i] = cell;
                    cell = cell.Right;
                }
            }
            RowFirsts = newRowFirsts;
            RowLasts = newRowLasts;
        }

        /// <summary>
        /// Removes the columns.
        /// </summary>
        /// <param name="colIndicesToRemove">The col indices to remove.</param>
        public void RemoveColumns(IList<int> colIndicesToRemove)
        {
            TopologyChanged = true;
            var numToRemove = colIndicesToRemove.Count;
            var removeIndices = colIndicesToRemove.OrderBy(i => i).ToArray();
            for (int i = 0; i < numToRemove; i++)
            {
                var cell = ColFirsts[removeIndices[i]];
                while (cell != null)
                {
                    var nextCell = cell.Down;
                    RemoveCell(cell);
                    if (cell.ColIndex == cell.RowIndex) Diagonals[cell.RowIndex] = null;
                    cell = nextCell;
                }
            }
            NumCols -= numToRemove;
            var newColFirsts = new SparseCell[NumCols];
            var newColLasts = new SparseCell[NumCols];
            var offset = 0;
            for (int i = 0; i < NumCols; i++)
            {
                while (offset < numToRemove && (i + offset) == removeIndices[offset])
                    offset++;
                newColFirsts[i] = ColFirsts[i + offset];
                newColLasts[i] = ColLasts[i + offset];
                var cell = ColFirsts[i + offset];
                while (cell != null)
                {
                    cell.ColIndex = i;
                    if (cell.RowIndex == i) Diagonals[i] = cell;
                    cell = cell.Down;
                }
            }
            ColFirsts = newColFirsts;
            ColLasts = newColLasts;
        }

        public void Transpose()
        {
            TopologyChanged = true;
            var tempArray = RowFirsts;
            RowFirsts = ColFirsts;
            ColFirsts = tempArray;
            tempArray = RowLasts;
            RowLasts = ColLasts;
            ColLasts = tempArray;
            foreach (var sparseCell in cellsRowbyRow)
            {
                var tempCell = sparseCell.Right;
                sparseCell.Right = sparseCell.Down;
                sparseCell.Down = tempCell;
                tempCell = sparseCell.Left;
                sparseCell.Left = sparseCell.Up;
                sparseCell.Up = tempCell;
                var tempIndex = sparseCell.RowIndex;
                sparseCell.RowIndex = sparseCell.ColIndex;
                sparseCell.ColIndex = tempIndex;
            }
            var tempLimit = NumRows;
            NumRows = NumCols;
            NumCols = tempLimit;
            //cellsRowbyRow.Clear();
            //for (int i = 0; i < NumRows; i++)
            //{
            //    var cell = RowFirsts[i];
            //    while (cell != null)
            //    {
            //        cellsRowbyRow.Add(cell);
            //        cell = cell.Right;
            //    }
            //}
        }

    }
}