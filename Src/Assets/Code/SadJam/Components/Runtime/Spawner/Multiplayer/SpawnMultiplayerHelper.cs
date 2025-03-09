#if MIRROR
using Mirror;
using UnityEngine;

namespace SadJam.Components
{
    [RequireComponent(typeof(Spawn))]
    public class SpawnMultiplayerHelper : NetworkBehaviour
    {
        public Spawn Spawner { get; private set; }

        protected virtual void Awake()
        {
            Spawner = GetComponent<Spawn>();
        }

        public void SpawnPrefabOnServer(GameObject spawn)
        {
            if (spawn.GetComponentInChildren<NetworkIdentity>() != null)
            {
                CmdSpawnPrefabOnServer(spawn.transform.position, spawn.transform.rotation, spawn.transform.localScale, netIdentity.connectionToClient);
            }
        }

        public void SpawnPrefabFromListOnServer(int prefabIndexInList, GameObject spawn)
        {
            if (spawn.GetComponentInChildren<NetworkIdentity>() != null)
            {
                CmdSpawnPrefabFromListOnServer(prefabIndexInList, spawn.transform.position, spawn.transform.rotation, spawn.transform.localScale, netIdentity.connectionToClient);
            }
        }

        [Command]
        private void CmdSpawnPrefabOnServer(Vector3 spawnPos, Quaternion spawnRot, Vector3 spawnScale, NetworkConnectionToClient networkConnection)
        {
            GameObject spawn = Instantiate(Spawner.Prefab);
            spawn.transform.localScale = spawnScale;
            spawn.transform.SetPositionAndRotation(spawnPos, spawnRot);

            if (!string.IsNullOrWhiteSpace(Spawner.SpawnName))
            {
                spawn.name = Spawner.SpawnName;
            }
            else
            {
                spawn.name = Spawner.Prefab.name;
            }

            NetworkServer.Spawn(spawn, networkConnection);
        }

        [Command]
        private void CmdSpawnPrefabFromListOnServer(int prefabIndexInList, Vector3 spawnPos, Quaternion spawnRot, Vector3 spawnScale, NetworkConnectionToClient networkConnection)
        {
            if (prefabIndexInList >= Spawner.PrefabList.Count)
            {
                Debug.LogError($"Prefab list index out of range of the PrefabList! Index: {prefabIndexInList}, PrefabList count: {Spawner.PrefabList.Count}", Spawner);
                return;
            }

            GameObject prefab = Spawner.PrefabList[prefabIndexInList];
            GameObject spawn = Instantiate(prefab);
            spawn.transform.localScale = spawnScale;
            spawn.transform.SetPositionAndRotation(spawnPos, spawnRot);

            if (!string.IsNullOrWhiteSpace(Spawner.SpawnName))
            {
                spawn.name = Spawner.SpawnName;
            }
            else
            {
                spawn.name = prefab.name;
            }

            NetworkServer.Spawn(spawn, networkConnection);
        }
    }
}
#endif
