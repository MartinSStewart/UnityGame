using Assets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class MathExtTests
    {
        [TestMethod]
        public void IntersectionTwoCirclesTest0()
        {
            var result = MathExt.IntersectionTwoCircles(new Vector2(), 1, new Vector2(1, 0), 1);
            var expected = new[]
            {
                new Vector2(0.5f, -0.8660254f),
                new Vector2(0.5f, 0.8660254f)
            };

            Assert.IsTrue((result[0] - expected[0]).Length < 0.001f);
            Assert.IsTrue((result[1] - expected[1]).Length < 0.001f);
        }

        [TestMethod]
        public void IntersectionTwoCirclesTest1()
        {
            var result = MathExt.IntersectionTwoCircles(new Vector2(), 1, new Vector2(1, 0), 2);
            var expected = new[]
            {
                new Vector2(1f, 0f),
                new Vector2(1f, 0f)
            };

            Assert.IsTrue((result[0] - expected[0]).Length < 0.001f);
            Assert.IsTrue((result[1] - expected[1]).Length < 0.001f);
        }

        /// <summary>
        /// A vector pointing to the right has an angle of 0 radians.
        /// </summary>
        [TestMethod]
        public void AngleVectorTest0()
        {
            var result = MathExt.AngleVector(new Vector2(1, 0));
            Assert.AreEqual(0, result);
        }

        /// <summary>
        /// A vector pointing down and to the right has an angle of PI/4 radians.
        /// </summary>
        [TestMethod]
        public void AngleVectorTest1()
        {
            var result = MathExt.AngleVector(new Vector2(1, -1));
            var expected = Math.PI / 4;
            Assert.AreEqual(expected, result, 0.0001);
        }

        [TestMethod]
        public void VectorFromAngleTest0()
        {
            var result = MathExt.VectorFromAngle(0);
            Assert.IsTrue(result.X == 1 && result.Y == 0);
        }

        [TestMethod]
        public void VectorFromAngleTest1()
        {
            var result = MathExt.AngleVector(new Vector2(1, 1));
            float value = (float)(Math.Sqrt(2) / 2);
            var expected = MathExt.AngleVector(new Vector2(value, value));
            Assert.AreEqual(result, expected, 0.001f);
        }
    }
}
