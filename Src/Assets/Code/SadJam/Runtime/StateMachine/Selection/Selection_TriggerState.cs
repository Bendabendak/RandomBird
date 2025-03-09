using System;
using System.Collections.Generic;

namespace SadJam.StateMachine
{
    [Serializable]
    public class Selection_TriggerState
    {
        public bool Local = true;
        public GlobalStateHolder GlobalStateHolder;
        public StateCategory Category;
        public TriggerState Trigger;

        public bool IsSet() => IsSet(null);
        public bool IsSet(LocalStateHolder holder)
        {
            HashSet<TriggerState.Data> s;
            if (Local)
            {
                if (holder == null) return false;

                if (holder.SetTriggers.TryGetValue(Category.Id, out s))
                {
                    return s.Contains(new(Trigger));
                }

                return false;
            }

            if (GlobalStateHolder == null) return false;

            if (GlobalStateHolder.SetTriggers.TryGetValue(Category.Id, out s))
            {
                return s.Contains(new(Trigger));
            }

            return false;
        }

        public void Set() => Set(null, null);
        public void Set(Dictionary<string, object> customData) => Set(null, customData);
        public void Set(LocalStateHolder holder) => Set(holder, null);
        public void Set(LocalStateHolder holder, Dictionary<string, object> customData)
        {
            if (Local)
            {
                if (holder != null)
                {
                    holder.SetTrigger(Category, Trigger, customData);
                }
            }
            else
            {
                GlobalStateHolder.SetTrigger(Category, Trigger, customData);
            }
        }

        public void SetGlobally() => SetGlobally(null, null);
        public void SetGlobally(Dictionary<string, object> customData) => SetGlobally(null, customData);
        public void SetGlobally(LocalStateHolder holder) => SetGlobally(holder, null);
        public void SetGlobally(LocalStateHolder holder, Dictionary<string, object> customData)
        {
            if (Local)
            {
                if (holder != null)
                {
                    holder.SetTriggerGlobally(Category, Trigger, customData);
                }
            }
            else
            {
                GlobalStateHolder.SetTrigger(Category, Trigger, customData);
            }
        }
    }
}

