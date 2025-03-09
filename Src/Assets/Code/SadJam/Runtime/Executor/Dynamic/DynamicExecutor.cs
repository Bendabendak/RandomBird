using JacksonDunstanIterator;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SadJam
{
    public abstract class DynamicExecutor : Executor, IExecutable
    {
        public enum ExecutorBehaviourType
        {
            BridgeExecutor,
            OnlyExecutor,
            OnlyExecutable
        }

        public struct ExecutorBehaviour
        {
            public ExecutorBehaviourType Type;
            public bool InGarbage;
            public bool OnlyOnePerObject;

            public ExecutorBehaviour(ExecutorBehaviourType type, bool inGarbage = false, bool onlyOnePerObject = false)
            {
                Type = type;
                InGarbage = inGarbage;
                OnlyOnePerObject = onlyOnePerObject; 
            }
        }

        [field: SerializeField, DebugOnly]
        public List<Behaviour> Executables { get; private set; } = new();
        public Action OnExecuted { get; set; }

        [field: SerializeField, DebugOnly]
        public int ExecutionOrder { get; set; } = 0;
        [field: SerializeField, DebugOnly, FormerlySerializedAs("<Delay>k__BackingField")]
        public float DelayIn { get; set; } = 0;
        [field: SerializeField, DebugOnly]
        public float SequentialDelayIn { get; set; } = 0;
        [field: SerializeField, DebugOnly]
        public float DelayOut { get; set; } = 0;
        [field: SerializeField, DebugOnly]
        public float SequentialDelayOut { get; set; } = 0;
        [field: SerializeField, DebugOnly]
        public List<string> StaticExecutors { get; set; } = new();
        [field: SerializeField, DebugOnly, FormerlySerializedAs("<LocalExecutors>k__BackingField")]
        public List<DynamicExecutor> DynamicExecutors { get; set; } = new();
        public Dictionary<string, object> CustomData { get; set; } = new();
        public float Delta { get; set; }

        public abstract ExecutorBehaviour Behaviour { get; }

        [NonSerialized]
        private HashSet<IExecutable> _queueToAdd = new();
        [NonSerialized]
        private List<IExecutable> _queueToRemove = new();
        [NonSerialized]
        private List<IExecutable> _runningSorted = new();
        [NonSerialized]
        private HashSet<IExecutable> _running = new();
        [NonSerialized]
        private List<Waiter> _waiting = new();
        [NonSerialized]
        private List<Waiter> _waitingSequential = new();

        private struct Waiter
        {
            public IExecutable Executable;
            public Behaviour ExecutableAsBehaviour;
            public float WaitTime;
        }

        [NonSerialized]
        private bool _awaken = false;
        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            Executables.RemoveAll(e => e == null);

            foreach (Behaviour b in Executables)
            {
                IExecutable ie = (IExecutable)b;
                if (_running.Add(ie))
                {
                    _runningSorted.Add(ie);
                }
            }

            _runningSorted.Begin().Sort(_runningSorted.End(), ExecutionOrderComparer);
            _awaken = true;

            foreach (DynamicExecutor l in DynamicExecutors)
            {
                if (!l.Executables.Contains(this))
                {
                    l.AddExecutable(this);
                }
            }
        }

        protected override void OnStartAndEnable()
        {
            base.OnStartAndEnable();

            foreach (string e in StaticExecutors)
            {
                AddToStaticExecutor(e);
            }
        }

        private void AddToStaticExecutor(string id)
        {
            if(!StaticExecutorInitializer.GetExecutor(id, out StaticExecutor ex))
            {
                Debug.LogError("Static executor with id " + id + " not found!");
                return;
            }

            ex.AddExecutable(this);
        }

        private static Predicate<string> _stringNullPredicate = l => l == null;
        private static Predicate<UnityEngine.Object> _unityObjectNullPredicate = l => l == null;
        private static Predicate<Behaviour> _executablesPredicate = b =>
        {
            if (b == null || (b is DynamicExecutor dE && dE.Behaviour.Type == ExecutorBehaviourType.OnlyExecutor))
            {
                return true;
            }

            return false;
        };
        protected override void OnValidate()
        {
            base.OnValidate();

            if (DelayIn < 0)
            {
                DelayIn = 0;
            }

            if (DelayOut < 0)
            {
                DelayOut = 0;
            }

            if (SequentialDelayIn < 0)
            {
                SequentialDelayIn = 0;
            }

            if (SequentialDelayOut < 0)
            {
                SequentialDelayOut = 0;
            }

            if (Behaviour.Type == ExecutorBehaviourType.OnlyExecutor)
            {
                if (DynamicExecutors.Count > 0)
                {
                    DynamicExecutors.Clear();
                }

                if (StaticExecutors.Count > 0)
                {
                    StaticExecutors.Clear();
                }
            }
            else
            {
                Executables.RemoveAll(_executablesPredicate);
                DynamicExecutors.RemoveAll(_unityObjectNullPredicate);
                StaticExecutors.RemoveAll(_stringNullPredicate);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (string ex in StaticExecutors)
            {
                RemoveFromStaticExecutor(ex);
            }
        }

        private void RemoveFromStaticExecutor(string id)
        {
            if (!StaticExecutorInitializer.IdExecutors.TryGetValue(id, out StaticExecutor ex))
            {
                Debug.LogError("Static executor with id " + id + " not found!");
                return;
            }

            ex.RemoveExecutable(this);
        }

        public void OnExecute()
        {
            if (Behaviour.Type == ExecutorBehaviourType.OnlyExecutor) return;

            DynamicExecutor_OnExecute();

            if (Behaviour.Type == ExecutorBehaviourType.OnlyExecutable)
            {
                ExecutePrivate(Delta);
            }
        }

        protected virtual void DynamicExecutor_OnExecute()
        {

        }

        public bool GetCustomData<T>(string key, out T data)
        {
            if (!CustomData.TryGetValue(key, out object s))
            {
                data = default;
                return false;
            }

            if (s is not T sT)
            {
                data = default;
                return false;
            }

            data = sT;
            return true;
        }

        public bool GetNumericalCustomData(string key, out double data)
        {
            if (!CustomData.TryGetValue(key, out object s))
            {
                data = default;
                return false;
            }

            if (!double.TryParse(s.ToString(), out data))
            {
                data = default;
                return false;
            }

            return true;
        }

        public bool AddExecutable(IExecutable e)
        {
            if (e is DynamicExecutor dynamicExecutor)
            {
                if (dynamicExecutor.Behaviour.Type == ExecutorBehaviourType.OnlyExecutor)
                {
                    return false;
                }
            }

            Behaviour b = (Behaviour)e;

            if (!_awaken)
            {
                AddReference(b);
                Executables.Add(b);

#if UNITY_EDITOR
                EditorUtilityExtensions.SetDirty(this, "Executable " + e.GetType().Name + " added to " + GetType().Name + " executor");
#endif
            }
            else
            {
                _queueToRemove.Remove(e);
                _queueToAdd.Add(e);
            }

            return true;
        }

        public void RemoveExecutable(IExecutable e)
        {
            Behaviour b = (Behaviour)e;

            if (!_awaken)
            {
                RemoveReference(b);
                Executables.Remove(b);

#if UNITY_EDITOR
                EditorUtilityExtensions.SetDirty(this, "Executable " + e.GetType().Name + " removed from " + GetType().Name + " executor");
#endif
            }
            else
            {
                _queueToAdd.Remove(e);
                _queueToRemove.Add(e);
            }
        }

        [NonSerialized]
        private float _lastTime;
        [NonSerialized]
        private float _lastDelta;
        public override void Execute(float delta, params KeyValuePair<string, object>[] customData)
        {
            if (Behaviour.Type == ExecutorBehaviourType.OnlyExecutable)
            {
                Debug.LogError("You cannot call " + nameof(Execute) + " in only executable dynamic executor! Change the " + nameof(Behaviour) + " to " + nameof(ExecutorBehaviourType.BridgeExecutor) + " or " + nameof(ExecutorBehaviourType.OnlyExecutor), gameObject);
                return;
            }

            ExecutePrivate(delta, customData);
        }

        private void ExecutePrivate(float delta, params KeyValuePair<string, object>[] customData)
        {
            AddQueueToExecutables();

            _lastTime = Time.realtimeSinceStartup;
            _lastDelta = delta;

            foreach (IExecutable e in _runningSorted)
            {
                Behaviour b = (Behaviour)e;
                if (b == null || !b.isActiveAndEnabled) continue;

                if (DelayOut > 0 || e.DelayIn > 0 || SequentialDelayOut > 0 || e.SequentialDelayIn > 0)
                {
                    bool isSequential = e.SequentialDelayIn > 0 || SequentialDelayOut > 0;
                    if (!isSequential)
                    {
                        Waiter w = new()
                        {
                            Executable = e,
                            ExecutableAsBehaviour = b,
                            WaitTime = e.DelayIn + DelayOut
                        };

                        _waiting.Add(w);
                    }
                    else
                    {
                        bool isFirst = true;
                        foreach(Waiter ww in _waitingSequential)
                        {
                            if (ww.Executable == e)
                            {
                                isFirst = false;
                                break;
                            }
                        }

                        Waiter w;

                        if (isFirst)
                        {
                            w = new()
                            {
                                Executable = e,
                                ExecutableAsBehaviour = b,
                                WaitTime = e.DelayIn + DelayOut
                            };
                        }
                        else
                        {
                            w = new()
                            {
                                Executable = e,
                                ExecutableAsBehaviour = b,
                                WaitTime = e.SequentialDelayIn + SequentialDelayOut,
                            };
                        }

                        _waitingSequential.Add(w);
                    }
                    continue;
                }

                Execute(e, customData);
            }

            OnExecuted?.Invoke();
        }

        private void AddQueueToExecutables()
        {
            if (_queueToRemove.Count > 0)
            {
                foreach (IExecutable e in _queueToRemove)
                {
                    if (_running.Remove(e))
                    {
                        _runningSorted.Remove(e);
                    }
                }

                _queueToRemove.Clear();
            }

            if (_queueToAdd.Count > 0)
            {
                foreach (IExecutable e in _queueToAdd)
                {
                    if (_running.Add(e))
                    {
                        _runningSorted.Add(e);
                    }
                }

                _queueToAdd.Clear();
                _runningSorted.Begin().Sort(_runningSorted.End(), ExecutionOrderComparer);
            }
        }

        private void Execute(IExecutable e, params KeyValuePair<string, object>[] customData)
        {
            e.CustomData = CustomData;

            if (customData != null && customData.Length > 0)
            {
                for (int i = 0; i < customData.Length; i++)
                {
                    e.CustomData[customData[i].Key] = customData[i].Value;
                }
            }

            e.Delta = _lastDelta + (Time.realtimeSinceStartup - _lastTime);

            e.OnExecute();
        }

        [NonSerialized]
        private HashSet<IExecutable> _waitingSequentialChecked = new();
        private void Update()
        {
            if (_waitingSequentialChecked.Count > 0)
            {
                _waitingSequentialChecked.Clear();
            }

            for (int i = 0; i < _waitingSequential.Count;)
            {
                Waiter w = _waitingSequential[i];

                if (_waitingSequentialChecked.Contains(w.Executable))
                {
                    i++;
                    continue;
                }

                w.WaitTime -= Time.deltaTime;
                _waitingSequentialChecked.Add(w.Executable);

                if (w.WaitTime <= 0)
                {
                    if (w.Executable != null && w.ExecutableAsBehaviour.isActiveAndEnabled)
                    {
                        Execute(w.Executable);
                    }

                    _waitingSequential.RemoveAt(i);
                }
                else
                {
                    _waitingSequential[i] = w;
                    i++;
                }
            }

            for (int i = 0; i < _waiting.Count;)
            {
                Waiter w = _waiting[i];

                w.WaitTime -= Time.deltaTime;
                if (w.WaitTime <= 0)
                {
                    if (w.Executable != null && w.ExecutableAsBehaviour.isActiveAndEnabled)
                    {
                        Execute(w.Executable);
                    }

                    _waiting.RemoveAt(i);
                }
                else
                {
                    _waiting[i] = w;
                    i++;
                }
            }

            DynamicExecutor_Update();
        }

        protected virtual void DynamicExecutor_Update()
        {

        }
    }
}
