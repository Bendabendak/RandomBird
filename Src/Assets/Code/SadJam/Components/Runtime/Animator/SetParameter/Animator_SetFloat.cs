using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SadJam.Components
{
    public class Animator_SetFloat : DynamicExecutor
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

            UpdateFloats();
        }

        [ContextMenu("Update Floats")]
        public void UpdateFloats()
        {
            if (Animator == null || Animator.runtimeAnimatorController == null || !Animator.isActiveAndEnabled)
            {
                Parameter.ChangeCollection(null);
                return;
            }

            Animator.Rebind();

            List<string> selection = Animator.parameters.Where(p => p.type == AnimatorControllerParameterType.Float).Select(p => p.name).ToList();
            Parameter.ChangeCollection(selection);
        }

        protected override void DynamicExecutor_OnExecute()
        {
            Animator.SetFloat(Parameter.Selected, Value.Size);
        }
    }
}
