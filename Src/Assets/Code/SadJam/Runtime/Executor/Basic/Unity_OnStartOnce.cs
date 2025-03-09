using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [ClassTypeAddress("Executor/Unity/OnStartOnce")]
    [DefaultExecutionOrder(10000)]
    public class Unity_OnStartOnce : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = true,
            OnlyOnePerObject = true
        };

        protected override void StartOnce()
        {
            base.StartOnce();

            Execute(Time.deltaTime);
        }
    }
}
