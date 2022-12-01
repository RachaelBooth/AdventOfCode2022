using NUnit.Framework;

namespace AoCBase.Tests
{
    public class ModularArithmeticTests
    {
        [TestCase(3,5)]
        [TestCase(6,8)]
        [TestCase(11,7)]
        [TestCase(9,4)]
        public void TestBezout(long a, long b)
        {
            var result = ModularArithmetic.FindBezoutCoefficients(a, b);
            Assert.AreEqual(result.gcd, result.s * a + result.t * b);
        }

        [TestCase(2,4,false)]
        [TestCase(3,4,true)]
        [TestCase(5,7,true)]
        [TestCase(9,5,true)]
        [TestCase(6,9,false)]
        public void TestInverse(long x, long mod, bool inverseExists)
        {
            var result = ModularArithmetic.GetInverse(mod, x);
            if (inverseExists)
            {
                Assert.IsTrue(result.HasValue);
                Assert.AreEqual(1, ModularArithmetic.MultiplyMod(mod,result.Value, x));
            }
            else
            {
                Assert.IsFalse(result.HasValue);
            }
        }
    }
}
