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

            Assert.IsTrue((result[0] - expected[0]).magnitude < 0.001f);
            Assert.IsTrue((result[1] - expected[1]).magnitude < 0.001f);
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

            Assert.IsTrue((result[0] - expected[0]).magnitude < 0.001f);
            Assert.IsTrue((result[1] - expected[1]).magnitude < 0.001f);
        }
    }
}
