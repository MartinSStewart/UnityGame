using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    /// <summary>
    /// Custom mesh implementation used in place of UnityEngine.Mesh to allow for VS unit testing. 
    /// Also supports fast lookup of adjacent triangles.
    /// </summary>
    public class ReadOnlyMesh
    {
        /// <summary>
        /// Indices pointing into the _vertices array. The indices are stored as [triangle index, edge index].
        /// </summary>
        readonly int[,] _triangles;
        readonly Vector3[] _vertices;
        public int TriangleCount { get { return _triangles.GetLength(0); } }
        /// <summary>
        /// Lookup table for finding neighboring triangles. The data is stored as [triangle index, edge index].
        /// </summary>
        readonly int?[,] _adjacentTriangles;

        public ReadOnlyMesh(UnityEngine.Mesh mesh)
            : this(mesh.vertices.Select(item => new Vector3(item)).ToArray(), mesh.triangles)
        {
        }

        public ReadOnlyMesh(Vector3[] vertices, int[] triangles)
        {
            Debug.Assert(triangles.Length % Constants.SidesOnTriangle == 0);

            _triangles = new int[triangles.Length / Constants.SidesOnTriangle, Constants.SidesOnTriangle];
            _adjacentTriangles = new int?[TriangleCount, Constants.SidesOnTriangle];
            _vertices = vertices.ToArray();

            for (int i = 0; i < TriangleCount; i++)
            {
                int index = i * Constants.SidesOnTriangle;
                _triangles[i, 0] = triangles[index];
                _triangles[i, 1] = triangles[index + 1];
                _triangles[i, 2] = triangles[index + 2];
            }
            
            UpdateAdjacentTriangles();
        }

        /// <summary>
        /// Returns copies of vertices to iterate through.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Vector3> GetVertices()
        {
            foreach (var v in _vertices)
            {
                yield return v;
            }
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

        public int?[] GetAdjacentTriangles(int triangleIndex)
        {
            return new[]
            {
                GetAdjacentTriangle(triangleIndex, 0),
                GetAdjacentTriangle(triangleIndex, 1),
                GetAdjacentTriangle(triangleIndex, 2)
            };
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
            var result = GetCommonEdge(triangleIndex, (int)temp);
            Debug.Assert(result != null, "Execution should not have reached this point.");
            return result;
        }

        /// <summary>
        /// Returns the common triangle edge for first triangle index (if one exists).
        /// </summary>
        /// <param name="triangleIndex"></param>
        /// <param name="triangleIndexAdjacent"></param>
        /// <returns></returns>
        public TriangleEdge GetCommonEdge(int triangleIndex, int adjacentTriangleIndex)
        {
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
            return null;
        }

        public Vector2 MeshToTriCoord(int triangleIndex, Vector3 coord)
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

            Vector3 yAxis = (triangle[1] - triangle[0]).Normalized();

            Vector3 xAxis = triangle[2] - triangle[0];
            //Adjust the xAxis so that it is orthogonal to the yAxis.
            Vector3 projection = Vector3.Dot(yAxis, xAxis) * yAxis;
            xAxis = (xAxis - projection).Normalized();

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

        /// <summary>
        /// Return the coord relative to the parent mesh.
        /// </summary>
        /// <returns></returns>
        public Vector3 TriToMeshCoord(int triangleIndex, Vector2 local)
        {
            var axis = TriangleXYAxis(triangleIndex);
            return TriangleLocalOrigin(triangleIndex) + axis[0] * local.X + axis[1] * local.Y;
        }

        public bool IsAdjacent(int triangleIndex0, int triangleIndex1)
        {
            return GetAdjacentTriangles(triangleIndex0).Contains(triangleIndex1);
        }

        //public Vector2 TriToAdjacentCoord(int triangleIndex, int edgeIndex)
        //{
        //    return TriToAdjacentCoord(triangleIndex, (int)GetAdjacentTriangle(triangleIndex, edgeIndex));
        //}

        /// <summary>
        /// Convert from this triangles coordinate system to the coordinate system of an adjacent triangle.
        /// </summary>
        /// <param name="triangleIndex"></param>
        /// <param name="triangleAdjacentIndex"></param>
        /// <returns></returns>
        public Vector2 TriToAdjacentCoord(int triangleIndex, int triangleAdjacentIndex)
        {
            Debug.Assert(IsAdjacent(triangleIndex, triangleAdjacentIndex));
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the free vertice in an adjacent triangle (aka the vertice that isn't part of the common edge) in surface coordinates relative to this triangle.
        /// </summary>
        /// <param name="triangleIndex">Array index of this triangle.</param>
        /// <param name="adjacentTriangleIndex">Array index of adjacent triangle.</param>
        /// <returns></returns>
        public Vector3 GetPlanarAdjacentTriangle(int triangleIndex, int adjacentTriangleIndex)
        {
            Debug.Assert(IsAdjacent(triangleIndex, adjacentTriangleIndex));

            int adjacentTriangleFreeIndex = GetTriangleIndices(adjacentTriangleIndex)
                .First(item => !GetTriangleIndices(triangleIndex).Contains(item));
            int triangleFreeIndex = GetTriangleIndices(triangleIndex)
                .First(item => !GetTriangleIndices(adjacentTriangleIndex).Contains(item));

            Vector3[] triangle = GetTriangle(triangleIndex);
            Vector2[] surfaceTriangle = GetSurfaceTriangle(triangleIndex);

            Vector3 adjacentTriangleFreeVertice = _vertices[adjacentTriangleFreeIndex];
            Vector3 triangleFreeVertice = _vertices[triangleFreeIndex];


            Vector3[] commonEdge = GetCommonEdge(triangleIndex, adjacentTriangleIndex)
                .Indices
                .Select(item => triangle[item])
                .ToArray();

            float angleOffset = MathExt.AngleOffAroundAxis(adjacentTriangleFreeVertice - commonEdge[0], triangleFreeVertice - commonEdge[0], commonEdge[1] - commonEdge[1]);
            return new Vector3();
            //var matrix = UnityEngine.Matrix4x4.TRS(new UnityEngine.Vector3(), new UnityEngine.Quaternion(commonEdge[1].X - commonEdge[0].X, commonEdge[1].Y - commonEdge[0].Y, commonEdge[1].z - commonEdge[0].z, angleOffset), UnityEngine.Vector3.one);
            //var matrix2 = Matrix4.Identity;
            //return new Vector3(matrix.MultiplyPoint(triangleFreeVertice.ToUnity()));
            //Vector3.ProjectOnPlane(adjacentTriangleFreeVertice, MathExt.GetTriangleNormal(triangle));
            //float[] distances = commonEdge.Select(item => (item - adjacentTriangleFreeVertice).Length).ToArray();


            //Vector2[] commonEdgeSurface = GetCommonEdge(triangleIndex, adjacentTriangleIndex)
            //    .Indices
            //    .Select(item => surfaceTriangle[item])
            //    .ToArray();

            //Vector2[] intersections = MathExt.IntersectionTwoCircles(commonEdgeSurface[0], distances[0], commonEdgeSurface[1], distances[1]);

            //LineF commonEdgeLine = new LineF(commonEdgeSurface);
            //return commonEdgeLine.GetSideOf(intersections[0]) == commonEdgeLine.GetSideOf(MeshToTriCoord(triangleIndex, triangleFreeVertice)) ?
            //    intersections[0] :
            //    intersections[1];
        }

        public int[] GetTriangleIndices(int triangleIndex)
        {
            return new[]
            {
                _triangles[triangleIndex, 0],
                _triangles[triangleIndex, 1],
                _triangles[triangleIndex, 2]
            };
        }
    }
}
