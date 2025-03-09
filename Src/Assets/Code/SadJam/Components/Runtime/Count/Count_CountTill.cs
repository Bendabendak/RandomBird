using TypeReferences;
using UnityEngine;

namespace SadJam.Components 
{
    [ClassTypeAddress("Executor/Count/CountTill")]
    public class Count_CountTill : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public int CountTill { get; private set; }

        private int _count = 0;
        protected override void DynamicExecutor_OnExecute()
        {
            if (_count == CountTill)
            {
                Execute(Delta);
                _count++;
            }

            if(_count < CountTill) 
            {
                _count++;
            }
        }
    }
}
