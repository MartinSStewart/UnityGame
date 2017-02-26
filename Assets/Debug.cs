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
                Break();
            }
        }

        public static void Fail(string message = null)
        {
            Break();
        }

        public static void Break()
        {
            // Place breakpoint here.
        }
    }
}
