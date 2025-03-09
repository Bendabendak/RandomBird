using System.Collections;
using System.Collections.Generic;

namespace SadJam
{
    public static class ListExtensions
    {
        public static void AddRange(this IList list, int index, IEnumerable collection)
        {
            int pos = index;
            foreach(object o in collection)
            {
                if (pos > list.Count - 1) return;

                list[pos] = o;

                pos++;
            }
        }

        public static T Next<T>(this IList<T> list, Direction2 dir, int index, int move)
        {
            if (dir == Direction2.backward)
            {
                return index <= 0 ? list[list.Count - move] : list[index - move];
            }

            return index >= list.Count - move ? list[0] : list[index + move];
        }

        public static T Prev<T>(this IList<T> list, Direction2 dir, int index, int move)
        {
            if (dir == Direction2.backward)
            {
                return index >= list.Count - move ? list[0] : list[index + move];
            }

            return index <= 0 ? list[list.Count - move] : list[index - move];
        }
    }
}
