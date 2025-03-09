using UnityEngine;
using TypeReferences;
using System.Collections.Generic;

namespace SadJam
{
    [ClassTypeAddress("Executor/Animation/OnEventExecutor")]
    public class Animation_OnEventExecutor : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        public static string CUSTOMDATA_EVENT_ID = "Animation_OnEventExecutor/EventID";

        [field: SerializeField]
        public StringComponent Out_EventID { get; private set; }
        public void ExecuteAnimationEvent(string name)
        {
            if (Out_EventID)
            {
                Out_EventID.Content = name;
            }

            Execute(Time.deltaTime, new KeyValuePair<string, object>(CUSTOMDATA_EVENT_ID, name));
        }
    }
}