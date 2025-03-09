using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam.StateMachine
{
    public class LocalStateHolder : SadJam.Component, IListener
    {
        [Serializable]
        public class StateCategorySelection
        {
            public StateCategory Category;
            public InheriteType Inherite = InheriteType.None;
        }

        public enum InheriteType
        {
            None,
            FullInherite,
            InheriteOnlyOnChange,
            InheriteOnlyOnParentChange
        }

        [field: SerializeField]
        public List<StateCategorySelection> Categories { get; private set; } = new();

        public Dictionary<string, HashSet<TriggerState.Data>> SetTriggers { get; private set; } = new();
        public Dictionary<string, State.Data> CategoryStates { get; private set; } = new();

        public ReadOnlyHashSet<IListener> Listeners => (ReadOnlyHashSet<IListener>)_listeners;
        [NonSerialized]
        private HashSet<IListener> _listeners = new();
        [NonSerialized]
        private HashSet<IListener> _listenersToAdd = new();
        [NonSerialized]
        private HashSet<IListener> _listenersToRemove = new();

        public LocalStateHolder InheritedStateHolder { get; private set; }

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

        public void ChangeToDefaultState()
        {
            CategoryStates.Clear();

            foreach (StateCategorySelection s in Categories)
            {
                CategoryStates[s.Category.Id] = new(s.Category.DefaultState);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            RegisterToLocalHolder();
        }

        public HashSet<TriggerState.Data> GetSetTriggers(StateCategory category)
        {
#if UNITY_EDITOR
            if (!ContainsCategory(category))
            {
                return new();
            }
#endif
            if (SetTriggers.TryGetValue(category.Id, out HashSet<TriggerState.Data> triggers)) return triggers;

            return new();
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

        public void ChangeStateGlobally(StateCategory category, State state) => ChangeStateGlobally(category, state, null);
        public void ChangeStateGlobally(StateCategory category, State state, Dictionary<string, object> customData)
        {
            StateCategorySelection selection;
            if (!ContainsCategory(category, out selection, false) || !ContainsState(category, state, false)) return;

            if (InheritedStateHolder == null)
            {
                ChangeStatePrivate(category, state, customData);
                return;
            }

            if (selection.Inherite == InheriteType.InheriteOnlyOnParentChange || selection.Inherite == InheriteType.None)
            {
                ChangeStatePrivate(category, state, customData);
            }

            InheritedStateHolder.ChangeStateGlobally(category, state, customData);
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

        public void SetTriggerGlobally(StateCategory category, TriggerState trigger) => SetTriggerGlobally(category, trigger, null);
        public void SetTriggerGlobally(StateCategory category, TriggerState trigger, Dictionary<string, object> customData)
        {
            StateCategorySelection selection;
            if (!ContainsCategory(category, out selection, false) || !ContainsTrigger(category, trigger, false)) return;

            if (InheritedStateHolder == null)
            {
                SetTriggerPrivate(category, trigger, customData);
                ReleaseTriggerPrivate(category, trigger);
                return;
            }

            if (selection.Inherite == InheriteType.InheriteOnlyOnParentChange || selection.Inherite == InheriteType.None)
            {
                SetTriggerPrivate(category, trigger, customData);
                ReleaseTriggerPrivate(category, trigger);
            }

            InheritedStateHolder.SetTriggerGlobally(category, trigger, customData);
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

        protected virtual void OnTransformParentChanged()
        {
            RegisterToLocalHolder();
        }

        private void RegisterToLocalHolder()
        {
            if (transform.parent == null)
            {
                if (InheritedStateHolder != null)
                {
                    InheritedStateHolder.RemoveListener(this);
                }

                InheritedStateHolder = null;

                return;
            }

            LocalStateHolder holder = transform.parent.GetComponentInParent<LocalStateHolder>();

            if (InheritedStateHolder != holder)
            {
                if (InheritedStateHolder != null)
                {
                    InheritedStateHolder.RemoveListener(this);
                }

                if (holder != null)
                {
                    holder.AddListener(this);

                    foreach (StateCategorySelection selection in Categories)
                    {
                        switch (selection.Inherite)
                        {
                            case InheriteType.InheriteOnlyOnParentChange:
                            case InheriteType.FullInherite:
                                foreach (KeyValuePair<string, State.Data> s in holder.CategoryStates)
                                {
                                    if (s.Key == selection.Category.Id)
                                    {
                                        ChangeState(selection.Category, s.Value.State);
                                        break;
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            InheritedStateHolder = holder;
        }

        public void OnStateChange(StateCategory category, State.Data newState)
        {
            if (InheritedStateHolder != null)
            {
                foreach (StateCategorySelection selection in Categories)
                {
                    if (category == selection.Category)
                    {
                        switch (selection.Inherite)
                        {
                            case InheriteType.InheriteOnlyOnChange:
                            case InheriteType.FullInherite:
                                ChangeState(category, newState.State, newState.CustomData);
                                break;
                        }

                        break;
                    }
                }
            }
        }

        public void OnTriggerSet(StateCategory category, TriggerState.Data trigger)
        {
            if (InheritedStateHolder != null)
            {
                foreach (StateCategorySelection selection in Categories)
                {
                    if (category == selection.Category)
                    {
                        switch (selection.Inherite)
                        {
                            case InheriteType.InheriteOnlyOnChange:
                            case InheriteType.FullInherite:
                                SetTrigger(category, trigger.Trigger, trigger.CustomData);
                                break;
                        }

                        break;
                    }
                }
            }
        }

        public void OnTriggerReleased(StateCategory category, TriggerState trigger)
        {
            if (InheritedStateHolder != null)
            {
                foreach (StateCategorySelection selection in Categories)
                {
                    if (category == selection.Category)
                    {
                        switch (selection.Inherite)
                        {
                            case InheriteType.InheriteOnlyOnChange:
                            case InheriteType.FullInherite:
                                ReleaseTriggerPrivate(category, trigger);
                                break;
                        }

                        break;
                    }
                }
            }
        }

        private bool ContainsCategory(StateCategory category, bool withWarning = true) => ContainsCategory(category, out _, withWarning);
        private bool ContainsCategory(StateCategory category, out StateCategorySelection selection, bool withWarning = true)
        {
            selection = null;
            foreach (StateCategorySelection s in Categories)
            {
                if (s.Category == category)
                {
                    selection = s;
                }
            }

            if (selection == null)
            {
                selection = default;

                if (withWarning)
                {
                    Debug.LogWarning("Category " + category.name + " not found in categories list!", gameObject);
                }

                return false;
            }

            return true;
        }

        private bool ContainsTrigger(StateCategory category, TriggerState trigger, bool withWarning = true)
        {
            if (!category.Triggers.Contains(trigger))
            {
                if (withWarning)
                {
                    Debug.LogWarning("Trigger " + trigger.name + " not found in category " + category.name + "!", gameObject);
                }

                return false;
            }

            return true;
        }

        private bool ContainsState(StateCategory category, State state, bool withWarning = true)
        {
            if (!category.States.Contains(state))
            {
                if (withWarning)
                {
                    Debug.LogWarning("State " + state.name + " not found in category " + category.name + "!", gameObject);
                }

                return false;
            }

            return true;
        }
    }
}
