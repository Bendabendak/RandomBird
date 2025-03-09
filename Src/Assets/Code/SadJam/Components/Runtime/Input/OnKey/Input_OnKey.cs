using UnityEngine;
using TypeReferences;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Input/OnKey/Click")]
    public class Input_OnKey : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public KeyCode Key { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (Input.GetKey(Key))
            {
                Execute(Delta);
            }
        }
    }
}