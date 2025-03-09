using SadJam;
using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Shop/Buyable/OnBoughtChanged")]
    public class Shop_Buyable_OnBoughtChanged : DynamicExecutor
    {
        public enum BoughtType
        {
            Bought,
            BoughtOut
        }

        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public IGameConfig_Shop_WithBuyables Config { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public IGameConfig_Shop_Buyable Item { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: Space, SerializeField]
        public BoughtType Bought { get; private set; } = BoughtType.Bought;

        [NonSerialized]
        private bool _bought = false;
        protected override void OnEnable()
        {
            base.OnEnable();

            _bought = false;
            IEnumerable<IGameConfig_Shop_Buyable> boughtList = Config.GetBought(Owner);
            if (boughtList == null) return;

            foreach (IGameConfig_Shop_Buyable c in boughtList)
            {
                if (c == Item)
                {
                    _bought = true;
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

            bool bought = false;
            IEnumerable<IGameConfig_Shop_Buyable> boughtList = Config.GetBought(Owner);
            if (boughtList == null) return;

            foreach (IGameConfig_Shop_Buyable c in boughtList)
            {
                if (c == Item)
                {
                    bought = true;
                    break;
                }
            }

            switch (Bought)
            {
                case BoughtType.Bought:
                    if (bought && !_bought)
                    {
                        _bought = bought;

                        Execute(Time.deltaTime);
                    }
                    break;
                case BoughtType.BoughtOut:
                    if (!bought && _bought)
                    {
                        _bought = bought;

                        Execute(Time.deltaTime);
                    }
                    break;
            }
        }
    }
}

