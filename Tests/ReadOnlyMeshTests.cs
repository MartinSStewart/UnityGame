using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets;
using UnitTests;

namespace Tests
{
    [TestClass]
    public class ReadOnlyMeshTests
    {
        public ReadOnlyMesh GetRandomTriangle(Random rand)
        {
            Vector3[] vertices = new Vector3[3]; 
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3((float)rand.NextDouble() * 1000 - 500, (float)rand.NextDouble() * 1000 - 500, (float)rand.NextDouble() * 1000 - 500);
            }
            return new ReadOnlyMesh(vertices, new[] { 0, 1, 2 });
        }

        [TestMethod]
        public void SimpleMeshTest0()
        {
            ReadOnlyMesh mesh = new ReadOnlyMesh(
                new[] {
                    new Vector3(),
                    new Vector3(1, 0, 0),
                    new Vector3(2, 0, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(0, 2, 0),
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
            var rand = new Random(123123);
            const double maxErrorDelta = 0.0001f;
            for (int i = 0; i < 1000; i++)
            {
                var result = GetRandomTriangle(rand).GetSurfaceTriangle(0);
                Assert.IsTrue(result[0].Length < maxErrorDelta);
            }

        }

        /// <summary>
        /// Test if the second point is in the correct spot on a bunch of random triangles.
        /// </summary>
        [TestMethod]
        public void SurfaceTrianglesSecondPointIsOnYAxis()
        {
            var rand = new Random(123123);
            const double maxErrorDelta = 0.01f;
            for (int i = 0; i < 1000; i++)
            {
                var mesh = GetRandomTriangle(rand);
                int triangleIndex = 0;
                var result = mesh.GetSurfaceTriangle(0);
                var expected = new Vector2(0, (mesh.GetTriangle(triangleIndex)[1] - mesh.GetTriangle(triangleIndex)[0]).Length);
                Assert.IsTrue((result[1] - expected).Length < maxErrorDelta);
            }
        }

        #region GetPlanarAdjacentTriangle tests

        [TestMethod]
        public void GetPlanarAdjacentTriangleTest0()
        {
            Vector3[] vertices = new[] {
                new Vector3(),
                new Vector3(0, 1, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0)
            };
            var quad = new ReadOnlyMesh(
                vertices,
                new[] {
                    0, 1, 2, //First triangle
                    1, 3, 2 //Second triangle
                });

            Vector3 result = quad.GetPlanarAdjacentTriangle(0, 1);
            Assert.IsTrue((result - vertices[3]).Length < 0.001f);
        }

        [TestMethod]
        public void GetPlanarAdjacentTriangleTest1()
        {
            Vector3[] vertices = new[] {
                new Vector3(-1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, -1, 0),
                new Vector3(0.70711f, 0.70711f, 1)
            };
            var quad = new ReadOnlyMesh(
                vertices,
                new[] {
                    0, 1, 2, //First triangle
                    1, 3, 2 //Second triangle
                });

            Vector3 result = quad.GetPlanarAdjacentTriangle(0, 1);
            Assert.IsTrue((result - new Vector3(1, 1, 0)).Length < 0.001f);
        }

        [TestMethod]
        public void GetPlanarAdjacentTriangleTest2()
        {
            Vector3[] vertices = new[] {
                new Vector3(-1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, -1, 0),
                new Vector3(-0.70711f, -0.70711f, 1)
            };
            var quad = new ReadOnlyMesh(
                vertices,
                new[] {
                    0, 1, 2, //First triangle
                    1, 3, 2 //Second triangle
                });

            Vector3 result = quad.GetPlanarAdjacentTriangle(0, 1);
            Assert.IsTrue((result - new Vector3(1, 1, 0)).Length < 0.001f);
        }

        [TestMethod]
        public void GetPlanarAdjacentTriangleTest3()
        {
            Vector3[] vertices = new[] {
                new Vector3(-1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, -1, 0),
                new Vector3(-0.70711f, -0.70711f, -1)
            };
            var quad = new ReadOnlyMesh(
                vertices,
                new[] {
                    0, 1, 2, //First triangle
                    1, 3, 2 //Second triangle
                });

            Vector3 result = quad.GetPlanarAdjacentTriangle(0, 1);
            Assert.IsTrue((result - new Vector3(1, 1, 0)).Length < 0.001f);
        }

        [TestMethod]
        public void GetPlanarAdjacentTriangleTest4()
        {
            var offset = new Vector3(1, 3, -4);
            Vector3[] vertices = new[] {
                new Vector3(-1, -1, 0) + offset,
                new Vector3(-1, 1, 0) + offset,
                new Vector3(1, -1, 0) + offset,
                new Vector3(-0.70711f, -0.70711f, -1) + offset
            };
            var quad = new ReadOnlyMesh(
                vertices,
                new[] {
                    0, 1, 2, //First triangle
                    1, 3, 2 //Second triangle
                });

            Vector3 result = quad.GetPlanarAdjacentTriangle(0, 1);
            Vector3 expected = new Vector3(1, 1, 0) + offset;
            Assert.IsTrue((result - expected).Length < 0.001f);
        }

        [TestMethod]
        public void GetPlanarAdjacentTriangleTest5()
        {
            var offset = new Vector3(1, 3, -4);
            Vector3[] vertices = new[] {
                new Vector3(-1, -1, 0) + offset,
                new Vector3(-1, 1, 0) + offset,
                new Vector3(1, -1, 0) + offset,
                new Vector3(-0.70711f, -0.70711f, -1) + offset
            };
            var quad = new ReadOnlyMesh(
                vertices,
                new[] {
                    0, 1, 2, //First triangle
                    2, 3, 1 //Second triangle reordered
                });

            Vector3 result = quad.GetPlanarAdjacentTriangle(0, 1);
            Vector3 expected = new Vector3(1, 1, 0) + offset;
            Assert.IsTrue((result - expected).Length < 0.001f);
        }

        [TestMethod]
        public void GetPlanarAdjacentTriangleTest6()
        {
            var offset = new Vector3(1, 3, -4);
            Vector3[] vertices = new[] {
                new Vector3(-1, -1, 0) + offset,
                new Vector3(-1, 1, 0) + offset,
                new Vector3(1, -1, 0) + offset,
                new Vector3(-0.70711f, -0.70711f, -1) + offset
            };
            var quad = new ReadOnlyMesh(
                vertices,
                new[] {
                    1, 0, 2, //First triangle reordered
                    2, 3, 1 //Second triangle reordered
                });

            Vector3 result = quad.GetPlanarAdjacentTriangle(0, 1);
            Vector3 expected = new Vector3(1, 1, 0) + offset;
            Assert.IsTrue((result - expected).Length < 0.001f);
        }

        #endregion

        [TestMethod]
        public void TriToAdjacentCoordTest0()
        {
            var quad = new ReadOnlyMesh(
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

            Vector3 expected = new Vector3(1, 1, 0);
            Vector2 v0 = quad.MeshToTriCoord(0, expected);
            Vector2 v1 = quad.TriToAdjacentCoord(0, 1, v0);
            Vector3 result = quad.TriToMeshCoord(1, v1);
            Assert.IsTrue((result - expected).Length < 0.001f);
        }

        [TestMethod]
        public void TriToAdjacentCoordTest1()
        {
            var quad = new ReadOnlyMesh(
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

            Random rand = new Random(123321);
            for (int i = 0; i < 10000; i++)
            {
                Vector3 expected = new Vector3((float)rand.NextDouble() * 4 - 2, (float)rand.NextDouble() * 4 - 2, 0);
                Vector2 v0 = quad.MeshToTriCoord(0, expected);
                Vector2 v1 = quad.TriToAdjacentCoord(0, 1, v0);
                Vector3 result = quad.TriToMeshCoord(1, v1);
                Assert.IsTrue((result - expected).Length < 0.001f);
            }
        }

        [TestMethod]
        public void TriToAdjacentCoordTest2()
        {
            var quad = new ReadOnlyMesh(
                new[] {
                    new Vector3(-0.2f, -0.1f, 0),
                    new Vector3(-0.2f, 1, 0),
                    new Vector3(1.4f, 0, 0),
                    new Vector3(0.7f, 2, 0)
                },
                new[] {
                    0, 1, 2, //First triangle
                    1, 3, 2 //Second triangle
                });

            Random rand = new Random(123321);
            for (int i = 0; i < 10000; i++)
            {
                Vector3 expected = new Vector3((float)rand.NextDouble() * 4 - 2, (float)rand.NextDouble() * 4 - 2, 0);
                Vector2 v0 = quad.MeshToTriCoord(0, expected);
                Vector2 v1 = quad.TriToAdjacentCoord(0, 1, v0);
                Vector3 result = quad.TriToMeshCoord(1, v1);
                Assert.IsTrue((result - expected).Length < 0.001f);
            }
        }

        [TestMethod]
        public void TriToAdjacentCoordTest3()
        {
            var quad = new ReadOnlyMesh(
                new[] {
                    new Vector3(-1, -1, 0),
                    new Vector3(-1, 1, 0),
                    new Vector3(1, -1, 0),
                    new Vector3(0, 0, 1.41422f)
                },
                new[] {
                    0, 1, 2, //First triangle
                    1, 3, 2 //Second triangle
                });

            
            Vector2 v0 = quad.MeshToTriCoord(0, new Vector3(-1, -1, 0));
            Vector2 v1 = quad.TriToAdjacentCoord(0, 1, v0);
            Vector3 result = quad.TriToMeshCoord(1, v1);
            Assert.IsTrue((result - new Vector3(0, 0, -1.41422f)).Length < 0.001f);
        }

        [TestMethod]
        public void TriToAdjacentCoordTest4()
        {
            var quad = new ReadOnlyMesh(
                new[] {
                    new Vector3(-1, -1, 0),
                    new Vector3(-1, 1, 0),
                    new Vector3(1, -1, 0),
                    new Vector3(0, 0, 1.41422f)
                },
                new[] {
                    2, 1, 0, //First triangle reordered
                    2, 1, 3 //Second triangle reordered
                });


            Vector2 v0 = quad.MeshToTriCoord(0, new Vector3(-0.5f, -0.5f, 0));
            Vector2 v1 = quad.TriToAdjacentCoord(0, 1, v0);
            Vector3 result = quad.TriToMeshCoord(1, v1);
            Assert.IsTrue((result - new Vector3(0, 0, -1.41422f/2)).Length < 0.001f);
        }

        [TestMethod]
        public void AdjacentEdgeByTriIndexTest0()
        {
            Vector3[] vertices = new[] {
                new Vector3(),
                new Vector3(0, 1, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0)
            };
            var quad = new ReadOnlyMesh(
                vertices,
                new[] {
                    0, 1, 2, //First triangle
                    1, 3, 2 //Second triangle
                });

            TriangleEdge result = quad.AdjacentEdgeByTriIndex(0, 1);
            var adjacentTri = quad.GetTriangle(1);
            Assert.IsTrue(adjacentTri[result.StartIndex] == new Vector3(0, 1, 0));
            Assert.IsTrue(adjacentTri[result.EndIndex] == new Vector3(1, 0, 0));
        }
    }
}
