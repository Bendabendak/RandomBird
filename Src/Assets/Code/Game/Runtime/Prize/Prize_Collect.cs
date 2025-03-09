using SadJam;
using UnityEngine;

namespace Game
{
    public class Prize_Collect : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Prize Prize { get; private set; }

        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: SerializeField]
        public Statistics_Key BalanceKey { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            Prize.CollectPrize(Owner, BalanceKey);
        }
    }
}
