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
    internal class CholeskyLCell : SparseCell
    {
        internal CholeskyLCell(int rowIndex, int colIndex, SparseCell aCell, SparseCell diagDenominatorCell) : base(rowIndex, colIndex)
        {
            ACell = aCell;
            DiagDenominatorCell = diagDenominatorCell;
        }
        private readonly SparseCell ACell;
        private readonly SparseCell DiagDenominatorCell;
        private int termCount;
        private readonly List<SparseCell> terms = new List<SparseCell>();

        internal void AddTripletTerm(SparseCell L_ik, SparseCell L_jk, SparseCell D_k)
        {
            terms.Add(L_ik);
            terms.Add(L_jk);
            terms.Add(D_k);
            termCount += 3;
        }

        internal override void Evaluate()
        {
            Value = ACell?.Value ?? 0.0;
            for (int i = 0; i < termCount; i+=3)
                Value -= terms[i].Value * terms[i + 1].Value * terms[i + 2].Value;
            Value /= DiagDenominatorCell.Value;
        }
    }
    internal class CholeskyDCell : SparseCell
    {
        internal CholeskyDCell(int rowIndex, int colIndex, SparseCell ACell) : base(rowIndex, colIndex)
        {
            this.ACell = ACell;
        }
        internal readonly SparseCell ACell;
        internal void AddDoubletTerm(SparseCell L_jk, SparseCell D_k)
        {
            terms.Add(L_jk);
            terms.Add(D_k);
            termCount += 2;
        }

        private int termCount;
        private readonly List<SparseCell> terms = new List<SparseCell>();

        internal override void Evaluate()
        {
            Value = ACell?.Value ?? 0.0;
            for (int i = 0; i < termCount; i+=2)
                Value -= terms[i].Value * terms[i].Value * terms[i + 1].Value;
        }
    }
    /// <summary>
    /// Class SparseCell.
    /// </summary>
    internal class SparseCell
    {
        internal virtual void Evaluate()
        { throw new NotSupportedException("Should only be called from one of the derived cell classes"); }
        /// <summary>
        /// The col index
        /// </summary>
        internal int ColIndex;

        /// <summary>
        /// Down
        /// </summary>
        internal SparseCell Down;

        /// <summary>
        /// The left
        /// </summary>
        internal SparseCell Left;

        /// <summary>
        /// The right
        /// </summary>
        internal SparseCell Right;

        /// <summary>
        /// The row index
        /// </summary>
        internal int RowIndex;

        /// <summary>
        /// Up
        /// </summary>
        internal SparseCell Up;

        /// <summary>
        /// The value
        /// </summary>
        internal double Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseCell" /> class.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <param name="value">The value.</param>
        internal SparseCell(int rowIndex, int colIndex, double value) : this(rowIndex, colIndex)
        {
            Value = value;
        }

        internal SparseCell(int rowIndex, int colIndex)
        {
            RowIndex = rowIndex;
            ColIndex = colIndex;
        }
    }
}