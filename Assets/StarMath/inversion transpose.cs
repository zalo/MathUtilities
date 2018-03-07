// ***********************************************************************
// Assembly         : StarMath
// Author           : MICampbell
// Created          : 05-14-2015
//
// Last Modified By : MICampbell
// Last Modified On : 07-07-2015
// ***********************************************************************
// <copyright file="inversion transpose.cs" company="Design Engineering Lab -- MICampbell">
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
        #region Matrix Inversion
        /// <summary>
        /// Inverses the matrix A only if the diagonal is all non-zero.
        /// A[i,i] != 0.0
        /// </summary>
        /// <param name="A">The matrix to invert. This matrix is unchanged by this function.</param>
        /// <returns>The inverted matrix, A^-1.</returns>
        /// <exception cref="System.ArithmeticException">Matrix cannnot be inverted. Can only invert sqare matrices.</exception>
        public static double[,] inverse(this double[,] A) // need bool IsSymmetric to switch to Cholesky
        {
            var length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("Matrix cannnot be inverted. Can only invert square matrices.");
            if (length == 1) return new[,] { { 1 / A[0, 0] } };

            int[] permutationVector;
            var LU = LUDecomposition(A, out permutationVector, length);
            return inverseWithLUResult(LU, permutationVector, length);
        }

        /// <summary>
        /// Inverses the matrix A only if the diagonal is all non-zero.
        /// A[i,i] != 0.0
        /// </summary>
        /// <param name="A">The matrix to invert. This matrix is unchanged by this function.</param>
        /// <returns>The inverted matrix, A^-1.</returns>
        /// <exception cref="ArithmeticException">Matrix cannnot be inverted. Can only invert sqare matrices.</exception>
        public static double[,] inverse(this int[,] A)
        {
            var length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("Matrix cannnot be inverted. Can only invert sqare matrices.");
            if (length == 1) return new[,] { { 1 / (double)A[0, 0] } };
            int[] permute;
            var LU = LUDecomposition(A, out permute, length);
            return inverseWithLUResult(LU, permute, length);
        }

        /// <summary>
        /// Inverses the with lu result.
        /// </summary>
        /// <param name="B">The b.</param>
        /// <param name="permute">The permute.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.Double[].</returns>
        private static double[,] inverseWithLUResult(double[,] B, int[] permute, int length)
        {
            // if the matrix is not square or is less than B 2x2, 
            // then this function won't work
            //todo: need to fix to take care of permute
            #region invert L

            for (var i = 0; i < length; i++)
            {
                B[permute[i], i] = 1.0 / B[permute[i], i];
                for (var j = i + 1; j < length; j++)
                {
                    var sum = 0.0;
                    for (var k = i; k < j; k++)
                        sum -= B[permute[j], k] * B[permute[k], i];
                    B[permute[j], i] = (sum == 0) ? 0.0 : sum / B[permute[j], j];
                }
            }

            #endregion

            #region invert U

            for (var i = 0; i < length; i++)
                for (var j = i + 1; j < length; j++)
                {
                    var sum = -B[permute[i], j];
                    for (var k = i + 1; k < j; k++)
                        sum -= B[permute[k], j] * B[permute[i], k];
                    B[permute[i], j] = sum;
                }

            #endregion

            #region final inversion

            for (var i = 0; i < length; i++)
                for (var j = 0; j < length; j++)
                {
                    if (j == i)
                    {
                        var sum = B[permute[i], i];
                        for (var k = i + 1; k < length; k++)
                            sum += B[permute[i], k] * B[permute[k], i];
                        B[permute[i], i] = sum;
                    }
                    else if (j < i)
                    {
                        var sum = 0.0;
                        for (var k = i; k < length; k++)
                            sum += B[permute[j], k] * B[permute[k], i];
                        B[permute[j], i] = sum;
                    }
                    else // then i<j
                    {
                        var sum = B[permute[j], i];
                        for (var k = j + 1; k < length; k++)
                            sum += B[permute[j], k] * B[permute[k], i];
                        B[permute[j], i] = sum;
                    }
                }

            #endregion

            var C = new double[length, length];
            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                    C[i, j] = B[i, permute[j]];
            return C;
        }

        #endregion

        #region LU Decomposition

        /// <summary>
        /// Returns the LU decomposition of A in a new matrix.
        /// </summary>
        /// <param name="A">The matrix to invert. This matrix is unchanged by this function.</param>
        /// <param name="L">The L matrix is output where the diagonal elements are included and not (necessarily) equal to one.</param>
        /// <param name="U">The U matrix is output where the diagonal elements are all equal to one.</param>
        /// <param name="permute">The permute.</param>
        /// <exception cref="System.ArithmeticException">Matrix cannot be inverted. Can only invert sqyare matrices.</exception>
        /// <exception cref="ArithmeticException">LU Decomposition can only be determined for square matrices.</exception>
        public static void LUDecomposition(double[,] A, out double[,] L, out double[,] U, out int[] permute)
        {
            var length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("LU Decomposition can only be determined for square matrices.");

            L = LUDecomposition(A, out permute, length);
            U = new double[length, length];
            for (var i = 0; i < length; i++)
            {
                U[permute[i], i] = 1.0;
                for (var j = i + 1; j < length; j++)
                {
                    U[permute[i], j] = L[permute[i], j];
                    L[permute[i], j] = 0.0;
                }
            }
        }

        /// <summary>
        /// Returns the LU decomposition of A in a new matrix.
        /// </summary>
        /// <param name="A">The matrix to invert. This matrix is unchanged by this function.</param>
        /// <param name="permutationVector">The resulting permutation vector - how the rows are re-ordered to
        /// create L and U.</param>
        /// <param name="length">The length/order/number of rows of matrix, A.</param>
        /// <param name="robustReorder">if set to <c>true</c> [robust reorder]. But this is an internal recursive call
        /// and should not be set outside.</param>
        /// <param name="lastZeroIndices">The last zero indices - is calculated in this function, but if it is already
        /// known, then...by all means.</param>
        /// <returns>A matrix of equal size to A that combines the L and U. Here the diagonals belongs to L and the U's diagonal
        /// elements are all 1.</returns>
        /// <exception cref="ArithmeticException">LU Decomposition can only be determined for square matrices.</exception>
        private static double[,] LUDecomposition(double[,] A, out int[] permutationVector, int length = -1,
            bool robustReorder = false, List<int>[] lastZeroIndices = null)
        {
            // This is an implementation of Crout’s LU Decomposition Algorithm
            if (length == -1) length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("LU Decomposition can only be determined for square matrices.");
            // this lastZeroIndices is a an array of the last column index in each row that contains a zero (or is
            // negligible. It is used to determine what other row to swap with, if the current row has a zero diagonal.
            if (lastZeroIndices == null)
            {
                lastZeroIndices = new List<int>[length];
                for (int i = 0; i < length; i++)
                {
                    lastZeroIndices[i] = new List<int>();
                    for (int j = 0; j < length; j++)
                        if (A[i, j].IsNegligible()) lastZeroIndices[i].Add(j);
                }
            }
            var B = (double[,])A.Clone();
            // start with the permutation vector as a simple count - this is equivalent to an identity permutation matrix
            permutationVector = Enumerable.Range(0, length).ToArray();
            // normalize row 0
            for (var i = 0; i < length; i++)
            {
                // call this function to see if a row swap is necessary. If robustReorder, then it is likely
                // that a different row will be chosen even if this one is good. 
                if (!findAndPivotRows(B, permutationVector, lastZeroIndices, i, length, robustReorder))
                    // the reorder function only returns false, when robustReorder is false, and the simpler/quicker
                    // approach did not work. So, the whole process is restarted with robustReorder set to true.
                    // this will only recurse once (essentially just reducing duplicate code with this recursion.
                    return LUDecomposition(A, out permutationVector, length, true, lastZeroIndices);

                // continue with the main body of Crout's LU decomposition approach
                var pI = permutationVector[i];
                for (var j = i; j < length; j++)
                {
                    var pJ = permutationVector[j];
                    // do a column of L
                    var sum = 0.0;
                    for (var k = 0; k < i; k++)
                        sum -= B[pJ, k] * B[permutationVector[k], i];
                    B[pJ, i] += sum;
                }
                for (var j = i + 1; j < length; j++)
                {
                    // do a row of U
                    var sum = 0.0;
                    for (var k = 0; k < i; k++)
                        sum += B[pI, k] * B[permutationVector[k], j];
                    B[pI, j] = (-sum + B[pI, j]) / B[pI, i];
                }
            }
            return B;
        }

        private static bool findAndPivotRows(double[,] B, int[] permutationVector, List<int>[] lastZeroIndices, int i,
            int length, bool robustReorder = false)
        {
            // if robustReorder is false, then this whole reorder process may be skipped if the diagonal is nonzero.
            if (!robustReorder && !B[permutationVector[i], i].IsNegligible()) return true;

            // the following 13 lines chose the subsequent row that has a nonzero candidate for this row's 
            // diagonal and has the most zeroes that are farthest along in the row. This metric is essentially,
            // (num of remaining zeroes in row)*(the average position of zeroes in this row). Multiplying these
            // two results in simply summing the positions of the remaining zeroes in the row. So, for a pair of rows:
            // [0 3 4 0 0 0 1 7]
            // [6 4 3 9 0 0 4 0] 
            // for the third position (i = 2), the first row would receive a score of 12 (3 + 4 + 5) while the second
            // row would get a score of 16 (4 + 5 + 7). This would mean use the second row first!
            // Is this a wacky idea? I'm not sure. It is all my own, but likely explored somewhere in the literature. 
            // Such a heuristic is not uncommon. In lieu of search the n! ways to recombine the rows, this is a shortcut
            // that seems to lead to some robustness. It is intended to be a quick way to avoid further problems in later row swaps.
            var newI = -1;
            var indexOfLargestSum = -1;
            for (int j = i + 1; j < length; j++)
            {
                if (!B[permutationVector[j], i].IsNegligible())
                {
                    var sumOfColumnsWhereZeroesExist = lastZeroIndices[permutationVector[j]].Sum(x => x > i ? x : 0);
                    if (sumOfColumnsWhereZeroesExist >= indexOfLargestSum)
                    {
                        indexOfLargestSum = sumOfColumnsWhereZeroesExist;
                        newI = j;
                    }
                }
            }
            if (newI == -1)
            {   // if there was no change to newI, then we have failed for the non robust case. Return false, and let the 
                // main LU decomp function start the robust approach
                if (!robustReorder) return false;
                // getting no change in newI for robustReorder is not necessarily a problem (it will happen in every
                // last row) if the diagonal is nonnegligible. If it is - then we got a problem...
                if (B[permutationVector[i], i].IsNegligible())
                    throw new ArithmeticException(
                        "A appears to be a singular matrix. The LU Decomposition is not possible to complete.");
                return true;
            }
            var temp = permutationVector[i];
            permutationVector[i] = permutationVector[newI];
            permutationVector[newI] = temp;
            return true;
        }

        /// <summary>
        /// Returns the LU decomposition of A in a new matrix.
        /// </summary>
        /// <param name="A">The matrix to invert. This matrix is unchanged by this function.</param>
        /// <param name="L">The L matrix is output where the diagonal elements are included and not (necessarily) equal to one.</param>
        /// <param name="U">The U matrix is output where the diagonal elements are all equal to one.</param>
        /// <param name="permute">The permute.</param>
        /// <exception cref="System.ArithmeticException">LU Decomposition can only be determined for square matrices.</exception>
        /// <exception cref="ArithmeticException">LU Decomposition can only be determined for square matrices.</exception>
        public static void LUDecomposition(int[,] A, out double[,] L, out double[,] U, out int[] permute)
        {
            var length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("LU Decomposition can only be determined for square matrices.");
            L = LUDecomposition(A, out permute, length);
            U = new double[length, length];
            for (var i = 0; i < length; i++)
            {
                U[i, i] = 1.0;
                for (var j = i + 1; j < length; j++)
                {
                    U[i, j] = L[i, j];
                    L[i, j] = 0.0;
                }
            }
        }

        /// <summary>
        /// Returns the LU decomposition of A in a new matrix.
        /// </summary>
        /// <param name="A">The matrix to invert. This matrix is unchanged by this function.</param>
        /// <param name="permutationVector">The resulting permutation vector - how the rows are re-ordered to
        /// create L and U.</param>
        /// <param name="length">The length/order/number of rows of matrix, A.</param>
        /// <param name="robustReorder">if set to <c>true</c> [robust reorder]. But this is an internal recursive call
        /// and should not be set outside.</param>
        /// <param name="zeroIndices">The last zero indices - is calculated in this function, but if it is already
        /// known, then...by all means.</param>
        /// <returns>A matrix of equal size to A that combines the L and U. Here the diagonals belongs to L and the U's diagonal
        /// elements are all 1.</returns>
        /// <exception cref="ArithmeticException">LU Decomposition can only be determined for square matrices.</exception>
        private static double[,] LUDecomposition(int[,] A, out int[] permutationVector, int length = -1,
            bool robustReorder = false, List<int>[] zeroIndices = null)
        {
            // This is an implementation of Crout’s LU Decomposition Algorithm
            if (length == -1) length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("LU Decomposition can only be determined for square matrices.");
            if (zeroIndices == null)
            {
                zeroIndices = new List<int>[length];
                for (int i = 0; i < length; i++)
                {
                    zeroIndices[i]= new List<int>();
                    for (int j = 0; j < length; j++)
                        if (A[i, j] == 0) zeroIndices[i].Add(j);
                }
            }
            var B = new double[length, length];
            for (var i = 0; i < length; i++)
                for (var j = 0; j < length; j++)
                    B[i, j] = A[i, j];
            permutationVector = Enumerable.Range(0, length).ToArray();
            // normalize row 0
            for (var i = 0; i < length; i++)
            {
                if (!findAndPivotRows(B, permutationVector, zeroIndices, i, length, robustReorder))
                    return LUDecomposition(A, out permutationVector, length, true, zeroIndices);

                var pI = permutationVector[i];
                for (var j = i; j < length; j++)
                {
                    var pJ = permutationVector[j];
                    // do a column of L
                    var sum = 0.0;
                    for (var k = 0; k < i; k++)
                        sum -= B[pJ, k] * B[permutationVector[k], i];
                    B[pJ, i] += sum;
                }
                for (var j = i + 1; j < length; j++)
                {
                    // do a row of U
                    var sum = 0.0;
                    for (var k = 0; k < i; k++)
                        sum += B[pI, k] * B[permutationVector[k], j];
                    B[pI, j] = (-sum + B[pI, j]) / B[pI, i];
                }
            }
            return B;
        }

        #endregion

        #region Cholesky Decomposition
        /// <summary>
        /// Returns the Cholesky decomposition of A in a new matrix. The new matrix is a lower triangular matrix, and
        /// the diagonals are the D matrix in the L-D-LT formulation. To get the L-LT format.
        /// </summary>
        /// <param name="A">The matrix to invert. This matrix is unchanged by this function.</param>
        /// <param name="NoSeparateDiagonal">if set to <c>true</c> [no separate diagonal].</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">Matrix cannot be inverted. Can only invert square matrices.</exception>
        /// <exception cref="ArithmeticException">Matrix cannot be inverted. Can only invert square matrices.</exception>
        public static double[,] CholeskyDecomposition(double[,] A, bool NoSeparateDiagonal = false)
        {
            var length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("Cholesky Decomposition can only be determined for square matrices.");
            var L = (double[,])A.Clone();

            for (var i = 0; i < length; i++)
            {
                double sum;
                for (var j = 0; j < i; j++)
                {
                    sum = 0.0;
                    for (int k = 0; k < j; k++)
                        sum += L[i, k] * L[j, k] * L[k, k];
                    L[i, j] = (L[i, j] - sum) / L[j, j];
                }
                sum = 0.0;
                for (int k = 0; k < i; k++)
                    sum += L[i, k] * L[i, k] * L[k, k];
                L[i, i] -= sum;
                for (int j = i + 1; j < length; j++)
                    L[i, j] = 0.0;
            }
            if (NoSeparateDiagonal)
                for (int i = 0; i < length; i++)
                {
                    if (L[i, i] < 0) throw new ArithmeticException("Cannot complete L-LT Cholesky Decomposition due to indefinite matrix (must be positive semidefinite).");
                    L[i, i] = Math.Sqrt(L[i, i]);
                }

            return L;
        }
        /// <summary>
        /// Returns the Cholesky decomposition of A in a new matrix. The new matrix is a lower triangular matrix, and
        /// the diagonals are the D matrix in the L-D-LT formulation. To get the L-LT format.
        /// </summary>
        /// <param name="A">The matrix to invert. This matrix is unchanged by this function.</param>
        /// <returns>System.Double[].</returns>
        /// <exception cref="System.ArithmeticException">Matrix cannot be inverted. Can only invert square matrices.</exception>
        /// <exception cref="ArithmeticException">Cholesky Decomposition can only be determined for square matrices.</exception>
        public static double[,] CholeskyDecomposition(int[,] A, bool NoSeparateDiagonal = false)
        {
            var length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("Cholesky Decomposition can only be determined for square matrices.");
            var L = (double[,])A.Clone();

            for (var i = 0; i < length; i++)
            {
                double sum;
                for (var j = 0; j < i; j++)
                {
                    sum = 0.0;
                    for (int k = 0; k < j; k++)
                        sum += L[i, k] * L[j, k] * L[k, k];
                    L[i, j] = (L[i, j] - sum) / L[j, j];
                }
                sum = 0.0;
                for (int k = 0; k < i; k++)
                    sum += L[i, k] * L[i, k] * L[k, k];
                L[i, i] -= sum;
                for (int j = i + 1; j < length; j++)
                    L[i, j] = 0.0;
            }
            if (NoSeparateDiagonal)
                for (int i = 0; i < length; i++)
                {
                    if (L[i, i] < 0) throw new ArithmeticException("Cannot complete L-LT Cholesky Decomposition due to indefinite matrix (must be positive semidefinite).");
                    L[i, i] = Math.Sqrt(L[i, i]);
                }

            return L;
        }

        #endregion

        #region Transpose

        /// <summary>
        /// Transposes the matrix, A.
        /// </summary>
        /// <param name="A">The matrix to transpose. This matrix is unchanged by this function.</param>
        /// <returns>The transpose of A.</returns>
        /// <exception cref="ArithmeticException">The matrix, A, is null.</exception>
        /// <exception cref="System.ArithmeticException">The matrix, A, is null.</exception>
        public static double[,] transpose(this double[,] A)
        {
            if (A == null) throw new ArithmeticException("The matrix, A, is null.");
            var numRows = A.GetLength(1);
            var numCols = A.GetLength(0);
            var C = new double[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[j, i];
            return C;
        }

        /// <summary>
        /// Transposes the matrix, A.
        /// </summary>
        /// <param name="A">The matrix to transpose. This matrix is unchanged by this function.</param>
        /// <returns>The transpose of A.</returns>
        /// <exception cref="ArithmeticException">The matrix, A, is null.</exception>
        /// <exception cref="System.ArithmeticException">The matrix, A, is null.</exception>
        public static int[,] transpose(this int[,] A)
        {
            if (A == null) throw new ArithmeticException("The matrix, A, is null.");
            var numRows = A.GetLength(1);
            var numCols = A.GetLength(0);

            var C = new int[numRows, numCols];

            for (var i = 0; i != numRows; i++)
                for (var j = 0; j != numCols; j++)
                    C[i, j] = A[j, i];
            return C;
        }

        #endregion

        #region Determinant

        /// <summary>
        /// Returns the determinant of matrix, A.
        /// </summary>
        /// <param name="A">The input argument matrix. This matrix is unchanged by this function.</param>
        /// <returns>a single value representing the matrix's determinant.</returns>
        /// <exception cref="System.ArithmeticException">The matrix, A, is null.
        /// or
        /// The determinant is only possible for square matrices.</exception>
        /// <exception cref="ArithmeticException">The matrix, A, is null.
        /// or
        /// The determinant is only possible for square matrices.</exception>
        public static double determinant(this double[,] A)
        {
            if (A == null) throw new ArithmeticException("The matrix, A, is null.");
            var length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("The determinant is only possible for square matrices.");
            if (length == 0) return 0.0;
            if (length == 1) return A[0, 0];
            if (length == 2) return (A[0, 0] * A[1, 1]) - (A[0, 1] * A[1, 0]);
            if (length == 3)
                return (A[0, 0] * A[1, 1] * A[2, 2])
                       + (A[0, 1] * A[1, 2] * A[2, 0])
                       + (A[0, 2] * A[1, 0] * A[2, 1])
                       - (A[0, 0] * A[1, 2] * A[2, 1])
                       - (A[0, 1] * A[1, 0] * A[2, 2])
                       - (A[0, 2] * A[1, 1] * A[2, 0]);
            return determinantBig(A, length);
        }

        /// <summary>
        /// Determinants the big.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.Double.</returns>
        private static double determinantBig(double[,] A, int length)
        {
            int[] permute;
            var L = LUDecomposition(A, out permute, length);
            var result = 1.0;
            for (var i = 0; i < length; i++)
                if (double.IsNaN(L[permute[i], i]))
                    return 0;
                else result *= L[permute[i], i];
            return result;
        }

        /// <summary>
        /// Returns the determinant of matrix, A.
        /// </summary>
        /// <param name="A">The input argument matrix. This matrix is unchanged by this function.</param>
        /// <returns>a single value representing the matrix's determinant.</returns>
        /// <exception cref="System.ArithmeticException">The matrix, A, is null.
        /// or
        /// The determinant is only possible for square matrices.</exception>
        /// <exception cref="ArithmeticException">The matrix, A, is null.
        /// or
        /// The determinant is only possible for square matrices.</exception>
        public static int determinant(this int[,] A)
        {
            if (A == null) throw new ArithmeticException("The matrix, A, is null.");
            var length = A.GetLength(0);
            if (length != A.GetLength(1))
                throw new ArithmeticException("The determinant is only possible for square matrices.");
            if (length == 0) return 0;
            if (length == 1) return A[0, 0];
            if (length == 2) return (A[0, 0] * A[1, 1]) - (A[0, 1] * A[1, 0]);
            if (length == 3)
                return (A[0, 0] * A[1, 1] * A[2, 2])
                       + (A[0, 1] * A[1, 2] * A[2, 0])
                       + (A[0, 2] * A[1, 0] * A[2, 1])
                       - (A[0, 0] * A[1, 2] * A[2, 1])
                       - (A[0, 1] * A[1, 0] * A[2, 2])
                       - (A[0, 2] * A[1, 1] * A[2, 0]);
            return determinantBig(A, length);
        }

        /// <summary>
        /// Returns the determinant of matrix, A. Only used internally for matrices larger than 3.
        /// </summary>
        /// <param name="A">The input argument matrix. This matrix is unchanged by this function.</param>
        /// <param name="length">The length of the side of the square matrix.</param>
        /// <returns>a single value representing the matrix's determinant.</returns>
        private static int determinantBig(int[,] A, int length)
        {
            int[] permute;
            var L = LUDecomposition(A, out permute, length);
            var result = 1.0;
            for (var i = 0; i < length; i++)
                if (double.IsNaN(L[permute[i], i]))
                    return 0;
                else result *= L[permute[i], i];
            return (int)result;
        }

        #endregion
    }
}