using UnityEngine;

namespace SadJam.Components
{
    public class Bounds_RendererElement : GameObjectBounds_Size
    {
        public enum Bounds_RendererType
        {
            World,
            Local
        }

        [field: SerializeField]
        public Renderer Renderer { get; private set; }
        [field: Space, SerializeField]
        public Bounds_RendererType Type { get; private set; }

        public override Bounds Bounds => GetBounds();
        public override Vector3 Size => GetBounds().size;

        private Bounds GetBounds()
        {
            if (Renderer == null) return new();

            if (Type == Bounds_RendererType.World)
            {
                return Renderer.bounds;
            }

            return Renderer.localBounds;
        }
    }
}
