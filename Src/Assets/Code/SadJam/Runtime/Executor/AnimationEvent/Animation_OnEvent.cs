using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [ClassTypeAddress("Executor/Animation/OnEvent")]
    public class Animation_OnEvent : DynamicExecutor
    {
        public StringComponent EventID;

        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void DynamicExecutor_OnExecute()
        {
            if (!EventID)
            {
                return;
            }

            if(!GetCustomData(Animation_OnEventExecutor.CUSTOMDATA_EVENT_ID, out string value))
            {
                Debug.LogWarning("Custom data " + Animation_OnEventExecutor.CUSTOMDATA_EVENT_ID + " not found!", gameObject);
                return;
            }

            if (EventID.Content == value)
            {
                Execute(Delta);
            }
        }
    }
}
