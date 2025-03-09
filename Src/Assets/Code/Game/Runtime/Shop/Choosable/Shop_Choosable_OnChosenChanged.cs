using SadJam;
using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Shop/Choosable/OnChosenChanged")]
    public class Shop_Choosable_OnChosenChanged : DynamicExecutor
    {
        public enum ChooseType
        {
            Chosen,
            ChosenOut
        }

        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public IGameConfig_Shop_WithChoosables Config { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public IGameConfig_Shop_Choosable Item { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: Space, SerializeField]
        public ChooseType Choose { get; private set; } = ChooseType.Chosen;

        [NonSerialized]
        private bool _chosen = false;
        protected override void OnEnable()
        {
            base.OnEnable();

            _chosen = false;
            IEnumerable<IGameConfig_Shop_Choosable> chosenList = Config.GetChosen(Owner);
            if (chosenList == null) return;

            foreach (IGameConfig_Shop_Choosable c in chosenList)
            {
                if (c == Item)
                {
                    _chosen = true;
                    break;
                }
            }

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
            if (ownerId != Owner.Id) return;

            bool chosen = false;
            IEnumerable<IGameConfig_Shop_Choosable> chosenList = Config.GetChosen(Owner);
            if (chosenList == null) return;

            foreach (IGameConfig_Shop_Choosable c in chosenList)
            {
                if (c == Item)
                {
                    chosen = true;
                    break;
                }
            }

            switch (Choose)
            {
                case ChooseType.Chosen:
                    if (chosen && !_chosen)
                    {
                        _chosen = chosen;

                        Execute(Time.deltaTime);
                    }
                    else
                    {
                        _chosen = chosen;
                    }
                    break;
                case ChooseType.ChosenOut:
                    if (!chosen && _chosen)
                    {
                        _chosen = chosen;

                        Execute(Time.deltaTime);
                    }
                    else
                    {
                        _chosen = chosen;
                    }
                    break;
            }
        }
    }
}
