using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCBase
{
    public class ValueCollection<T>
    {
        public readonly HashSet<T> _values;

        public ValueCollection(HashSet<T> values)
        {
            _values = values;
        }

        public ValueCollection(params T[] values)
        {
            _values = new HashSet<T>(values);
        }

        public void Add(T value)
        {
            _values.Add(value);
        }

        public bool Contains(T value)
        {
            return _values.Contains(value);
        }

        public IEnumerable<ValueCollection<T>> GetSubsets()
        {
            if (Count() == 0)
            {
                return new List<ValueCollection<T>> { new ValueCollection<T>() };
            }

            var arbitraryValue = _values.First();
            var rest = _values.ToHashSet();
            rest.Remove(arbitraryValue);
            var restCollection = new ValueCollection<T>(rest);

            return restCollection.GetSubsets().SelectMany(s =>
            {
                var withAdded = s.Clone();
                withAdded.Add(arbitraryValue);
                return new List<ValueCollection<T>> { s, withAdded };
            });
        }

        public int Count()
        {
            return _values.Count;
        }

        public int Aggregate(int seed, Func<int, T, int> accumulator)
        {
            return _values.Aggregate(seed, accumulator);
        }

        public bool IsSupersetOf(ValueCollection<T> other)
        {
            return _values.IsSupersetOf(other._values);
        }

        public IEnumerable<T> Where(Func<T, bool> filter)
        {
            return _values.Where(filter);
        }

        public ValueCollection<T> Clone()
        {
            return new ValueCollection<T>(_values.ToHashSet());
        }

        public ValueCollection<T> Without(ValueCollection<T> other)
        {
            var copy = _values.ToHashSet();
            copy.ExceptWith(other._values);
            return new ValueCollection<T>(copy);
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is ValueCollection<T> vc && _values.SetEquals(vc._values);
        }

        public List<T> ToList()
        {
            return _values.ToList();
        }

        public static bool operator ==(ValueCollection<T> lhs, ValueCollection<T> rhs) => lhs.Equals(rhs);

        public static bool operator !=(ValueCollection<T> lhs, ValueCollection<T> rhs) => !lhs.Equals(rhs);
    }
}
