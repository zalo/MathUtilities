// ***********************************************************************
// Assembly         : StarMath
// Author           : Matthew I. Campbell
// Created          : 02-28-2016
// Originally Modified from    : Timothy A. Davis's CSparse code, 2006-2014
// Last Modified By : Matt Campbell
// Last Modified On : 03-15-2016
// ***********************************************************************
// <copyright file="ApproximateMinimumDegree.cs" company="Design Engineering Lab -- MICampbell">
//     2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
namespace StarMathLib
{
    /// <summary>
    /// Approximate Minimum Degree ordering.
    /// </summary>
    internal static class ApproximateMinimumDegree
    {
        /// <summary>
        /// Generate minimum degree ordering of A+A' (if A is symmetric) or A'A.
        /// </summary>
        /// <param name="A">Column-compressed matrix</param>
        /// <returns>amd(A+A') if A is symmetric, or amd(A'A) otherwise, null on
        /// error or for natural ordering</returns>
        /// <remarks>See Chapter 7.1 (Fill-reducing orderings: Minimum degree ordering) in
        /// "Direct Methods for Sparse Linear Systems" by Tim Davis.</remarks>
        internal static int[] Generate(CompressedColumnStorage A)
        {
            return Generate(SymbolicColumnStorage.ConstructMatrix(A), A.ncols);
        }

