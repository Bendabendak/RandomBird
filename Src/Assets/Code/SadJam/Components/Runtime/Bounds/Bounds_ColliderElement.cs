using UnityEngine;

namespace SadJam.Components
{
    public class Bounds_ColliderElement : GameObjectBounds_Size
    {
        [field: SerializeField]
        public Collider Collider { get; private set; }

        public override Vector3 Size => GetBounds().size;
        public override Bounds Bounds => GetBounds();

        private Bounds GetBounds()
        {
            if (Collider == null) return new();

            return Collider.bounds;
        }
    }
}
