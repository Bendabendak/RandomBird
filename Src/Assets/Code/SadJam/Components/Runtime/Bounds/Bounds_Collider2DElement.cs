using UnityEngine;

namespace SadJam.Components
{
    public class Bounds_Collider2DElement : GameObjectBounds_Size
    {
        [field: SerializeField]
        public Collider2D Collider2D { get; private set; }

        public override Vector3 Size => GetBounds().size;
        public override Bounds Bounds => GetBounds();

        private Bounds GetBounds()
        {
            if (Collider2D == null) return new();

            return Collider2D.bounds;
        }
    }
}
