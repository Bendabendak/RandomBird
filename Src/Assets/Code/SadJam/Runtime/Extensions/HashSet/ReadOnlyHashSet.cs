using System.Collections;
using System.Collections.Generic;

namespace SadJam
{
    public interface IReadonlyHashSet<T> : IReadOnlyCollection<T>
    {
        public bool Contains(T i);
    }

    public struct ReadOnlyHashSet<T> : IReadonlyHashSet<T>
    {
        public int Count => _set.Count;

        private readonly HashSet<T> _set;
        public ReadOnlyHashSet(HashSet<T> set)
        {
            this._set = set;
        }

        public static implicit operator ReadOnlyHashSet<T>(HashSet<T> set)
        {
            return new(set);
        }

        public bool Contains(T i) => _set.Contains(i);

        public IEnumerator<T> GetEnumerator() => _set.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _set.GetEnumerator();
    }
}
