using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [ClassTypeAddress("Executor/Unity/OnDisable")]
    [DefaultExecutionOrder(10000)]
    public class Unity_OnDisable : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = true,
            OnlyOnePerObject = true
        };

        protected override void OnDisable()
        {
            base.OnDisable();

            Execute(Time.deltaTime);
        }
    }
}
