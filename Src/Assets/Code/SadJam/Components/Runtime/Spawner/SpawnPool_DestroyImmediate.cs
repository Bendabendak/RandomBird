using System;
using UnityEngine;

namespace SadJam.Components
{
    public class SpawnPool_DestroyImmediate : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [NonSerialized]
        private SpawnPool.EntityInfo _entityInfo;
        protected override void Start()
        {
            base.Start();

            if (!SpawnPool.GetEntityInfo(gameObject, out _entityInfo))
            {
                Debug.LogError(gameObject.name + " is not part of SpawnPool!", gameObject);
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            SpawnPool.DestroyImmediate(_entityInfo);
        }
    }
}
