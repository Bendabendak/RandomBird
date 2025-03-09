using UnityEngine;
using TypeReferences;
using System.Collections.Generic;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Trigger/OnStay")]
    public class Trigger_OnStay : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected virtual void OnTriggerStay2D(Collider2D collider)
        {
            if (!isActiveAndEnabled) return;

            Execute(Time.deltaTime, new KeyValuePair<string, object>("collider", collider));
        }
    }
}
