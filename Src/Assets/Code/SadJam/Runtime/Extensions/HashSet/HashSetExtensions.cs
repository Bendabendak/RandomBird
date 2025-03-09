using System.Collections.Generic;

namespace SadJam
{
    public static class HashSetExtensions
    {
        public static ReadOnlyHashSet<T> AsReadOnly<T>(this HashSet<T> s) => new(s);
    }
}
