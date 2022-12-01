using System.Collections.Generic;

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

        public ValueCollection<T> Clone()
        {
            return new ValueCollection<T>(_values);
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is ValueCollection<T> vc && _values.SetEquals(vc._values);
        }

        public static bool operator ==(ValueCollection<T> lhs, ValueCollection<T> rhs) => lhs.Equals(rhs);

        public static bool operator !=(ValueCollection<T> lhs, ValueCollection<T> rhs) => !lhs.Equals(rhs);
    }
}
