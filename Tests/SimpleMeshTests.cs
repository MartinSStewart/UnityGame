using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets;
using UnityEngine;

namespace Tests
{
    [TestClass]
    public class SimpleMeshTests
    {
        [TestMethod]
        public void SimpleMeshTest0()
        {
            SimpleMesh mesh = new SimpleMesh(
                new[] {
                    new Vector3(),
                    new Vector3(1, 0),
                    new Vector3(2, 0),
                    new Vector3(0, 1),
                    new Vector3(1, 1),
                    new Vector3(0, 2),
                },
                new[] {
                    0, 1, 3,
                    1, 2, 4,
                    1, 3, 4,
                    3, 4, 5
                });

            Assert.IsTrue(true, "Just check that SimpleMesh is instantiated without crashing.");
        }
    }
}
