using UnityEngine;
using TypeReferences;
using System;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Collision/OnExit")]
    public class Collision_OnExit : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected virtual void OnCollisionExit2D(Collision2D collision)
        {
            if (!isActiveAndEnabled) return;

            Execute(Time.deltaTime);
        }
    }
}

