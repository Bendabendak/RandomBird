using UnityEngine;
using TypeReferences;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Input/OnKey/Up")]
    public class Input_OnKey_Up : DynamicExecutor
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
            if (Input.GetKeyUp(Key))
            {
                Execute(Delta);
            }
        }
    }
}
