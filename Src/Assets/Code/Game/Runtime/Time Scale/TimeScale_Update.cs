using SadJam;
using UnityEngine;

namespace Game
{
    public class TimeScale_Update : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public TimeScale_Config Config { get; }

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            Time.timeScale = Config.Scale;
        }
    }
}
