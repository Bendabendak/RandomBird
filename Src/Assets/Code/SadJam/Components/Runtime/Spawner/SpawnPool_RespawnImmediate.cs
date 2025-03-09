
using System;
using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Spawner/RespawnImmediate")]
    public class SpawnPool_RespawnImmediate : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [Flags]
        public enum RespawnType
        {
            None = 0,
            Position = 1,
            Rotation = 2,
            Scale = 4
        }

        [field: SerializeField]
        public RespawnType KeepAfterRespawn { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            Vector3 scale = transform.localScale;

            SpawnPool.DestroyImmediate(gameObject);

            if (this == null || gameObject == null) return;

            SpawnPool.Activate(gameObject, (bool activatedFromThis) =>
            {
                if (!activatedFromThis || this == null || gameObject == null) return;

                if (KeepAfterRespawn.HasFlag(RespawnType.Position))
                {
                    gameObject.transform.position = pos;
                }

                if (KeepAfterRespawn.HasFlag(RespawnType.Rotation))
                {
                    gameObject.transform.rotation = rot;
                }

                if (KeepAfterRespawn.HasFlag(RespawnType.Scale))
                {
                    gameObject.transform.localScale = scale;
                }

                Execute(Delta);
            });
        }
    }
}

