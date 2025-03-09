using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SadJam.Components
{
    public class Animator_SetInt : DynamicExecutor
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

            UpdateIntegers();
        }

        [ContextMenu("Update Integers")]
        public void UpdateIntegers()
        {
            if (Animator == null || Animator.runtimeAnimatorController == null || !Animator.isActiveAndEnabled)
            {
                Parameter.ChangeCollection(null);
                return;
            }

            Animator.Rebind();

            List<string> selection = Animator.parameters.Where(p => p.type == AnimatorControllerParameterType.Int).Select(p => p.name).ToList();
            Parameter.ChangeCollection(selection);
        }

        protected override void DynamicExecutor_OnExecute()
        {
            Animator.SetInteger(Parameter.Selected, (int)Value.Size);
        }
    }
}
