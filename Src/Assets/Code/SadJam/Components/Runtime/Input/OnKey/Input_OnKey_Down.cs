using UnityEngine;
using TypeReferences;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Input/OnKey/Down")]
    public class Input_OnKey_Down : DynamicExecutor
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
            if (Input.GetKeyDown(Key))
            {
                Execute(Delta);
            }
        }
    }
}
