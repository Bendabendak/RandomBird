using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Particle/Impact/Spawn")]
    public class Particle_Collision_SpawnPool : SpawnPool
    {
        [field: SerializeField, Space]
        public LayerMask LayersToCollideWith { get; private set; }
        [field: SerializeField]
        public bool ParentToCollider { get; private set; } = true;

        protected override void DynamicExecutor_OnExecute()
        {
            if (CustomData.ContainsKey("collision"))
            {
                if (CustomData["collision"] is Collision2D collision)
                {
                    if (collision.contactCount > 0)
                    {
                        List<ContactPoint2D> contactPoints = new();
                        collision.GetContacts(contactPoints);

                        foreach (ContactPoint2D point in contactPoints)
                        {
                            if ((LayersToCollideWith.value & (1 << point.collider.gameObject.layer)) > 0)
                            {
                                Spawn(Prefab, (EntityInfo spawn) =>
                                {
                                    spawn.Entity.transform.position = new(point.point.x, point.point.y);
                                    spawn.Entity.transform.up = point.normal;
                                    spawn.Entity.transform.localScale = new(Prefab.transform.localScale.x, Prefab.transform.localScale.y);

                                    if (ParentToCollider)
                                    {
                                        spawn.Entity.transform.parent = point.collider.gameObject.transform;
                                    }

                                    Execute(Delta);
                                });
                            }
                        }
                    }

                    return;
                }
            }

            Debug.LogWarning("Collision key not found! Make sure you're using correct executor.", gameObject);
        }
    }
}
