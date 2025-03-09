using SadJam;
using UnityEngine;

namespace Game
{
    public class OnGameStart : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false, 
            OnlyOnePerObject =false
        };

        protected override void OnEnable()
        {
            base.OnEnable();

            Game.NetworkManager.OnGameStart -= OnGameStartCallback;
            Game.NetworkManager.OnGameStart += OnGameStartCallback;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Game.NetworkManager.OnGameStart -= OnGameStartCallback;
        }

        private void OnGameStartCallback()
        {
            Execute(Time.deltaTime);
        }
    }
}
