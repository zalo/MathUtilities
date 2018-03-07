// ***********************************************************************
// Assembly         : StarMath
// Author           : MICampbell
// Created          : 05-14-2015
//
// Last Modified By : MICampbell
// Last Modified On : 07-07-2015
// ***********************************************************************
// <copyright file="constants.cs" company="Design Engineering Lab -- MICampbell">
//     2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
namespace StarMathLib
{
    /// <summary>
    /// The main class in the StarMathLib. All functions are static
    /// functions located here.
    /// </summary>
    public static partial class StarMath
    {
        /// <summary>
        ///     The cell width
        /// </summary>
        public static int PrintCellWidth { get; set; } = 10;

        /// <summary>
        ///     The number decimals
        /// </summary>
        internal static int PrintNumDecimals { get; set; } = 3;

        /// <summary>
        ///     The maximum error for using gauss seidel
        /// </summary>
        internal static double MaxErrorForUsingGaussSeidel { get; set; } = 3.0;

        /// <summary>
        ///     The gauss seidel maximum error
        /// </summary>
        internal static double GaussSeidelMaxError { get; set; } = 1e-9;

        /// <summary>
        ///     The gauss seidel diagonal dominance ratio
        /// </summary>
        internal static double GaussSeidelDiagonalDominanceRatio { get; set; } = 0.3;

        /* in some simple studies, GaussSeidel failed when ALL diagonals were lower than 0.3 (and higher than -0.5)
         * so it may seem imprudent to set the diagonal dominance ratio so high. But this is only to throw out
         * cases in which ANY of the diagonals are lower than this value. */

        /// <summary>
        ///     The gauss seidel minimum matrix size
        /// </summary>
        internal static int GaussSeidelMinimumMatrixSize { get; set; } = 200;

        /// <summary>
        ///     The gauss seidel maximum iteration factor
        /// </summary>
        internal static int GaussSeidelMaxIterationFactor { get; set; } = 1;

        /// <summary>
        ///     The gauss seidel relaxation omega
        /// </summary>
        internal static double GaussSeidelRelaxationOmega { get; set; } = 1.25;

        /// <summary>
        ///     The maximum sv diter
        /// </summary>
        internal static int MaxSvDiter { get; set; } = 1000;

        /// <summary>
        ///     The starting carol seed
        /// </summary>
        internal static int StartingCarolSeed { get; set; } = 3;

        /// <summary>
        ///     Gets or sets the  tolerance for the equality functions: IsPracticallySame, IsNegligible, IsGreaterThanNonNegligible
        ///     IsLessThanNonNegligible.
        /// </summary>
        /// <value>The equality tolerance.</value>
        public static double EqualityTolerance { get; set; } = DefaultEqualityTolerance;
        private const double DefaultEqualityTolerance  = 1e-15;
        /// <summary>
        /// Determines whether [is practically same] [the specified x].
        /// the norm is within 1e-15
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="optionalTolerance">An optional tolerance.</param>
        /// <returns><c>true</c> if [is practically same] [the specified x]; otherwise, <c>false</c>.</returns>
        public static bool IsPracticallySame(this double x, double y, double optionalTolerance = DefaultEqualityTolerance)
        {
            return IsNegligible(x - y, optionalTolerance);
        }

        /// <summary>
        /// Determines whether [is practically same] [the specified x].
        /// the norm is within 1e-15
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="optionalTolerance">An optional tolerance.</param>
        /// <returns><c>true</c> if [is practically same] [the specified x]; otherwise, <c>false</c>.</returns>
        public static bool IsPracticallySame(this double[] x, double[] y, double optionalTolerance = double.NaN)
        {
            if (double.IsNaN(optionalTolerance)) optionalTolerance = EqualityTolerance;
            var n = x.GetLength(0);
            if (n != y.GetLength(0)) return false;
            return IsNegligible(x.subtract(y), optionalTolerance);
        }

        /// <summary>
        /// Determines whether the specified x is negligible (|x| lte 1e-15).
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="optionalTolerance">An optional tolerance.</param>
        /// <returns><c>true</c> if the specified x is negligible; otherwise, <c>false</c>.</returns>
        public static bool IsNegligible(this double[] x, double optionalTolerance = DefaultEqualityTolerance)
        {
            return (x.norm2(true) <= optionalTolerance);
        }

        /// <summary>
        /// Determines whether the specified x is negligible (|x| lte 1e-15).
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="optionalTolerance">An optional tolerance.</param>
        /// <returns><c>true</c> if the specified x is negligible; otherwise, <c>false</c>.</returns>
        public static bool IsNegligible(this double x, double optionalTolerance = DefaultEqualityTolerance)
        {
            return (Math.Abs(x) <= optionalTolerance);
        }

        /// <summary>
        ///     Determines whether [is greater than] [the specified y] and not practically the same.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns><c>true</c> if [is greater than non negligible] [the specified y]; otherwise, <c>false</c>.</returns>
        public static bool IsGreaterThanNonNegligible(this double x, double y = 0)
        {
            return (x > y && !IsPracticallySame(x, y));
        }

        /// <summary>
        ///     Determines whether [is less than] [the specified y] and not practically the same.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns><c>true</c> if [is less than non negligible] [the specified y]; otherwise, <c>false</c>.</returns>
        public static bool IsLessThanNonNegligible(this double x, double y = 0)
        {
            return (x < y && !IsPracticallySame(x, y));
        }
    }
}