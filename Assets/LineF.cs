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
        public float Length { get { return Delta.Length; } }
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

        public LineF(IList<Vector2> line)
        {
            Debug.Assert(line.Count == 2);
            _vertices[0] = line[0];
            _vertices[1] = line[1];
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
            double p = (this[1].X - this[0].X) * (point.Y - this[0].Y) - (this[1].Y - this[0].Y) * (point.X - this[0].X);
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

        /// <summary>
        /// Returns the T value of the nearest point on this line to a vector.
        /// </summary>
        /// <returns></returns>
        public float NearestT(Vector2 v, bool isSegment)
        {
            Vector2 vDelta = _vertices[1] - _vertices[0];
            double t = ((v.X - _vertices[0].X) * vDelta.X + (v.Y - _vertices[0].Y) * vDelta.Y) / (Math.Pow(vDelta.X, 2) + Math.Pow(vDelta.Y, 2));
            if (isSegment)
            {
                t = Math.Min(Math.Max(t, 0), 1);
            }
            return (float)t;
        }
    }
}
