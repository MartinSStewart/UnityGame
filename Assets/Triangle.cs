using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets
{
    public struct Triangle
    {
        readonly int i0, i1, i2;

        public int[] Indices { get { return new[] { i0, i1, i2 }; } }

        public int this[int index] { get { return Indices[index]; } }

        public Triangle(IList<int> vertexIndices)
        {
            Debug.Assert(vertexIndices.Count == Constants.SidesOnTriangle);
            i0 = vertexIndices[0];
            i1 = vertexIndices[1];
            i2 = vertexIndices[2];
        }

        public Triangle(int vertexIndex0, int vertexIndex1, int vertexIndex2)
        {
            i0 = vertexIndex0;
            i1 = vertexIndex1;
            i2 = vertexIndex2;
        }

        public Triangle SetValue(int vertexIndex, int index)
        {
            switch (index)
            {
                case 0: return new Triangle(vertexIndex, i1, i2);
                case 1: return new Triangle(i0, vertexIndex, i2);
                case 2: return new Triangle(i0, i1, vertexIndex);
                default: throw new IndexOutOfRangeException();
            }
        }
    }
}