using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public static class Debug
    {
        public static void Assert(bool condition, string message = null)
        {
            if (!condition)
            {
                Break();
                throw new Exception(message);
            }
        }

        public static void Fail(string message = null)
        {
            Break();
            throw new Exception(message);
        }

        public static void Break()
        {
            // Place breakpoint here.
        }
    }
}
