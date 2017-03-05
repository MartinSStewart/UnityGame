﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets;
using UnityEngine;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class SurfaceCoordTests
    {
        public ReadonlyMesh GetAxisAlignedTriangle()
        {
            return new ReadonlyMesh(new[]
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
            ReadonlyMesh triangle = GetAxisAlignedTriangle();
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
            ReadonlyMesh triangle = GetAxisAlignedTriangle();
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
            ReadonlyMesh triangle = GetAxisAlignedTriangle();
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
            ReadonlyMesh triangle = new ReadonlyMesh(
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
            ReadonlyMesh triangle = new ReadonlyMesh(
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
            ReadonlyMesh triangle = new ReadonlyMesh(
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

        public ReadonlyMesh GetAxisAlignedQuad()
        {
            return new ReadonlyMesh(
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
            Assert.IsTrue(SurfaceCoord.Equals(result, expected, 0.001f));
            Assert.IsTrue((result.GetLocalCoord() - new Vector3(0.1f, 0f, 0f)).magnitude < 0.001f);
        }

        [TestMethod]
        public void MoveTest1()
        {
            var quad = GetAxisAlignedQuad();

            var coord = new SurfaceCoord(quad, 0, new Vector2(0.1f, 0.1f));

            var result = coord.Move(new Vector2(0.7f, 0.7f));
            var expected = new SurfaceCoord(quad, 1, new Vector2(0.2f, 0.8f), 270);
            Assert.IsTrue(SurfaceCoord.Equals(result, expected, 0.001f));
            Assert.IsTrue((result.GetLocalCoord() - new Vector3(0.8f, 0.8f, 0f)).magnitude < 0.001f);
        }

        [TestMethod]
        public void MoveTest2()
        {
            var quad = GetAxisAlignedQuad();

            var coord = new SurfaceCoord(quad, 0, new Vector2(0.1f, 0.1f));

            var result = coord.Move(new Vector2(0f, 0.85f));
            var expected = new SurfaceCoord(quad, 1, new Vector2(0.05f, 0.1f), 270);
            Assert.IsTrue(SurfaceCoord.Equals(result, expected, 0.001f));
            Assert.IsTrue((result.GetLocalCoord() - new Vector3(0.1f, 0.95f, 0f)).magnitude < 0.001f);
        }

        [TestMethod]
        public void MoveTest3()
        {
            var quad = new ReadonlyMesh(
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
            Assert.IsTrue(SurfaceCoord.Equals(result, expected, 0.001f));
            Assert.IsTrue((result.GetLocalCoord() - new Vector3(0.8f, 0.8f, 0f)).magnitude < 0.001f);
        }

        [TestMethod]
        public void MoveTest4()
        {
            var quad = new ReadonlyMesh(
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
            Assert.IsTrue((result.GetLocalCoord() - new Vector3(0.8f, 0.8f, 0f)).magnitude < 0.001f);
        }

        [TestMethod]
        public void MoveTest5()
        {
            var quad = new ReadonlyMesh(
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
            Assert.IsTrue((result.GetLocalCoord() - new Vector3(0.8f, 0.8f, 0f)).magnitude < 0.001f);
        }

        [TestMethod]
        public void UnityQuadMoveTest0()
        {
            var quad = new ReadonlyMesh(
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

        void AssertOnFlatMesh(ReadonlyMesh mesh, int triangleIndex, Vector2 startPoint, Vector2 movement)
        {
            Assets.Debug.Assert(mesh.GetVertices().All(item => item.z == 0), "Mesh needs to be completely on xy plane.");

            var start = new SurfaceCoord(mesh, triangleIndex, mesh.TriangleSurfaceCoord(triangleIndex, startPoint));

            Vector2 v0 = mesh.GetLocalCoord(triangleIndex, start.Coord);
            Vector2 v1 = mesh.GetLocalCoord(triangleIndex, start.Coord + MathExt.VectorFromAngle(start.Rotation, 1));

            Vector2 localMove = mesh.TriangleSurfaceCoord(triangleIndex, movement + startPoint) - start.Coord;
            var result = start.Move(localMove);
            Vector2 v2 = mesh.GetLocalCoord(result.TriangleIndex, result.Coord);
            Vector2 v3 = mesh.GetLocalCoord(result.TriangleIndex, result.Coord + MathExt.VectorFromAngle(result.Rotation, 1));

            Assert.AreEqual(MathExt.AngleVector(v3 - v2), MathExt.AngleVector(v1 - v0), 0.001);
            Assert.IsTrue(((Vector2)result.GetLocalCoord() - (startPoint + movement)).magnitude < 0.001f);
        }
    }
}
