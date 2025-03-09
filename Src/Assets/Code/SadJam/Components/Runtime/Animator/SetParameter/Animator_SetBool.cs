using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SadJam.Components 
{
    public class Animator_SetBool : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Animator Animator { get; private set; }
        [field: SerializeField]
        public Selection Parameter { get; private set; }
        [field: SerializeField]
        public StructComponent<float> Value { get; private set; }

        public override void Validate()
        {
            base.Validate();

            UpdateBooleans();
        }

        [ContextMenu("Update Booleans")]
        public void UpdateBooleans()
        {
            if (Animator == null || Animator.runtimeAnimatorController == null || !Animator.isActiveAndEnabled)
            {
                Parameter.ChangeCollection(null);
                return;
            }

            Animator.Rebind();

            List<string> selection = Animator.parameters.Where(p => p.type == AnimatorControllerParameterType.Bool).Select(p => p.name).ToList();
            Parameter.ChangeCollection(selection);
        }

        protected override void DynamicExecutor_OnExecute()
        {
            Animator.SetBool(Parameter.Selected, Mathf.Approximately(Mathf.Min(Value.Size, 1), 1));
        }
    }
}
