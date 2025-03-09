using SadJam;
using UnityEngine;

namespace Game
{
    public class Multiplayer_Player_OnStatisticsOwnerSet : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Multiplayer_Player_StatisticsOwner StatisticsOwner { get; private set; }

        protected override void OnStartAndEnable()
        {
            base.OnStartAndEnable();

            StatisticsOwner.OnOwnerChanged -= OnOwnerChanged;
            StatisticsOwner.OnOwnerChanged += OnOwnerChanged;

            if (StatisticsOwner.IsInitialized && StatisticsOwner.Config is Statistics_Owner owner)
            {
                OnOwnerChanged(owner);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            StatisticsOwner.OnOwnerChanged -= OnOwnerChanged;
        }

        private void OnOwnerChanged(Statistics_Owner owner)
        {
            if (owner != null)
            {
                Execute(Time.deltaTime);
            }
        }
    }
}
