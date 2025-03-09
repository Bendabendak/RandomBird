using UnityEngine;

namespace SadJam
{
    public class StringComponent : Component
    {
        [field: SerializeField]
        public virtual string Content { get; set; }

        public static implicit operator string(StringComponent c) => c.Content;

        public override string ToString() => Content;
    }
}
