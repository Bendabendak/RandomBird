using Mirror;
using SadJam;

namespace Game
{
    public class ShutdownClient : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void DynamicExecutor_OnExecute()
        {
            NetworkClient.Shutdown();
        }
    }
}
