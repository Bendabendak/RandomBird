using UnityEngine;

namespace SadJam.Components
{
    public class Bounds_CustomElement : GameObjectBounds_Size
    {
        [field: SerializeField]
        public Vector3 BoundsSize { get; private set; }
        public override Vector3 Size => BoundsSize;

        public override Bounds Bounds => new(transform.position, Size);
    }
}

