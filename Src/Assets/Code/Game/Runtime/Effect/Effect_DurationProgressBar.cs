using SadJam;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Effect_DurationProgressBar : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Image Image { get; private set; }

        [field: Space, SerializeField]
        public Effect_Apply RandomEffectApply { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (RandomEffectApply.EffectToApply.Duration <= 0)
            {
                Image.fillAmount = 0;
                return;
            }

            Image.fillAmount = RandomEffectApply.TimeElapsed / RandomEffectApply.EffectToApply.Duration;
        }
    }
}
