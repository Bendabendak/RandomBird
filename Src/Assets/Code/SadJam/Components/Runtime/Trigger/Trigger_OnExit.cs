using UnityEngine;
using TypeReferences;
using System.Collections.Generic;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Trigger/OnExit")]
    public class Trigger_OnExit : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected virtual void OnTriggerExit2D(Collider2D collider)
        {
            if (!isActiveAndEnabled) return;

            Execute(Time.deltaTime, new KeyValuePair<string, object>("collider", collider));
        }
    }
}
