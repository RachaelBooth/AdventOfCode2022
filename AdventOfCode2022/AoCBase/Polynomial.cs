using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoCBase
{
    public class Polynomial 
    {
        private Dictionary<int, BigInteger> terms;

        public Polynomial(Dictionary<int, BigInteger> terms)
        {
            this.terms = new Dictionary<int, BigInteger>(terms.Where(t => t.Value != 0));
        }

        public Polynomial(params BigInteger[] coefficients)
        {
            terms = new Dictionary<int, BigInteger>();
            var i = 0;
            while (i < coefficients.Length)
            {
                if (coefficients[i] != 0)
                {
                    terms.Add(i, coefficients[i]);
                }
                i++;
            }
        }

        public Polynomial(params (int exponent, BigInteger coefficient)[] terms)
        {
            var rawTerms = new Dictionary<int, BigInteger>();
            foreach (var (exponent, coefficient) in terms)
            {
                rawTerms.AddToValue(exponent, coefficient);
            }
            this.terms = new Dictionary<int, BigInteger>(rawTerms.Where(t => t.Value != 0));
        }

        public Polynomial(IEnumerable<(int exponent, BigInteger coefficient)> terms) : this(terms.ToArray()) {}

        public Polynomial RaiseToPower(int power)
        {
            if (power == 0)
            {
                return new Polynomial(1);
            }

            if (power < 0)
            {
                throw new NotImplementedException("Can't raise a polynomial to a negative power");
            }

            var current = this;
            var p = 1;
            while (p < power)
            {
                current *= this;
                p++;
            }
            return current;
        }

        public Polynomial Substitute(Polynomial sub)
        {
            // TODO: Could maybe optimise this?
            return terms.Select(t => sub.RaiseToPower(t.Key) * t.Value).Aggregate((curr, next) => curr + next);
        }

        /// <summary>
        /// Substitues this polynomial into itself repeatedly, reducing by given modulus.
        /// Throws exception if polynomial is not linear.
        /// </summary>
        /// <param name="times"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Polynomial SubstituteLinearRepeating(long times, long modulus)
        {
            if (terms.Keys.Any(k => k > 1))
            {
                throw new Exception("Can use substitute linear repeating on non-linear polynomial");
            }

            var a = terms.ReadWithDefault(1, 0);
            var b = terms.ReadWithDefault(0, 0);
            
            if (a == 0)
            {
                return new Polynomial(b);
            }
            if (a == 1)
            {
                return new Polynomial((times * b) % modulus, 1);
            }

            var xCoefficient = BigInteger.ModPow(a, times, modulus);

            var inv = ModularArithmetic.GetInverse(modulus, a - 1);

            if (!inv.HasValue)
            {
                // No inverse, so can't use mod pow - hopefully in these cases it's not going to get too large...
                var constTerm = ModularArithmetic.MultiplyMod(modulus, (BigInteger.Pow(a, (int) times) - 1) / (a - 1), b);
                return new Polynomial(constTerm, xCoefficient);
            }
            else
            {
                var constTerm = ModularArithmetic.MultiplyMod(modulus, BigInteger.ModPow(a, times, modulus) - 1, b, inv.Value);
                return new Polynomial(constTerm, xCoefficient);
            }
        }

        public BigInteger Evaluate(BigInteger value)
        {
            return terms.Aggregate(BigInteger.Zero, (curr, next) => curr + (next.Value * BigInteger.Pow(value, next.Key)));
        }

        public BigInteger EvaluateMod(BigInteger value, BigInteger modulus)
        {
            return terms.Aggregate(BigInteger.Zero, (curr, next) => (curr + (next.Value * BigInteger.ModPow(value, next.Key, modulus)) % modulus));
        }

        public override int GetHashCode()
        {
            return terms.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is not Polynomial poly)
            {
                return false;
            }

            return terms.All(t => poly.terms.ReadWithDefault(t.Key, 0) == t.Value) && poly.terms.All(t => terms.ReadWithDefault(t.Key, 0) == t.Value);
        }

        public static bool operator ==(Polynomial lhs, Polynomial rhs) => lhs.Equals(rhs);

        public static bool operator !=(Polynomial lhs, Polynomial rhs) => !lhs.Equals(rhs);

        public static Polynomial operator -(Polynomial poly) => new(poly.terms.Select(t => (t.Key, -t.Value)));

        public static Polynomial operator +(Polynomial lhs, Polynomial rhs) => new(lhs.terms.Select(t => (t.Key, t.Value)).Concat(rhs.terms.Select(t => (t.Key, t.Value))));

        public static Polynomial operator -(Polynomial lhs, Polynomial rhs) => lhs + (-rhs);

        public static Polynomial operator *(Polynomial lhs, Polynomial rhs) => new(lhs.terms.SelectMany(t => rhs.terms.Select(m => (t.Key + m.Key, t.Value * m.Value))));

        public static Polynomial operator *(Polynomial lhs, BigInteger rhs) => new(lhs.terms.Select(t => (t.Key, t.Value * rhs)));

        public static Polynomial operator *(BigInteger lhs, Polynomial rhs) => rhs * lhs;

        public static Polynomial operator %(Polynomial poly, BigInteger modulus) => new(poly.terms.Select(t => (t.Key, t.Value % modulus)));

        public override string ToString() => string.Join(" + ", terms.OrderByDescending(t => t.Key).Select(t => $"{t.Value}{(t.Key == 0 ? "" : "x^" + t.Key)}"));
    }
}