using Mirror;
using SadJam;
using UnityEngine;

namespace Game
{
    public class StartMultiplayerGame : DynamicExecutor
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

            Game.NetworkManager.StartGame();

            Execute(Time.deltaTime);
        }
    }
}
