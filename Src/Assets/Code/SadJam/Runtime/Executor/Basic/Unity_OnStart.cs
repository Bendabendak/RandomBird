using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [ClassTypeAddress("Executor/Unity/OnStart")]
    [DefaultExecutionOrder(10000)]
    public class Unity_OnStart : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = true,
            OnlyOnePerObject = true
        };

        protected override void Start()
        {
            base.Start();

            Execute(Time.deltaTime);
        }
    }
}
