// ***********************************************************************
// Assembly         : StarMath
// Author           : MICampbell
// Created          : 05-14-2015
//
// Last Modified By : MICampbell
// Last Modified On : 07-07-2015
// ***********************************************************************
// <copyright file="print functions.cs" company="Design Engineering Lab -- MICampbell">
//     2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace StarMathLib
{
    public static partial class StarMath
    {
        /// <summary>
        /// Makes the print string.
        /// </summary>
        /// <param name="A">The matrix, A.</param>
        /// <returns>System.String.</returns>
        public static string MakePrintString(this double[,] A)
        {
            if (A == null) return "<null>";
            var format = "{0:F" + PrintNumDecimals + "}";
            var p = "";
            for (var i = 0; i < A.GetLength(0); i++)
            {
                p += "| ";
                for (var j = 0; j < A.GetLength(1); j++)
                    p += formatCell(format, A[i, j]) + ",";
                p = p.Remove(p.Length - 1);
                p += " |\n";
            }
            p = p.Remove(p.Length - 1);
            return p;
        }

        /// <summary>
        /// Makes the print string.
        /// </summary>
        /// <param name="A">The A.</param>
        /// <returns>System.String.</returns>
        public static string MakePrintString(this IEnumerable<double> A)
        {
            if (A == null) return "<null>";
            var format = "{0:F" + PrintNumDecimals + "}";
            var p = "{ ";
            foreach (var d in A)
                p += formatCell(format, d) + ",";
            p = p.Remove(p.Length - 1);
            p += " }";
            return p;
        }

        /// <summary>
        /// Makes the print string.
        /// </summary>
        /// <param name="A">The A.</param>
        /// <returns>System.String.</returns>
        public static string MakePrintString(this int[,] A)
        {
            if (A == null) return "<null>";
            const string format = "{0:F}";
            var p = "";
            for (var i = 0; i < A.GetLength(0); i++)
            {
                p += "| ";
                for (var j = 0; j < A.GetLength(1); j++)
                    p += formatCell(format, A[i, j]) + ",";
                p = p.Remove(p.Length - 1);
                p += " |\n";
            }
            p = p.Remove(p.Length - 1);
            return p;
        }

        /// <summary>
        /// Makes the print string.
        /// </summary>
        /// <param name="A">The A.</param>
        /// <returns>System.String.</returns>
        public static string MakePrintString(this IEnumerable<int> A)
        {
            if (A == null) return "<null>";
            var format = "{0:F}";
            var p = "{ ";
            foreach (var d in A)
                p += formatCell(format, d) + ",";
            p = p.Remove(p.Length - 1);
            p += " }";
            return p;
        }

        /// <summary>
        /// Formats the cell.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="p">The p.</param>
        /// <returns>System.Object.</returns>
        private static object formatCell(string format, double p)
        {
            var numStr = string.Format(format, p);
            numStr = numStr.TrimEnd('0');
            numStr = numStr.TrimEnd('.');
            var padAmt = ((double) (PrintCellWidth - numStr.Length))/2;
            numStr = numStr.PadLeft((int) Math.Floor(padAmt + numStr.Length));
            numStr = numStr.PadRight(PrintCellWidth);
            return numStr;
        }
    }
}