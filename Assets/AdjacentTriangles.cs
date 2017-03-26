using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public struct AdjacentTriangles
    {
        readonly int? i0, i1, i2;

        public int?[] Indices { get { return new[] { i0, i1, i2 }; } }

        public int? this[int index] { get { return Indices[index]; } }

        public AdjacentTriangles(IList<int?> triangleIndices)
        {
            Debug.Assert(triangleIndices.Count == Constants.SidesOnTriangle);
            i0 = triangleIndices[0];
            i1 = triangleIndices[1];
            i2 = triangleIndices[2];
        }

        public AdjacentTriangles(int? triangleIndex0, int? triangleIndex1, int? triangleIndex2)
        {
            i0 = triangleIndex0;
            i1 = triangleIndex1;
            i2 = triangleIndex2;
        }

        public AdjacentTriangles SetValue(int? triangleIndex, int index)
        {
            switch (index)
            {
                case 0: return new AdjacentTriangles(triangleIndex, i1, i2);
                case 1: return new AdjacentTriangles(i0, triangleIndex, i2);
                case 2: return new AdjacentTriangles(i0, i1, triangleIndex);
                default: throw new IndexOutOfRangeException();
            }
        }
    }
}
