using System;
using UnityEngine;

namespace SadJam.Components
{
    public class Bounds_PrefabFromConfig : GameObjectBounds_Size
    {
        [GameConfigSerializeProperty]
        public IGameConfig_Spawnable Config { get; }

        public override Bounds Bounds => GetBounds();
        public override Vector3 Size => GetBounds().size;

        private Bounds GetBounds()
        {
            if (_bounds == null) return new();

            return _bounds.Bounds;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            SetBounds();
        }

        [NonSerialized]
        private Bounds_Element _bounds;
        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            SetBounds();
        }

        [OnGameConfigChanged(nameof(Config))]
        private void OnConfigChanged(string affected)
        {
            if (GameConfig.IsFieldAffected(affected, nameof(IGameConfig_Spawnable.Prefabs), nameof(IGameConfig_Spawnable)))
            {
                SetBounds();
            }
        }

        private void SetBounds()
        {
            if (Config == null || Config.Prefabs == null)
            {
                _bounds = null;
                return;
            }

            foreach(GameObject g in Config.Prefabs)
            {
                if (!g.TryGetBoundsComponent(out _bounds))
                {
                    _bounds = null;
                    Debug.LogError(g.name + " doesn't contain " + nameof(Bounds_Element) + "!", gameObject);
                    continue;
                }

                break;
            }
        }
    }
}
