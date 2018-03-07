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
using StarMathLib.Sparse_Matrix;

//using StarMathLib.CSparse;

//using StarMathLib.CSparseClasses;

namespace StarMathLib
{

    /// <summary>
    /// Class SparseMatrix.
    /// </summary>
    public partial class SparseMatrix
    {
        private SymbolicFactorization symbolicFactorizationMat;
        double[] D;
        CompressedColumnStorage FactorizationMatrix;
        CompressedColumnStorage MatrixInCCS;



        /// <summary>
        /// Solves the system of equations where this Sparse Matrix is 'A' in Ax = b.
        /// The resulting x is returned.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="initialGuess">The initial guess.</param>
        /// <param name="IsASymmetric">The is a symmetric.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">Spare Matrix must be square to solve Ax = b.
        /// or
        /// Sparse Matrix must be have the same number of rows as the vector, b.</exception>
        /// <exception cref="ArithmeticException">Spare Matrix must be square to solve Ax = b.
        /// or
        /// Sparse Matrix must be have the same number of rows as the vector, b.</exception>
        public IList<double> solve(IList<double> b, IList<double> initialGuess = null,
            bool IsASymmetric = false)
        {
            if (NumRows != NumCols)
                throw new ArithmeticException("Spare Matrix must be square to solve Ax = b.");
            if (NumRows != b.Count)
                throw new ArithmeticException("Sparse Matrix must be have the same number of rows as the vector, b.");
            List<int>[] potentialDiagonals;
            if (isGaussSeidelAppropriate(b, out potentialDiagonals, ref initialGuess))
                return SolveIteratively(b, initialGuess, potentialDiagonals);
            /****** need code to determine when to switch between *****
             ****** this analytical approach and the SOR approach *****/
            return SolveAnalytically(b, IsASymmetric);
        }

        public double[] solve(IList<double> bValues, IList<int> bIndices, IList<double> initialGuess = null,
    bool IsASymmetric = false)
        {
            if (NumRows != NumCols)
                throw new ArithmeticException("Spare Matrix must be square to solve Ax = b.");
            List<int>[] potentialDiagonals;
            if (isGaussSeidelAppropriate(bValues, bIndices, out potentialDiagonals, ref initialGuess))
                return SolveIteratively(bValues, bIndices, initialGuess, potentialDiagonals);
            /****** need code to determine when to switch between *****
             ****** this analytical approach and the SOR approach *****/
            return SolveAnalytically(bValues, bIndices, IsASymmetric);
        }

        private double[] SolveIteratively(IList<double> bValues, IList<int> bIndices, IList<double> initialGuess, List<int>[] potentialDiagonals)
        {
            throw new NotImplementedException();
        }

        private bool isGaussSeidelAppropriate(IList<double> bValues, IList<int> bIndices, out List<int>[] potentialDiagonals, ref IList<double> initialGuess)
        {
            throw new NotImplementedException();
        }

        private double[] SolveAnalytically(IList<double> bValues, IList<int> bIndices, bool isASymmetric)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Solves the system of equations analytically.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="IsASymmetric">if set to <c>true</c> [a is symmetric].</param>
        /// <param name="potentialDiagonals">The potential diagonals.</param>
        /// <returns>System.Double[].</returns>
        public IList<double> SolveAnalytically(IList<double> b, bool IsASymmetric = false)
        {
           if (IsASymmetric)
            {
                if (ValuesChanged || TopologyChanged)
                    MatrixInCCS = convertToCCS(this);
                if (TopologyChanged)
                    symbolicFactorizationMat = CSparse.SymbolicAnalysisLDL(MatrixInCCS);
                if (ValuesChanged || TopologyChanged)
                    CSparse.FactorizeLDL(MatrixInCCS, symbolicFactorizationMat, out D, out FactorizationMatrix);
                TopologyChanged = ValuesChanged = false;
                return CSparse.SolveLDL(b, FactorizationMatrix, D, symbolicFactorizationMat.InversePermute);
            }
            else
            {
                var ccs = convertToCCS(this);
                var columnPermutation = ApproximateMinimumDegree.Generate(
                    new SymbolicColumnStorage(ccs), NumCols);
                CompressedColumnStorage L, U;
                int[] pinv;
                // Numeric LU factorization
                CSparse.FactorizeLU(ccs, columnPermutation, out L, out U, out pinv);
                var x = CSparse.ApplyInverse(pinv, b, NumCols); // x = b(p)
                CSparse.SolveLower(L, x); // x = L\x.
                CSparse.SolveUpper(U, x); // x = U\x.
                return CSparse.ApplyInverse(columnPermutation, x, NumCols); // b(q) = x
            }
        }

        private static CompressedColumnStorage convertToCCS(SparseMatrix sparseMatrix)
        {
            var cellCounter = 0;
            var colCounter = 0;
            var ccs = new CompressedColumnStorage(sparseMatrix.NumRows, sparseMatrix.NumCols, sparseMatrix.NumNonZero);
            var columnPointers = new int[sparseMatrix.NumCols + 1];
            columnPointers[0] = cellCounter;
            var rowIndices = new int[sparseMatrix.NumNonZero];
            var values = new double[sparseMatrix.NumNonZero];

            foreach (var topcell in sparseMatrix.ColFirsts)
            {
                var cell = topcell;
                while (cell != null)
                {
                    values[cellCounter] = cell.Value;
                    rowIndices[cellCounter] = cell.RowIndex;
                    cell = cell.Down;
                    cellCounter++;
                }
                columnPointers[++colCounter] = cellCounter;
            }
            ccs.ColumnPointers = columnPointers;
            ccs.RowIndices = rowIndices;
            ccs.Values = values;
            return ccs;
        }

        #region Gauss-Seidel or Successive Over-Relaxation

