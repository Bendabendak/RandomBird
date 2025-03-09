using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [DefaultExecutionOrder(-10000)]
    [ClassTypeAddress("Executor/Unity/OnDestroy")]
    public class Unity_OnDestroy : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Execute(Time.deltaTime);
        }
    }
}
