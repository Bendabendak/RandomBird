using SadJam.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Spawner/SpawnPool")]
    public class SpawnPool : Spawn
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        public enum SpawnPoolBehaviourType
        {
            Ignore,
            DespawnAllOnDespawn
        }

        public enum EntityStateType
        {
            Disabled,
            Disabling,
            Enabled,
            Enabling
        }

        [field: Space, SerializeField]
        public int PoolExpand { get; private set; } = 1;
        [field: Space, SerializeField]
        public SpawnPoolBehaviourType PoolBehaviour { get; private set; } = SpawnPoolBehaviourType.DespawnAllOnDespawn;

        internal readonly static Dictionary<Type, List<SpawnPool>> _spawnPools = new();

        [NonSerialized]
        protected Dictionary<GameObject, HashSet<EntityInfoPrivate>> _activePool = new();
        [NonSerialized]
        protected uint _activeCount = 0;
        [NonSerialized]
        protected Dictionary<GameObject, HashSet<EntityInfoPrivate>> _disabledPool = new();
        [NonSerialized]
        protected uint _disablingCount = 0;
        [NonSerialized]
        protected Dictionary<GameObject, HashSet<EntityInfoPrivate>> _disablingPool = new();
        [NonSerialized]
        protected uint _disabledCount = 0;
        [NonSerialized]
        protected Dictionary<GameObject, HashSet<EntityInfoPrivate>> _entities = new();
        [NonSerialized]
        protected uint _entitiesCount = 0;

        protected readonly static Dictionary<GameObject, EntityInfoPrivate> _entityData = new();

        public Action<EntityInfo> OnDisabled { get; set; }
        public Action OnDisabledAll { get; set; }

        public Action<EntityInfo> OnActivatedLate { get; set; }
        public Action OnActivatedAllLate { get; set; }
        public Action<EntityInfo> OnActivated { get; set; }
        public Action OnActivatedAll { get; set; }

        public class EntityInfo
        {
            public SpawnPool SpawnPool { get; set; }
            public GameObject Prefab { get; set; }
            public GameObject Entity { get; set; }
            public EntityStateType State { get; set; }
            public Action OnDisableInformed { get; set; }
            public Action OnAboutToDisable { get; set; }
            public Action OnDisabled { get; set; }
            public Action OnAboutToActivate { get; set; }
            public Action OnActivated { get; set; }
            public Action OnCleared { get; set; }
            public bool IsNotEnabled => State == EntityStateType.Disabled || State == EntityStateType.Disabling;
            public bool IsNotDisabled => State == EntityStateType.Enabled || State == EntityStateType.Enabling;
        }

        protected class EntityInfoPrivate : EntityInfo
        {
            public bool FreshSpawned { get; set; }
            public LocalStateHolder StateHolder { get; set; }
            public MonoBehaviour[] Components { get; set; }
            public DynamicExecutor[] DynamicExecutors { get; set; }
            public Queue<TaskInOrder> OrderOfTasks { get; set; }
            public Coroutine OrderOfTasksExecutor { get; set; }
            public SpawnPoolElement PoolElement { get; set; }
        }

        protected struct TaskInOrder
        {
            public IEnumerator Coroutine;
            public Action Task;
        }

        [NonSerialized]
        private static WaitForEndOfFrame _waitForEndOfFrame = new();
        private IEnumerator CoroutineInOrderExecutor(EntityInfoPrivate entityInfo)
        {
            while (true)
            {
                if (entityInfo == null) break;

                if (entityInfo.OrderOfTasks.Count > 0)
                {
                    TaskInOrder t = entityInfo.OrderOfTasks.Dequeue();
                    t.Task?.Invoke();

                    if (t.Coroutine != null)
                    {
                        yield return StaticCoroutine.Start(t.Coroutine);
                    }
                }

                yield return _waitForEndOfFrame;
            }
        }

        protected void AddTaskToExecuteInOrder(EntityInfoPrivate entityInfo, Action action)
        {
            if (entityInfo.OrderOfTasks.Count <= 0)
            {
                action?.Invoke();
                return;
            }

            entityInfo.OrderOfTasks.Enqueue(new() { Task = action });

            if (entityInfo.OrderOfTasksExecutor == null)
            {
                entityInfo.OrderOfTasksExecutor = StaticCoroutine.Start(CoroutineInOrderExecutor(entityInfo));
            }
        }

        protected void AddCoroutineToExecuteInOrder(EntityInfoPrivate entityInfo, IEnumerator enumerator)
        {
            entityInfo.OrderOfTasks.Enqueue(new() { Coroutine = enumerator });

            if (entityInfo.OrderOfTasksExecutor == null)
            {
                entityInfo.OrderOfTasksExecutor = StaticCoroutine.Start(CoroutineInOrderExecutor(entityInfo));
            }
        }

        [NonSerialized]
        private bool _initialized = false;
        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            Type t = GetType();
            if (!_spawnPools.TryGetValue(t, out List<SpawnPool> pools))
            {
                pools = new();
                _spawnPools[t] = pools;
            }

            if (!pools.Contains(this))
            {
                pools.Add(this);
            }

            _initialized = true;

            if (PrefabFromConfig != null)
            {
                ChangePrefabListImmediate(PrefabFromConfig.Prefabs);
            }

            if (_entitiesCount < PoolExpand)
            {
                ExpandPool(PoolExpand);
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            if (PrefabFromConfig != null)
            {
                GameObject spawn = PrefabFromConfig.Spawn(gameObject);

                if (spawn != null)
                {
                    Spawn(spawn, (EntityInfo entity) =>
                    {
                        Execute(Delta);
                    });
                }
            }
            else
            {
                Spawn(Prefab, (EntityInfo entity) =>
                {
                    Execute(Delta);
                });
            }
        }

        [OnGameConfigChanged(nameof(PrefabFromConfig))]
        private void OnConfigChanged(string affected)
        {
            if (GameConfig.IsFieldAffected(affected, nameof(IGameConfig_Spawnable.Prefabs), nameof(IGameConfig_Spawnable)))
            {
                ChangePrefabList(PrefabFromConfig.Prefabs);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (PoolExpand <= 0) 
            {
                PoolExpand = 1;
            }
        }

        protected virtual void OnDespawn()
        {
            if (PoolBehaviour == SpawnPoolBehaviourType.DespawnAllOnDespawn)
            {
                DestroyAll();
            }
        }

        public override void ChangeSpawnName(string name)
        {
            base.ChangeSpawnName(name);

            foreach(HashSet<EntityInfoPrivate> e in _entities.Values)
            {
                foreach(EntityInfoPrivate g in e)
                {
                    g.Entity.name = name;
                }
            }
        }

        public void ChangePrefabImmediate(GameObject prefab)
        {
            base.ChangePrefab(prefab);

            ClearPoolImmediate();
        }

        public override void ChangePrefab(GameObject prefab) => ChangePrefab(prefab, null);
        public void ChangePrefab(GameObject prefab, Action done)
        {
            base.ChangePrefab(prefab);

            ClearPool(done);
        }

        public override void ChangePrefabList(IEnumerable<GameObject> prefabs) => ChangePrefabList(prefabs, null);
        public void ChangePrefabList(IEnumerable<GameObject> prefabs, Action done)
        {
            base.ChangePrefabList(prefabs);

            ClearPool(done);
        }

        public void ChangePrefabListImmediate(IEnumerable<GameObject> prefabs)
        {
            base.ChangePrefabList(prefabs);

            ClearPoolImmediate();
        }

        public void ClearPoolImmediate()
        {
            DestroyAllImmediate();

            RemoveAllEntities();
        }

        public void ClearPool(Action done = null)
        {
            DestroyAll(Done);

            void Done()
            {
                RemoveAllEntities();

                done?.Invoke();
            }
        }

        [NonSerialized]
        private Dictionary<GameObject, HashSet<EntityInfoPrivate>> _removeAllEntitiesCache = new();
        private void RemoveAllEntities()
        {
            foreach (GameObject e in _entities.Keys)
            {
                _activePool.Remove(e);
                _disabledPool.Remove(e);
                _disablingPool.Remove(e);
            }

            _removeAllEntitiesCache.Clear();
            foreach (KeyValuePair<GameObject, HashSet<EntityInfoPrivate>> e in _entities)
            {
                _entityData.Remove(e.Key);

                _removeAllEntitiesCache[e.Key] = e.Value;
            }

            _entities.Clear();
            _entitiesCount = 0;
            _activeCount = 0;
            _disabledCount = 0;
            _disablingCount = 0;

            foreach (HashSet<EntityInfoPrivate> e in _removeAllEntitiesCache.Values)
            {
                foreach (EntityInfoPrivate g in e)
                {
                    g.State = EntityStateType.Disabled;
                    SpawnPool_OnEntityRemoved(g);
                    g.OnCleared?.Invoke();

                    UnityEngine.Object.Destroy(g.Entity);
                }
            }

            ExpandPool(PoolExpand);
        }

        public static void ClearEntityImmediate(EntityInfo entity)
        {
            if (entity is not EntityInfoPrivate entityPrivate)
            {
                Debug.LogError("Invalid entity info!");
                return;
            }

            DestroyImmediate(entityPrivate);
            entityPrivate.SpawnPool.RemoveEntity(entityPrivate);
        }

        public static void ClearEntity(EntityInfo entity, Action done = null)
        {
            if (entity is not EntityInfoPrivate entityPrivate)
            {
                Debug.LogError("Invalid entity info!");
                return;
            }

            Destroy(entity, Done);

            void Done(bool finished)
            {
                entityPrivate.SpawnPool.RemoveEntity(entityPrivate);
                done?.Invoke();
            }
        }

        private void RemoveEntity(EntityInfoPrivate entity)
        {
            _entityData.Remove(entity.Entity);
            _activePool[entity.Prefab].Remove(entity);
            _disabledPool[entity.Prefab].Remove(entity);
            _disablingPool[entity.Prefab].Remove(entity);

            entity.State = EntityStateType.Disabled;

            SpawnPool_OnEntityRemoved(entity);

            UnityEngine.Object.Destroy(entity.Entity);
        }

        protected virtual void SpawnPool_OnEntityRemoved(EntityInfo entity) { }

        [NonSerialized]
        private List<EntityInfoPrivate> _destroyAllCache = new();
        public void DestroyAllImmediate() => DestroyAllImmediate(true, false);
        private void DestroyAllImmediate(bool withCallback, bool forceDisable)
        {
            if (_activePool.Count <= 0 && _disablingPool.Count <= 0) return;

            _destroyAllCache.Clear();

            foreach (HashSet<EntityInfoPrivate> k in _activePool.Values)
            {
                foreach(EntityInfoPrivate e in k)
                {
                    _destroyAllCache.Add(e);
                }
            }

            foreach (HashSet<EntityInfoPrivate> k in _disablingPool.Values)
            {
                foreach (EntityInfoPrivate e in k)
                {
                    _destroyAllCache.Add(e);
                }
            }

            for (int i = _destroyAllCache.Count - 1; i >= 0; i--)
            {
                EntityInfoPrivate entityInfo = _destroyAllCache[i];

                if (entityInfo == null)
                {
                    if (withCallback)
                    {
                        SpawnPool_OnEntityDisabled(entityInfo);
                    }
                    continue;
                }

                if (forceDisable)
                {
                    StaticCoroutine.Start(DisableEntityForce(entityInfo, withCallback));
                }
                else
                {
                    DisableEntity(entityInfo, withCallback);
                }
            }
        }

        public void DestroyAll(Action done = null) => DestroyAll(true, done);
        private void DestroyAll(bool withCallback, Action done = null)
        {
            if (_activeCount <= 0)
            {
                done?.Invoke();
                return;
            }

            _destroyAllCache.Clear();

            foreach (HashSet<EntityInfoPrivate> k in _activePool.Values)
            {
                foreach (EntityInfoPrivate e in k)
                {
                    _destroyAllCache.Add(e);
                }
            }

            int doneCount = 0;
            int destroyCount = _destroyAllCache.Count;

            for (int i = _destroyAllCache.Count - 1; i >= 0; i--)
            {
                EntityInfoPrivate entityInfo = _destroyAllCache[i];

                if (entityInfo == null)
                {
                    if (withCallback)
                    {
                        SpawnPool_OnEntityDisabled(entityInfo);
                    }

                    Done(true);
                    continue;
                }

                InformEntityToDisable(entityInfo, withCallback, Done);
            }

            void Done(bool destroyedFromThis)
            {
                doneCount++;

                if (doneCount >= destroyCount)
                {
                    done?.Invoke();
                }
            }
        }

        public static bool GetEntityInfo(GameObject entity, out EntityInfo info)
        {
            bool result = GetPrivateEntityInfo(entity, out EntityInfoPrivate pInfo);

            info = pInfo;

            return result;
        }

        private static bool GetPrivateEntityInfo(GameObject entity, out EntityInfoPrivate info)
        {
            if (_entityData.TryGetValue(entity, out EntityInfoPrivate data))
            {
                info = data;

                return true;
            }

            info = default;
            return false;
        }

        public new static void DestroyImmediate(UnityEngine.Object obj)
        {
            if (obj is GameObject entity && GetPrivateEntityInfo(entity, out EntityInfoPrivate info))
            {
                DestroyImmediate(info);
            }
            else
            {
                UnityEngine.Object.Destroy(obj);
            }
        }
        public static void DestroyImmediate(EntityInfo entity)
        {
            if (entity is EntityInfoPrivate data)
            {
                data.SpawnPool.DisableEntity(data, true);
            }
            else
            {
                Debug.LogError("Invalid entity info!");
            }
        }

        public static void Destroy(EntityInfo entityInfo) => Destroy(entityInfo, null);
        public static void Destroy(EntityInfo entityInfo, Action<bool> done)
        {
            if (entityInfo is EntityInfoPrivate data)
            {
                data.SpawnPool.InformEntityToDisable(data, true, done);
            }
            else
            {
                Debug.LogError("Invalid entity info!");
            }
        }
        public new static void Destroy(UnityEngine.Object obj) => Destroy(obj, null);
        public static void Destroy(UnityEngine.Object obj, Action<bool> done)
        {
            if (obj is GameObject entity && GetPrivateEntityInfo(entity, out EntityInfoPrivate info))
            {
                Destroy(info, done);
            }
            else
            {
                UnityEngine.Object.Destroy(obj);
                done?.Invoke(true);
            }
        }

        public static bool IsPartOfSpawnPool(GameObject entity)
        {
            return _entityData.ContainsKey(entity);
        }

        protected virtual void SpawnPool_OnSpawn(EntityInfo entityInfo)
        {

        }

        protected virtual void SpawnPool_OnEntityActivated(EntityInfo entityInfo) 
        {

        }

        protected virtual void SpawnPool_OnEntityActivatedLate(EntityInfo entityInfo)
        {

        }

        protected virtual void SpawnPool_OnEntityAboutToActivate(EntityInfo entityInfo) 
        { 

        }

        protected sealed override void OnSpawn(GameObject spaw) { }

        public bool Spawn(GameObject prefab) => Spawn(prefab, null);
        public bool Spawn(GameObject prefab, Action<EntityInfo> done)
        {
            if (SpawnLimit > 0 && _activeCount - _waitingToDisable.Count >= SpawnLimit)
            {
                return false;
            }

            EntityInfoPrivate e;
            if (!GetEntity(prefab, out e)) return false;

            ActivateEntity(e, true, (bool activatedFromThis) => 
            {
                SpawnPool_OnSpawn(e);
                done?.Invoke(e);
            });

            return true;
        }

        public static void Activate(GameObject entity) => Activate(entity, null);
        public static void Activate(GameObject entity, Action<bool> done)
        {
            if (GetPrivateEntityInfo(entity, out EntityInfoPrivate info))
            {
                Activate(info, done);
            }
        }
        public static void Activate(EntityInfo entity) => Activate(entity, null);
        public static void Activate(EntityInfo entity, Action<bool> done)
        {
            if (entity is EntityInfoPrivate data)
            {
                data.SpawnPool.ActivateEntity(data, true, done);
            }
            else
            {
                Debug.LogError("Invalid entity info!");
            }
        }

        private IEnumerable<EntityInfoPrivate> SpawnFreshCycle()
        {
            if (!_initialized) yield break;
            if (Prefab == null && PrefabList.Count <= 0) yield break;

            if (Prefab != null)
            {
                bool prefabWasActive = Prefab.activeSelf;
                if (prefabWasActive)
                {
                    Prefab.SetActive(false);
                }
                GameObject s = SpawnPrefab(null, true);

                Spawn(s, Prefab, out EntityInfoPrivate entityInfo);
                if (prefabWasActive)
                {
                    Prefab.SetActive(true);
                }

                yield return entityInfo;
            }

            for (int prefabIndex = 0; prefabIndex < PrefabList.Count; prefabIndex++)
            {
                GameObject p = PrefabList[prefabIndex];
                if (p == Prefab) continue;
                if (SpawnLimit > 0 && _activeCount - _waitingToDisable.Count >= SpawnLimit) break;

                bool prefabWasActive = p.activeSelf;
                if (prefabWasActive)
                {
                    p.SetActive(false);
                }
                GameObject s = SpawnPrefabFromList(p, null, true);

                Spawn(s, p, out EntityInfoPrivate entityInfo);
                if (prefabWasActive)
                {
                    p.SetActive(true);
                }

                yield return entityInfo;
            }

            void Spawn(GameObject spawn, GameObject prefab, out EntityInfoPrivate entityInfo)
            {
                if (!spawn.TryGetComponent(out SpawnPoolElement poolElement))
                {
                    poolElement = spawn.AddComponent<SpawnPoolElement>();
                }

                entityInfo = new()
                {
                    Entity = spawn,
                    OrderOfTasks = new(),
                    Prefab = prefab,
                    FreshSpawned = true,
                    SpawnPool = this,

                    Components = spawn.GetComponentsInChildren<MonoBehaviour>(),
                    DynamicExecutors = spawn.GetComponentsInChildren<DynamicExecutor>(),

                    StateHolder = spawn.GetComponent<LocalStateHolder>(),

                    State = EntityStateType.Disabled,
                    PoolElement = poolElement
                };

                _entityData[spawn] = entityInfo;

                if (!_entities.TryGetValue(prefab, out HashSet<EntityInfoPrivate> entities))
                {
                    entities = new() { entityInfo };
                    _entities[prefab] = entities;

                    _entitiesCount = 1;
                }
                else
                {
                    if (entities.Add(entityInfo))
                    {
                        _entitiesCount++;
                    }
                }

                spawn.BroadcastMessage("Awake", SendMessageOptions.DontRequireReceiver);
                spawn.SetActive(false);

                DisableEntity(entityInfo, false);

                SpawnPool_OnEntityFreshSpawned(spawn);
                OnSpawned?.Invoke(this);
            }
        }

        public bool ExpandPool(int size)
        {
            for (uint i = 0; i < size;)
            {
                uint iBefore = i;
                foreach (EntityInfoPrivate spawn in SpawnFreshCycle())
                {
                    i++;
                }

                if (i <= iBefore) break;
            }

            return true;
        }

        protected virtual void SpawnPool_OnEntityFreshSpawned(GameObject entity)
        {

        }


        private void DisableEntity(EntityInfoPrivate entityInfo, bool withCallback = true)
        {
            AddCoroutineToExecuteInOrder(entityInfo, DisableEntityForce(entityInfo, withCallback));
        }
        private IEnumerator DisableEntityForce(EntityInfoPrivate entityInfo, bool withCallback = true)
        {
            if (entityInfo.Entity == null) yield break;

            entityInfo.State = EntityStateType.Disabled;

            yield return null;

            if (!_disabledPool.TryGetValue(entityInfo.Prefab, out HashSet<EntityInfoPrivate> disabledEntities))
            {
                disabledEntities = new() { entityInfo };
                _disabledPool[entityInfo.Prefab] = disabledEntities;
                _disabledCount = 1;
            }
            else
            {
                if (disabledEntities.Contains(entityInfo))
                {
                    yield break;
                }

                if (_disabledPool[entityInfo.Prefab].Add(entityInfo))
                {
                    _disabledCount++;
                }
            }

            if (_disablingPool.TryGetValue(entityInfo.Prefab, out HashSet<EntityInfoPrivate> disablingEntities))
            {
                if (disablingEntities.Remove(entityInfo) && _disablingCount > 0)
                {
                    _disablingCount--;
                }
            }

            if (_activePool.TryGetValue(entityInfo.Prefab, out HashSet<EntityInfoPrivate> activeEntities))
            {
                if (activeEntities.Remove(entityInfo) && _activeCount > 0)
                {
                    _activeCount--;
                }
            }

            if (withCallback)
            {
                SpawnPool_OnEntityAboutToDisable(entityInfo);
            }

            entityInfo.OnAboutToDisable?.Invoke();

            if (entityInfo.Entity != null)
            {
                if (entityInfo.Entity != null && entityInfo.Entity.transform != null && entityInfo.Entity.transform.parent != null)
                {
                    entityInfo.Entity.transform.SetParent(null, false);
                }

                if (entityInfo.Entity != null)
                {
                    entityInfo.Entity.SetActive(false);
                }

                entityInfo.Entity.BroadcastMessage("OnDestroy", SendMessageOptions.DontRequireReceiver);

                foreach (MonoBehaviour component in entityInfo.Components)
                {
                    if (component == null) continue;

                    component.StopAllCoroutines();
                }

                if (entityInfo.StateHolder != null)
                {
                    entityInfo.StateHolder.ChangeState(SpawnConfig.SpawnStateCategory, SpawnConfig.DeactivedState);
                }
            }

            if (withCallback)
            {
                SpawnPool_OnEntityDisabled(entityInfo);

                OnDisabled?.Invoke(entityInfo);

                if (_activeCount <= 0 && _disablingCount <= 0)
                {
                    OnDisabledAll?.Invoke();
                }
            }

            if (_waitingToDisable.TryGetValue(entityInfo, out List<Action<bool>> disableActions))
            {
                _waitingToDisable.Remove(entityInfo);

                if (disableActions.Count > 0)
                {
                    disableActions[disableActions.Count - 1]?.Invoke(true);

                    for (int i = disableActions.Count - 2; i >= 0; i--)
                    {
                        disableActions[i]?.Invoke(false);
                    }
                }
            }

            entityInfo.OnDisabled?.Invoke();
        }

        [NonSerialized]
        private Dictionary<EntityInfoPrivate, List<Action<bool>>> _waitingToDisable = new();
        private void InformEntityToDisable(EntityInfoPrivate entityInfo, bool withCallback = true, Action<bool> done = null)
        {
            AddTaskToExecuteInOrder(entityInfo, ()=> { InformEntityToDisableForce(entityInfo, withCallback, done); });
        }
        private void InformEntityToDisableForce(EntityInfoPrivate entityInfo, bool withCallback = true, Action<bool> done = null)
        {
            if (entityInfo.Entity == null) return;

            if (_disabledPool.TryGetValue(entityInfo.Prefab, out HashSet<EntityInfoPrivate> disabledEntities))
            {
                if (disabledEntities.Contains(entityInfo))
                {
                    entityInfo.State = EntityStateType.Disabled;

                    done?.Invoke(false);
                    return;
                }
            }

            if (!_disablingPool.TryGetValue(entityInfo.Prefab, out HashSet<EntityInfoPrivate> disablingEntities))
            {
                disablingEntities = new() { entityInfo };
                _disablingPool[entityInfo.Prefab] = disablingEntities;
                _disablingCount = 1;
            }
            else
            {
                if (disablingEntities.Contains(entityInfo))
                {
                    done?.Invoke(false);
                    return;
                }

                if (disablingEntities.Add(entityInfo))
                {
                    _disablingCount++;
                }
            }

            if (_activePool.TryGetValue(entityInfo.Prefab, out HashSet<EntityInfoPrivate> activeEntities))
            {
                if (activeEntities.Remove(entityInfo) && _activeCount > 0)
                {
                    _activeCount--;
                }
            }

            entityInfo.State = EntityStateType.Disabling;

            if (_waitingToDisable.TryGetValue(entityInfo, out List<Action<bool>> l))
            {
                l.Add(done);
                return;
            }
            
            _waitingToDisable[entityInfo] = new() { done };

            entityInfo.Entity.BroadcastMessage("OnDespawn", SendMessageOptions.DontRequireReceiver);

            if (entityInfo.StateHolder != null)
            {
                entityInfo.StateHolder.ChangeState(SpawnConfig.SpawnStateCategory, SpawnConfig.DespawnedState);
            }

            if (withCallback)
            {
                SpawnPool_OnEntityDisableInformed(entityInfo);
            }

            entityInfo.OnDisableInformed?.Invoke();

            if (entityInfo.StateHolder == null)
            {
                DisableEntity(entityInfo);
            }
        }

        protected virtual void SpawnPool_OnEntityDisabled(EntityInfo entityInfo)
        {

        }

        protected virtual void SpawnPool_OnEntityDisableInformed(EntityInfo entityInfo)
        {

        }

        protected virtual void SpawnPool_OnEntityAboutToDisable(EntityInfo entityInfo)
        {

        }

        private void ActivateEntity(EntityInfoPrivate entityInfo, bool withCallback = true, Action<bool> done = null)
        {
            AddCoroutineToExecuteInOrder(entityInfo, ActivateEntityForce(entityInfo, withCallback, done));
        }
        private IEnumerator ActivateEntityForce(EntityInfoPrivate entityInfo, bool withCallback = true, Action<bool> done = null)
        {
            if (entityInfo.Entity == null) yield break;

            if (entityInfo.State == EntityStateType.Disabling)
            {
                yield return DisableEntityForce(entityInfo, withCallback);
            }

            entityInfo.State = EntityStateType.Enabling;

            if (!_activePool.TryGetValue(entityInfo.Prefab, out HashSet<EntityInfoPrivate> activeEntities))
            {
                activeEntities = new() { entityInfo };
                _activePool[entityInfo.Prefab] = activeEntities;
                _activeCount = 1;
            }
            else
            {
                if (activeEntities.Contains(entityInfo))
                {
                    done?.Invoke(false);
                    yield break;
                }

                if (activeEntities.Add(entityInfo))
                {
                    _activeCount++;
                }
            }

            if (_disabledPool.TryGetValue(entityInfo.Prefab, out HashSet<EntityInfoPrivate> disabledEntities))
            {
                if (disabledEntities.Remove(entityInfo) && _disabledCount > 0)
                {
                    _disabledCount--;
                }
            }

            if (withCallback)
            {
                SpawnPool_OnEntityAboutToActivate(entityInfo);
            }

            entityInfo.OnAboutToActivate?.Invoke();

            foreach (DynamicExecutor e in entityInfo.DynamicExecutors)
            {
                e.CustomData = CustomData;
            }

            yield return null;

            if (entityInfo.Entity != null && entityInfo.Entity.transform != null)
            {
                ResetTransform(entityInfo.Prefab, entityInfo.Entity);
            }

            bool wasActive;
            if (entityInfo.Entity != null)
            {
                wasActive = entityInfo.Entity.activeSelf;

                if (!wasActive)
                {
                    entityInfo.Entity.SetActive(true);
                    entityInfo.Entity.BroadcastMessage("Awake", SendMessageOptions.DontRequireReceiver);
                }

                if (entityInfo.StateHolder != null)
                {
                    entityInfo.StateHolder.ChangeState(SpawnConfig.SpawnStateCategory, SpawnConfig.SpawnedState);
                    entityInfo.StateHolder.SetTrigger(SpawnConfig.SpawnStateCategory, SpawnConfig.AwakeTrigger);
                }
            }
            else
            {
                wasActive = true;
            }

            if (withCallback)
            {
                SpawnPool_OnEntityActivated(entityInfo);

                OnActivated?.Invoke(entityInfo);

                if (_disabledCount <= 0)
                {
                    OnActivatedAll?.Invoke();
                }
            }

            if (!wasActive && !entityInfo.FreshSpawned)
            {
                yield return null;

                if (entityInfo.Entity != null)
                {
                    entityInfo.Entity.BroadcastMessage("Start", SendMessageOptions.DontRequireReceiver);
                }
            }

            entityInfo.State = EntityStateType.Enabled;

            if (entityInfo.Entity != null)
            {
                if (entityInfo.StateHolder != null)
                {
                    entityInfo.StateHolder.SetTrigger(SpawnConfig.SpawnStateCategory, SpawnConfig.StartTrigger);
                }

                if (entityInfo.FreshSpawned)
                {
                    entityInfo.FreshSpawned = false;
                }
            }

            if (withCallback)
            {
                SpawnPool_OnEntityActivatedLate(entityInfo);

                OnActivatedLate?.Invoke(entityInfo);

                if (_disabledCount <= 0)
                {
                    OnActivatedAllLate?.Invoke();
                }
            }

            entityInfo.OnActivated?.Invoke();

            done?.Invoke(true);
        }

        private void ResetTransform(GameObject prefab, GameObject entity)
        {
            if (Parent)
            {
                if (entity.transform.parent != Parent.transform)
                {
                    entity.transform.SetParent(Parent.transform, false);
                }
            }
            else
            {
                if (entity.transform.parent != null)
                {
                    entity.transform.SetParent(null, false);
                }
            }

            if (UseSpawnerRotation)
            {
                entity.transform.rotation = transform.rotation;
            }
            else
            {
                if (SpawnRotation != null)
                {
                    entity.transform.rotation = SpawnRotation.Size;
                }
                else
                {
                    entity.transform.rotation = prefab.transform.rotation;
                }
            }

            if (entity.TryGetComponent(out RectTransform rectT))
            {
                if (UseSpawnerPosition)
                {
                    if (gameObject.TryGetComponent(out RectTransform rectS))
                    {
                        rectT.anchoredPosition = rectS.anchoredPosition;
                    }
                }
                else
                {
                    if (SpawnPosition != null)
                    {
                        rectT.anchoredPosition = SpawnPosition.Size;
                    }
                    else
                    {
                        if (prefab.TryGetComponent(out RectTransform rectP))
                        {
                            rectT.anchoredPosition = rectP.anchoredPosition;
                        }
                    }
                }
            }
            else
            {
                if (UseSpawnerPosition)
                {
                    entity.transform.position = transform.position;
                }
                else
                {
                    if (SpawnPosition != null)
                    {
                        entity.transform.position = SpawnPosition.Size;
                    }
                    else
                    {
                        entity.transform.localPosition = prefab.transform.localPosition;
                    }
                }
            }

            if (UseSpawnerScale)
            {
                entity.transform.localScale = transform.localScale;
            }
            else
            {
                if (SpawnScale != null)
                {
                    entity.transform.localScale = SpawnScale.Size;
                }
                else
                {
                    entity.transform.localScale = prefab.transform.localScale;
                }
            }
        }

        private bool GetEntity(GameObject prefab, out EntityInfoPrivate entityInfo)
        {
#if UNITY_EDITOR
            if (prefab != Prefab && !PrefabList.Contains(prefab))
            {
                Debug.LogError("Prefab " + prefab.name + " is not equal to target prefab and is not contained in the prefab list!", gameObject);
                entityInfo = default;
                return false;
            }
#endif

            if (_disabledPool.TryGetValue(prefab, out HashSet<EntityInfoPrivate> disabled) && disabled != null)
            {
                foreach (EntityInfoPrivate i in disabled)
                {
                    if(i.OrderOfTasks.Count <= 0)
                    {
                        entityInfo = i;
                        return true;
                    }
                }
            }

            entityInfo = default;
            foreach (EntityInfoPrivate data in SpawnFreshCycle())
            {
                if (data.Prefab == prefab)
                {
                    entityInfo = data;
                }
            }

            if (entityInfo == default)
            {
                return false;
            }

            return true;
        }
    }
}
