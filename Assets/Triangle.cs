using System.Collections.Generic;
using System.Linq;

namespace Assets
{
    public struct Triangle
    {
        readonly int[] _vertices;

        public int this[int index]
        {
            get { return _vertices[index]; }
            set
            {
                _vertices[index] = value;
            }
        }

        public Triangle(IList<int> indices)
        {
            Debug.Assert(indices.Count == Constants.SidesOnTriangle);
            _vertices = indices.ToArray();
        }

        public Triangle(int i0, int i1, int i2)
        {
            _vertices = new[] { i0, i1, i2 };
        }
    }
}