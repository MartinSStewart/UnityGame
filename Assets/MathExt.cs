using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class IntersectCoord
    {
        public Vector2 Position;
        /// <summary>T value for the first line.</summary>
        public double First;
        /// <summary>T value for the second line.</summary>
        public double Last;

        const float EqualityEpsilon = 0.0000001f;
    }

    public static class MathExt
    {
        

        /// <summary>Tests if two lines intersect.</summary>
        /// <returns>Location where the two lines intersect. TFirst is relative to the first line.</returns>
        public static IntersectCoord LineLineIntersect(LineF line0, LineF line1, bool segmentOnly)
        {

            double ua, ub;
            double ud = (line1[1].y - line1[0].y) * (line0[1].x - line0[0].x) - (line1[1].x - line1[0].x) * (line0[1].y - line0[0].y);
            if (ud != 0)
            {
                ua = ((line1[1].x - line1[0].x) * (line0[0].y - line1[0].y) - (line1[1].y - line1[0].y) * (line0[0].x - line1[0].x)) / ud;
                ub = ((line0[1].x - line0[0].x) * (line0[0].y - line1[0].y) - (line0[1].y - line0[0].y) * (line0[0].x - line1[0].x)) / ud;
                if (segmentOnly)
                {
                    if (ua < 0 || ua > 1 || ub < 0 || ub > 1)
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
            return new IntersectCoord
            {
                Position = Lerp(new Vector2(line0[0].x, line0[0].y), new Vector2(line0[1].x, line0[1].y), ua),
                First = ua,
                Last = ub
            };
        }

        public static double Lerp(double value0, double value1, double T)
        {
            return value0 * (1 - T) + value1 * T;
        }

        public static Vector2 Lerp(Vector2 vector0, Vector2 vector1, double T)
        {
            return vector0 * (float)(1 - T) + vector1 * (float)T;
        }

        public static Vector3 Lerp(Vector3 vector0, Vector3 vector1, double T)
        {
            return vector0 * (float)(1 - T) + vector1 * (float)T;
        }

        /// <summary>
        /// Tests if a point is contained in a polygon.
        /// </summary>
        /// <remarks>
        /// Code was found here http://dominoc925.blogspot.com/2012/02/c-code-snippet-to-determine-if-point-is.html
        /// </remarks>
        public static bool PointInPolygon(Vector2 point, IList<Vector2> polygon)
        {
            Debug.Assert(polygon != null);
            Debug.Assert(polygon.Count >= 2);
            bool isInside = false;
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }
    }
}
