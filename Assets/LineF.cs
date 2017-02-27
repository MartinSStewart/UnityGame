using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    [DebuggerDisplay("LineF {this[0]}, {this[1]}")]
    public class LineF
    {
        public float Length { get { return Delta.magnitude; } }
        readonly Vector2[] _vertices = new Vector2[2];
        public Vector2 Delta { get { return (this[1] - this[0]); } }

        public Vector2 this[int index]
        {
            get { return _vertices[index]; }
            set { _vertices[index] = value;
            }
        }

        public LineF(Vector2 start, Vector2 end)
        {
            _vertices[0] = start;
            _vertices[1] = end;
        }

        public Vector2 Lerp(double t)
        {
            return MathExt.Lerp(_vertices[0], _vertices[1], t);
        }

        /// <summary>
        /// Returns whether a point is left or right of this line.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="ignoreEdgeCase">Whether or not to treat points exactly on the line as to the right of it instead.</param>
        public Side GetSideOf(Vector2 point, bool ignoreEdgeCase = true)
        {
            double p = (this[1].x - this[0].x) * (point.y - this[0].y) - (this[1].y - this[0].y) * (point.x - this[0].x);
            if (p > 0)
            {
                return Side.Left;
            }
            if (p == 0 && !ignoreEdgeCase)
            {
                return Side.Neither;
            }
            return Side.Right;
        }
    }
}
