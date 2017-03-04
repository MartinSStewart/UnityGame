using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    /// <summary>
    /// Custom mesh implementation used in place of UnityEngine.Mesh to allow for VS unit testing. 
    /// Also supports fast lookup of adjacent triangles.
    /// </summary>
    public class SimpleMesh
    {
        /// <summary>
        /// Indices pointing into the _vertices array. The indices are stored as [triangle index, edge index].
        /// </summary>
        readonly int[,] _triangles;
        readonly Vector3[] _vertices;
        /// <summary>
        /// Lookup table for finding neighboring triangles. The data is stored as [triangle index, edge index].
        /// </summary>
        readonly int?[,] _adjacentTriangles;

        public SimpleMesh(Mesh mesh)
            : this(mesh.vertices, mesh.triangles)
        {
        }

        public SimpleMesh(Vector3[] vertices, int[] triangles)
        {
            Debug.Assert(triangles.Length % Constants.SidesOnTriangle == 0);

            _triangles = new int[triangles.Length / Constants.SidesOnTriangle, Constants.SidesOnTriangle];
            _adjacentTriangles = new int?[_triangles.GetLength(0), Constants.SidesOnTriangle];
            _vertices = vertices.ToArray();

            for (int i = 0; i < _triangles.GetLength(0); i++)
            {
                int index = i * Constants.SidesOnTriangle;
                _triangles[i, 0] = triangles[index];
                _triangles[i, 1] = triangles[index + 1];
                _triangles[i, 2] = triangles[index + 2];
            }
            
            UpdateAdjacentTriangles();
        }

        public void Translate(Vector3 v)
        {
            for (int i = 0; i < _vertices.Length; i++)
            {
                _vertices[i] += v;
            }
        }

        public void Scale(float scale)
        {
            for (int i = 0; i < _vertices.Length; i++)
            {
                _vertices[i] *= scale;
            }
        }

        public Vector3[] GetTriangle(int triangleIndex)
        {
            return new[]
            {
                _vertices[_triangles[triangleIndex, 0]],
                _vertices[_triangles[triangleIndex, 1]],
                _vertices[_triangles[triangleIndex, 2]]
            };
        }

        public void UpdateAdjacentTriangles()
        {
            for (int i = 0; i < _adjacentTriangles.GetLength(0); i++)
            {
                for (int j = 0; j < _adjacentTriangles.GetLength(1); j++)
                {
                    if (_adjacentTriangles[i,j] != null)
                    {
                        continue;
                    }
                    int? triangleIndex = CalculateAdjacentTriangle(i, j);
                    if (triangleIndex != null)
                    {
                        Debug.Assert(triangleIndex > i);

                        _adjacentTriangles[i, j] = triangleIndex;
                        var edge = GetAdjacentEdge(i, j);
                        Debug.Assert(edge != null);
                        _adjacentTriangles[(int)triangleIndex, edge.GetLeadingIndex()] = i;
                    }
                }
            }
        }

        public int? GetAdjacentTriangle(int triangleIndex, int edgeIndex)
        {
            return _adjacentTriangles[triangleIndex, edgeIndex];
        }

        int? CalculateAdjacentTriangle(int triangleIndex, int edgeIndex)
        {
            Debug.Assert(edgeIndex >= 0 && edgeIndex < Constants.SidesOnTriangle);

            int vertexIndice0 = _triangles[triangleIndex, edgeIndex];
            int vertexIndice1 = _triangles[triangleIndex, (edgeIndex + 1) % Constants.SidesOnTriangle];

            for (int i = 0; i < _triangles.Length / Constants.SidesOnTriangle; i++)
            {
                if (i == triangleIndex)
                {
                    continue;
                }
                if (_triangles[i, 0] == vertexIndice0 ||
                    _triangles[i, 1] == vertexIndice0 ||
                    _triangles[i, 2] == vertexIndice0)
                {
                    if (_triangles[i, 0] == vertexIndice1 ||
                        _triangles[i, 1] == vertexIndice1 ||
                        _triangles[i, 2] == vertexIndice1)
                    {
                        return i;
                    }
                }
            }
            return null;
        }

        public TriangleEdge GetAdjacentEdge(int triangleIndex, int edgeIndex)
        {
            int? temp = GetAdjacentTriangle(triangleIndex, edgeIndex);
            if (temp == null)
            {
                return null;
            }
            int adjacentTriangleIndex = (int)temp;

            for (int i = 0; i < Constants.SidesOnTriangle; i++)
            {
                for (int j = 0; j < Constants.SidesOnTriangle; j++)
                {
                    if (_triangles[triangleIndex, i] == _triangles[adjacentTriangleIndex, j])
                    {
                        int iNext = (i + 1) % Constants.SidesOnTriangle;
                        int jNext = (j + 1) % Constants.SidesOnTriangle;
                        int jPrev = (j + Constants.SidesOnTriangle - 1) % Constants.SidesOnTriangle;
                        if (_triangles[triangleIndex, iNext] == _triangles[adjacentTriangleIndex, jNext])
                        {
                            return new TriangleEdge(j, jNext);
                        }
                        else if (_triangles[triangleIndex, iNext] == _triangles[adjacentTriangleIndex, jPrev])
                        {
                            return new TriangleEdge(j, jPrev);
                        }
                    }
                }
            }
            Debug.Assert(true, "Execution should not have reached this point.");
            return null;
        }

        public Vector2 TriangleSurfaceCoord(int triangleIndex, Vector3 coord)
        {
            var axis = TriangleXYAxis(triangleIndex);
            Vector3 v = coord - TriangleLocalOrigin(triangleIndex);
            return new Vector2(Vector3.Dot(axis[0], v), Vector3.Dot(axis[1], v));
        }

        /// <summary>
        /// Get the origin of the surface relative to the parent mesh.
        /// </summary>
        /// <returns></returns>
        public Vector3 TriangleLocalOrigin(int triangleIndex)
        {
            return GetTriangle(triangleIndex)[0];
        }

        /// <summary>
        /// Get the xy axis of the surface relative the parent mesh.
        /// </summary>
        /// <returns></returns>
        public Vector3[] TriangleXYAxis(int triangleIndex)
        {
            Vector3[] triangle = GetTriangle(triangleIndex);

            Vector3 yAxis = (triangle[1] - triangle[0]).normalized;

            Vector3 xAxis = triangle[2] - triangle[0];
            //Adjust the xAxis so that it is orthogonal to the yAxis.
            Vector3 projection = Vector3.Dot(yAxis, xAxis) * yAxis;
            xAxis = (xAxis - projection).normalized;

            return new[] { xAxis, yAxis };
        }

        /// <summary>
        /// Get triangle relative to surface coordinates.
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetSurfaceTriangle(int triangleIndex)
        {
            Vector3[] triangle = GetTriangle(triangleIndex);
            Vector3 origin = TriangleLocalOrigin(triangleIndex);
            Vector3[] axis = TriangleXYAxis(triangleIndex);

            Vector2[] surfaceTriangle = new Vector2[Constants.SidesOnTriangle];
            for (int i = 0; i < triangle.Length; i++)
            {
                Vector3 v = triangle[i] - origin;
                surfaceTriangle[i] = new Vector2(Vector3.Dot(axis[0], v), Vector3.Dot(axis[1], v));
            }
            return surfaceTriangle;
        }
    }
}
