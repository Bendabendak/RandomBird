using System;
using UnityEngine;

namespace SadJam.Components
{
    public class Camera_KeepHorizontalViewPort_Orthographic : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Camera Camera { get; private set; }
        [field: SerializeField]
        public StructComponent<float> DesiredDistance { get; private set; }

        [NonSerialized]
        private Vector2 _lastCameraSize = new();
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
            if (!DesiredDistance || !Camera) return;

            if (_lastCameraSize.x == Camera.pixelWidth && _lastCameraSize.y == Camera.pixelHeight) return;

            _lastCameraSize = new(Camera.pixelWidth, Camera.pixelHeight);

            Camera.orthographicSize = FovToOrthographic(Camera.fieldOfView) / ((float)Camera.pixelWidth / Camera.pixelHeight);
        }

        private float FovToOrthographic(float fov) => DesiredDistance * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
    }
}
