using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [DefaultExecutionOrder(-10000)]
    [ClassTypeAddress("Executor/Unity/OnAwake")]
    public class Unity_OnAwake : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = true,
            OnlyOnePerObject = true
        };

        protected override void Awake()
        {
            base.Awake();

            Execute(Time.deltaTime);
        }
    }
}
