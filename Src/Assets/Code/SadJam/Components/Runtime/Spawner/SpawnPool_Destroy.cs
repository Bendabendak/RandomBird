using System;
using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Spawner/PoolDestroy")]
    public class SpawnPool_Destroy : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
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
            SpawnPool.Destroy(_entityInfo, (bool fromThis) =>
            {
                Execute(Delta);
            });
        }
    }
}
