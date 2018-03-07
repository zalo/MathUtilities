// ***********************************************************************
// Assembly         : StarMath
// Author           : Matthew I. Campbell
// Created          : 02-28-2016
// Originally Modified from    : Timothy A. Davis's CSparse code, 2006-2014
// Last Modified By : Matt Campbell
// Last Modified On : 03-15-2016
// ***********************************************************************
// <copyright file="CSparseNetMain.cs" company="Design Engineering Lab -- MICampbell">
//     2014
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;

namespace StarMathLib
{
    /// <summary>
    /// Class CSparse.
    /// </summary>
    internal static class CSparse
    {
        /// <summary>
        /// The tol
        /// </summary>
        private const double tol = 1.0;

        #region solve LU Decomposition

        /// <summary>
        /// Solve a lower triangular system by forward elimination, Lx=b.
        /// </summary>
        /// <param name="L">The l.</param>
        /// <param name="x">The x.</param>
        internal static void SolveLower(CompressedColumnStorage L, double[] x)
        {
            var colPointers = L.ColumnPointers;
            var rowIndices = L.RowIndices;
            var values = L.Values;

            for (var j = 0; j < L.ncols; j++)
            {
                x[j] /= values[colPointers[j]];
                var k = colPointers[j + 1];
                for (var p = colPointers[j] + 1; p < k; p++)
                    x[rowIndices[p]] -= values[p]*x[j];
            }
        }

        /// <summary>
        /// Solve an upper triangular system by backward elimination, Ux=b.
        /// </summary>
        /// <param name="U">The u.</param>
        /// <param name="x">The x.</param>
        internal static void SolveUpper(CompressedColumnStorage U, double[] x)
        {
            var colPointers = U.ColumnPointers;
            var rowIndices = U.RowIndices;
            var values = U.Values;

            for (var j = U.ncols - 1; j >= 0; j--)
            {
                x[j] /= values[colPointers[j + 1] - 1];
                var k = colPointers[j + 1] - 1;
                for (var p = colPointers[j]; p < k; p++)
                    x[rowIndices[p]] -= values[p]*x[j];
            }
        }

        /// <summary>
        /// [L,U,pinv] = lu(A, [q lnz unz]). lnz and unz can be guess.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="columnPermutation">The column permutation.</param>
        /// <param name="L">The l.</param>
        /// <param name="U">The u.</param>
        /// <param name="pinv">The pinv.</param>
        /// <exception cref="System.Exception">No pivot element found.</exception>
        /// <exception cref="Exception">No pivot element found.</exception>
        internal static void FactorizeLU(CompressedColumnStorage A, int[] columnPermutation,
            out CompressedColumnStorage L,
            out CompressedColumnStorage U, out int[] pinv)
        {
            var n = A.ncols;
            var x = new double[n];
            int i;
            var xi = new int[2*n]; // Workspace
            var lnz = 0;
            var unz = 0;
            int[] li;
            L = new CompressedColumnStorage(n, n, 4*A.ColumnPointers[n] + n);
            U = new CompressedColumnStorage(n, n, 4*A.ColumnPointers[n] + n);
            pinv = new int[n];
            for (i = 0; i < n; i++) // No rows pivotal yet.
                pinv[i] = -1;
            var lp = L.ColumnPointers;
            var up = U.ColumnPointers;

            // Now compute L(:,k) and U(:,k)
            for (var k = 0; k < n; k++)
            {
                // Triangular solve
                lp[k] = lnz; // L(:,k) starts here
                up[k] = unz; // U(:,k) starts here

                if (lnz + n > L.Values.Length) L.Resize(2*L.Values.Length + n);
                if (unz + n > U.Values.Length) U.Resize(2*U.Values.Length + n);

                li = L.RowIndices;
                var ui = U.RowIndices;
                var lx = L.Values;
                var ux = U.Values;
                var col = columnPermutation != null ? columnPermutation[k] : k;
                var top = SolveSp(L, A, col, xi, x, pinv, true);

                // Find pivot
                var ipiv = -1;
                double a = -1;
                for (var p = top; p < n; p++)
                {
                    i = xi[p]; // x(i) is nonzero
                    if (pinv[i] < 0) // Row i is not yet pivotal
                    {
                        double t;
                        if ((t = Math.Abs(x[i])) > a)
                        {
                            a = t; // Largest pivot candidate so far
                            ipiv = i;
                        }
                    }
                    else // x(i) is the entry U(pinv[i],k)
                    {
                        ui[unz] = pinv[i];
                        ux[unz++] = x[i];
                    }
                }
                if (ipiv == -1 || a <= 0.0)
                    throw new Exception("No pivot element found.");
                if (pinv[col] < 0 && Math.Abs(x[col]) >= a*tol)
                    ipiv = col;
                // Divide by pivot
                var pivot = x[ipiv];
                ui[unz] = k; // last entry in U(:,k) is U(k,k)
                ux[unz++] = pivot;
                pinv[ipiv] = k; // ipiv is the kth pivot row
                li[lnz] = ipiv; // first entry in L(:,k) is L(k,k) = 1
                lx[lnz++] = 1.0;
                for (var p = top; p < n; p++) // L(k+1:n,k) = x / pivot
                {
                    i = xi[p];
                    if (pinv[i] < 0) // x(i) is an entry in L(:,k)
                    {
                        li[lnz] = i; // save unpermuted row in L
                        lx[lnz++] = x[i]/pivot; // scale pivot column
                    }
                    x[i] = 0.0; // x [0..n-1] = 0 for next k
                }
            }

            // Finalize L and U
            lp[n] = lnz;
            up[n] = unz;
            li = L.RowIndices; // fix row indices of L for final pinv
            for (var p = 0; p < lnz; p++)
            {
                li[p] = pinv[li[p]];
            }

            // Remove extra space from L and U
            L.Resize(0);
            U.Resize(0);
        }


