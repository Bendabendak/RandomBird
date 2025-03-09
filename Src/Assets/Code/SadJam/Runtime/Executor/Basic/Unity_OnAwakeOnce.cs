using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [DefaultExecutionOrder(-10000)]
    [ClassTypeAddress("Executor/Unity/OnAwakeOnce")]
    public class Unity_OnAwakeOnce : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = true,
            OnlyOnePerObject = true
        };

        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            Execute(Time.deltaTime);
        }
    }
}
