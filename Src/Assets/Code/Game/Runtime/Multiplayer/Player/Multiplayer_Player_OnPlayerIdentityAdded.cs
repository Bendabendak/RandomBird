using SadJam;
using UnityEngine;

namespace Game
{
    public class Multiplayer_Player_OnPlayerIdentityAdded : DynamicExecutor
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

            foreach (Multiplayer_Player_Identity id in Multiplayer_Player_Identity.PlayerIdentities)
            {
                if (id == null || !id.IsInitialized) continue;

                Execute(Time.deltaTime);
            }

            Multiplayer_Player_Identity.OnIdentityAdded -= OnIdentityAdded;
            Multiplayer_Player_Identity.OnIdentityAdded += OnIdentityAdded;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Multiplayer_Player_Identity.OnIdentityAdded -= OnIdentityAdded;
        }

        private void OnIdentityAdded(Multiplayer_Player_Identity identity)
        {
            Execute(Time.deltaTime);
        }
    }
}