        /// <summary>
        /// Solve Gx=b(:,k), where G is either upper (lo=false) or lower (lo=true)
        /// triangular.
        /// </summary>
        /// <param name="G">lower or upper triangular matrix in column-compressed form</param>
        /// <param name="B">right hand side, b=B(:,k)</param>
        /// <param name="k">use kth column of B as right hand side</param>
        /// <param name="xi">size 2*n, nonzero pattern of x in xi[top..n-1]</param>
        /// <param name="x">size n, x in x[xi[top..n-1]]</param>
        /// <param name="pinv">mapping of rows to columns of G, ignored if null</param>
        /// <param name="lo">true if lower triangular, false if upper</param>
        /// <returns>top, -1 in error</returns>
        private static int SolveSp(CompressedColumnStorage G, CompressedColumnStorage B,
            int k, int[] xi, double[] x, int[] pinv, bool lo)
        {
            if (xi == null || x == null) return -1;
            var gColPointers = G.ColumnPointers;
            var gRowIndices = G.RowIndices;
            var gValues = G.Values;
            var bColPointers = B.ColumnPointers;
            var bRowIndices = B.RowIndices;
            var bValues = B.Values;
            var n = G.ncols;

            // xi[top..n-1]=Reach(B(:,k))
            var top = ApproximateMinimumDegree.Reach(gColPointers, gRowIndices, bColPointers, bRowIndices, n, k, xi,
                pinv);
            for (var i = top; i < n; i++)
                x[xi[i]] = 0; // clear x
            for (var i = bColPointers[k]; i < bColPointers[k + 1]; i++)
                x[bRowIndices[i]] = bValues[i]; // scatter B
            for (var px = top; px < n; px++)
            {
                var j = xi[px];
                var J = pinv != null ? pinv[j] : j;
                if (J < 0) continue; // column J is empty
                x[j] /= gValues[lo ? gColPointers[J] : gColPointers[J + 1] - 1]; // x(j) /= G(j,j)
                var p = lo ? gColPointers[J] + 1 : gColPointers[J]; // lo: L(j,j) 1st entry
                var q = lo ? gColPointers[J + 1] : gColPointers[J + 1] - 1;
                for (; p < q; p++)
                    x[gRowIndices[p]] -= gValues[p]*x[j]; // x(i) -= G(i,j) * x(j)
            }
            // Return top of stack.
            return top;
        }

        #endregion

        #region solve LDL via Cholesky