        /// <summary>
        /// Generates the specified c.
        /// </summary>
        /// <param name="C">The c.</param>
        /// <param name="n">The n.</param>
        /// <returns>System.Int32[].</returns>
        internal static int[] Generate(SymbolicColumnStorage C, int n)
        {
            //  int  e, i, j, k;
            int j, k;
            int lemax = 0;
            int mindeg = 0;
            int nel = 0;

            var Cp = C.ColumnPointers;
            var cnz = Cp[n];

            // Find dense threshold
            var dense = Math.Max(16, 10 * (int)Math.Sqrt(n));
            dense = Math.Min(n - 2, dense);

            // add elbow room to C
            if (!C.Resize(cnz + cnz / 5 + 2 * n)) return null;

            var P = new int[n + 1];
            var W = new int[n + 1]; // get workspace
            var w = new int[n + 1];
            var degree = new int[n + 1];

            var elen = new int[n + 1]; // Initialized to 0's

            // Initialize quotient graph
            for (k = 0; k < n; k++)
            {
                W[k] = Cp[k + 1] - Cp[k];
            }
            W[n] = 0;
            var nzmax = C.RowIndices.Length;
            var Ci = C.RowIndices;

            for (int i = 0; i <= n; i++)
            {
                P[i] = -1;
                w[i] = 1; // node i is alive
                degree[i] = W[i]; // degree of node i
            }

            var next = new int[n + 1];
            var hhead = new int[n + 1];
            var head = new int[n + 1];
            var nv = new int[n + 1];

            Array.Copy(P, next, n + 1);
            Array.Copy(P, head, n + 1); // degree list i is empty
            Array.Copy(P, hhead, n + 1); // hash list i is empty
            Array.Copy(w, nv, n + 1); // node i is just one node

            var mark = Clear(0, 0, w, n); // clear w
            elen[n] = -2; // n is a dead element
            Cp[n] = -1; // n is a root of assembly tree
            w[n] = 0; // n is a dead element

            // Initialize degree lists
            for (int i = 0; i < n; i++)
            {
                int d = degree[i];
                if (d == 0) // node i is empty
                {
                    elen[i] = -2; // element i is dead
                    nel++;
                    Cp[i] = -1; // i is a root of assembly tree
                    w[i] = 0;
                }
                else if (d > dense) // node i is dense
                {
                    nv[i] = 0; // absorb i into element n
                    elen[i] = -1; // node i is dead
                    nel++;
                    Cp[i] = -(n + 2); // FLIP(n)
                    nv[n]++;
                }
                else
                {
                    if (head[d] != -1) P[head[d]] = i;
                    next[i] = head[d]; // put node i in degree list d
                    head[d] = i;
                }
            }
            while (nel < n) // while (selecting pivots) do
            {
                // Select node of minimum approximate degree
                for (k = -1; mindeg < n && (k = head[mindeg]) == -1; mindeg++) ;
                if (next[k] != -1) P[next[k]] = -1;
                head[mindeg] = next[k]; // remove k from degree list
                var elenk = elen[k];
                var nvk = nv[k];
                nel += nvk; // nv[k] nodes of A eliminated

                // Garbage collection
                int p;
                if (elenk > 0 && cnz + mindeg >= nzmax)
                {
                    for (j = 0; j < n; j++)
                    {
                        if ((p = Cp[j]) >= 0) // j is a live node or element
                        {
                            Cp[j] = Ci[p]; // save first entry of object
                            Ci[p] = -(j + 2); // first entry is now CS_FLIP(j)
                        }
                    }
                    int q;
                    for (q = 0, p = 0; p < cnz;) // scan all of memory
                    {
                        if ((j = flip(Ci[p++])) >= 0) // found object j
                        {
                            Ci[q] = Cp[j]; // restore first entry of object
                            Cp[j] = q++; // new pointer to object j
                            int k3;
                            for (k3 = 0; k3 < W[j] - 1; k3++)
                            {
                                Ci[q++] = Ci[p++];
                            }
                        }
                    }
                    cnz = q; // Ci [cnz...nzmax-1] now free
                }

                // Construct new element
                var dk = 0;
                nv[k] = -nvk; // flag k as in Lk
                p = Cp[k];
                var pk1 = elenk == 0 ? p : cnz;
                var pk2 = pk1;
                int k1;
                int ln;
                int nvi;
                for (k1 = 1; k1 <= elenk + 1; k1++)
                {
                    int pj, e;
                    if (k1 > elenk)
                    {
                        e = k; // search the nodes in k
                        pj = p; // list of nodes starts at Ci[pj]*/
                        ln = W[k] - elenk; // length of list of nodes in k
                    }
                    else
                    {
                        e = Ci[p++]; // search the nodes in e
                        pj = Cp[e];
                        ln = W[e]; // length of list of nodes in e
                    }
                    int k2;
                    for (k2 = 1; k2 <= ln; k2++)
                    {
                        int i = Ci[pj++];
                        if ((nvi = nv[i]) <= 0) continue; // node i dead, or seen
                        dk += nvi; // degree[Lk] += size of node i
                        nv[i] = -nvi; // negate nv[i] to denote i in Lk
                        Ci[pk2++] = i; // place i in Lk
                        if (next[i] != -1) P[next[i]] = P[i];
                        if (P[i] != -1) // remove i from degree list
                        {
                            next[P[i]] = next[i];
                        }
                        else
                        {
                            head[degree[i]] = next[i];
                        }
                    }
                    if (e != k)
                    {
                        Cp[e] = -(k + 2); // absorb e into k // FLIP(k)
                        w[e] = 0; // e is now a dead element
                    }
                }
                if (elenk != 0) cnz = pk2; // Ci [cnz...nzmax] is free
                degree[k] = dk; // external degree of k - |Lk\i|
                Cp[k] = pk1; // element k is in Ci[pk1..pk2-1]
                W[k] = pk2 - pk1;
                elen[k] = -2; // k is now an element

                // Find set differences
                mark = Clear(mark, lemax, w, n); // clear w if necessary
                int pk;
                int eln;
                for (pk = pk1; pk < pk2; pk++) // scan 1: find |Le\Lk|
                {
                    int i = Ci[pk];
                    if ((eln = elen[i]) <= 0) continue; // skip if elen[i] empty
                    nvi = -nv[i]; // nv [i] was negated
                    var wnvi = mark - nvi;
                    for (p = Cp[i]; p <= Cp[i] + eln - 1; p++) // scan Ei
                    {
                        int e = Ci[p];
                        if (w[e] >= mark)
                        {
                            w[e] -= nvi; // decrement |Le\Lk|
                        }
                        else if (w[e] != 0) // ensure e is a live element
                        {
                            w[e] = degree[e] + wnvi; // 1st time e seen in scan 1
                        }
                    }
                }

                // Degree update
                int h;
                for (pk = pk1; pk < pk2; pk++) // scan2: degree update
                {
                    int i = Ci[pk]; // consider node i in Lk
                    var p1 = Cp[i];
                    var p2 = p1 + elen[i] - 1;
                    var pn = p1;
                    var d = 0;
                    for (h = 0, d = 0, p = p1; p <= p2; p++) // scan Ei
                    {
                        int e = Ci[p];
                        if (w[e] != 0) // e is an unabsorbed element
                        {
                            var dext = w[e] - mark;
                            if (dext > 0)
                            {
                                d += dext; // sum up the set differences
                                Ci[pn++] = e; // keep e in Ei
                                h += e; // compute the hash of node i
                            }
                            else
                            {
                                Cp[e] = -(k + 2); // aggressive absorb. e.k // FLIP(k)
                                w[e] = 0; // e is a dead element
                            }
                        }
                    }
                    elen[i] = pn - p1 + 1; // elen[i] = |Ei|
                    var p3 = pn;
                    var p4 = p1 + W[i];
                    for (p = p2 + 1; p < p4; p++) // prune edges in Ai
                    {
                        j = Ci[p];
                        int nvj;
                        if ((nvj = nv[j]) <= 0) continue; // node j dead or in Lk
                        d += nvj; // degree(i) += |j|
                        Ci[pn++] = j; // place j in node list of i
                        h += j; // compute hash for node i
                    }
                    if (d == 0) // check for mass elimination
                    {
                        Cp[i] = -(k + 2); // absorb i into k // FLIP(k)
                        nvi = -nv[i];
                        dk -= nvi; // |Lk| -= |i|
                        nvk += nvi; // |k| += nv[i]
                        nel += nvi;
                        nv[i] = 0;
                        elen[i] = -1; // node i is dead
                    }
                    else
                    {
                        degree[i] = Math.Min(degree[i], d); // update degree(i)
                        Ci[pn] = Ci[p3]; // move first node to end
                        Ci[p3] = Ci[p1]; // move 1st el. to end of Ei
                        Ci[p1] = k; // add k as 1st element in of Ei
                        W[i] = pn - p1 + 1; // new len of adj. list of node i
                        h = (h < 0 ? -h : h) % n; // finalize hash of i
                        next[i] = hhead[h]; // place i in hash bucket
                        hhead[h] = i;
                        P[i] = h; // save hash of i in last[i]
                    }
                } // scan2 is done
                degree[k] = dk; // finalize |Lk|
                lemax = Math.Max(lemax, dk);
                mark = Clear(mark + lemax, lemax, w, n); // clear w

                // Supernode detection
                for (pk = pk1; pk < pk2; pk++)
                {
                    int i = Ci[pk];
                    if (nv[i] >= 0) continue; // skip if i is dead
                    h = P[i]; // scan hash bucket of node i
                    i = hhead[h];
                    hhead[h] = -1; // hash bucket will be empty
                    for (; i != -1 && next[i] != -1; i = next[i], mark++)
                    {
                        ln = W[i];
                        eln = elen[i];
                        for (p = Cp[i] + 1; p <= Cp[i] + ln - 1; p++) w[Ci[p]] = mark;
                        var jlast = i;
                        for (j = next[i]; j != -1;) // compare i with all j
                        {
                            var ok = (W[j] == ln) && (elen[j] == eln);
                            for (p = Cp[j] + 1; ok && p <= Cp[j] + ln - 1; p++)
                            {
                                if (w[Ci[p]] != mark) ok = false; // compare i and j
                            }
                            if (ok) // i and j are identical
                            {
                                Cp[j] = -(i + 2); // absorb j into i // FLIP(i)
                                nv[i] += nv[j];
                                nv[j] = 0;
                                elen[j] = -1; // node j is dead
                                j = next[j]; // delete j from hash bucket
                                next[jlast] = j;
                            }
                            else
                            {
                                jlast = j; // j and i are different
                                j = next[j];
                            }
                        }
                    }
                }

                // Finalize new element
                for (p = pk1, pk = pk1; pk < pk2; pk++) // finalize Lk
                {
                    int i = Ci[pk];
                    if ((nvi = -nv[i]) <= 0) continue; // skip if i is dead
                    nv[i] = nvi; // restore nv[i]
                    int d = degree[i] + dk - nvi; // compute external degree(i)
                    d = Math.Min(d, n - nel - nvi);
                    if (head[d] != -1) P[head[d]] = i;
                    next[i] = head[d]; // put i back in degree list
                    P[i] = -1;
                    head[d] = i;
                    mindeg = Math.Min(mindeg, d); // find new minimum degree
                    degree[i] = d;
                    Ci[p++] = i; // place i in Lk
                }
                nv[k] = nvk; // # nodes absorbed into k
                if ((W[k] = p - pk1) == 0) // length of adj list of element k
                {
                    Cp[k] = -1; // k is a root of the tree
                    w[k] = 0; // k is now a dead element
                }
                if (elenk != 0) cnz = p; // free unused space in Lk
            }

            // Postordering
            for (int i = 0; i < n; i++) Cp[i] = -(Cp[i] + 2); // fix assembly tree // FLIP(Cp[i])
            for (j = 0; j <= n; j++) head[j] = -1;
            for (j = n; j >= 0; j--) // place unordered nodes in lists
            {
                if (nv[j] > 0) continue; // skip if j is an element
                next[j] = head[Cp[j]]; // place j in list of its parent
                head[Cp[j]] = j;
            }
            for (int e = n; e >= 0; e--) // place elements in lists
            {
                if (nv[e] <= 0) continue; // skip unless e is an element
                if (Cp[e] != -1)
                {
                    next[e] = head[Cp[e]]; // place e in list of its parent
                    head[Cp[e]] = e;
                }
            }
            k = 0;
            for (int i = 0; i <= n; i++) // postorder the assembly tree
            {
                if (Cp[i] == -1)
                    k = TreeDepthFirstSearch(i, k, head, next, P, w);
            }
            return P;
        }

