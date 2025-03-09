using SadJam;
using SadJam.Components;
using TypeReferences;
using UnityEngine;

namespace Game 
{
    [ClassTypeAddress("Executor/Game/PlayerDead/Spawn")]
    public class PlayerDead_Spawn : SpawnPool
    {
        [GameConfigSerializeProperty]
        public Player_Config Config { get; }

        [OnGameConfigChanged(nameof(Config))]
        private void OnConfigChanged(string affected)
        {
            if (GameConfig.IsFieldAffected(affected, nameof(Player_Config.PlayerDead)))
            {
                ChangePrefab(Config.PlayerDead);
            }
        }

        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            ChangePrefabImmediate(Config.PlayerDead);
        }

        protected override void SpawnPool_OnEntityActivated(EntityInfo entityInfo)
        {
            base.SpawnPool_OnEntityActivated(entityInfo);

            if (CustomData.ContainsKey("collision")) 
            {
                if (CustomData["collision"] is Collision2D collision) 
                {
                    if (collision.contacts.Length <= 0) return;

                    Rigidbody2D rb = entityInfo.Entity.GetComponentInChildren<Rigidbody2D>();

                    if (rb) 
                    {
                        rb.velocity = Quaternion.AngleAxis(45f, Vector3.forward) * collision.contacts[0].normal * 15f;
                    }
                }
            }
        }
    }
}
