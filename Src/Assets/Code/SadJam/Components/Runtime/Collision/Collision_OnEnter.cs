using UnityEngine;
using System.Collections.Generic;
using TypeReferences;
using System;

namespace SadJam.Components 
{
    [ClassTypeAddress("Executor/Collision/OnEnter")]
    public class Collision_OnEnter : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public int MaxCollisions { get; private set; } = 0;
        [field: SerializeField]
        public float ClearIntervalAfterExit { get; private set; } = 0;

        [NonSerialized]
        private Trigger_LimitController _limitController;
        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            _limitController = new(MaxCollisions);
        }

        [NonSerialized]
        private float _lastExitTime = 0;
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isActiveAndEnabled) return;

            if (ClearIntervalAfterExit > 0 && Time.time - _lastExitTime >= ClearIntervalAfterExit)
            {
                _limitController.Clear();
            }

            if (!_limitController.LimitReached(collision.collider))
            {
                Execute(Time.deltaTime, new KeyValuePair<string, object>("collision", collision), new KeyValuePair<string, object>("collider", collision.otherCollider));
            }
        }

        protected virtual void OnCollisionExit2D(Collision2D collision)
        {
            if (!isActiveAndEnabled) return;

            _lastExitTime = Time.time;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _limitController.Clear();
        }
    }
}
