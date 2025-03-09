using UnityEngine;

namespace SadJam.Components
{
    public abstract class GameObjectBounds_Size : StructComponent<Vector3>, Bounds_Element
    {
        public abstract Bounds Bounds { get; }

        public static void DrawBounds(Bounds bounds)
        {
            Gizmos.color = new(0, 1, 0);

            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        protected virtual void OnDrawGizmosSelected()
        {
             DrawBounds(Bounds);
        }
    }
}
