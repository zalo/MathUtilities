// ***********************************************************************
// Assembly         : StarMath
// Author           : MICampbell
// Created          : 05-14-2015
//
// Last Modified By : MICampbell
// Last Modified On : 07-07-2015
// ***********************************************************************
// <copyright file="solve.cs" company="Design Engineering Lab -- MICampbell">
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
        /// Solves the specified A matrix.
        /// </summary>
        /// <param name="A">The A.</param>
        /// <param name="b">The b.</param>
        /// <param name="initialGuess">The initial guess.</param>
        /// <param name="IsASymmetric">Is matrix A symmetric.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">Matrix, A, must be square.
        /// or
        /// Matrix, A, must be have the same number of rows as the vector, b.</exception>
        public static double[] solve(double[,] A, IList<double> b, IList<double> initialGuess = null,
            Boolean IsASymmetric = false)
        {
            var length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("Matrix, A, must be square.");
            if (length != b.Count)
                throw new ArithmeticException("Matrix, A, must be have the same number of rows as the vector, b.");
            if (length == 3)
                return solveViaCramersRule3(A, b);
            if (length == 2)
                return solveViaCramersRule2(A, b);
            List<int>[] potentialDiagonals;
            if (isGaussSeidelAppropriate(A, b, out potentialDiagonals, ref initialGuess, length))
                return SolveIteratively(A, b, initialGuess, length, potentialDiagonals);

            return SolveAnalytically(A, b, IsASymmetric);
        }

        private static double[] solveViaCramersRule3(double[,] a, IList<double> b)
        {
            var denominator = determinant(a);

            return new[]
            {
                ((b[0] * a[1, 1] * a[2, 2])
                 + (a[0, 1] * a[1, 2] * b[2])
                 + (a[0, 2] * b[1] * a[2, 1])
                 - (b[0] * a[1, 2] * a[2, 1])
                 - (a[0, 1] * b[1] * a[2, 2])
                 - (a[0, 2] * a[1, 1] * b[2]))/ denominator,
                ( (a[0, 0] * b[1] * a[2, 2])
                  + (b[0] * a[1, 2] * a[2, 0])
                  + (a[0, 2] * a[1, 0] * b[2])
                  - (a[0, 0] * a[1, 2] * b[2])
                  - (b[0] * a[1, 0] * a[2, 2])
                  - (a[0, 2] * b[1] * a[2, 0]))/denominator,
                ( (a[0, 0] * a[1, 1] * b[2])
                  + (a[0, 1] * b[1] * a[2, 0])
                  + (b[0] * a[1, 0] * a[2, 1])
                  - (a[0, 0] * b[1] * a[2, 1])
                  - (a[0, 1] * a[1, 0] * b[2])
                  - (b[0] * a[1, 1] * a[2, 0]))/denominator
            };
        }

        private static double[] solveViaCramersRule2(double[,] a, IList<double> b)
        {
            var denominator = a[0, 0] * a[1, 1] - a[0, 1] * a[1, 0];
            return new[]
            {
                (b[0]*a[1,1]-b[1]*a[0,1])/denominator,
                (b[1]*a[0,0]-b[0]*a[1,0])/denominator
            };
        }


        /// <summary>
        /// Solves the specified A.
        /// </summary>
        /// <param name="A">The A.</param>
        /// <param name="b">The b.</param>
        /// <param name="initialGuess">The initial guess.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">Matrix, A, must be square.
        /// or
        /// Matrix, A, must be have the same number of rows as the vector, b.</exception>
        public static double[] solve(int[,] A, IList<double> b, IList<double> initialGuess = null)
        {
            var length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("Matrix, A, must be square.");
            if (length != b.Count)
                throw new ArithmeticException("Matrix, A, must be have the same number of rows as the vector, b.");

            var B = new double[length, length];
            for (var i = 0; i < length; i++)
                for (var j = 0; j < length; j++)
                    B[i, j] = A[i, j];
            return solve(B, b, initialGuess);
        }

        /// <summary>
        /// Solves the specified A.
        /// </summary>
        /// <param name="A">The A.</param>
        /// <param name="b">The b.</param>
        /// <param name="initialGuess">The initial guess.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">Matrix, A, must be square.
        /// or
        /// Matrix, A, must be have the same number of rows as the vector, b.</exception>
        public static double[] solve(double[,] A, IList<int> b, IList<double> initialGuess = null)
        {
            return solve(A, b.Select(Convert.ToDouble).ToArray(), initialGuess);
        }

        /// <summary>
        /// Solves the specified A.
        /// </summary>
        /// <param name="A">The A.</param>
        /// <param name="b">The b.</param>
        /// <param name="initialGuess">The initial guess.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">Matrix, A, must be square.
        /// or
        /// Matrix, A, must be have the same number of rows as the vector, b.</exception>
        public static double[] solve(int[,] A, IList<int> b, IList<double> initialGuess = null)
        {
            return solve(A, b.Select(Convert.ToDouble).ToArray(), initialGuess);
        }

        /// <summary>
        /// Solves the by inverse.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="b">The b.</param>
        /// <param name="IsASymmetric">Is A known to be Symmetric?</param>
        /// <param name="potentialDiagonals">The potential diagonals.</param>
        /// <returns>System.Double[].</returns>
        public static double[] SolveAnalytically(double[,] A, IList<double> b, bool IsASymmetric = false)
        {
            var length = b.Count;
            if (IsASymmetric)
            {
                var L = CholeskyDecomposition(A);
                var x = new double[length];
                // forward substitution
                for (int i = 0; i < length; i++)
                {
                    var sumFromKnownTerms = 0.0;
                    for (int j = 0; j < i; j++)
                        sumFromKnownTerms += L[i, j] * x[j];
                    x[i] = (b[i] - sumFromKnownTerms);
                }

                for (int i = 0; i < length; i++)
                    x[i] /= L[i, i];

                // backward substitution
                for (int i = length - 1; i >= 0; i--)
                {
                    var sumFromKnownTerms = 0.0;
                    for (int j = i + 1; j < length; j++)
                        sumFromKnownTerms += L[j, i] * x[j];
                    x[i] -= sumFromKnownTerms;
                }
                return x;
            }
            else
            {
                int[] permutationVector;
                var LU = LUDecomposition(A, out permutationVector, length);
                var x = new double[length];
                // forward substitution
                for (int i = 0; i < length; i++)
                {
                    var sumFromKnownTerms = 0.0;
                    for (int j = 0; j < i; j++)
                        sumFromKnownTerms += LU[permutationVector[i], j] * x[j];
                    x[i] = (b[permutationVector[i]] - sumFromKnownTerms) / LU[permutationVector[i], i];
                }
                // backward substitution
                for (int i = length - 1; i >= 0; i--)
                {
                    var sumFromKnownTerms = 0.0;
                    for (int j = i + 1; j < length; j++)
                        sumFromKnownTerms += LU[permutationVector[i], j] * x[j];
                    x[i] -= sumFromKnownTerms;
                }
                return x;
            }
        }

        #region Gauss-Seidel or Successive Over-Relaxation

        /// <summary>
        /// Determines whether [Gauss-Seidel is appropriate] [the specified a].
        /// </summary>
        /// <param name="A">the matrix, A</param>
        /// <param name="b">the right-hand-side values, b</param>
        /// <param name="potentialDiagonals">The potential rows.</param>
        /// <param name="initialGuess">The initial guess.</param>
        /// <param name="length">The length.</param>
        /// <returns><c>true</c> if [is gauss seidel appropriate] [the specified a]; otherwise, <c>false</c>.</returns>
        private static bool isGaussSeidelAppropriate(double[,] A, IList<double> b, out List<int>[] potentialDiagonals,
            ref IList<double> initialGuess, int length)
        {
            potentialDiagonals = null;
            if (length < GaussSeidelMinimumMatrixSize) return false;
            if (initialGuess == null)
                initialGuess = makeInitialGuess(A, b, length);
            var error = norm1(subtract(b, multiply(A, initialGuess, length, length), length)) / norm1(b);
            if (error > MaxErrorForUsingGaussSeidel) return false;
            return findPotentialDiagonals(A, out potentialDiagonals, length, GaussSeidelDiagonalDominanceRatio);
        }

        /// <summary>
        /// Finds the potential diagonals.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="potentialDiagonals">The potential diagonals.</param>
        /// <param name="length">The length.</param>
        /// <param name="minimalConsideration">The minimal consideration.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool findPotentialDiagonals(double[,] A, out List<int>[] potentialDiagonals, int length,
            double minimalConsideration)
        {
            potentialDiagonals = new List<int>[length];
            for (var i = 0; i < length; i++)
            {
                var rowNorm1 = A.GetRow(i).norm1();
                var potentialIndices = new List<int>();
                for (var j = 0; j < length; j++)
                    if (Math.Abs(A[i, j]) / (rowNorm1 - Math.Abs(A[i, j])) > minimalConsideration)
                        potentialIndices.Add(j);
                if (potentialIndices.Count == 0) return false;
                potentialDiagonals[i] = potentialIndices;
            }
            return potentialDiagonals.SelectMany(x => x).Distinct().Count() == length;
        }

        /// <summary>
        /// Solves the gauss seidel.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="b">The b.</param>
        /// <param name="initialGuess">The initial guess.</param>
        /// <param name="length">The length.</param>
        /// <param name="potentialDiagonals">The potential indices.</param>
        /// <returns>System.Double[].</returns>
        public static double[] SolveIteratively(double[,] A, IList<double> b,
            IList<double> initialGuess = null, int length = -1, List<int>[] potentialDiagonals = null)
        {
            if (length < 0) length = b.Count;
            //double[,] C;
            //double[] d;

            var x = new double[length];
            if (initialGuess == null)
                initialGuess = makeInitialGuess(A, b, length);
            for (var i = 0; i < length; i++)
                x[i] = initialGuess[i];

            if (potentialDiagonals == null &&
                !findPotentialDiagonals(A, out potentialDiagonals, length, GaussSeidelDiagonalDominanceRatio))
                return null;
            var order = Enumerable.Range(0, length).ToArray();
            if (!order.All(rowIndex => potentialDiagonals[rowIndex].Contains(rowIndex)))
                order = reorderMatrixForDiagonalDominance(A, length, potentialDiagonals);
            if (order == null) return null;
            var bNorm1 = norm1(b);
            var error = norm1(subtract(b, multiply(A, x, length, length), length)) / bNorm1;
            var success = error <= GaussSeidelMaxError;
            var xWentNaN = false;
            var iteration = length * length * GaussSeidelMaxIterationFactor;
            while (!xWentNaN && !success && iteration-- > 0)
            {
                for (var i = 0; i < length; i++)
                {
                    var adjust = b[order[i]];
                    for (var j = 0; j < length; j++)
                        if (i != j)
                            adjust -= A[order[i], j] * x[j];
                    x[i] = (1 - GaussSeidelRelaxationOmega) * x[i] +
                           GaussSeidelRelaxationOmega * adjust / A[order[i], i];
                }
                xWentNaN = x.Any(double.IsNaN);
                error = norm1(subtract(b, multiply(A, x, length, length), length)) / bNorm1;
                success = error <= GaussSeidelMaxError;
            }
            if (!success) return null;

            return x;
        }

        /// <summary>
        /// Ifs the initial guess is null.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="b">The b.</param>
        /// <param name="length">The length.</param>
        private static double[] makeInitialGuess(double[,] A, IList<double> b, int length)
        {
            var initialGuess = new double[length];
            var initGuessValue = SumAllElements(b) / SumAllElements(A);
            for (var i = 0; i < length; i++) initialGuess[i] = initGuessValue;
            return initialGuess;
        }

        /// <summary>
        /// Reorders the matrix for diagonal dominance and returns the permutation vector.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="length">The length.</param>
        /// <param name="potentialIndices">The potential indices.</param>
        /// <returns>System.Int32[].</returns>
        private static int[] reorderMatrixForDiagonalDominance(double[,] A, int length, List<int>[] potentialIndices)
        {
            var popularity = new int[length];
            for (var i = 0; i < length; i++)
                popularity[i] = potentialIndices.Count(r => r.Contains(i));
            var orderToAddress = makeLinearProgression(length, 1).OrderBy(x => popularity[x]).ToList();
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
                        possibleIndicesForRow = possibleIndicesForRow.OrderBy(r => Math.Abs(A[r, colIndex])).ToList();
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