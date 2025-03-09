using System;
using UnityEngine;

namespace SadJam.Components
{
    public class SpawnPoolElement : MonoBehaviour
    {
        [field: NonSerialized]
        public SpawnPool.EntityInfo EntityInfo { get; private set; }

        protected virtual void Start()
        {
            if (!SpawnPool.GetEntityInfo(gameObject, out SpawnPool.EntityInfo entityInfo))
            {
                Debug.LogError(gameObject.name + " is not part of SpawnPool!", gameObject);
                return;
            }

            if (entityInfo == null || entityInfo.SpawnPool == null)
            {
                Destroy(this);
            }

            EntityInfo = entityInfo;
        }

        protected virtual void OnDisable()
        {
            if (EntityInfo != null && EntityInfo.SpawnPool != null && (EntityInfo.State != SpawnPool.EntityStateType.Disabled || transform.parent != null))
            {
                SpawnPool.DestroyImmediate(EntityInfo);
            }
        }
    }
}
