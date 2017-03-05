using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets;
using UnityEngine;
using UnitTests;

namespace Tests
{
    [TestClass]
    public class SimpleMeshTests
    {
        public SimpleMesh GetRandomTriangle(System.Random rand)
        {
            Vector3[] vertices = new Vector3[3];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3((float)rand.NextDouble() * 1000 - 500, (float)rand.NextDouble() * 1000 - 500, (float)rand.NextDouble() * 1000 - 500);
            }
            return new SimpleMesh(vertices, new[] { 0, 1, 2 });
        }

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

        /// <summary>
        /// Test if the first point is in the correct spot on a bunch of random triangles.
        /// </summary>
        [TestMethod]
        public void SurfaceTrianglesFirstPointIsOnOrigin()
        {
            var rand = new System.Random(123123);
            const double maxErrorDelta = 0.0001f;
            for (int i = 0; i < 1000; i++)
            {
                var result = GetRandomTriangle(rand).GetSurfaceTriangle(0);
                Assert.IsTrue(result[0].magnitude < maxErrorDelta);
            }

        }

        /// <summary>
        /// Test if the second point is in the correct spot on a bunch of random triangles.
        /// </summary>
        [TestMethod]
        public void SurfaceTrianglesSecondPointIsOnYAxis()
        {
            var rand = new System.Random(123123);
            const double maxErrorDelta = 0.01f;
            for (int i = 0; i < 1000; i++)
            {
                var mesh = GetRandomTriangle(rand);
                int triangleIndex = 0;
                var result = mesh.GetSurfaceTriangle(0);
                var expected = new Vector2(0, (mesh.GetTriangle(triangleIndex)[1] - mesh.GetTriangle(triangleIndex)[0]).magnitude);
                Assert.IsTrue((result[1] - expected).magnitude < maxErrorDelta);
            }
        }
    }
}
