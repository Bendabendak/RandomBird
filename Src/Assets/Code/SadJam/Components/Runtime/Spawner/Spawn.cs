using SadJam.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;
using UnityEngine.Serialization;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Spawner/Spawn")]
    public class Spawn : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Spawn_Config SpawnConfig { get; private set; }

        [Space, SerializeField, FormerlySerializedAs("<Prefab>k__BackingField")]
        private GameObject _prefab;
        public GameObject Prefab 
        {
            get
            {
                if (PrefabFromConfig != null)
                {
                    IEnumerator<GameObject> iterator = PrefabFromConfig.Prefabs.GetEnumerator();
                    if (!iterator.MoveNext()) return null;

                    return iterator.Current;
                }

                return _prefab;
            }
        }
        [GameConfigSerializeProperty]
        public IGameConfig_Spawnable PrefabFromConfig { get; }

        [field: Space, SerializeField]
        public string SpawnName { get; private set; } = "";
        [field: SerializeField, Space]
        public StructComponent<Vector3> SpawnPosition { get; private set; }
        [field: SerializeField]
        public bool UseSpawnerPosition { get; private set; } = false;
        [field: SerializeField]
        public StructComponent<Quaternion> SpawnRotation { get; private set; }
        [field: SerializeField]
        public bool UseSpawnerRotation { get; private set; } = false;
        [field: SerializeField]
        public StructComponent<Vector3> SpawnScale { get; private set; }
        [field: SerializeField]
        public bool UseSpawnerScale { get; private set; } = false;
        [field: SerializeField, Space]
        public GameObject Parent { get; private set; }
        [field: Space, SerializeField, Abs]
        public int SpawnLimit { get; private set; } = 0;

        public List<GameObject> PrefabList { get; private set; } = new();

        public static Action<Spawn> OnSpawned { get; set; }

        [NonSerialized]
        private int _spawnCount = 0;
        protected override void DynamicExecutor_OnExecute()
        {
            if (SpawnLimit > 0 && _spawnCount >= SpawnLimit) return;

            _spawnCount++;
            GameObject spawn = SpawnPrefab(Parent, SpawnPosition, SpawnRotation, SpawnScale, true);
            ActiveteSpawn(spawn);

            OnSpawn(spawn);
        }

        protected virtual void OnSpawn(GameObject spawn) 
        {

        }

        public virtual void ChangePrefab(GameObject prefab)
        {
            if (PrefabFromConfig != null) return;

            _prefab = prefab;
            OnPrefabChanged();
        }

        protected virtual void OnPrefabChanged()
        {

        }

        public virtual void ChangeSpawnName(string name)
        {
            SpawnName = name;
        }

        public virtual void ChangeParentInScene(GameObject parent)
        {
            Parent = parent;
        }

        public virtual void ChangePrefabList(IEnumerable<GameObject> prefabs)
        {
            PrefabList.Clear();

            foreach (GameObject prefab in prefabs)
            {
                if (prefab == null) continue;
                if (PrefabList.Contains(prefab)) continue;

                PrefabList.Add(prefab);
            }
        }

        public virtual void ActiveteSpawn(GameObject spawn)
        {
            if (spawn.TryGetComponent(out LocalStateHolder stateHolder))
            {
                stateHolder.ChangeState(SpawnConfig.SpawnStateCategory, SpawnConfig.SpawnedState);
                stateHolder.SetTrigger(SpawnConfig.SpawnStateCategory, SpawnConfig.AwakeTrigger);

                StaticCoroutine.Start(SpawnCallCor(spawn, stateHolder));
            }
        }

        private IEnumerator SpawnCallCor(GameObject entity, LocalStateHolder holder)
        {
            yield return 0;

            if (entity == null) yield break;

            if (holder != null)
            {
                holder.SetTrigger(SpawnConfig.SpawnStateCategory, SpawnConfig.StartTrigger);
            }
        }

        public GameObject SpawnPrefabFromList(GameObject prefab, GameObject parent, StructComponent<Vector3> spawnPosition, bool worldPositionStays = false) => SpawnPrefabFromList(prefab, parent, spawnPosition, null, null, worldPositionStays);
        public GameObject SpawnPrefabFromList(GameObject prefab, GameObject parent, StructComponent<Vector3> spawnPosition, StructComponent<Vector3> spawnScale, bool worldPositionStays = false) => SpawnPrefabFromList(prefab, parent, spawnPosition, null, spawnScale, worldPositionStays);
        public GameObject SpawnPrefabFromList(GameObject prefab, GameObject parent, StructComponent<Vector3> spawnPosition, StructComponent<Quaternion> spawnRotation, bool worldPositionStays = false) => SpawnPrefabFromList(prefab, parent, spawnPosition, spawnRotation, null, worldPositionStays);
        public GameObject SpawnPrefabFromList(GameObject prefab, GameObject parent, StructComponent<Vector3> spawnPosition, StructComponent<Quaternion> spawnRotation, StructComponent<Vector3> spawnScale, bool worldPositionStays = false)
        {
            Vector3? spawnPos = null;
            Quaternion? spawnRot = null;
            Vector3? spawnSc = null;

            if (spawnPosition != null)
            {
                spawnPos = spawnPosition;
            }

            if (spawnRotation != null)
            {
                spawnRot = spawnRotation;
            }

            if (spawnScale != null)
            {
                spawnSc = spawnScale;
            }

            return SpawnPrefabFromList(prefab, parent, spawnPos, spawnRot, spawnSc, worldPositionStays);
        }
        public GameObject SpawnPrefabFromList(GameObject prefab, GameObject parent, bool worldPositionStays = false) => SpawnPrefabFromList(prefab, parent, null, null, null, worldPositionStays);
        public GameObject SpawnPrefabFromList(GameObject prefab, GameObject parent, Vector3 spawnPosition, bool worldPositionStays = false) => SpawnPrefabFromList(prefab, parent, (Vector3?)spawnPosition, null, null, worldPositionStays);
        public GameObject SpawnPrefabFromList(GameObject prefab, GameObject parent, Vector3 spawnPosition, Vector3 spawnScale, bool worldPositionStays = false) => SpawnPrefabFromList(prefab, parent, (Vector3?)spawnPosition, null, (Vector3?)spawnScale, worldPositionStays);
        public GameObject SpawnPrefabFromList(GameObject prefab, GameObject parent, Vector3 spawnPosition, Quaternion spawnRotation, bool worldPositionStays = false) => SpawnPrefabFromList(prefab, parent, (Vector3?)spawnPosition, (Quaternion?)spawnRotation, null, worldPositionStays);
        public GameObject SpawnPrefabFromList(GameObject prefab, GameObject parent, Vector3 spawnPosition, Quaternion spawnRotation, Vector3 spawnScale, bool worldPositionStays = false) => SpawnPrefabFromList(prefab, parent, (Vector3?)spawnPosition, (Quaternion?)spawnRotation, (Vector3?)spawnScale, worldPositionStays);
        private GameObject SpawnPrefabFromList(GameObject prefab, GameObject parent, Vector3? spawnPosition, Quaternion? spawnRotation, Vector3? spawnScale, bool worldPositionStays = false)
        {
            int prefabIndex = PrefabList.IndexOf(prefab);
            if (prefabIndex < 0)
            {
                Debug.LogError("Prefab not found in the PrefabList. Add prefab to the PrefabList to spawn it!", gameObject);
                return null;
            }

            GameObject p = PrefabList[prefabIndex];
            GameObject spawn = InstantiateAndTransform(p, parent, spawnPosition, spawnRotation, spawnScale, worldPositionStays);

            OnSpawned?.Invoke(this);

            return spawn;
        }

        public GameObject SpawnPrefab(GameObject parent, StructComponent<Vector3> spawnPosition, bool worldPositionStays = false) => SpawnPrefab(parent, spawnPosition, null, null, worldPositionStays);
        public GameObject SpawnPrefab(GameObject parent, StructComponent<Vector3> spawnPosition, StructComponent<Vector3> spawnScale, bool worldPositionStays = false) => SpawnPrefab(parent, spawnPosition, null, spawnScale, worldPositionStays);
        public GameObject SpawnPrefab(GameObject parent, StructComponent<Vector3> spawnPosition, StructComponent<Quaternion> spawnRotation, bool worldPositionStays = false) => SpawnPrefab(parent, spawnPosition, spawnRotation, null, worldPositionStays);
        public GameObject SpawnPrefab(GameObject parent, StructComponent<Vector3> spawnPosition, StructComponent<Quaternion> spawnRotation, StructComponent<Vector3> spawnScale, bool worldPositionStays = false)
        {
            Vector3? spawnPos = null;
            Quaternion? spawnRot = null;
            Vector3? spawnSc = null;

            if (spawnPosition != null)
            {
                spawnPos = spawnPosition;
            }

            if (spawnRotation != null)
            {
                spawnRot = spawnRotation;
            }

            if(spawnScale != null)
            {
                spawnSc = spawnScale;
            }

            return SpawnPrefab(parent, spawnPos, spawnRot, spawnSc, worldPositionStays);
        }
        public GameObject SpawnPrefab(GameObject parent, bool worldPositionStays = false) => SpawnPrefab(parent, null, null, null, worldPositionStays);
        public GameObject SpawnPrefab(GameObject parent, Vector3 spawnPosition, bool worldPositionStays = false) => SpawnPrefab(parent, (Vector3?)spawnPosition, null, null, worldPositionStays);
        public GameObject SpawnPrefab(GameObject parent, Vector3 spawnPosition, Vector3 spawnScale, bool worldPositionStays = false) => SpawnPrefab(parent, (Vector3?)spawnPosition, null, (Vector3?)spawnScale, worldPositionStays);
        public GameObject SpawnPrefab(GameObject parent, Vector3 spawnPosition, Quaternion spawnRotation, bool worldPositionStays = false) => SpawnPrefab(parent, (Vector3?)spawnPosition, (Quaternion?)spawnRotation, null, worldPositionStays);
        public GameObject SpawnPrefab(GameObject parent, Vector3 spawnPosition, Quaternion spawnRotation, Vector3 spawnScale, bool worldPositionStays = false) => SpawnPrefab(parent, (Vector3?)spawnPosition, (Quaternion?)spawnRotation, (Vector3?)spawnScale, worldPositionStays);
        private GameObject SpawnPrefab(GameObject parent, Vector3? spawnPosition, Quaternion? spawnRotation, Vector3? spawnScale, bool worldPositionStays = false)
        {
            GameObject spawn = InstantiateAndTransform(Prefab, parent, spawnPosition, spawnRotation, spawnScale, worldPositionStays);

            OnSpawned?.Invoke(this);

            return spawn;
        }

        private GameObject InstantiateAndTransform(GameObject prefab, GameObject parent, Vector3? spawnPosition, Quaternion? spawnRotation, Vector3? spawnScale, bool worldPositionStays = false)
        {
            Vector3 spawnPos;
            if (UseSpawnerPosition)
            {
                spawnPos = transform.position;
            }
            else
            {
                if (spawnPosition != null)
                {
                    spawnPos = (Vector3)spawnPosition;
                }
                else
                {
                    spawnPos = prefab.transform.position;
                }
            }

            Quaternion spawnRot;
            if (UseSpawnerRotation)
            {
                spawnRot = transform.rotation;
            }
            else
            {
                if (spawnRotation != null)
                {
                    spawnRot = (Quaternion)spawnRotation;
                }
                else
                {
                    spawnRot = prefab.transform.rotation;
                }
            }

            Vector3 spawnSc;
            if (UseSpawnerScale)
            {
                spawnSc = transform.localScale;
            }
            else
            {
                if (spawnScale != null)
                {
                    spawnSc = (Vector3)spawnScale;
                }
                else
                {
                    spawnSc = prefab.transform.localScale;
                }
            }

            Vector3 prefabSc = prefab.transform.localScale;
            prefab.transform.localScale = spawnSc;

            GameObject spawn = Instantiate(prefab);

            prefab.transform.localScale = prefabSc;

            if (!string.IsNullOrWhiteSpace(SpawnName))
            {
                spawn.name = SpawnName;
            }
            else
            {
                spawn.name = prefab.name;
            }

            if (parent != null)
            {
                spawn.transform.SetParent(parent.transform, worldPositionStays);
            }
            else
            {
                spawn.transform.SetParent(null, worldPositionStays);
            }

            spawn.transform.SetPositionAndRotation(spawnPos, spawnRot);

            return spawn;
        }
    }
}
