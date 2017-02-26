using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets;
using UnityEngine;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class SurfaceCoordTests
    {
        public SimpleMesh GetAxisAlignedTriangle()
        {
            return new SimpleMesh(new[]
                {
                    new Vector3(),
                    new Vector3(0, 1, 0),
                    new Vector3(1, 0, 0)
                }, new[] { 0, 1, 2 });
        }

        [TestMethod]
        public void GetWorldCoordTest0()
        {
            var coord = new SurfaceCoord(GetAxisAlignedTriangle(), 0, new Vector2());

            Vector3 result = coord.GetLocalCoord();
            Assert.IsTrue(result == new Vector3());
        }

        [TestMethod]
        public void GetWorldCoordTest1()
        {
            var coord = new SurfaceCoord(GetAxisAlignedTriangle(), 0, new Vector2(1, 0));

            Vector3 result = coord.GetLocalCoord();
            Assert.IsTrue(result == new Vector3(1, 0, 0));
        }

        [TestMethod]
        public void GetWorldCoordTest2()
        {
            var coord = new SurfaceCoord(GetAxisAlignedTriangle(), 0, new Vector2(0, 1));

            Vector3 result = coord.GetLocalCoord();
            Assert.IsTrue(result == new Vector3(0, 1, 0));
        }

        [TestMethod]
        public void GetWorldCoordTest3()
        {
            var coord = new SurfaceCoord(GetAxisAlignedTriangle(), 0, new Vector2(0.2f, 0.6f));

            Vector3 result = coord.GetLocalCoord();
            Assert.IsTrue(result == new Vector3(0.2f, 0.6f, 0));
        }

        [TestMethod]
        public void GetWorldCoordTest4()
        {
            SimpleMesh triangle = GetAxisAlignedTriangle();
            Vector3 offset = new Vector3(5f, 4f, 2f);
            triangle.Translate(offset);
            var coord = new SurfaceCoord(triangle, 0, new Vector2(0.2f, 0.6f));

            Vector3 result = coord.GetLocalCoord();
            Vector3 expected = new Vector3(0.2f, 0.6f, 0) + offset;
            Assert.IsTrue(result == expected);
        }

        /// <summary>
        /// Scaling the triangle should not affect the result because SurfaceCoord uses x,y rather than u,v coordinates.
        /// </summary>
        [TestMethod]
        public void GetWorldCoordTest5()
        {
            SimpleMesh triangle = GetAxisAlignedTriangle();
            float scale = 3;
            triangle.Scale(scale);
            var coord = new SurfaceCoord(triangle, 0, new Vector2(0.2f, 0.6f));

            Vector3 result = coord.GetLocalCoord();
            Vector3 expected = new Vector3(0.2f, 0.6f, 0);
            Assert.IsTrue(result == expected);
        }

        [TestMethod]
        public void GetWorldCoordTest6()
        {
            SimpleMesh triangle = GetAxisAlignedTriangle();
            float scale = 3;
            triangle.Scale(scale);
            Vector3 offset = new Vector3(5f, 4f, 2f);
            triangle.Translate(offset);
            var coord = new SurfaceCoord(triangle, 0, new Vector2(0.2f, 0.6f));

            Vector3 result = coord.GetLocalCoord();
            Vector3 expected = new Vector3(0.2f, 0.6f, 0) + offset;
            Assert.IsTrue(result == expected);
        }

        /// <summary>
        /// A triangle standing on its side still produces correct results.
        /// </summary>
        [TestMethod]
        public void GetWorldCoordTest7()
        {
            SimpleMesh triangle = new SimpleMesh(
                new[] {
                    new Vector3(),
                    new Vector3(0, 0, 1),
                    new Vector3(1, 0, 0)
                },
                new[] { 0, 1, 2 });

            var coord = new SurfaceCoord(triangle, 0, new Vector2(0.2f, 0.6f));

            Vector3 result = coord.GetLocalCoord();
            Vector3 expected = new Vector3(0.2f, 0, 0.6f);
            Assert.IsTrue(result == expected);
        }

        /// <summary>
        /// Scaling the y-axis should not affect the result.
        /// </summary>
        [TestMethod]
        public void GetWorldCoordTest8()
        {
            SimpleMesh triangle = new SimpleMesh(
                new[] {
                    new Vector3(),
                    new Vector3(0, 0, 1),
                    new Vector3(0.5f, 0, 0)
                },
                new[] { 0, 1, 2 });

            var coord = new SurfaceCoord(triangle, 0, new Vector2(0.2f, 0.6f));

            Vector3 result = coord.GetLocalCoord();
            Vector3 expected = new Vector3(0.2f, 0, 0.6f);
            Assert.IsTrue(result == expected);
        }

        /// <summary>
        /// Nor should skewing it.
        /// </summary>
        [TestMethod]
        public void GetWorldCoordTest9()
        {
            SimpleMesh triangle = new SimpleMesh(
                new[]
                {
                    new Vector3(),
                    new Vector3(0, 0, 1),
                    new Vector3(0.5f, 0, 0.3f)
                },
                new[] { 0, 1, 2 });

            var coord = new SurfaceCoord(triangle, 0, new Vector2(0.2f, 0.6f));

            Vector3 result = coord.GetLocalCoord();
            Vector3 expected = new Vector3(0.2f, 0, 0.6f);
            Assert.IsTrue(result == expected);
        }

        SimpleMesh GetRandomTriangle()
        {
            //Create random number generator for getting random triangles.
            System.Random rand = new System.Random(123123);
            Vector3[] vertices = new Vector3[3];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3((float)rand.NextDouble() * 1000 - 500, (float)rand.NextDouble() * 1000 - 500, (float)rand.NextDouble() * 1000 - 500);
            }
            return new SimpleMesh(vertices, new[] { 0, 1, 2 });
        }

        /// <summary>
        /// Test if the first point is in the correct spot on a bunch of random triangles.
        /// </summary>
        [TestMethod]
        public void SurfaceTrianglesFirstPointIsOnOrigin()
        {
            const double maxErrorDelta = 0.0001f;
            for (int i = 0; i < 1000; i++)
            {
                var coord = new SurfaceCoord(GetRandomTriangle(), 0, new Vector2(0.2f, 0.6f));

                var result = coord.GetSurfaceTriangle();
                Assert.IsTrue(result[0].magnitude < maxErrorDelta);
            }

        }

        /// <summary>
        /// Test if the second point is in the correct spot on a bunch of random triangles.
        /// </summary>
        [TestMethod]
        public void SurfaceTrianglesSecondPointIsOnYAxis()
        {
            const double maxErrorDelta = 0.0001f;
            for (int i = 0; i < 1000; i++)
            {
                var coord = new SurfaceCoord(GetRandomTriangle(), 0, new Vector2(0.2f, 0.6f));

                var result = coord.GetSurfaceTriangle();
                var expected = new Vector2(0, (coord.Mesh.GetTriangle(coord.TriangleIndex)[1] - coord.Mesh.GetTriangle(coord.TriangleIndex)[0]).magnitude);
                Assert.IsTrue((result[1] - expected).magnitude < maxErrorDelta);
            }
        }

        public SimpleMesh GetAxisAlignedQuad()
        {
            return new SimpleMesh(
                new[] {
                    new Vector3(),
                    new Vector3(0, 1, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 1, 0)
                },
                new[] {
                    0, 1, 2, //First triangle
                    1, 3, 2 //Second triangle
                });
        }

        [TestMethod]
        public void MoveTest0()
        {
            var triangle = GetAxisAlignedTriangle();

            var coord = new SurfaceCoord(triangle, 0, new Vector2(0.1f, 0.1f));

            var result = coord.Move(new Vector2(0f, -1f));
            var expected = new SurfaceCoord(triangle, 0, new Vector2(0.1f, 0f));
            Assert.IsTrue(SurfaceCoord.Equals(result, expected));
            Assert.IsTrue(result.GetLocalCoord() == new Vector3(0.1f, 0f, 0f));
        }

        [TestMethod]
        public void MoveTest1()
        {
            var quad = GetAxisAlignedQuad();

            var coord = new SurfaceCoord(quad, 0, new Vector2(0.1f, 0.1f));

            var result = coord.Move(new Vector2(0.7f, 0.7f));
            var expected = new SurfaceCoord(quad, 1, new Vector2(0.2f, 0.8f), 270);
            Assert.IsTrue(SurfaceCoord.Equals(result, expected));
            Assert.IsTrue(result.GetLocalCoord() == new Vector3(0.8f, 0.8f, 0f));
        }

        [TestMethod]
        public void MoveTest2()
        {
            var quad = GetAxisAlignedQuad();

            var coord = new SurfaceCoord(quad, 0, new Vector2(0.1f, 0.1f));

            var result = coord.Move(new Vector2(0f, 0.85f));
            var expected = new SurfaceCoord(quad, 1, new Vector2(0.05f, 0.1f), 270);
            Assert.IsTrue(SurfaceCoord.Equals(result, expected));
            Assert.IsTrue(result.GetLocalCoord() == new Vector3(0.1f, 0.95f, 0f));
        }

        [TestMethod]
        public void MoveTest3()
        {
            var quad = new SimpleMesh(
                new[] {
                    new Vector3(),
                    new Vector3(0, 1, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 1, 0)
                },
                new[] {
                    0, 1, 2, // First triangle.
                    2, 3, 1 // Second triangle, now reversed order.
                });

            var coord = new SurfaceCoord(quad, 0, new Vector2(0.1f, 0.1f));

            var result = coord.Move(new Vector2(0.7f, 0.7f));
            var expected = new SurfaceCoord(quad, 1, new Vector2(0.2f, 0.8f), 270);
            Assert.IsTrue(SurfaceCoord.Equals(result, expected));
            Assert.IsTrue(result.GetLocalCoord() == new Vector3(0.8f, 0.8f, 0f));
        }

        [TestMethod]
        public void MoveTest4()
        {
            var quad = new SimpleMesh(
                new[] {
                    new Vector3(),
                    new Vector3(0, 1, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 1, 0)
                },
                new[] {
                    0, 1, 2, //First triangle
                    3, 1, 2 //Second triangle, now with 2 indices flipped.
                });

            var coord = new SurfaceCoord(quad, 0, new Vector2(0.1f, 0.1f));

            var result = coord.Move(new Vector2(0.7f, 0.7f));
            Assert.IsTrue(result.GetLocalCoord() == new Vector3(0.8f, 0.8f, 0f));
        }

        [TestMethod]
        public void MoveTest5()
        {
            var quad = new SimpleMesh(
                new[] {
                    new Vector3(),
                    new Vector3(0, 1, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(2f, 2f, 0) //Last vertice is moved.
                },
                new[] {
                    0, 1, 2, //First triangle
                    3, 1, 2 //Second triangle
                });

            var coord = new SurfaceCoord(quad, 0, new Vector2(0.1f, 0.1f));

            var result = coord.Move(new Vector2(0.7f, 0.7f));
            Assert.IsTrue(result.GetLocalCoord() == new Vector3(0.8f, 0.8f, 0f));
        }
    }
}
