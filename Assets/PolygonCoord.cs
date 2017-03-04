using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class PolygonCoord
    {
        /// <summary>
        /// Index of the edge within the polygon.
        /// </summary>
        public readonly int EdgeIndex;

        /// <summary>
        /// Value between [0,1] that represents the position along the edge.
        /// </summary>
        public readonly float EdgeT;

        public PolygonCoord(int edgeIndex, float edgeT)
        {
            Debug.Assert(edgeT >= 0 && edgeT <= 1);
            EdgeIndex = edgeIndex;
            EdgeT = edgeT;
        }
    }
}
