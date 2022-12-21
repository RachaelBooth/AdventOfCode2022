using NUnit.Framework;

namespace AoCBase.Tests
{
    public class PolynomialTests
    {
        [Test]
        public void TestEquality()
        {
            var p = new IntegerPolynomial(1, 2, 0, 1);
            var q = new IntegerPolynomial((0, 1), (3, 1), (1, 2));
            Assert.IsTrue(p == q);
            Assert.IsTrue(p.Equals(q));
        }

        [TestCase(0, 5, 23, 7)]
        [TestCase(1, 3, 13, 4)]
        [TestCase(3, -1, 7, 20)]
        [TestCase(-19, 4, 64, 10)]
        [TestCase(4, 7, 10, 3)]
        public void SubstituteLinearRepeatingMatchesMultipleSubstitutions(int xCoefficient, int constTerm, int modulus, int timesToRepeat)
        {
            var result = new IntegerPolynomial(constTerm, xCoefficient).SubstituteLinearRepeating(timesToRepeat, modulus);

            var multipleSubsResult = new IntegerPolynomial(constTerm, xCoefficient);
            var times = 1;
            while (times < timesToRepeat)
            {
                multipleSubsResult = multipleSubsResult.Substitute(new IntegerPolynomial(constTerm, xCoefficient)) % modulus;
                times++;
            }

            Assert.That(result == multipleSubsResult);
        }
    }
}
