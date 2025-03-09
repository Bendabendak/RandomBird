using SadJam;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Statistics/OnChanged")]
    public class Statistics_OnChanged : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [GameConfigSerializeProperty]
        public Statistics_Key SaveKey { get; }

        [field: Space, SerializeField]
        public bool AlwaysExecute { get; private set; } = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            Statistics.OnChanged -= OnChanged;
            Statistics.OnChanged += OnChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Statistics.OnChanged -= OnChanged;
        }

        private void OnChanged(string ownerId, Statistics.DataEntry data)
        {
            if (!AlwaysExecute)
            {
                if (Owner != null && ownerId != Owner.Id) return;
                if (SaveKey != null && !data.Verify(SaveKey)) return;
            }

            Execute(Time.deltaTime);
        }
    }
}
