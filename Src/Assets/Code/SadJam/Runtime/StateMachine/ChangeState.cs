using UnityEngine;

namespace SadJam.StateMachine
{
    public class ChangeState : StateListener
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
                foreach (Selection_State s in States)
                {
                    s.ChangeStateGlobally(LocalStateHolder, CustomData);
                }
            }
            else
            {
                foreach (Selection_State s in States)
                {
                    s.ChangeState(LocalStateHolder, CustomData);
                }
            }
        }
    }
}