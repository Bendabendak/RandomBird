using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [ClassTypeAddress("Executor/Unity/OnEnable")]
    [DefaultExecutionOrder(10000)]
    public class Unity_OnEnable : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = true,
            OnlyOnePerObject = true
        };

        protected override void OnEnable() 
        {
            base.OnEnable();
            Execute(Time.deltaTime);
        }
    }
}
