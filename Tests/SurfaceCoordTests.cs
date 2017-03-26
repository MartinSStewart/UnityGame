using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class SurfaceCoordTests
    {
        public ReadOnlyMesh GetAxisAlignedTriangle()
        {
            return new ReadOnlyMesh(new[]
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
            ReadOnlyMesh triangle = GetAxisAlignedTriangle();
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
            ReadOnlyMesh triangle = GetAxisAlignedTriangle();
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
            ReadOnlyMesh triangle = GetAxisAlignedTriangle();
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
            ReadOnlyMesh triangle = new ReadOnlyMesh(
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
            ReadOnlyMesh triangle = new ReadOnlyMesh(
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
            ReadOnlyMesh triangle = new ReadOnlyMesh(
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

        public ReadOnlyMesh GetAxisAlignedQuad()
        {
            return new ReadOnlyMesh(
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
            Assert.IsTrue(SurfaceCoord.AlmostEquals(result, expected, 0.001f));
            Assert.IsTrue((result.GetLocalCoord() - new Vector3(0.1f, 0f, 0f)).Length < 0.001f);
        }

        [TestMethod]
        public void MoveTest1()
        {
            var quad = GetAxisAlignedQuad();

            //var coord = new SurfaceCoord(quad, 0, new Vector2(0.1f, 0.1f));

            //var result = coord.Move(new Vector2(0.7f, 0.7f));
            //var expected = new SurfaceCoord(quad, 1, new Vector2(0.2f, 0.8f), 270);
            //Assert.IsTrue(SurfaceCoord.AlmostEquals(result, expected, 0.001f));
            //Assert.IsTrue((result.GetLocalCoord() - new Vector3(0.8f, 0.8f, 0f)).Length < 0.001f);

            AssertOnFlatMesh(quad, 0, new Vector2(0.1f, 0.1f), new Vector2(0.7f, 0.7f));
        }

        [TestMethod]
        public void MoveTest2()
        {
            var quad = GetAxisAlignedQuad();

            AssertOnFlatMesh(quad, 0, new Vector2(0.1f, 0.1f), new Vector2(0f, 0.85f));
        }

        [TestMethod]
        public void MoveTest3()
        {
            var quad = new ReadOnlyMesh(
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
            Assert.IsTrue(SurfaceCoord.AlmostEquals(result, expected, 0.001f));
            Assert.IsTrue((result.GetLocalCoord() - new Vector3(0.8f, 0.8f, 0f)).Length < 0.001f);
        }

        [TestMethod]
        public void MoveTest4()
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
                    3, 1, 2 //Second triangle, now with 2 indices flipped.
                });

            var coord = new SurfaceCoord(quad, 0, new Vector2(0.1f, 0.1f));

            var result = coord.Move(new Vector2(0.7f, 0.7f));
            Assert.IsTrue((result.GetLocalCoord() - new Vector3(0.8f, 0.8f, 0f)).Length < 0.001f);
        }

        [TestMethod]
        public void MoveTest5()
        {
            var quad = new ReadOnlyMesh(
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
            Assert.IsTrue((result.GetLocalCoord() - new Vector3(0.8f, 0.8f, 0f)).Length < 0.001f);
        }

        [TestMethod]
        public void MoveTest6()
        {
            var quad = new ReadOnlyMesh(
                new[] {
                    new Vector3(),
                    new Vector3(0, 1, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(2f, 2f, 0)
                },
                new[] {
                    0, 1, 2,
                    3, 1, 2
                });

            AssertOnFlatMesh(quad, 0, new Vector2(0.4f, 0.1f), new Vector2(0.2f, 0.6f));
        }

        [TestMethod]
        public void MoveTest7()
        {
            var quad = new ReadOnlyMesh(
                new[] {
                    new Vector3(),
                    new Vector3(0, 1, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(2f, 2f, 0)
                },
                new[] {
                    0, 1, 2,
                    2, 1, 3
                });

            AssertOnFlatMesh(quad, 0, new Vector2(0.3f, 0.1f), new Vector2(0.2f, 0.8f));
        }

        /// <summary>
        /// Verify that SurfaceCoord movement works correctly on the default quad in Unity.
        /// </summary>
        [TestMethod]
        public void UnityQuadMoveTest0()
        {
            var quad = new ReadOnlyMesh(
                new[] {
                    new Vector3(-0.5f, -0.5f, 0f),
                    new Vector3(0.5f, 0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(-0.5f, 0.5f, 0)
                },
                new[] {
                    0, 1, 2, //First triangle
                    1, 0, 3 //Second triangle
                });

            AssertOnFlatMesh(quad, 0, new Vector2(0f, -0.4f), new Vector2(0f, 0.8f));
        }

        [TestMethod]
        public void UnityQuadMoveTest1()
        {
            var quad = new ReadOnlyMesh(
                new[] {
                    new Vector3(-0.5f, -0.5f, 0f),
                    new Vector3(0.5f, 0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(-0.5f, 0.5f, 0)
                },
                new[] {
                    0, 1, 2, //First triangle
                    1, 0, 3 //Second triangle
                });

            AssertOnFlatMesh(quad, 0, new Vector2(0.2f, -0.4f), new Vector2(-0.1f, 0.8f));
        }

        [TestMethod]
        public void MoveOnBentQuadTest0()
        {
            var indices = new[] {
                0, 1, 2, //First triangle
                1, 0, 3 //Second triangle
            };
            var quad = new ReadOnlyMesh(
                new[] {
                    new Vector3(-0.5f, -0.5f, 0f),
                    new Vector3(0.5f, 0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(-0.5f, 0.5f, 0)
                },
                indices);

            var quadBent = new ReadOnlyMesh(
                new[] {
                    new Vector3(-0.5f, -0.5f, 0f),
                    new Vector3(0.5f, 0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(0f, 0f, (float)(0.5 * Math.Sqrt(2)))
                },
                indices);

            AssertOnBentMesh(quad, quadBent, 0, new Vector2(0f, -0.4f), new Vector2(0f, 0.8f));
        }

        [TestMethod]
        public void MoveOnBentQuadTest1()
        {
            var indices = new[] {
                0, 1, 2, //First triangle
                1, 0, 3 //Second triangle
            };
            var quad = new ReadOnlyMesh(
                new[] {
                    new Vector3(-0.5f, -0.5f, 0f),
                    new Vector3(0.5f, 0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(-0.5f, 0.5f, 0)
                },
                indices);

            var quadBent = new ReadOnlyMesh(
                new[] {
                    new Vector3(-0.5f, -0.5f, 0f),
                    new Vector3(0.5f, 0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(0f, 0f, (float)(0.5 * Math.Sqrt(2)))
                },
                indices);

            AssertOnBentMesh(quad, quadBent, 0, new Vector2(0.2f, -0.4f), new Vector2(-0.1f, 0.8f));
        }

        [TestMethod]
        public void MoveOnBentQuadTest2()
        {
            var indices = new[] {
                0, 1, 2, //First triangle
                3, 0, 1 //Second triangle reversed order
            };
            var quad = new ReadOnlyMesh(
                new[] {
                    new Vector3(-0.5f, -0.5f, 0f),
                    new Vector3(0.5f, 0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(-0.5f, 0.5f, 0)
                },
                indices);

            var quadBent = new ReadOnlyMesh(
                new[] {
                    new Vector3(-0.5f, -0.5f, 0f),
                    new Vector3(0.5f, 0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(0f, 0f, (float)(0.5 * Math.Sqrt(2)))
                },
                indices);

            AssertOnBentMesh(quad, quadBent, 0, new Vector2(0.2f, -0.4f), new Vector2(-0.1f, 0.8f));
        }

        void AssertOnFlatMesh(ReadOnlyMesh mesh, int triangleIndex, Vector2 startPoint, Vector2 movement)
        {
            SurfaceCoord start, end;
            Vector2 localMove;
            ComputeLocalMovement(mesh, triangleIndex, startPoint, movement, out start, out localMove, out end);

            var result = (Vector2)end.GetLocalCoord();
            var expected = startPoint + movement;
            Assert.IsTrue((result - expected).Length < 0.001f);
        }

        void AssertDirectionOnFlatMesh(ReadOnlyMesh mesh, int triangleIndex, Vector2 startPoint, Vector2 movement)
        {
            SurfaceCoord start, result;
            Vector2 localMove;
            ComputeLocalMovement(mesh, triangleIndex, startPoint, movement, out start, out localMove, out result);

            Vector2 v0 = (Vector2)mesh.TriToMeshCoord(triangleIndex, start.Coord);
            Vector2 v1 = (Vector2)mesh.TriToMeshCoord(triangleIndex, start.Coord + MathExt.VectorFromAngle(start.Rotation, 1));
            Vector2 v2 = (Vector2)mesh.TriToMeshCoord(result.TriangleIndex, result.Coord);
            Vector2 v3 = (Vector2)mesh.TriToMeshCoord(result.TriangleIndex, result.Coord + MathExt.VectorFromAngle(result.Rotation, 1));

            double expectedAngle = MathExt.AngleVector(v1 - v0);
            double resultAngle = MathExt.AngleVector(v3 - v2);
            Assert.AreEqual(expectedAngle, resultAngle, 0.1);
            Assert.IsTrue(((Vector2)result.GetLocalCoord() - (startPoint + movement)).Length < 0.001f);
        }

        /// <summary>
        /// Assert that the movement on a flat mesh matches its bent counterpart.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="meshBent"></param>
        /// <param name="triangleIndex"></param>
        /// <param name="startPoint"></param>
        /// <param name="movement"></param>
        void AssertOnBentMesh(ReadOnlyMesh mesh, ReadOnlyMesh meshBent, int triangleIndex, Vector2 startPoint, Vector2 movement)
        {
            SurfaceCoord start, result;
            Vector2 localMove;
            ComputeLocalMovement(mesh, triangleIndex, startPoint, movement, out start, out localMove, out result);

            var startBent = new SurfaceCoord(meshBent, triangleIndex, start.Coord);
            var resultBent = startBent.Move(localMove);

            Assert.IsTrue((result.Coord - resultBent.Coord).Length < 0.001f);
            Assert.IsTrue(Math.Abs(result.Rotation - resultBent.Rotation) < 0.001f);
        }

        private static void ComputeLocalMovement(ReadOnlyMesh mesh, int triangleIndex, Vector2 startPoint, Vector2 movement, out SurfaceCoord start, out Vector2 localMove, out SurfaceCoord result)
        {
            Debug.Assert(mesh.Vertices.All(item => item.Z == 0), "Mesh needs to be completely on xy plane.");
            Vector2 surfaceCoord = mesh.MeshToTriCoord(triangleIndex, (Vector3)startPoint);
            Debug.Assert(MathExt.PointInPolygon(surfaceCoord, mesh.GetSurfaceTriangle(triangleIndex)));
            start = new SurfaceCoord(mesh, triangleIndex, surfaceCoord);

            localMove = mesh.MeshToTriCoord(triangleIndex, (Vector3)(movement + startPoint)) - start.Coord;
            result = start.Move(localMove);
        }

        /// <summary>
        /// If we don't move outside of the starting triangle then direction should not change.
        /// </summary>
        [TestMethod]
        public void MoveCorrectRotationTest0()
        {
            var quad = GetAxisAlignedQuad();

            float expected = (float)Math.PI / 10;
            var coord = new SurfaceCoord(quad, 0, new Vector2(0.1f, 0.1f), expected);
            float result = coord.Move(new Vector2(0.01f, 0.01f)).Rotation;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void MoveCorrectRotationTest1()
        {
            var quad = GetAxisAlignedQuad();

            int count = 30;
            for (int i = 0; i < count; i++)
            {
                double expected = i * 2 * Math.PI / count;
                var coord = new SurfaceCoord(quad, 0, new Vector2(0.2f, 0.2f), (float)(expected + 3 * Math.PI/2));
                var moved = coord.Move(new Vector2(0.6f, 0.6f));
                double diff = MathExt.AngleDiff(expected, moved.Rotation);
                Assert.IsTrue(Math.Abs(diff) < 0.0001);
            }
        }

        [TestMethod]
        public void MoveCorrectRotationTest2()
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

            int count = 30;
            for (int i = 0; i < count; i++)
            {
                double expected = i * 2 * Math.PI / count;
                var coord = new SurfaceCoord(quad, 0, new Vector2(0.2f, 0.2f), (float)(expected + 3 * Math.PI / 2));
                var moved = coord.Move(new Vector2(0.6f, 0.6f));
                double diff = MathExt.AngleDiff(expected, moved.Rotation);
                Assert.IsTrue(Math.Abs(diff) < 0.0001);
            }
        }
    }
}
