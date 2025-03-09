using TypeReferences;

namespace SadJam.StateMachine
{
    [ClassTypeAddress("Executor/StateMachine/OnState")]
    public class OnState : StateListener
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void DynamicExecutor_OnExecute()
        {
            if (RunningState == RunningStateType.Running)
            {
                Execute(Delta);
            }
        }
    }
}
