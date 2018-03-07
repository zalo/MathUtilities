// ***********************************************************************
// Assembly         : StarMath
// Author           : MICampbell
// Created          : 05-14-2015
//
// Last Modified By : MICampbell
// Last Modified On : 07-07-2015
// ***********************************************************************
// <copyright file="3D transforms.cs" company="Design Engineering Lab -- MICampbell">
//     2014
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace StarMathLib
{
    public static partial class StarMath
    {
        #region 3D Coordinate Transforms

        /// <summary>
        ///     Creates a translated coordinate frame.
        /// </summary>
        /// <param name="tx">Amount of translation in x.</param>
        /// <param name="ty">Amount of translation in y.</param>
        /// <param name="tz">Amount of translation in z.</param>
        /// <returns>4-by-4 matrix translated by the amount specified.</returns>
        public static double[,] Translate(double tx, double ty, double tz)
        {
            var T = makeIdentity(4);

            T[0, 3] = tx;
            T[1, 3] = ty;
            T[2, 3] = tz;

            return T;
        }

        /// <summary>
        ///     Creates a rotation matrix about the X-axis.
        /// </summary>
        /// <param name="angle">The amount of angle in degrees (positive is counter-clockwise).</param>
        /// <param name="inRadians">if set to <c>true</c> [in radians].</param>
        /// <returns>4-by-4 matrix rotated by the amount specified.</returns>
        public static double[,] RotationX(double angle, bool inRadians = false)
        {
            var rotx = makeIdentity(4);
            if (!inRadians)
                angle = (Math.PI*angle)/180;

            rotx[1, 1] = rotx[2, 2] = Math.Cos(angle);
            rotx[1, 2] = -Math.Sin(angle);
            rotx[2, 1] = Math.Sin(angle);

            return rotx;
        }

        /// <summary>
        ///     Creates a rotation matrix about the Y-axis.
        /// </summary>
        /// <param name="angle">The amount of angle in degrees (positive is counter-clockwise).</param>
        /// <param name="inRadians">if set to <c>true</c> [in radians].</param>
        /// <returns>4-by-4 matrix rotated by the amount specified.</returns>
        public static double[,] RotationY(double angle, bool inRadians = false)
        {
            var roty = makeIdentity(4);
            if (!inRadians)
                angle = (Math.PI*angle)/180;

            roty[0, 0] = roty[2, 2] = Math.Cos(angle);
            roty[2, 0] = -Math.Sin(angle);
            roty[0, 2] = Math.Sin(angle);

            return roty;
        }

        /// <summary>
        ///     Creates a rotation matrix about the Z-axis.
        /// </summary>
        /// <param name="angle">The amount of angle in degrees (positive is counter-clockwise).</param>
        /// <param name="inRadians">if set to <c>true</c> [in radians].</param>
        /// <returns>4-by-4 matrix rotated by the amount specified.</returns>
        public static double[,] RotationZ(double angle, bool inRadians = false)
        {
            var rotz = makeIdentity(4);
            if (!inRadians)
                angle = (Math.PI*angle)/180;

            rotz[0, 0] = rotz[1, 1] = Math.Cos(angle);
            rotz[1, 0] = Math.Sin(angle);
            rotz[0, 1] = -Math.Sin(angle);

            return rotz;
        }

        #endregion
    }
}