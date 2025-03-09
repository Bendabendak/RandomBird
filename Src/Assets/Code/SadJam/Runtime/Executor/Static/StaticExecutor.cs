using System;
using System.Collections.Generic;
using UnityEngine;
using JacksonDunstanIterator;

namespace SadJam
{
    public abstract class StaticExecutor : Executor
    {
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

        public void AddExecutable(IExecutable e)
        {
            _queueToRemove.Remove(e);
            _queueToAdd.Add(e);
        }

        public void RemoveExecutable(IExecutable e)
        {
            _queueToAdd.Remove(e);
            _queueToRemove.Add(e);
        }

        [NonSerialized]
        private float _lastTime;
        [NonSerialized]
        private float _lastDelta;
        public override void Execute(float delta, params KeyValuePair<string, object>[] customData)
        {
            AddQueueToExecutables();
            
            _lastTime = Time.realtimeSinceStartup;
            _lastDelta = delta;

            foreach (IExecutable e in _runningSorted)
            {
                Behaviour b = (Behaviour)e;
                if (b == null || !b.isActiveAndEnabled) continue;

                if (e.DelayIn > 0 || e.SequentialDelayIn > 0)
                {
                    bool isSequential = e.SequentialDelayIn > 0;
                    if (!isSequential)
                    {
                        Waiter w = new()
                        {
                            Executable = e,
                            ExecutableAsBehaviour = b,
                            WaitTime = e.DelayIn
                        };

                        _waiting.Add(w);
                    }
                    else
                    {
                        bool isFirst = true;
                        foreach (Waiter ww in _waitingSequential)
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
                                WaitTime = e.DelayIn
                            };
                        }
                        else
                        {
                            w = new()
                            {
                                Executable = e,
                                ExecutableAsBehaviour = b,
                                WaitTime = e.SequentialDelayIn,
                            };
                        }

                        _waitingSequential.Add(w);
                    }
                    continue;
                }

                Execute(e, customData);
            }
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

            StaticExecutor_Update();
        }

        protected virtual void StaticExecutor_Update()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomStaticExecutor : Attribute
    {
        public string Id;

        public CustomStaticExecutor(string id) 
        {
            Id = id;
        }
    }
}