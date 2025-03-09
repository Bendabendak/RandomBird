using UnityEngine;

namespace SadJam.Components
{
    public class GameObject_Disable : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public GameObject Target { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            Target.SetActive(false);
        }
    }
}
