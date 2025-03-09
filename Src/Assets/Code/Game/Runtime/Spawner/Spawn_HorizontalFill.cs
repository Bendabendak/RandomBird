using SadJam;
using SadJam.Components;
using System;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Spawn/HorizontalFill")]
    public class Spawn_HorizontalFill : SpawnPool
    {
        [field: Space]
        [GameConfigSerializeProperty]
        public IGameConfig_HorizontalLineupable HorizontalLineupableConfig { get; }

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
            if (HorizontalLineupableConfig != null && !HorizontalLineupableConfig.WithBounds) return;

            if (!Prefab.TryGetBoundsComponent(out _bounds))
            {
                Debug.LogError("Prefab doesn't contain Bounds_Element component!", gameObject);
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            bool withBounds;
            float spaceBetween;

            if (HorizontalLineupableConfig != null)
            {
                withBounds = HorizontalLineupableConfig.WithBounds;
                spaceBetween = HorizontalLineupableConfig.SpaceBetween;
            }
            else
            {
                withBounds = _bounds != null;
                spaceBetween = 0;
            }

            float boundsExtentsX;
            float boundsSizeX;

            if (withBounds)
            {
                Bounds b = _bounds.Bounds;

                boundsExtentsX = b.extents.x;
                boundsSizeX = b.size.x;
            }
            else
            {
                boundsExtentsX = 0;
                boundsSizeX = 0;
            }

            if (spaceBetween + boundsSizeX == 0)
            {
                Debug.LogError("Bounds size + space between is equal to 0!", gameObject);
                return;
            }

            int diff = Mathf.Max(SpawnMinimum, (int)Mathf.Floor((FillRange.Size.x + boundsExtentsX + spaceBetween) / (boundsSizeX + spaceBetween)));

            for (int i = 0; i < diff; i++)
            {
                base.DynamicExecutor_OnExecute();
            }
        }
    }
}
