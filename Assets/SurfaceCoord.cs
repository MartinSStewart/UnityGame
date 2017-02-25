using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class SurfaceCoord
    {
        public readonly SimpleMesh Mesh;
        public readonly int TriangleIndex;
        public readonly Vector2 Coord;
        public readonly float Rotation;
        public readonly bool FrontSide;

        const int SidesOnTriangle = 3;

        public SurfaceCoord(SimpleMesh mesh, int triangleIndex, Vector2 coord = new Vector2(), float rotation = 0, bool frontSide = true)
        {
            Mesh = mesh;
            TriangleIndex = triangleIndex;
            Coord = coord;
            FrontSide = frontSide;
            Rotation = rotation;
        }

        public Vector3[] GetTriangle()
        {
            int index = TriangleIndex * SidesOnTriangle;
            return new[]
            {
                Mesh.Vertices[Mesh.Triangles[index]],
                Mesh.Vertices[Mesh.Triangles[index + 1]],
                Mesh.Vertices[Mesh.Triangles[index + 2]]
            };
        }

        public int[] GetTriangleIndices()
        {
            int index = TriangleIndex * SidesOnTriangle;
            return new[]
            {
                Mesh.Triangles[index],
                Mesh.Triangles[index + 1],
                Mesh.Triangles[index + 2]
            };
        }

        /// <summary>
        /// Get triangle relative to surface coordinates.
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetSurfaceTriangle()
        {
            Vector3[] triangle = GetTriangle();
            Vector3 origin = GetLocalOrigin();
            Vector3[] axis = GetXYAxis();

            Vector2[] surfaceTriangle = new Vector2[SidesOnTriangle];
            for (int i = 0; i < triangle.Length; i++)
            {
                Vector3 v = triangle[i] - origin;
                surfaceTriangle[i] = new Vector2(Vector3.Dot(axis[0], v), Vector3.Dot(axis[1], v));
            }
            return surfaceTriangle;
        }

        /// <summary>
        /// Get the origin of the surface relative to the parent mesh.
        /// </summary>
        /// <returns></returns>
        Vector3 GetLocalOrigin()
        {
            return GetTriangle()[0];
        }

        /// <summary>
        /// Get the xy axis of the surface relative the parent mesh.
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetXYAxis()
        {
            Vector3[] triangle = GetTriangle();

            Vector3 yAxis = (triangle[1] - triangle[0]).normalized;

            Vector3 xAxis = triangle[2] - triangle[0];
            //Adjust the xAxis so that it is orthogonal to the yAxis.
            Vector3 projection = Vector3.Dot(yAxis, xAxis) * yAxis;
            xAxis = (xAxis - projection).normalized;

            return new[] { xAxis, yAxis };
        }

        /// <summary>
        /// Return the coord relative to the parent mesh.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLocalCoord()
        {
            return GetLocalCoord(Coord);
        }

        /// <summary>
        /// Return the coord relative to the parent mesh.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLocalCoord(Vector2 local)
        {
            var axis = GetXYAxis();

            return GetLocalOrigin() + axis[0] * local.x + axis[1] * local.y;
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
                coord0.TriangleIndex == coord1.TriangleIndex;// && 
                //coord0.Rotation == coord1.Rotation;
        }

        public SurfaceCoord Move(Vector2 v)
        {
            if (!MathExt.PointInPolygon(Coord, GetSurfaceTriangle()))
            {
                
            }
            return _move(v);
        }

        SurfaceCoord _move(Vector2 v)
        {
            if (v.magnitude == 0)
            {
                return new SurfaceCoord(Mesh, TriangleIndex, Coord, Rotation, FrontSide);
            }
            var surfaceTriangle = GetSurfaceTriangle();

            IntersectCoord nearest = null;
            int? nearestEdge = null;
            LineF movement = new LineF(Coord, Coord + v);
            for (int i = 0; i < surfaceTriangle.Length; i++)
            {
                int iNext = (i + 1) % surfaceTriangle.Length;
                LineF edge = new LineF(surfaceTriangle[i], surfaceTriangle[iNext]);
                var intersection = MathExt.LineLineIntersect(movement, edge, true);
                
                if (intersection != null && intersection.First > 0.001)
                {
                    nearest = intersection;
                    nearestEdge = i;
                    // Since the intersection starts inside the triangle, we know there can be at most one intersection.
                    break;
                }
            }

            if (nearest != null)
            {
                int? triangleIndexNext = GetAdjacentTriangle((int)nearestEdge);
                if (triangleIndexNext == null)
                {
                    return new SurfaceCoord(Mesh, TriangleIndex, nearest.Position, Rotation);
                }
                else
                {
                    var triangle = GetTriangle();
                    LineF commonEdge = new LineF(triangle[(int)nearestEdge], triangle[((int)nearestEdge + 1) % SidesOnTriangle]);
                    var surfaceCoord = new SurfaceCoord(Mesh, (int)triangleIndexNext)
                        .GetSurfaceCoord(commonEdge.Lerp(nearest.Last));
                    var coord = new SurfaceCoord(
                        Mesh,
                        (int)triangleIndexNext,
                        surfaceCoord, 
                        Rotation);

                    var surfaceTriangleNext = new SurfaceCoord(Mesh, (int)triangleIndexNext).GetSurfaceTriangle();
                    EdgeIndex edgeIndexNext = GetAdjacentTriangleEdgeIndex((int)nearestEdge);
                    LineF edge = new LineF(surfaceTriangle[(int)nearestEdge], surfaceTriangle[(int)(nearestEdge + 1) % SidesOnTriangle]);
                    LineF edgeNext = new LineF(surfaceTriangleNext[edgeIndexNext.Start], surfaceTriangleNext[edgeIndexNext.End]);

                    float movementLeft = (v - (nearest.Position - Coord)).magnitude;
                    
                    float angle0 = Vector2.Angle(edge.Delta, v);
                    float rotationOffset = Vector2.Angle(edge.Delta, edgeNext.Delta);
                    Vector2 vNext = edgeNext.Delta.normalized.Rotate(angle0) * movementLeft;
                    //if (MathExt.IsClockwise(surfaceTriangleNext) != MathExt.IsClockwise(surfaceTriangle))
                    if ((edgeIndexNext.Start + 1) % SidesOnTriangle == edgeIndexNext.End)
                    {
                        Vector2 normal = edgeNext.Delta; //new Vector2(-edgeNext.Delta.y, edgeNext.Delta.x);
                        vNext = vNext.Mirror(normal);
                    }
                    
                    var coordNext = new SurfaceCoord(coord.Mesh, coord.TriangleIndex, coord.Coord, coord.Rotation + rotationOffset);
                    return coordNext._move(vNext);
                }
            }
            else
            {
                return new SurfaceCoord(Mesh, TriangleIndex, Coord + v, Rotation);
            }
        }

        public Vector2 GetSurfaceCoord(Vector3 coord)
        {
            var axis = GetXYAxis();
            Vector3 v = coord - GetLocalOrigin();
            return new Vector2(Vector3.Dot(axis[0], v), Vector3.Dot(axis[1], v));
        }

        EdgeIndex GetAdjacentTriangleEdgeIndex(int edgeIndex)
        {
            var triangle = GetTriangle();
            int? adjacentTriangleIndex = GetAdjacentTriangle(edgeIndex);
            if (adjacentTriangleIndex == null)
            {
                return null;
            }
            var adjacentTriangle = new SurfaceCoord(Mesh, (int)adjacentTriangleIndex).GetTriangle();

            for (int i = 0; i < SidesOnTriangle; i++)
            {
                for (int j = 0; j < SidesOnTriangle; j++)
                {
                    if (triangle[i] == adjacentTriangle[j])
                    {
                        int iNext = (i + 1) % SidesOnTriangle;
                        int jNext = (j + 1) % SidesOnTriangle;
                        int jPrev = (j + SidesOnTriangle - 1) % SidesOnTriangle;
                        if (triangle[iNext] == adjacentTriangle[jNext])
                        {
                            return new EdgeIndex(j, jNext);
                        }
                        else if (triangle[iNext] == adjacentTriangle[jPrev])
                        {
                            return new EdgeIndex(j, jPrev);
                        }
                        else
                        {
                            Debug.Assert(true, "Execution should not have reached this point.");
                            return null;
                        }
                    }
                }
            }
            Debug.Assert(true, "Execution should not have reached this point.");
            return null;
        }

        class EdgeIndex
        {
            public readonly int Start, End;

            public EdgeIndex(int startIndex, int endIndex)
            {
                Debug.Assert(startIndex < SidesOnTriangle && startIndex >= 0);
                Debug.Assert(endIndex < SidesOnTriangle && endIndex >= 0);
                Start = startIndex;
                End = endIndex;
            }
        }

        int? GetAdjacentTriangle(int edgeIndex)
        {
            Debug.Assert(edgeIndex >= 0 && edgeIndex < SidesOnTriangle);

            int index = TriangleIndex * SidesOnTriangle;
            int vertexIndice0 = Mesh.Triangles[index + edgeIndex];
            int vertexIndice1 = Mesh.Triangles[index + (edgeIndex + 1) % SidesOnTriangle];

            for (int i = 0; i < Mesh.Triangles.Length / SidesOnTriangle; i++)
            {
                if (i == TriangleIndex)
                {
                    continue;
                }
                int i1 = i * 3;
                if (Mesh.Triangles[i1] == vertexIndice0 || 
                    Mesh.Triangles[i1 + 1] == vertexIndice0 || 
                    Mesh.Triangles[i1 + 2] == vertexIndice0)
                {
                    if (Mesh.Triangles[i1] == vertexIndice1 ||
                        Mesh.Triangles[i1 + 1] == vertexIndice1 ||
                        Mesh.Triangles[i1 + 2] == vertexIndice1)
                    {
                        return i;
                    }
                }
            }
            return null;
        }
    }
}