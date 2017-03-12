using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class SurfaceCoord
    {
        public readonly ReadOnlyMesh Mesh;
        public readonly int TriangleIndex;
        public readonly Vector2 Coord;
        public readonly float Rotation;
        public readonly bool FrontSide;

        public SurfaceCoord(
            ReadOnlyMesh mesh, 
            int triangleIndex, 
            Vector2 coord = new Vector2(), 
            float rotation = 0, 
            bool frontSide = true)
        {
            Debug.Assert(triangleIndex >= 0 && mesh.TriangleCount > triangleIndex, "TriangleIndex is out of bounds.");
            Mesh = mesh;
            TriangleIndex = triangleIndex;
            Rotation = rotation;
            Coord = coord;
            FrontSide = frontSide;
        }

        /// <summary>
        /// Return the coord relative to the parent mesh.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLocalCoord()
        {
            return Mesh.TriToMeshCoord(TriangleIndex, Coord);
        }

        /// <summary>
        /// Tests the equality of two SurfaceCoords.  Note that the Mesh field is compared by reference.
        /// </summary>
        /// <param name="coord0"></param>
        /// <param name="coord1"></param>
        /// <returns></returns>
        public static bool Equals(SurfaceCoord coord0, SurfaceCoord coord1)
        {
            return coord0.Mesh == coord1.Mesh &&
                coord0.Coord == coord1.Coord &&
                coord0.TriangleIndex == coord1.TriangleIndex && 
                coord0.Rotation == coord1.Rotation;
        }

        /// <summary>
        /// Tests the equality of two SurfaceCoords.  Note that the Mesh field is compared by reference.
        /// </summary>
        /// <param name="coord0"></param>
        /// <param name="coord1"></param>
        /// <returns></returns>
        public static bool AlmostEquals(SurfaceCoord coord0, SurfaceCoord coord1, float delta)
        {
            return coord0.Mesh == coord1.Mesh &&
                (coord0.Coord - coord1.Coord).Length <= delta &&
                coord0.TriangleIndex == coord1.TriangleIndex &&
                Math.Abs(coord0.Rotation - coord1.Rotation) <= delta;
        }

        SurfaceCoord AdjustCoord()
        {
            var triangle = Mesh.GetSurfaceTriangle(TriangleIndex);
            Vector2 incenter = MathExt.GetTriangleIncenter(triangle);
            float scaleFactor = 0.9999f;
            var triangleScaled = triangle.Select(item => (item - incenter) * scaleFactor + incenter).ToArray();
            if (!MathExt.PointInPolygon(Coord, triangleScaled))
            {
                var nearest = MathExt.PointPolygonNearest(triangleScaled, Coord);

                LineF edge = new LineF(triangleScaled[nearest.EdgeIndex], triangleScaled[(nearest.EdgeIndex + 1) % triangleScaled.Length]);
                Vector2 adjustedCoord = edge.Lerp(nearest.EdgeT);
                return new SurfaceCoord(Mesh, TriangleIndex, adjustedCoord, Rotation, FrontSide);
            }
            return this;
        }

        public SurfaceCoord Move(Vector2 v)
        {
            Debug.Assert(MathExt.PointInPolygon(Coord, Mesh.GetSurfaceTriangle(TriangleIndex)), "Coord should never be outside of triangle when calling this method.");

            if (v.Length == 0)
            {
                return new SurfaceCoord(Mesh, TriangleIndex, Coord, Rotation, FrontSide);
            }

            var surfaceTriangle = Mesh.GetSurfaceTriangle(TriangleIndex);
            IntersectCoord nearest = null;
            int? nearestEdge = null;
            LineF movement = new LineF(Coord, Coord + v);
            for (int i = 0; i < surfaceTriangle.Length; i++)
            {
                int iNext = (i + 1) % surfaceTriangle.Length;
                LineF edge = new LineF(surfaceTriangle[i], surfaceTriangle[iNext]);
                var intersection = MathExt.LineLineIntersect(movement, edge, true);

                if (intersection != null)
                {
                    nearest = intersection;
                    nearestEdge = i;
                    // Since the intersection starts inside the triangle, we know there can be at most one intersection.
                    break;
                }
            }

            if (nearest != null)
            {
                int? triangleIndexNext = Mesh.GetAdjacentTriangle(TriangleIndex, (int)nearestEdge);
                if (triangleIndexNext == null)
                {
                    return new SurfaceCoord(Mesh, TriangleIndex, nearest.Position, Rotation, FrontSide).AdjustCoord();
                }
                else
                {
                    var surfaceTriangleNext = Mesh.GetSurfaceTriangle((int)triangleIndexNext);
                    TriangleEdge edgeIndexNext = Mesh.AdjacentEdge(TriangleIndex, (int)nearestEdge);
                    LineF edge = new LineF(surfaceTriangle[(int)nearestEdge], surfaceTriangle[(int)(nearestEdge + 1) % Constants.SidesOnTriangle]);
                    LineF edgeNext = new LineF(surfaceTriangleNext[edgeIndexNext.StartIndex], surfaceTriangleNext[edgeIndexNext.EndIndex]);

                    float movementLeft = (v - (nearest.Position - Coord)).Length;

                    float angle0 = (float)(Vector2.Angle(edge.Delta, v));
                    float angle1 = (float)Vector2.Angle(edge.Delta, MathExt.VectorFromAngle(Rotation, 1));
                    //float rotationOffset;
                    Vector2 vNext = edgeNext.Delta.Normalized().Rotate(angle0) * movementLeft;
                    Vector2 direction = edgeNext.Delta.Normalized().Rotate(angle1);

                    Vector2 coordNext;
                    bool flipped = (edgeIndexNext.StartIndex + 1) % Constants.SidesOnTriangle != edgeIndexNext.EndIndex;
                    if (flipped)
                    {
                        //rotationOffset = Vector2.Angle(edgeNext.Delta, edge.Delta);
                        //rotationOffset = Vector2.Angle(v, vNext);
                        coordNext = edgeNext.Lerp(nearest.Last);
                    }
                    else
                    {
                        Vector2 normal = edgeNext.Delta;
                        //Vector2 normal = new Vector2(edgeNext.Delta.Y, -edge.Delta.X);
                        vNext = vNext.Mirror(normal);
                        direction = direction.Mirror(normal);

                        //rotationOffset = -Vector2.Angle(edge.Delta, edgeNext.Delta);
                        //rotationOffset = Vector2.Angle(v, vNext);
                        coordNext = edgeNext.Lerp(nearest.Last);
                    }

                    Debug.Assert(
                        (Mesh.TriToMeshCoord((int)triangleIndexNext, coordNext) - Mesh.TriToMeshCoord(TriangleIndex, nearest.Position)).Length < 0.0001f,
                        "There shouldn't be a jump in 3d position when moving between triangle edges.");

                    return new SurfaceCoord(Mesh, (int)triangleIndexNext, coordNext, (float)MathExt.AngleVector(direction), FrontSide ^ flipped)
                        .AdjustCoord()
                        .Move(vNext);
                }
            }
            else
            {
                return new SurfaceCoord(Mesh, TriangleIndex, Coord + v, Rotation, FrontSide).AdjustCoord();
            }
        }

        public SurfaceCoord Rotate(float rotation)
        {
            return new SurfaceCoord(Mesh, TriangleIndex, Coord, Rotation + rotation, FrontSide);
        }
    }
}