using SadJam;
using SadJam.Components;
using System;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Spawn/VerticalFill")]
    public class Spawn_VerticalFill : SpawnPool
    {
        [field: Space]
        [GameConfigSerializeProperty]
        public IGameConfig_VerticalLineupable VerticalLineupableConfig { get; }

        [field: Space, SerializeField]
        public StructComponent<Vector3> FillRange { get; private set; }
        [field: Space, SerializeField]
        public int SpawnMinimum { get; private set; } = 0;

        [NonSerialized]
        private Bounds_Element _bounds;
        protected override void OnPrefabChanged()
        {
            base.OnPrefabChanged();

            GetPrefabBounds();
        }

        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            GetPrefabBounds();
        }

        private void GetPrefabBounds()
        {
            if (VerticalLineupableConfig != null && !VerticalLineupableConfig.WithBounds) return;

            if (!Prefab.TryGetBoundsComponent(out _bounds))
            {
                Debug.LogError("Prefab doesn't contain Bounds_Element component!", gameObject);
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            bool withBounds;
            float spaceBetween;

            if (VerticalLineupableConfig != null)
            {
                withBounds = VerticalLineupableConfig.WithBounds;
                spaceBetween = VerticalLineupableConfig.SpaceBetween;
            }
            else
            {
                withBounds = _bounds != null;
                spaceBetween = 0;
            }

            float boundsExtentsY;
            float boundsSizeY;

            if (withBounds)
            {
                Bounds b = _bounds.Bounds;

                boundsExtentsY = b.extents.y;
                boundsSizeY = b.size.y;
            }
            else
            {
                boundsExtentsY = 0;
                boundsSizeY = 0;
            }

            if (spaceBetween + boundsSizeY == 0)
            {
                Debug.LogError("Bounds size + space between is equal to 0!", gameObject);
                return;
            }

            int diff = Mathf.Max(SpawnMinimum, (int)Mathf.Floor((FillRange.Size.y + boundsExtentsY + spaceBetween) / (boundsSizeY + spaceBetween)));

            for (int i = 0; i < diff; i++)
            {
                base.DynamicExecutor_OnExecute();
            }
        }
    }
}