        /// <summary>
        /// Flips the specified i.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns>System.Int32.</returns>
        private static int flip(int i)
        {
            return -i - 2;
        }

        /// <summary>
        /// Clears the specified mark.
        /// </summary>
        /// <param name="mark">The mark.</param>
        /// <param name="lemax">The lemax.</param>
        /// <param name="w">The w.</param>
        /// <param name="n">The n.</param>
        /// <returns>System.Int32.</returns>
        private static int Clear(int mark, int lemax, int[] w, int n)
        {
            if (mark < 2 || (mark + lemax < 0))
            {
                int k;
                for (k = 0; k < n; k++)
                {
                    if (w[k] != 0)
                        w[k] = 1;
                }
                mark = 2;
            }
            return mark; // at this point, w [0..n-1] < mark holds
        }

        // xi [top...n-1] = nodes reachable from graph of G*P' via nodes in B(:,k).
        // xi [n...2n-1] used as workspace
        /// <summary>
        /// Reaches the specified gp.
        /// </summary>
        /// <param name="Gp">The gp.</param>
        /// <param name="Gi">The gi.</param>
        /// <param name="Bp">The bp.</param>
        /// <param name="Bi">The bi.</param>
        /// <param name="n">The n.</param>
        /// <param name="k">The k.</param>
        /// <param name="xi">The xi.</param>
        /// <param name="pinv">The pinv.</param>
        /// <returns>System.Int32.</returns>
        internal static int Reach(int[] Gp, int[] Gi, int[] Bp, int[] Bi, int n, int k, int[] xi, int[] pinv)
        {
            int top = n;
            for (int p = Bp[k]; p < Bp[k + 1]; p++) //if (!CS_MARKED(Gp, Bi[p]))
                if (!(Gp[Bi[p]] < 0)) // start a dfs at unmarked node i
                    top = DepthFirstSearch(Bi[p], Gp, Gi, top, xi, xi, n, pinv);
            for (int p = top; p < n; p++) //CS_MARK(Gp, xi[p]);
                Gp[xi[p]] = -Gp[xi[p]] - 2; // restore G
            return top;
        }

