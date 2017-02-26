using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class TriangleEdge
    {
        public readonly int StartIndex, EndIndex;

        public TriangleEdge(int startIndex, int endIndex)
        {
            Debug.Assert(startIndex < Constants.SidesOnTriangle && startIndex >= 0);
            Debug.Assert(endIndex < Constants.SidesOnTriangle && endIndex >= 0);
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        /// <summary>
        /// Get the index of the point this edge starts at.  
        /// This is different from StartIndex since StartIndex and EndIndex can be in reverse order.
        /// </summary>
        /// <returns></returns>
        public int GetLeadingIndex()
        {
            if ((StartIndex == 2 && EndIndex == 0) || (EndIndex == 2 && StartIndex == 0))
            {
                return 2;
            }
            else
            {
                return Math.Min(StartIndex, EndIndex);
            }
        }
    }
}
