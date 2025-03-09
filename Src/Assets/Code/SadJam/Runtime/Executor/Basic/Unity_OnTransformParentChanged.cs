using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [ClassTypeAddress("Executor/Unity/OnTransformParentChanged")]
    public class Unity_OnTransformParentChanged : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected virtual void OnTransformParentChanged()
        {
            Execute(Time.deltaTime);
        }
    }
}