        /// <summary>
        /// Depth-first-search of the graph of a matrix, starting at node j.
        /// </summary>
        /// <param name="j">starting node</param>
        /// <param name="Gp">graph to search (modified, then restored)</param>
        /// <param name="Gi">graph to search</param>
        /// <param name="top">stack[top..n-1] is used on input</param>
        /// <param name="xi">size n, stack containing nodes traversed</param>
        /// <param name="pstack">size n, work array</param>
        /// <param name="offset">the index of the first element in array pstack</param>
        /// <param name="pinv">mapping of rows to columns of G, ignored if null</param>
        /// <returns>new value of top, -1 on error</returns>
        private static int DepthFirstSearch(int j, int[] Gp, int[] Gi, int top, int[] xi,
            int[] pstack, int offset, int[] pinv)
        {
            var head = 0;

            if (xi == null || pstack == null) return -1;

            xi[0] = j; // initialize the recursion stack
            while (head >= 0)
            {
                j = xi[head]; // get j from the top of the recursion stack
                var jnew = pinv != null ? pinv[j] : j;
                if (!(Gp[j] < 0))
                {
                    //CS_MARK(Gp, j);
                    Gp[j] = -Gp[j] - 2; // mark node j as visited
                    pstack[offset + head] = jnew < 0
                        ? 0
                        : // CS_UNFLIP(Gp[jnew]);
                        (Gp[jnew] < 0 ? -Gp[jnew] - 2 : Gp[jnew]);
                }
                var done = true;
                var p2 = jnew < 0
                    ? 0
                    : // CS_UNFLIP(Gp[jnew + 1]);
                    (Gp[jnew + 1] < 0 ? -Gp[jnew + 1] - 2 : Gp[jnew + 1]);

                int p;
                for (p = pstack[offset + head]; p < p2; p++) // examine all neighbors of j
                {
                    var i = Gi[p];
                    if (Gp[i] < 0) continue; // skip visited node i
                    pstack[offset + head] = p; // pause depth-first search of node j
                    xi[++head] = i; // start dfs at node i
                    done = false; // node j is not done
                    break; // break, to start dfs (i)
                }
                if (done) // depth-first search at node j is done
                {
                    head--; // remove j from the recursion stack
                    xi[--top] = j; // and place in the output stack
                }
            }
            return top;
        }


        /// <summary>
        /// Depth-first search and postorder of a tree rooted at node j
        /// </summary>
        /// <param name="j">postorder of a tree rooted at node j</param>
        /// <param name="k">number of nodes ordered so far</param>
        /// <param name="head">head[i] is first child of node i; -1 on output</param>
        /// <param name="next">next[i] is next sibling of i or -1 if none</param>
        /// <param name="post">postordering</param>
        /// <param name="stack">size n, work array</param>
        /// <returns>new value of k, -1 on error</returns>
        private static int TreeDepthFirstSearch(int j, int k, int[] head, int[] next, int[] post, int[] stack)
        {
            var top = 0;
            stack[0] = j; // place j on the stack
            while (top >= 0) // while (stack is not empty)
            {
                var p = stack[top];
                var i = head[p];
                if (i == -1)
                {
                    top--; // p has no unordered children left
                    post[k++] = p; // node p is the kth postordered node
                }
                else
                {
                    head[p] = next[i]; // remove i from children of p
                    top++;
                    stack[top] = i; // start dfs on child node i
                }
            }
            return k;
        }
    }
}