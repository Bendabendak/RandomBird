using SadJam;
using SadJam.Components;
using UnityEngine;

namespace Game
{
    public class Camera_SetAnchorGameObject_WithDirection : Camera_SetAnchorGameObject
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

            Camera cam;
            if (Camera == null)
            {
                cam = Camera.main;
            }
            else
            {
                cam = Camera;
            }

            if (HorizontalDirectionConfig != null && HorizontalDirectionConfig.HorizontalDirection < 0)
            {
                float diff = cam.transform.position.x - transform.position.x;

                transform.position = new(cam.transform.position.x + diff, transform.position.y);
            }

            if (VerticalDirectionConfig != null && VerticalDirectionConfig.VerticalDirection < 0)
            {
                float diff = cam.transform.position.y - transform.position.y;

                transform.position = new(transform.position.x, transform.position.y + diff);
            }
        }
    }
}