        /// <summary>
        /// Symbolics the analysis LDL.
        /// </summary>
        /// <param name="A">a.</param>
        /// <returns>SymbolicFactorization.</returns>
        internal static SymbolicFactorization SymbolicAnalysisLDL(CompressedColumnStorage A)
        {
            var n = A.ncols;
            var aColPointers = A.ColumnPointers;
            var aRowIndices = A.RowIndices;

            // P = amd(A+A') or natural
            var permute = ApproximateMinimumDegree.Generate(A);
            var invPermute = Invert(permute);

            // Output: column pointers and elimination tree.
            var lp = new int[n + 1];
            var parent = new int[n];

            // Workspace
            var lnz = new int[n];
            var flag = new int[n];

            for (var k = 0; k < n; k++)
            {
                // L(k,:) pattern: all nodes reachable in etree from nz in A(0:k-1,k) 
                parent[k] = -1; // parent of k is not yet known 
                flag[k] = k; // mark node k as visited 
                lnz[k] = 0; // count of nonzeros in column k of L 
                var kk = permute != null ? permute[k] : k;
                var p2 = aColPointers[kk + 1];
                int p;
                for (p = aColPointers[kk]; p < p2; p++)
                {
                    // A(i,k) is nonzero (original or permuted A) 
                    var i = invPermute != null ? invPermute[aRowIndices[p]] : aRowIndices[p];
                    if (i < k)
                    {
                        // follow path from i to root of etree, stop at flagged node 
                        for (; flag[i] != k; i = parent[i])
                        {
                            // find parent of i if not yet determined 
                            if (parent[i] == -1) parent[i] = k;
                            lnz[i]++; // L(k,i) is nonzero 
                            flag[i] = k; // mark i as visited 
                        }
                    }
                }
            }
            // construct Lp index array from Lnz column counts 
            lp[0] = 0;
            for (var k = 0; k < n; k++)
                lp[k + 1] = lp[k] + lnz[k];
            return new SymbolicFactorization
            {
                ParentIndices = parent,
                ColumnPointers = lp,
                PermutationVector = permute,
                InversePermute = invPermute
            };
        }

        /// <summary>
        /// Compute the numeric LDL' factorization of PAP'.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <param name="S">The symobolic factorization.</param>
        /// <param name="D">The diagonals of LDL.</param>
        /// <param name="L">The lower triangular matrix.</param>
        /// <exception cref="System.Exception">Diagonal element is zero.</exception>
        /// <exception cref="Exception">Diagonal element is zero.</exception>
        internal static void FactorizeLDL(CompressedColumnStorage A, SymbolicFactorization S, out double[] D,
            out CompressedColumnStorage L)
        {
            var n = A.ncols;

            var aColPointers = A.ColumnPointers;
            var aRowIndices = A.RowIndices;
            var aValues = A.Values;

            var parent = S.ParentIndices;
            var P = S.PermutationVector;
            var Pinv = S.InversePermute;

            D = new double[n];
            L = new CompressedColumnStorage(n, n, S.ColumnPointers[n]);

            Array.Copy(S.ColumnPointers, L.ColumnPointers, n + 1);

            var lColPointers = L.ColumnPointers;
            var lRowIndices = L.RowIndices;
            var lValues = L.Values;

            // Workspace
            var y = new double[n];
            var pattern = new int[n];
            var flag = new int[n];
            var lnz = new int[n];

            for (var k = 0; k < n; k++)
            {
                // compute nonzero Pattern of kth row of L, in topological order
                y[k] = 0.0; // Y(0:k) is now all zero
                var top = n;
                flag[k] = k; // mark node k as visited
                lnz[k] = 0; // count of nonzeros in column k of L
                var kk = P != null ? P[k] : k;
                var p2 = aColPointers[kk + 1];

                for (var p = aColPointers[kk]; p < p2; p++)
                {
                    var i = Pinv != null ? Pinv[aRowIndices[p]] : aRowIndices[p]; // get A(i,k)
                    if (i <= k)
                    {
                        y[i] += aValues[p]; // scatter A(i,k) into Y (sum duplicates)
                        int len;
                        for (len = 0; flag[i] != k; i = parent[i])
                        {
                            pattern[len++] = i; // L(k,i) is nonzero
                            flag[i] = k; // mark i as visited
                        }
                        while (len > 0)
                            pattern[--top] = pattern[--len];
                    }
                }
                // compute numerical values kth row of L (a sparse triangular solve)
                D[k] = y[k]; // get D(k,k) and clear Y(k)
                y[k] = 0.0;
                for (; top < n; top++)
                {
                    var i = pattern[top]; // Pattern [top:n-1] is pattern of L(:,k)
                    var yi = y[i];
                    y[i] = 0.0;
                    p2 = lColPointers[i] + lnz[i];
                    int p;
                    for (p = lColPointers[i]; p < p2; p++)
                        y[lRowIndices[p]] -= lValues[p]*yi;
                    var l_ki = yi/D[i];
                    D[k] -= l_ki*yi;
                    lRowIndices[p] = k; // store L(k,i) in column form of L
                    lValues[p] = l_ki;
                    lnz[i]++; // increment count of nonzeros in col i
                }
                if (D[k] == 0.0) // failure, D(k,k) is zero
                    throw new Exception("Diagonal element is zero.");
            }
        }

