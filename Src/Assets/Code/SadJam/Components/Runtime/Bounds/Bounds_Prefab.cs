using System;
using UnityEngine;

namespace SadJam.Components
{
    public class Bounds_Prefab : GameObjectBounds_Size
    {
        [field: SerializeField]
        public GameObject Prefab { get; private set; }

        public override Bounds Bounds => GetBounds();
        public override Vector3 Size => GetBounds().size;

        private Bounds GetBounds()
        {
            if (_bounds == null) return new();

            return _bounds.Bounds;
        }

        [NonSerialized]
        private Bounds_Element _bounds;
        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            GetBoundsElement();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            GetBoundsElement();
        }

        private void GetBoundsElement()
        {
            if (Prefab == null) return;

            if (!Prefab.TryGetBoundsComponent(out _bounds))
            {
                _bounds = null;
                Debug.LogError(Prefab.name + " doesn't contain " + nameof(Bounds_Element) + "!", gameObject);
            }
        }
    }
}
