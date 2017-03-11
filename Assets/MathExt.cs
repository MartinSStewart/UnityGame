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

        /// <summary>
        /// Returns true if vertices are ordered clockwise, false they are counter-clockwise.  It is assumed that the polygon they form is simple.
        /// </summary>
        /// <param name="polygon"></param>
        public static bool IsClockwise(IList<Vector2> polygon)
        {
            Debug.Assert(polygon.Count >= 3);
            double signedArea = 0;
            for (int i = 0; i < polygon.Count; i++)
            {
                int iNext = (i + 1) % polygon.Count;
                signedArea += (polygon[i].x * polygon[iNext].y - polygon[iNext].x * polygon[i].y);
            }
            Debug.Assert(signedArea != 0, "Polygon has 0 area.");
            return Math.Sign(signedArea) == -1;
        }

        /// <summary>
        /// Get projection of this vector onto v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        public static Vector2 Project(this Vector2 v0, Vector2 v1)
        {
            var normal = v1.normalized;
            return normal * Vector2.Dot(v0, normal);
        }

        /// <summary>
        /// Reflects this vector across a normal.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector2 Mirror(this Vector2 v, Vector2 normal)
        {
            return v - 2 * (v - Project(v, normal));
        }

        /// <summary>
        /// Mods value without ending up with negative values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static double ValueWrap(double value, double mod)
        {
            value = value % mod;
            if (value < 0)
            {
                return mod + value;
            }
            return value;
        }

        /// <summary>
        /// Returns a copy of the polygon with the new winding order set.
        /// </summary>
        /// <param name="polygon">A polygon represented as a list of vectors.</param>
        /// <param name="clockwise">Clockwise if true, C.Clockwise if false.</param>
        /// <returns></returns>
        public static Vector2[] SetWinding(Vector2[] polygon, bool clockwise)
        {
            if (IsClockwise(polygon) != clockwise)
            {
                return polygon.Reverse().ToArray();
            }
            return polygon;
        }

        /// <summary>
        /// Get incenter of a given triangle. The incenter defined as the intersection point of 3 edges that are angle bisectors of the triangle vertices.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        public static Vector2 GetTriangleIncenter(Vector2[] triangle)
        {
            Debug.Assert(triangle.Length == 3);
            var edgeLength = new[]
            {
                (triangle[2] - triangle[1]).magnitude,
                (triangle[0] - triangle[2]).magnitude,
                (triangle[1] - triangle[0]).magnitude
            };
            return (edgeLength[0] * triangle[0] + edgeLength[1] * triangle[1] + edgeLength[2] * triangle[2]) / edgeLength.Sum();
        }

        /// <summary>
        /// Find the nearest PolygonCoord on the polygon relative to provided point.
        /// </summary>
        public static PolygonCoord PointPolygonNearest(IList<Vector2> polygon, Vector2 point)
        {
            var nearest = new PolygonCoord(0, 0);
            double distanceMin = -1;
            for (int i = 0; i < polygon.Count; i++)
            {
                int iNext = (i + 1) % polygon.Count;
                var edge = new LineF(polygon[i], polygon[iNext]);
                double distance = PointLineDistance(point, edge, true);
                if (distanceMin == -1 || distance < distanceMin)
                {
                    nearest = new PolygonCoord(i, edge.NearestT(point, true));
                    distanceMin = distance;
                }
            }
            return nearest;
        }

        public static double PointLineDistance(Vector2 point, LineF line, bool isSegment)
        {
            Vector2 v;
            Vector2 vDelta = line[1] - line[0];
            if (vDelta.x == 0 && vDelta.y == 0)
            {
                v = line[0];
            }
            else
            {
                double t = ((point.x - line[0].x) * vDelta.x + (point.y - line[0].y) * vDelta.y) / (Math.Pow(vDelta.x, 2) + Math.Pow(vDelta.y, 2));
                Debug.Assert(double.IsNaN(t) == false);
                if (isSegment) { t = Math.Min(Math.Max(t, 0), 1); }
                v = line[0] + vDelta * (float)t;
            }
            double distance = (point - v).magnitude;
            Debug.Assert(distance >= 0);
            return distance;
        }

        /// <summary>
        /// Instantiates a vector using a direction (in degrees) and a length.
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Vector2 VectorFromAngle(double degrees, double length)
        {
            double radians = degrees * Mathf.Deg2Rad;
            return new Vector2((float)(-Math.Cos(radians) * length), (float)(Math.Sin(radians) * length));
        }

        public static double AngleVector(Vector2 v0)
        {
            Debug.Assert(v0 != Vector2.zero, "Vector must have non-zero length.");
            double val = Math.Atan2(v0.x, v0.y);

            if (double.IsNaN(val))
            {
                return 0;
            }
            double radians = (val + 2 * Math.PI) % (2 * Math.PI) - Math.PI / 2;
            return radians * Mathf.Rad2Deg;
        }

        public static Vector2[] IntersectionTwoCircles(Vector2 center0, double radius0, Vector2 center1, double radius1)
        {
            /* error handling is missing complettely - left as an exercise 

                  A1
                 /| \
             r1 / |  \ r2
               /  |   \
              /   |h   \
             /g1  |     \          (g1 means angle gamma1)
            C1----P-----C2
               d1   d2
            */
            double dx = center0.x - center1.x;
            double dy = center0.y - center1.y;
            double d = Math.Sqrt(dx * dx + dy * dy); // d = |C1-C2|
            double gamma1 = Math.Acos((radius1 * radius1 + d * d - radius0 * radius0) / (2 * radius1 * d)); // law of cosines
            double d1 = radius0 * Math.Cos(gamma1); // basic math in right triangle
            double h = radius0 * Math.Sin(gamma1);
            double px = center0.x + (center1.x - center0.x) / d * d1;
            double py = center0.y + (center1.y - center0.y) / d * d1;
            // (-dy, dx)/d is (C2-C1) normalized and rotated by 90 degrees

            return new[]
            {
                new Vector2((float)(px + (-dy) / d * h), (float)(py + (+dx) / d * h)),
                new Vector2((float)(px - (-dy) / d * h), (float)(py - (+dx) / d * h))
            };
        }
    }
}
