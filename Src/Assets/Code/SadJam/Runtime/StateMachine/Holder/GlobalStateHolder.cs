using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam.StateMachine
{
    [CreateAssetMenu(fileName = "GlobalStateHolder", menuName = "SadJam/StateMachine/GlobalStateHolder")]
    public class GlobalStateHolder : ScriptableObject
    {
        [field: SerializeField]
        public List<StateCategory> Categories { get; private set; } = new();

        public Dictionary<string, HashSet<TriggerState.Data>> SetTriggers { get; private set; } = new();
        public Dictionary<string, State.Data> CategoryStates { get; private set; } = new();

        public ReadOnlyHashSet<IListener> Listeners => (ReadOnlyHashSet<IListener>)_listeners;
        [NonSerialized]
        private HashSet<IListener> _listeners = new();
        [NonSerialized]
        private HashSet<IListener> _listenersToAdd = new();
        [NonSerialized]
        private HashSet<IListener> _listenersToRemove = new();


        public void AddListener(IListener listener)
        {
            _listenersToRemove.Remove(listener);
            _listenersToAdd.Add(listener);
        }

        public void RemoveListener(IListener listener)
        {
            _listenersToAdd.Remove(listener);
            _listenersToRemove.Add(listener);
        }

        public void ChangeToDefaultState() => ChangeToDefaultState(null);
        public void ChangeToDefaultState(Dictionary<string, object> customData)
        {
            foreach (StateCategory s in Categories)
            {
                CategoryStates[s.Id] = new(s.DefaultState, customData);
            }
        }

        public Dictionary<string, object> GetCustomData(StateCategory category) => GetStateData(category).CustomData;
        public State GetState(StateCategory category) => GetStateData(category).State;
        private State.Data GetStateData(StateCategory category)
        {
#if UNITY_EDITOR
            if (!ContainsCategory(category))
            {
                return default;
            }
#endif
            State.Data state;

            if (CategoryStates.TryGetValue(category.Id, out state)) return state;

            state = new(category.DefaultState);
            return state;
        }

        private void CacheListeners()
        {
            if (_listenersToRemove.Count > 0)
            {
                foreach (IListener l in _listenersToRemove)
                {
                    _listeners.Remove(l);
                }

                _listenersToRemove.Clear();
            }

            if (_listenersToAdd.Count > 0)
            {
                foreach (IListener l in _listenersToAdd)
                {
                    _listeners.Add(l);
                }

                _listenersToAdd.Clear();
            }

            _listeners.RemoveWhere(_listenerNullPredicate);
        }

        [NonSerialized]
        private bool _changingState = false;
        [NonSerialized]
        private Action _stateChanged = null;

        private static Predicate<IListener> _listenerNullPredicate = (IListener l) => l == null;
        public void ChangeState(StateCategory category, State state) => ChangeState(category, state, null);
        public void ChangeState(StateCategory category, State state, Dictionary<string, object> customData)
        {
#if UNITY_EDITOR
            if (!ContainsCategory(category) || !ContainsState(category, state)) return;
#endif
            ChangeStatePrivate(category, state, customData);
        }
        private void ChangeStatePrivate(StateCategory category, State state, Dictionary<string, object> customData)
        {
            if (_changingState)
            {
                _stateChanged += StateChanged;

                void StateChanged()
                {
                    _stateChanged -= StateChanged;

                    ChangeStatePrivate(category, state, customData);
                }

                return;
            }

            _changingState = true;
            State.Data stateData = new(state, customData);
            CategoryStates[category.Id] = stateData;

            CacheListeners();

            foreach (IListener l in _listeners)
            {
                l.OnStateChange(category, stateData);
            }

            _changingState = false;
            _stateChanged?.Invoke();
        }

        public void SetTrigger(StateCategory category, TriggerState trigger) => SetTrigger(category, trigger, null);
        public void SetTrigger(StateCategory category, TriggerState trigger, Dictionary<string, object> customData)
        {
#if UNITY_EDITOR
            if (!ContainsCategory(category) || !ContainsTrigger(category, trigger)) return;
#endif

            SetTriggerPrivate(category, trigger, customData);
            ReleaseTriggerPrivate(category, trigger);
        }

        private void SetTriggerPrivate(StateCategory category, TriggerState trigger, Dictionary<string, object> customData)
        {
            if (_changingState)
            {
                _stateChanged += StateChanged;

                void StateChanged()
                {
                    _stateChanged -= StateChanged;

                    SetTriggerPrivate(category, trigger, customData);
                }

                return;
            }

            _changingState = true;

            if (!SetTriggers.TryGetValue(category.Id, out HashSet<TriggerState.Data> triggers))
            {
                triggers = new();
                SetTriggers[category.Id] = triggers;
            }

            TriggerState.Data triggerData = new(trigger, customData);
            triggers.Add(triggerData);

            CacheListeners();

            foreach (IListener l in Listeners)
            {
                l.OnTriggerSet(category, triggerData);
            }

            _changingState = false;
            _stateChanged?.Invoke();
        }

        private void ReleaseTriggerPrivate(StateCategory category, TriggerState trigger)
        {
            if (SetTriggers.TryGetValue(category.Id, out HashSet<TriggerState.Data> setTriggers))
            {
                if (_changingState)
                {
                    _stateChanged += StateChanged;

                    void StateChanged()
                    {
                        _stateChanged -= StateChanged;

                        ReleaseTriggerPrivate(category, trigger);
                    }

                    return;
                }

                _changingState = true;

                setTriggers.Remove(new(trigger));

                CacheListeners();

                foreach (IListener l in Listeners)
                {
                    l.OnTriggerReleased(category, trigger);
                }

                _changingState = false;
                _stateChanged?.Invoke();
            }
        }

        private bool ContainsCategory(StateCategory category)
        {
            if (!Categories.Contains(category))
            {
                Debug.LogWarning("Category " + category.name + " not found in categories list!", this);
                return false;
            }

            return true;
        }

        private bool ContainsTrigger(StateCategory category, TriggerState trigger)
        {
            if (!category.Triggers.Contains(trigger))
            {
                Debug.LogWarning("Trigger " + trigger.name + " not found in category " + category.name + "!", this);
                return false;
            }

            return true;
        }

        private bool ContainsState(StateCategory category, State state)
        {
            if (!category.States.Contains(state))
            {
                Debug.LogWarning("State " + state.name + " not found in category " + category.name + "!", this);
                return false;
            }

            return true;
        }
    }
}
