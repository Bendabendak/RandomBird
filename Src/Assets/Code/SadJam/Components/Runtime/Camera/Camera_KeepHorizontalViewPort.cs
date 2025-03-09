using UnityEngine;

namespace SadJam.Components
{
    public class Camera_KeepHorizontalViewPort : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Camera Camera { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            ChangeFov();
        }

        public override void Validate()
        {
            ChangeFov();
        }

        private void ChangeFov()
        {
            if (!Camera) return;

            Camera.fieldOfView = Camera.fieldOfView / ((float)Camera.pixelWidth / Camera.pixelHeight);
        }
    }
}
