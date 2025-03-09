using UnityEngine;

namespace SadJam.StateMachine
{
    public class SetTriggerState : StateListener
    {
        [field: SerializeField, Space]
        public bool AffectParents { get; private set; } = false;

        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void DynamicExecutor_OnExecute()
        {
            if (AffectParents)
            {
                foreach (Selection_TriggerState s in Triggers)
                {
                    s.SetGlobally(LocalStateHolder, CustomData);
                }
            }
            else
            {
                foreach (Selection_TriggerState s in Triggers)
                {
                    s.Set(LocalStateHolder, CustomData);
                }
            }
        }
    }
}
