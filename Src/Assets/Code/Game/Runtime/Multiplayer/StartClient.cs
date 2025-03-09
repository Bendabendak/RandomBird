using Mirror;
using SadJam;

namespace Game
{
    public class StartClient : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void DynamicExecutor_OnExecute()
        {
            if (NetworkServer.active)
            {
                NetworkServer.Shutdown();
            }

            if (NetworkClient.active)
            {
                NetworkClient.Shutdown();
            }

            Mirror.NetworkManager.singleton.StartClient();
        }
    }
}
