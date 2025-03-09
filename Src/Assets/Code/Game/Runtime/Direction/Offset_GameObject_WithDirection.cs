using SadJam;
using SadJam.Components;
using UnityEngine;

namespace Game
{
    public class Offset_GameObject_WithDirection : Offset_GameObject
    {
        [field: Space]
        [GameConfigSerializeProperty]
        public IGameConfig_HorizontalDirectional HorizontalDirectionConfig { get; }
        [field: SerializeField]
        public bool ExecuteOnHorizontalDirectionChange { get; private set; } = true;

        [field: Space]
        [GameConfigSerializeProperty]
        public IGameConfig_VerticalDirectional VerticalDirectionConfig { get; }
        [field: SerializeField]
        public bool ExecuteOnVerticalDirectionChange { get; private set; } = true;

        [OnGameConfigChanged(nameof(HorizontalDirectionConfig))]
        private void OnHorizontalDirectionConfigChanged(string affected)
        {
            if (ExecuteOnHorizontalDirectionChange && GameConfig.IsFieldAffected(affected, nameof(IGameConfig_HorizontalDirectional.HorizontalDirection), nameof(IGameConfig_HorizontalDirectional)))
            {
                Delta = Time.deltaTime;
                OnExecute();
            }
        }

        [OnGameConfigChanged(nameof(VerticalDirectionConfig))]
        private void OnVerticalDirectionConfigChanged(string affected)
        {
            if (ExecuteOnVerticalDirectionChange && GameConfig.IsFieldAffected(affected, nameof(IGameConfig_VerticalDirectional.VerticalDirection), nameof(IGameConfig_VerticalDirectional)))
            {
                Delta = Time.deltaTime;
                OnExecute();
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            if (HorizontalDirectionConfig != null && HorizontalDirectionConfig.HorizontalDirection < 0)
            {
                transform.localPosition = new(-transform.localPosition.x, transform.localPosition.y);
            }

            if (VerticalDirectionConfig != null && VerticalDirectionConfig.VerticalDirection < 0)
            {
                transform.localPosition = new(transform.localPosition.x, -transform.localPosition.y);
            }
        }
    }
}
