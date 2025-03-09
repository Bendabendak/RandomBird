using TypeReferences;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/BridgeExecutor")]
    public class BridgeExecutor : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            Execute(Delta);
        }
    }
}
