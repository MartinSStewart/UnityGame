using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public static class Debug
    {
        public static void Assert(bool condition, string message = null)
        {
            if (!condition)
            {
                //Place a breakpoint here so Unity stops execution at the correct place.
            }
        }
    }
}
