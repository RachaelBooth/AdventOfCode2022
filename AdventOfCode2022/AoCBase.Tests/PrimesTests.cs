using System.Linq;
using NUnit.Framework;

namespace AoCBase.Tests
{
    public class PrimesTests
    {
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(1, false)]
        [TestCase(250, false)]
        [TestCase(337, true)]
        public void TestIsPrime(int value, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, Primes.IsPrime(value));
        }

        [TestCase(2)]
        [TestCase(1)]
        [TestCase(6)]
        [TestCase(9)]
        [TestCase(18)]
        [TestCase(17)]
        [TestCase(175)]
        public void TestFactorise(int value)
        {
            var factors = Primes.Factorise(value);
            var multiplied = factors.Aggregate(1, (current, factor) => current * factor);
            Assert.AreEqual(value, multiplied);
        }
    }
}