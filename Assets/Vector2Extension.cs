using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public static class Vector2Extension
    {
        /// <summary>
        /// Returns a rotated copy of a vector.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="radians"></param>
        /// <returns></returns>
        /// <remarks>Code found here: http://answers.unity3d.com/questions/661383/whats-the-most-efficient-way-to-rotate-a-vector2-o.html </remarks>
        public static Vector2 Rotate(this Vector2 v, float radians)
        {
            double sin = Math.Sin(radians);
            double cos = Math.Cos(radians);
            return new Vector2((float)((cos * v.X) - (sin * v.Y)), (float)((sin * v.X) + (cos * v.Y)));
        }

        /// <summary>
        /// Returns a component-wise multiplication of two vectors.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2 Multiply(this Vector2 v, Vector2 vector)
        {
            return new Vector2(v.X * vector.X, v.Y * vector.Y);
        }
    }
}