        /// <summary>
        /// Solves a linear system Ax=b, where A is symmetric positive definite.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="L">The lower triangular matrix.</param>
        /// <param name="D">The diagonals.</param>
        /// <param name="inversePermute">The inverse permute.</param>
        /// <returns>System.Double[].</returns>
        internal static double[] SolveLDL(IList<double> b, CompressedColumnStorage L, double[] D,
            int[] inversePermute)
        {
            var n = L.ncols;
            var x = ApplyInverse(inversePermute, b, n); // x = P*b
            var lValues = L.Values;
            var lColPointers = L.ColumnPointers;
            var lRowIndices = L.RowIndices;
            // Solve lower triangular system by forward elimination, x = L\x.
            for (var i = 0; i < n; i++)
            {
                var end = lColPointers[i + 1];
                for (var p = lColPointers[i]; p < end; p++)
                    x[lRowIndices[p]] -= lValues[p]*x[i];
            }
            // Solve diagonal system, x = D\x.
            for (var i = 0; i < n; i++)
                x[i] /= D[i];
            // Solve upper triangular system by backward elimination, x = L'\x.
            for (var i = n - 1; i >= 0; i--)
            {
                var end = lColPointers[i + 1];
                for (var p = lColPointers[i]; p < end; p++)
                    x[i] -= lValues[p]*x[lRowIndices[p]];
            }
            return Apply(inversePermute, x, n); // b = P'*x
        }

        #endregion

        #region Permutation

        /// <summary>
        /// Permutes a vector, x=P*b.
        /// </summary>
        /// <param name="p">Permutation vector.</param>
        /// <param name="b">Input vector.</param>
        /// <param name="n">Length of p, b and x.</param>
        /// <returns>System.Double[].</returns>
        /// <remarks>p = null denotes identity.</remarks>
        internal static double[] Apply(int[] p, IList<double> b, int n)
        {
            var x = new double[n];
            for (var k = 0; k < n; k++)
                x[k] = b[p[k]];
            return x;
        }

        /// <summary>
        /// Permutes a vector, x = P'b.
        /// </summary>
        /// <param name="p">Permutation vector.</param>
        /// <param name="b">Input vector.</param>
        /// <param name="n">Length of p, b, and x.</param>
        /// <returns>System.Double[].</returns>
        /// <remarks>p = null denotes identity.</remarks>
        internal static double[] ApplyInverse(int[] p, IList<double> b, int n)
        {
            var x = new double[n];
            for (var k = 0; k < n; k++)
                x[p[k]] = b[k];
            return x;
        }

        /// <summary>
        /// Inverts a permutation vector.
        /// </summary>
        /// <param name="p">A permutation vector.</param>
        /// <returns>Returns pinv[i] = k if p[k] = i on input.</returns>
        internal static int[] Invert(int[] p)
        {
            int k, n = p.Length;
            var pinv = new int[n];
            // Invert the permutation.
            for (k = 0; k < n; k++)
                pinv[p[k]] = k;

            return pinv;
        }

        #endregion
    }
}