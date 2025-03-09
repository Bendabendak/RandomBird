using SadJam;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [DefaultExecutionOrder(1000)]
    [ClassTypeAddress("Executor/Game/Multiplayer/OnClientConnected")]
    [CustomStaticExecutor("Gtf2Vqb7FUqemS2Gqhfuxg")]
    public class OnClientConnected : StaticExecutor
    {
        protected override void OnStartAndEnable()
        {
            base.OnStartAndEnable();

            Game.NetworkManager.OnClientConnectedEvent -= OnConnected;
            Game.NetworkManager.OnClientConnectedEvent += OnConnected;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Game.NetworkManager.OnClientConnectedEvent -= OnConnected;
        }

        private void OnConnected()
        {
            Execute(Time.deltaTime);
        }
    }
}
