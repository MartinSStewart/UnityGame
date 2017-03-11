using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public static class PathFinding
    {
        public static List<int> ComputeCoarsePath(SurfaceCoord start, SurfaceCoord end)
        {
            Debug.Assert(start.Mesh == end.Mesh);
            if (start.TriangleIndex == end.TriangleIndex)
            {
                return new List<int>() { start.TriangleIndex };
            }

            var distance = new float[start.Mesh.TriangleCount];
            for (int i = 0; i < distance.Length; i++)
            {
                distance[i] = float.MaxValue;
            }

            var pathSearch = new Queue<int>();
            distance[start.TriangleIndex] = 0;
            pathSearch.Enqueue(start.TriangleIndex);

            while (pathSearch.Any())
            {
                int index = pathSearch.Dequeue();

                var adjacents = start.Mesh.GetAdjacentTriangles(index);
                for (int i = 0; i < Constants.SidesOnTriangle; i++)
                {
                    if (adjacents[i] == null)
                    {
                        continue;
                    }




                    //var nodeAdjacent = pathSearch.FirstOrDefault(item => item.TriangleIndex == adjacents[i]) ??
                    //    new SearchNode(
                    //        (int)adjacents[i],
                    //        ,
                    //        node);

                }
            }

            return GetPath(new List<int>(), start.Mesh, distance, end.TriangleIndex);
        }

        private static List<int> GetPath(List<int> path, ReadOnlyMesh mesh, float[] distances, int triangleIndex)
        {
            // If distance is 0 then we found the starting point.
            if (distances[triangleIndex] == 0)
            {
                return path;
            }

            int previous = (int)GetPrevious(mesh, distances, triangleIndex);
            path.Insert(0, previous);
            return GetPath(path, mesh, distances, previous);
        }

        /// <summary>
        /// Get the index of the previous triangle on this path.  This is the one with the smallest distance value.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="distances"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static int? GetPrevious(ReadOnlyMesh mesh, float[] distances, int index)
        {
            var adjacents = mesh.GetAdjacentTriangles(index);
            int? previous = null;
            float minDistance = float.MaxValue;
            for (int i = 0; i < Constants.SidesOnTriangle; i++)
            {
                if (adjacents[i] == null)
                {
                    continue;
                }
                int adjacentIndex = (int)adjacents[i];
                if (adjacents[i] != null && distances[adjacentIndex] < minDistance)
                {
                    previous = adjacents[i];
                    minDistance = distances[adjacentIndex];
                }
            }
            return previous;
        }
    }
}
