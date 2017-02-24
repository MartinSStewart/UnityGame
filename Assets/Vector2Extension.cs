using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }
}
