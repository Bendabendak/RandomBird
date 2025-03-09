using SadJam;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/BirdShop/OnSaved")]
    public class BirdShop_OnSaved : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public BirdShop_Config Config { get; }
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

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
                if (Config.SaveKey != null && !data.Verify(Config.SaveKey)) return;
            }

            Execute(Time.deltaTime);
        }
    }
}

