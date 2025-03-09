namespace SadJam.StateMachine
{
    public interface IListener
    {
        public void OnStateChange(StateCategory category, State.Data newState);
        public void OnTriggerSet(StateCategory category, TriggerState.Data trigger);
        public void OnTriggerReleased(StateCategory category, TriggerState trigger);
    }
}
