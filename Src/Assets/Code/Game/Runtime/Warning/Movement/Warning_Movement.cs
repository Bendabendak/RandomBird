using SadJam;
using SadJam.Components;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Warning/Movement")]
    public class Warning_Movement : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Warning_Config Config { get; }

        [field: Space, SerializeField]
        public AnimationClips Clip { get; private set; }
        [field: Space, SerializeField]
        public bool ContinueEvenWhenNotFinished { get; private set; } = false;

        protected override void DynamicExecutor_OnExecute()
        {
            foreach(SadJam.Components.AnimationClip c in Clip.Clips)
            {
                c.LoopCycles = (int)(Config.Duration / c.Clip.length);
            }

            Clip.Play(this, 1, (bool finished) =>
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
