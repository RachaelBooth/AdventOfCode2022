using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoCBase
{
    public static class DictionaryHelpers
    {
        /// <summary>
        /// This will add an empty list if key not present. Change that if you don't want.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<U> GetValues<T, U>(this Dictionary<T, List<U>> dict, T key)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, new List<U>());
            }

            return dict[key];
        }

        /// <summary>
        /// This will add an empty hashset if key not present. Change that if you don't want.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static HashSet<U> GetValues<T, U>(this Dictionary<T, HashSet<U>> dict, T key)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, new HashSet<U>());
            }

            return dict[key];
        }

        public static V ReadWithDefault<T, U, V>(this Dictionary<T, Dictionary<U, V>> dict, T key1, U key2, V defaultValue = default)
        {
            if (dict.ContainsKey(key1) && dict[key1].ContainsKey(key2))
            {
                return dict[key1][key2];
            }
            return defaultValue;
        }

        public static void Add<T, U, V>(this Dictionary<T, Dictionary<U, V>> dict, T key1, U key2, V value)
        {
            if (!dict.ContainsKey(key1))
            {
                dict.Add(key1, new Dictionary<U, V>());
            }

            dict[key1].Add(key2, value);
        }

        public static void AddOptions<T, U>(this Dictionary<T, HashSet<U>> dict, T key, params U[] values)
        {
            dict.GetValues(key).UnionWith(values.ToHashSet());
        }

        public static void AddOptions<T, U>(this Dictionary<T, List<U>> dict, T key, params U[] values)
        {
            dict.GetValues(key).AddRange(values);
        }

        public static void AddOptionsToStart<T, U>(this Dictionary<T, List<U>> dict, T key, params U[] values)
        {
            dict.GetValues(key).InsertRange(0, values);
        }

        // TODO: Make this safer
        public static List<U> RemoveFirst<T, U>(this Dictionary<T, List<U>> dict, T key, int count = 1)
        {
            var start = dict.GetValues(key).Take(count).ToList();
            dict.GetValues(key).RemoveRange(0, count);
            return start;
        }

        /// <summary>
        /// U must have + defined, but can't restrict for that at compile time because C# argh.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        // QQ: Get C# preview features, then should be able to use generic maths stuff
        public static void AddToValue<T, U>(this Dictionary<T, U> dict, T key, U value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
            else
            {
                dict[key] += (dynamic) value;
            }
        }

        public static void AddToValue<T>(this Dictionary<T, Vector2> dict, T key, Vector2 value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
            else
            {
                dict[key] += value;
            }
        }

        public static U ReadWithDefault<T, U>(this Dictionary<T, U> dict, T key, U defaultValue = default, bool addDefaultToDict = false)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }

            if (addDefaultToDict)
            {
                dict.Add(key, defaultValue);
                return dict[key];
            }

            return defaultValue;
        }

        public static U ReadWithDefault<T, U>(this Dictionary<T, U> dict, T key, Func<U> defaultConstructor, bool addDefaultToDict = false)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }

            if (addDefaultToDict)
            {
                dict.Add(key, defaultConstructor());
                return dict[key];
            }

            return defaultConstructor();
        }

        public static void AddIfKeyNotPresent<T, U>(this Dictionary<T, U> dict, T key, U value)
        {
            dict.AddIfKeyNotPresent(key, () => value);
        }

        public static void AddIfKeyNotPresent<T, U>(this Dictionary<T, U> dict, T key, Func<U> valueConstructor)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, valueConstructor());
            }
        }

        public static bool ContainsMatching<T, U>(this Dictionary<T, List<U>> dict, T key, Func<U, bool> matchFunction)
        {
            if (!dict.ContainsKey(key))
            {
                return false;
            }

            return dict[key].Any(matchFunction);
        }

        public static void RestrictTo<T, U>(this Dictionary<T, List<U>> dict, T key, params U[] values)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = dict[key].Intersect(values).ToList();
            }
        }

        public static void Exclude<T, U>(this Dictionary<T, List<U>> dict, T key, params U[] values)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = dict[key].Except(values).ToList();
            }
        }
    }
}
