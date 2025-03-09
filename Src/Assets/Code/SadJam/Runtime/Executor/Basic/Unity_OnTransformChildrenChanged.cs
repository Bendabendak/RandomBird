using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [ClassTypeAddress("Executor/Unity/OnTransformChildrenChanged")]
    public class Unity_OnTransformChildrenChanged : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected virtual void OnTransformChildrenChanged()
        {
            Execute(Time.deltaTime);
        }
    }
}
