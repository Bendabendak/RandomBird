using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SadJam.Components
{
    public class Animator_SetTrigger : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field:SerializeField]
        public Animator Animator { get; private set; }
        [field:SerializeField]
        public List<Selection> Triggers { get; private set; } = new();

        public override void Validate()
        {
            base.Validate();

            UpdateTriggers();
        }

        [ContextMenu("Update Triggers")]
        public void UpdateTriggers()
        {
            if (Animator == null || Animator.runtimeAnimatorController == null || !Animator.isActiveAndEnabled)
            {
                foreach (Selection s in Triggers)
                {
                    s.ChangeCollection(null);
                }
                return;
            }

            Animator.Rebind();

            List<string> selection = Animator.parameters.Where(p => p.type == AnimatorControllerParameterType.Trigger).Select(p => p.name).ToList();
            foreach (Selection s in Triggers)
            {
                s.ChangeCollection(selection);
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            foreach(Selection s in Triggers)
            {
                Animator.SetTrigger(s);
            }
        }
    }
}
