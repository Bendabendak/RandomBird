using SadJam;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Effect_SetImage : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Image Image { get; private set; }

        [GameConfigSerializeProperty]
        public IGameConfig_Effect EffectToApply { get; }

        [field: Space, SerializeField]
        public bool ExecuteOnIconChanged { get; private set; } = false;

        [OnGameConfigChanged(nameof(EffectToApply))]
        private void OnConfigChanged(string affected)
        {
            if (ExecuteOnIconChanged && GameConfig.IsFieldAffected(affected, nameof(IGameConfig_Effect.Icon), nameof(IGameConfig_Effect)))
            {
                OnExecute();
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            Image.sprite = EffectToApply.Icon;
        }
    }
}
