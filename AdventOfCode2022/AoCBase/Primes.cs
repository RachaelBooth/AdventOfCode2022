using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCBase
{
    public class Primes
    {
        // First 50 primes
        private int[] _primes = {
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103,
            107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223,
            227, 229
        };

        private int maxChecked = 230;
        private int maxPrime = 229;

        private Primes() {}

        private static readonly Lazy<Primes> instance = new Lazy<Primes>(() => new Primes());

        public static int GetNextPrimeGeq(int min)
        {
            return instance.Value.NextPrime(min);
        }

        public static bool IsPrime(int candidate)
        {
            return instance.Value.CheckIsPrime(candidate);
        }

        public static List<int> Factorise(int value)
        {
            return instance.Value.GetFactors(value);
        }

        private List<int> GetFactors(int value, int minFactorIndex = 0)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException("value", "Can't factorise numbers less than 1");
            }

            if (value == 1)
            {
                return new List<int>();
            }

            if (CheckIsPrime(value))
            {
                return new List<int> {value};
            }

            for (var i = minFactorIndex; i < _primes.Length; i++)
            {
                var p = _primes[i];
                if (value % p == 0)
                {
                    return GetFactors(value / p, i).Append(p).ToList();
                }
            }

            // Shouldn't be possible to get here, value is either prime or has a prime factor less than it (and checking if it is prime means we have found all primes less than it)
            throw new Exception($"Error: no prime factors of {value} found");
        }

        private bool CheckIsPrime(int candidate)
        {
            ExtendPrimes(candidate);
            return _primes.Contains(candidate);
        }

        private int NextPrime(int min)
        {
            ExtendPrimes(min);
            while (min > maxPrime)
            {
                ExtendPrimes(maxChecked * 2);
            }

            var i = 0;
            var p = _primes[i];
            while (p < min)
            {
                i++;
                p = _primes[i];
            }

            return p;
        }

        private void ExtendPrimes(int end)
        {
            if (end <= maxChecked)
            {
                return;
            }

            var notPrime = new HashSet<int>();
            foreach (var p in _primes)
            {
                // N.B. Integer division, rounded towards 0 - which is what we want here
                var initialMultiplier = maxChecked / p;
                if (initialMultiplier == 1)
                {
                    initialMultiplier++;
                }

                var val = p * initialMultiplier;
                while (val <= end)
                {
                    notPrime.Add(val);
                    val = val + p;
                }
            }

            var newPrimes = _primes.ToList();
            for (var candidate = maxChecked + 1; candidate <= end; candidate++)
            {
                if (!notPrime.Contains(candidate))
                {
                    newPrimes.Add(candidate);
                    maxPrime = candidate;
                    var val = candidate * 2;
                    while (val <= end)
                    {
                        notPrime.Add(val);
                        val = val + candidate;
                    }
                }
            }

            _primes = newPrimes.ToArray();
            maxChecked = end;
        }
    }
}
