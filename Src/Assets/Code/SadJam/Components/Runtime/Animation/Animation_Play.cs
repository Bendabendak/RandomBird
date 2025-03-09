using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Animation/Play")]
    public class Animation_Play : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public AnimationClips Clip { get; private set; }
        [field: SerializeField]
        public int MaxLoopCycles { get; private set; } = -1;

        [field: Space, SerializeField]
        public bool ContinueEvenWhenNotFinished { get; private set; } = false;

        protected override void DynamicExecutor_OnExecute()
        {
            Clip.Play(this, (bool finished) =>
            {
                if (!finished)
                {
                    if (ContinueEvenWhenNotFinished)
                    {
                        Execute(Delta);
                    }
                }
                else
                {
                    Execute(Delta);
                }
            });
        }
    }
}
