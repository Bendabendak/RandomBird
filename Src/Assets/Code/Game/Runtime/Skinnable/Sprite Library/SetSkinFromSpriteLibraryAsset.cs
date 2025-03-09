using SadJam;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Game
{
    public class SetSkinFromSpriteLibraryAsset : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public IGameConfig_SkinnableFromSpriteLibraryAsset Config { get; }

        [field: Space, SerializeField]
        public SpriteLibrary Library { get; private set; }

        [field: Space, SerializeField]
        public bool ExecuteOnSkinChange { get; private set; } = false;

        [OnGameConfigChanged(nameof(Config))]
        private void OnConfigChanged(string affected)
        {
            if (!ExecuteOnSkinChange) return;

            if (GameConfig.IsFieldAffected(affected, nameof(IGameConfig_SkinnableFromSpriteLibraryAsset.Skin), nameof(IGameConfig_SkinnableFromSpriteLibraryAsset)))
            {
                OnExecute();
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            SetSkin();
        }

        public void SetSkin()
        {
            Library.spriteLibraryAsset = Config.Skin;
        }
    }
}
