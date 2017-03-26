using System.Collections.Generic;
using System.Linq;

namespace Assets
{
    public struct Triangle
    {
        readonly int[] _indices;

        public int[] Indices { get { return _indices.ToArray(); } }

        public int this[int index]
        {
            get { return _indices[index]; }
            set
            {
                _indices[index] = value;
            }
        }

        public Triangle(IList<int> indices)
        {
            Debug.Assert(indices.Count == Constants.SidesOnTriangle);
            _indices = indices.ToArray();
        }

        public Triangle(int i0, int i1, int i2)
        {
            _indices = new[] { i0, i1, i2 };
        }
    }
}