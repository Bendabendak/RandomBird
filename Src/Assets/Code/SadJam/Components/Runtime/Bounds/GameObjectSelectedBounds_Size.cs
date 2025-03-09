using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    public class GameObjectSelectedBounds_Size : GameObjectBounds_Size
    {
        public override Vector3 Size => GetBounds().size;

        public override Bounds Bounds => GetBounds();

        [field: SerializeField]
        public Vector3 Offset { get; private set; } = Vector3.zero;

        [field: Space, SerializeField]
        public List<GameObjectBounds_Size> Elements { get; private set; }

        protected override void OnValidate()
        {
            base.OnValidate();

            GetBounds();
        }

        protected override void StartOnce()
        {
            base.StartOnce();

            GetBounds();
        }

        private Bounds GetBounds()
        {
            if (Elements == null || Elements.Count <= 0)
            {
                return new(transform.position, Offset);
            }

            bool allNull = true;
            Bounds bounds = new();
            foreach (GameObjectBounds_Size e in Elements)
            {
                if (e == null || e == this) continue;

                bounds = e.Bounds;
                allNull = false;
                break;
            }

            if (allNull)
            {
                return new(transform.position, Offset);
            }

            foreach (GameObjectBounds_Size e in Elements)
            {
                if (e == null || e == this) continue;

                bounds.Encapsulate(e.Bounds);
            }

            bounds.size += Offset;

            return bounds;
        }
    }
}
