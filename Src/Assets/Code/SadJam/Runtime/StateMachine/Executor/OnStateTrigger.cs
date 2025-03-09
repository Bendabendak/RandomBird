using TypeReferences;
using UnityEngine;

namespace SadJam.StateMachine
{
    [ClassTypeAddress("Executor/StateMachine/OnStateTrigger")]
    public class OnStateTrigger : StateListener
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void OnTriggerState() => Execute(Time.deltaTime);
    }
}
