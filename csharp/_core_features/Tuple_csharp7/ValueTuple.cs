/* C# 7 Tuples - to support the final usage
 * 
 * (T1, T2) Data() => return (v1, v2)
 * (T1, T2, T3) Data() => return (v1, v2, v3)
 * 
 * var (a, b) = Data();
 * (string x, int y) = Data();
 * 
 * note: 
 *   - T1 Item1, T2 Item1, T3 Item1, ...  the is hard structure for analyzer.
 *   - ValueTuple<...> it cannot be used in inheritance chain even if it's declared as class,  
 *                     because of internal limitation.
 * 
 * https://github.com/3F
 */

using System.Collections.Generic;

namespace System
{
    internal static class Hash
    {
        public static int GetHashCode(int h1, int h2)
        {
            int polynom(int r, int x)
            {
                unchecked {
                    return (r << 5) + r ^ x;
                }
            };

            int h = 0;
            h = polynom(h, h1);
            h = polynom(h, h2);

            return h;
        }

        public static string ToString(params object[] items)
        {
            string ret = "(";
            for(int i = 0, l = items.Length - 1; i < items.Length; ++i)
            {
                ret += items[i]?.ToString();
                if(i < l) {
                    ret += ", ";
                }
            }
            ret += ")";
            return ret;
        }
    }

    internal static class EqComparer<T>
    {
        private static EqualityComparer<T> eq = EqualityComparer<T>.Default;

        public static bool Equals(T x, T y)
        {
            return eq.Equals(x, y);
        }

        public static int GetHashCode(T obj)
        {
            return eq.GetHashCode(obj);
        }
    }

    public struct ValueTuple<T1, T2>: IEquatable<ValueTuple<T1, T2>>
    {
        public T1 Item1;
        public T2 Item2;

        public override bool Equals(object obj)
        {
            return (obj is ValueTuple<T1, T2>)
                    && Equals((ValueTuple<T1, T2>)obj);
        }

        public bool Equals(ValueTuple<T1, T2> data)
        {
            return EqComparer<T1>.Equals(Item1, data.Item1)
                    && EqComparer<T2>.Equals(Item2, data.Item2);
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode(
                EqComparer<T1>.GetHashCode(Item1),
                EqComparer<T2>.GetHashCode(Item2)
            );
        }

        public override string ToString()
        {
            return Hash.ToString(Item1, Item2);
        }

        public ValueTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }

    public struct ValueTuple<T1, T2, T3>: IEquatable<ValueTuple<T1, T2, T3>>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public override bool Equals(object obj)
        {
            return (obj is ValueTuple<T1, T2, T3>)
                    && Equals((ValueTuple<T1, T2, T3>)obj);
        }

        public bool Equals(ValueTuple<T1, T2, T3> data)
        {
            return EqComparer<T1>.Equals(Item1, data.Item1)
                    && EqComparer<T2>.Equals(Item2, data.Item2)
                    && EqComparer<T3>.Equals(Item3, data.Item3);
        }

        public override int GetHashCode()
        {
            var h = Hash.GetHashCode(
                EqComparer<T1>.GetHashCode(Item1),
                EqComparer<T2>.GetHashCode(Item2)
            );
            return Hash.GetHashCode(h, EqComparer<T3>.GetHashCode(Item3));
        }

        public override string ToString()
        {
            return Hash.ToString(Item1, Item2, Item3);
        }

        public ValueTuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }
    }
}