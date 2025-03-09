using UnityEngine;

namespace SadJam.Components
{
    public class Game_SetTargetFrameRate : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field:SerializeField]
        public StructComponent<float> FrameRate { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            Application.targetFrameRate = (int)FrameRate.Size;
        }
    }
}
