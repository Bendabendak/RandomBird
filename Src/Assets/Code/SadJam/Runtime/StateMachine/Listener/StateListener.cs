using System.Collections.Generic;
using UnityEngine;

namespace SadJam.StateMachine
{
    public abstract class StateListener : DynamicExecutor, IListener
    {
        public enum RunningStateType
        {
            Running,
            Idle
        }

        [field: SerializeField]
        public List<Selection_State> States { get; private set; } = new();
        [field: SerializeField]
        public List<Selection_TriggerState> Triggers { get; private set; } = new();

        public RunningStateType RunningState { get; private set; } = RunningStateType.Idle;
        public LocalStateHolder LocalStateHolder { get; private set; }

        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            RegisterToGlobalHolders();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            RegisterToLocalHolder();

            UpdateRunningState();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            RunningState = RunningStateType.Idle;
        }

        protected virtual void OnTransformParentChanged()
        {
            RegisterToLocalHolder();
        }

        private void RegisterToLocalHolder()
        {
            LocalStateHolder holder = GetComponentInParent<LocalStateHolder>();

            if (LocalStateHolder != holder)
            {
                if (LocalStateHolder != null)
                {
                    foreach (Selection_State s in States)
                    {
                        if (!s.Local) continue;

                        LocalStateHolder.RemoveListener(this);
                    }

                    foreach(Selection_TriggerState s in Triggers)
                    {
                        if(!s.Local) continue;

                        LocalStateHolder.RemoveListener(this);
                    }
                }

                if (holder != null)
                {
                    foreach (Selection_State s in States)
                    {
                        if (!s.Local) continue;

                        holder.AddListener(this);
                    }

                    foreach (Selection_TriggerState s in Triggers)
                    {
                        if (!s.Local) continue;

                        holder.AddListener(this);
                    }
                }
            }

            LocalStateHolder = holder;
        }

        private void RegisterToGlobalHolders()
        {
            foreach (Selection_State s in States)
            {
                if (s.Local) continue;

                s.GlobalStateHolder.AddListener(this);
            }

            foreach (Selection_TriggerState s in Triggers)
            {
                if (s.Local) continue;

                s.GlobalStateHolder.AddListener(this);
            }
        }

        protected virtual void OnDisableState()
        {

        }

        protected virtual void OnEnableState()
        {

        }

        protected virtual void OnTriggerState()
        {

        }

        public void OnStateChange(StateCategory category, State.Data newState)
        {
            if (this == null || !isActiveAndEnabled) return;

            RunningStateType r = RunningState;
            UpdateRunningState();

            if (newState.CustomData != null && newState.CustomData != CustomData)
            {
                foreach (KeyValuePair<string, object> kvp in newState.CustomData)
                {
                    CustomData[kvp.Key] = kvp.Value;
                }
            }

            if (RunningState != r)
            {
                switch (RunningState)
                {
                    case RunningStateType.Idle:
                        OnDisableState();
                        break;
                    case RunningStateType.Running:
                        OnEnableState();
                        break;
                }
            }
        }

        public void OnTriggerReleased(StateCategory category, TriggerState trigger)
        {
            if (this == null || !isActiveAndEnabled) return;

            RunningStateType r = RunningState;
            UpdateRunningState();

            if (RunningState != r && RunningState == RunningStateType.Running && Triggers.Count > 0)
            {
                OnTriggerState();
            }
        }
        public void OnTriggerSet(StateCategory category, TriggerState.Data trigger)
        {
            if (this == null || !isActiveAndEnabled) return;

            RunningStateType r = RunningState;
            UpdateRunningState();

            if (trigger.CustomData != null && trigger.CustomData != CustomData)
            {
                foreach (KeyValuePair<string, object> kvp in trigger.CustomData)
                {
                    CustomData[kvp.Key] = kvp.Value;
                }
            }

            if (RunningState != r && RunningState == RunningStateType.Running && Triggers.Count > 0)
            {
                OnTriggerState();
            }
        }

        private void UpdateRunningState()
        {
            bool enabled = false;

            if (States.Count > 0)
            {
                enabled = true;

                foreach(Selection_State s in States)
                {
                    if (!s.Enabled(LocalStateHolder))
                    {
                        enabled = false;
                        break;
                    }
                }

                if (enabled)
                {
                    foreach (Selection_TriggerState s in Triggers)
                    {
                        if (!s.IsSet(LocalStateHolder))
                        {
                            enabled = false;
                            break;
                        }
                    }
                }
            }
            else if(Triggers.Count > 0)
            {
                enabled = true;

                foreach (Selection_TriggerState s in Triggers)
                {
                    if (!s.IsSet(LocalStateHolder))
                    {
                        enabled = false;
                        break;
                    }
                }
            }

            if (RunningState == RunningStateType.Running && !enabled)
            {
                RunningState = RunningStateType.Idle;
            }
            else if (RunningState == RunningStateType.Idle && enabled)
            {
                RunningState = RunningStateType.Running;
            }
        }
    }
}
