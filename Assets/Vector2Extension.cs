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
        /// <param name="degrees"></param>
        /// <returns></returns>
        /// <remarks>Code found here: http://answers.unity3d.com/questions/661383/whats-the-most-efficient-way-to-rotate-a-vector2-o.html </remarks>
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float sin = UnityEngine.Mathf.Sin(degrees * UnityEngine.Mathf.Deg2Rad);
            float cos = UnityEngine.Mathf.Cos(degrees * UnityEngine.Mathf.Deg2Rad);

            float tx = v.X;
            float ty = v.Y;
            v.X = (cos * tx) - (sin * ty);
            v.Y = (sin * tx) + (cos * ty);
            return v;
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
