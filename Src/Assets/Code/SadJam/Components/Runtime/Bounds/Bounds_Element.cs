using UnityEngine;

namespace SadJam.Components
{
    public interface Bounds_Element
    {
        public Bounds Bounds { get; }
    }

    public static class BoundsExtents
    {
        public static bool TryGetBoundsComponent(this GameObject gameObject, out Bounds_Element component)
        {
            component = gameObject.GetComponentInChildren<Bounds_Element>(true);

            if (component == null)
            {
                component = default;
                return false;
            }

            return true;
        }
    }
}
