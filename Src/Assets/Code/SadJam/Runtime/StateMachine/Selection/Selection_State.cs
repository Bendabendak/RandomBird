using System;
using System.Collections.Generic;

namespace SadJam.StateMachine
{
    [Serializable]
    public class Selection_State
    {
        public bool Local = true;
        public GlobalStateHolder GlobalStateHolder;
        public StateCategory Category;
        public State State;

        public bool Enabled() => Enabled(null);
        public bool Enabled(LocalStateHolder holder)
        {
            if (Local)
            {
                if (holder == null) return false;

                return holder.GetState(Category) == State;
            }

            if (GlobalStateHolder == null) return false;

            return GlobalStateHolder.GetState(Category) == State;
        }

        public void ChangeState() => ChangeState(null, null);
        public void ChangeState(LocalStateHolder holder) => ChangeState(holder, null);
        public void ChangeState(Dictionary<string, object> customData) => ChangeState(null, customData);
        public void ChangeState(LocalStateHolder holder, Dictionary<string, object> customData)
        {
            if (Local)
            {
                if (holder != null)
                {
                    holder.ChangeState(Category, State, customData);
                }
            }
            else
            {
                GlobalStateHolder.ChangeState(Category, State, customData);
            }
        }

        public void ChangeStateGlobally() => ChangeStateGlobally(null, null);
        public void ChangeStateGlobally(Dictionary<string, object> customData) => ChangeStateGlobally(null, customData);
        public void ChangeStateGlobally(LocalStateHolder holder) => ChangeStateGlobally(holder, null);
        public void ChangeStateGlobally(LocalStateHolder holder, Dictionary<string, object> customData)
        {
            if (Local)
            {
                if (holder != null)
                {
                    holder.ChangeStateGlobally(Category, State, customData);
                }
            }
            else
            {
                GlobalStateHolder.ChangeState(Category, State, customData);
            }
        }
    }
}