        private bool isGaussSeidelAppropriate(IList<double> b, out List<int>[] potentialDiagonals,
            ref IList<double> initialGuess)
        {
            potentialDiagonals = null;
            if (NumRows < StarMath.GaussSeidelMinimumMatrixSize) return false;
            if (initialGuess == null)
                initialGuess = makeInitialGuess(b);
            var error = StarMath.norm1(StarMath.subtract(b, multiply(initialGuess))) / b.norm1();
            if (error > StarMath.MaxErrorForUsingGaussSeidel) return false;
            return findPotentialDiagonals(out potentialDiagonals, StarMath.GaussSeidelDiagonalDominanceRatio);
        }

        private bool findPotentialDiagonals(out List<int>[] potentialDiagonals,
            double minimalConsideration)
        {
            potentialDiagonals = new List<int>[NumRows];
            for (var i = 0; i < NumRows; i++)
            {
                var cell = RowFirsts[i];
                var norm1 = 0.0;
                do
                {
                    norm1 += Math.Abs(cell.Value);
                    cell = cell.Right;
                } while (cell != null);
                var potentialIndices = new List<int>();
                cell = RowFirsts[i];
                do
                {
                    if (Math.Abs(cell.Value) / (norm1 - cell.Value) > minimalConsideration)
                        potentialIndices.Add(cell.ColIndex);
                    cell = cell.Right;
                } while (cell != null);
                if (potentialIndices.Count == 0) return false;
                potentialDiagonals[i] = potentialIndices;
            }
            return potentialDiagonals.SelectMany(x => x).Distinct().Count() == NumRows;
        }

        public double[] SolveIteratively(IList<double> b,
            IList<double> initialGuess = null, List<int>[] potentialDiagonals = null)
        {
            double[] x;
            if (initialGuess == null)
                x = makeInitialGuess(b);
            else x = initialGuess.ToArray();

            if (potentialDiagonals == null &&
                !findPotentialDiagonals(out potentialDiagonals, StarMath.GaussSeidelDiagonalDominanceRatio))
                return null;
            var order = Enumerable.Range(0, NumRows).ToArray();
            if (!order.All(rowIndex => potentialDiagonals[rowIndex].Contains(rowIndex)))
                order = reorderMatrixForDiagonalDominance(NumRows, potentialDiagonals);
            if (order == null) return null;
            var bNorm1 = StarMath.norm1(b);
            var error = StarMath.norm1(StarMath.subtract(b, multiply(x))) / bNorm1;
            var success = error <= StarMath.GaussSeidelMaxError;
            var xWentNaN = false;
            var iteration = NumRows * StarMath.GaussSeidelMaxIterationFactor;
            while (!xWentNaN && !success && iteration-- > 0)
            {
                for (var i = 0; i < NumRows; i++)
                {
                    var rowI = order[i];
                    var diagCell = Diagonals[i];
                    var adjust = b[rowI];
                    var cell = RowFirsts[rowI];
                    do
                    {
                        if (cell != diagCell)
                            adjust -= cell.Value * x[cell.ColIndex];
                        cell = cell.Right;
                    } while (cell != null);
                    x[rowI] = (1 - StarMath.GaussSeidelRelaxationOmega) * x[rowI] +
                           StarMath.GaussSeidelRelaxationOmega * adjust / this[rowI, i];
                }
                xWentNaN = x.Any(double.IsNaN);
                error = StarMath.norm1(StarMath.subtract(b, multiply(x))) / bNorm1;
                success = error <= StarMath.GaussSeidelMaxError;
            }
            if (!success) return null;

            return x;
        }

        private double[] makeInitialGuess(IList<double> b)
        {
            var initialGuess = new double[NumRows];
            var initGuessValue = StarMath.SumAllElements(b) / SumAllElements();
            for (var i = 0; i < NumRows; i++) initialGuess[i] = initGuessValue;
            return initialGuess;
        }
        private int[] reorderMatrixForDiagonalDominance(int length, List<int>[] potentialIndices)
        {
            var popularity = new int[length];
            for (var i = 0; i < length; i++)
                popularity[i] = potentialIndices.Count(r => r.Contains(i));
            var orderToAddress = StarMath.makeLinearProgression(length, 1).OrderBy(x => popularity[x]).ToList();
            var stack = new Stack<int[]>();
            var seed = new int[length];
            int[] candidate;
            var solutionFound = false;
            for (var i = 0; i < length; i++) seed[i] = -1;
            stack.Push(seed);
            do
            {
                candidate = stack.Pop();
                var numToFill = candidate.Count(x => x == -1);
                if (numToFill == 0) solutionFound = true;
                else
                {
                    var colIndex = orderToAddress[length - numToFill];
                    var possibleIndicesForRow = new List<int>();
                    for (var oldRowIndex = 0; oldRowIndex < length; oldRowIndex++)
                    {
                        if (!potentialIndices[oldRowIndex].Contains(colIndex)) continue;
                        if (candidate.Contains(oldRowIndex)) continue;
                        possibleIndicesForRow.Add(oldRowIndex);
                    }
                    if (possibleIndicesForRow.Count == 1)
                    {
                        candidate[colIndex] = possibleIndicesForRow[0];
                        stack.Push(candidate);
                    }
                    else
                    {
                        possibleIndicesForRow = possibleIndicesForRow.OrderBy(r => Math.Abs(this[r, colIndex])).ToList();
                        foreach (var i in possibleIndicesForRow)
                        {
                            var child = (int[])candidate.Clone();
                            child[colIndex] = i;
                            stack.Push(child);
                        }
                    }
                }
            } while (!solutionFound && stack.Any());
            if (solutionFound) return candidate;
            return null;
        }

        #endregion
    }
}

