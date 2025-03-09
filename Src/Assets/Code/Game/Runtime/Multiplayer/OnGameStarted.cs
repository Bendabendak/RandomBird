using SadJam;
using System;
using UnityEngine;

namespace Game
{
    public class OnGameStarted : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new() 
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void OnStartAndEnable()
        {
            base.OnStartAndEnable();

            Game.NetworkManager.OnGameStarted -= OnStarted;
            Game.NetworkManager.OnGameStarted += OnStarted;

            OnStarted();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Game.NetworkManager.OnGameStarted -= OnStarted;
        }

        private void OnStarted()
        {
            if (Game.NetworkManager.StartGameCounter <= 0)
            {
                Execute(Time.deltaTime);
            }
        }
    }
}
