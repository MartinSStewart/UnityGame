using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class SimpleMesh
    {
        /// <summary>
        /// Indices pointing into the _vertices array.  The indices are stored as [triangle index, edge index].
        /// </summary>
        readonly int[,] _triangles;
        readonly Vector3[] _vertices;
        /// <summary>
        /// Lookup table for finding neighboring triangles.  The data is stored as [triangle index, edge index].
        /// </summary>
        readonly int?[,] _adjacentTriangles;

        public SimpleMesh(Mesh mesh)
            : this(mesh.vertices, mesh.triangles)
        {
        }

        public SimpleMesh(Vector3[] vertices, int[] triangles)
        {
            Debug.Assert(triangles.Length % 3 == 0);

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
                        _adjacentTriangles[(int)triangleIndex, GetAdjacentEdge(i, j).GetLeadingIndex()] = i;
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
            var triangle = GetTriangle(triangleIndex);
            int? adjacentTriangleIndex = GetAdjacentTriangle(triangleIndex, edgeIndex);
            if (adjacentTriangleIndex == null)
            {
                return null;
            }
            var adjacentTriangle = GetTriangle((int)adjacentTriangleIndex);

            for (int i = 0; i < Constants.SidesOnTriangle; i++)
            {
                for (int j = 0; j < Constants.SidesOnTriangle; j++)
                {
                    if (triangle[i] == adjacentTriangle[j])
                    {
                        int iNext = (i + 1) % Constants.SidesOnTriangle;
                        int jNext = (j + 1) % Constants.SidesOnTriangle;
                        int jPrev = (j + Constants.SidesOnTriangle - 1) % Constants.SidesOnTriangle;
                        if (triangle[iNext] == adjacentTriangle[jNext])
                        {
                            return new TriangleEdge(j, jNext);
                        }
                        else if (triangle[iNext] == adjacentTriangle[jPrev])
                        {
                            return new TriangleEdge(j, jPrev);
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
    }
}
