using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoCBase
{
    public class ModularArithmetic
    {
        private ModularArithmetic() {}

        private static readonly Lazy<ModularArithmetic> Instance = new Lazy<ModularArithmetic>(() => new ModularArithmetic());

        private Dictionary<(BigInteger modulus, BigInteger value), BigInteger?> inversesCache = new();
        private Dictionary<(BigInteger a, BigInteger b), (BigInteger gcd, BigInteger s, BigInteger t)> bezoutCache = new();

        public static int NonNegativeMod(int value, int modulus)
        {
            var result = value % modulus;
            while (result < 0)
            {
                result += modulus;
            }

            return result;
        }

        public static int NonNegativeMod(long value, int modulus)
        {
            var result = value % modulus;
            while (result < 0)
            {
                result += modulus;
            }

            return (int) result;
        }

        public static BigInteger NonNegativeMod(BigInteger value, BigInteger modulus)
        {
            var result = value % modulus;
            while (result < 0)
            {
                result += modulus;
            }

            return result;
        }

        public static BigInteger PositiveMod(BigInteger value, BigInteger modulus)
        {
            var result = value % modulus;
            while (result <= 0)
            {
                result += modulus;
            }

            return result;
        }

        public static BigInteger MultiplyMod(BigInteger modulus, params BigInteger[] values)
        {
            if (values.Length > 2)
            {
                return values.Aggregate((c, next) => MultiplyMod(modulus, c, next));
            }

            if (values.Length == 1)
            {
                return values[0] % modulus;
            }

            var res = BigInteger.Multiply(values[0], values[1]);
            return res % modulus;
        }

        /// <summary>
        /// Returns the GCD of x and y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static BigInteger GreatestCommonDivisor(BigInteger x, BigInteger y)
        {
            if (x < 0 || y < 0)
            {
                throw new ArgumentException("GCD only implemented for non-negative integers");
            }

            if (x < y)
            {
                var o = y;
                y = x;
                x = o;
            }

            if (y == 0)
            {
                return x;
            }

            return GreatestCommonDivisor(y, x % y);
        }

        /// <summary>
        /// Returns gcd, and coefficients such that gcd = sa + tb
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static (BigInteger gcd, BigInteger s, BigInteger t) FindBezoutCoefficients(BigInteger a, BigInteger b)
        {
            return Instance.Value.FindBezoutCoefficientsWithCache(a, b);
        }

        private (BigInteger gcd, BigInteger s, BigInteger t) FindBezoutCoefficientsWithCache(BigInteger a, BigInteger b)
        {
            if (!bezoutCache.ContainsKey((a, b)))
            {
                var coefficients = CalculateBezoutCoefficients(a, b);
                bezoutCache.Add((a, b), coefficients);
                bezoutCache.Add((b, a), (coefficients.gcd, coefficients.t, coefficients.s));
            }

            return bezoutCache[(a, b)];
        }

        private static (BigInteger gcd, BigInteger s, BigInteger t) CalculateBezoutCoefficients(BigInteger a, BigInteger b)
        {
            if (a < b)
            {
                var flipped = FindBezoutCoefficients(b, a);
                return (flipped.gcd, flipped.t, flipped.s);
            }
            
            if (b <= 0)
            {
                throw new ArgumentException("a and b must be positive");
            }
            
            var R = new List<BigInteger> { a, b };
            var Q = new List<BigInteger> { 0 };
            var S = new List<BigInteger> { 1, 0 };
            var T = new List<BigInteger> { 0, 1 };
            
            var i = 1;
            while (true)
            {
                R.Add(R[i - 1] % R[i]);
                if (R[i + 1] == 0)
                {
                    // Done
                    return (R[i], S[i], T[i]);
                }
                
                Q.Add((R[i - 1] - R[i + 1]) / R[i]);
                S.Add(S[i - 1] - Q[i] * S[i]);
                T.Add(T[i - 1] - Q[i] * T[i]);
                i++;
            }
        }

        public static BigInteger SumMod(BigInteger modulus, params BigInteger[] summands)
        {
            return summands.Aggregate((current, next) => (current + next) % modulus);
        }

        public static BigInteger? GetInverse(BigInteger x, BigInteger modulus)
        {
            return Instance.Value.GetInverseWithCache(x, modulus);
        }

        public BigInteger? GetInverseWithCache(BigInteger x, BigInteger modulus)
        {
            var x_m = NonNegativeMod(x, modulus);
            if (!inversesCache.ContainsKey((modulus, x_m)))
            {
                var bezout = FindBezoutCoefficients(modulus, x_m);
                if (bezout.gcd == 1)
                {
                    var inv = bezout.t;
                    while (inv < 0)
                    {
                        inv += modulus;
                    }
                    inversesCache.Add((modulus, x_m), inv);
                }
                else
                {
                    inversesCache.Add((modulus, x_m), null);
                }
            }

            return inversesCache[(modulus, x_m)];
        }

        public static (BigInteger value, BigInteger modulus) ReduceModularEquations(params (BigInteger value, BigInteger modulus)[] congruences)
        {
            if (congruences.Count() == 1)
            {
                return congruences.First();
            }

            var reduced = ReduceModularEquationPair(congruences[0], congruences[1]);
            var updated = congruences.Skip(2).Append(reduced).ToArray();
            return ReduceModularEquations(updated);
        }

        private static (BigInteger value, BigInteger modulus) ReduceModularEquationPair((BigInteger value, BigInteger modulus) first, (BigInteger value, BigInteger modulus) second)
        {
            var bezoutCoefficients = FindBezoutCoefficients(first.modulus, second.modulus);
            if (bezoutCoefficients.gcd != 1)
            {
                // You probably shouldn't be using this in this case - something may have gone wrong
                // Reevaluate if it does
                throw new Exception("Attempted to reduce modular equation pair with moduli not coprime");
            }
            var modulus = first.modulus * second.modulus;
            var value = MultiplyMod(modulus, first.value, bezoutCoefficients.t, second.modulus) + MultiplyMod(modulus, second.value, bezoutCoefficients.s, first.modulus);
            return (value % modulus, modulus);
        }
    }
}
