using System;
using UnityEngine;

namespace SadJam.Components
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-9999)]
    public class Camera_SetStretchGameObject : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [Flags]
        public enum ScaleType
        {
            MatchWidth = 1,
            MatchHeight = 2
        }

        [field: SerializeField]
        public ScaleType Stretch { get; private set; } = ScaleType.MatchWidth;
        [field: SerializeField]
        public bool Relative { get; private set; } = false;

        [field: SerializeField]
        public GameObjectBounds_Size BoundsComponent { get; private set; }

        [field: Space, SerializeField]
        public Vector2 Offset { get; private set; }
        [field: SerializeField]
        public Vector2 Multiply { get; private set; } = Vector2.one;

        [field: SerializeField, Space]
        public bool Preview { get; private set; } = false;

        protected override void StartOnce()
        {
            base.StartOnce();

            gameObject.TryGetBoundsComponent(out _bounds);
        }

        [NonSerialized]
        private Bounds_Element _bounds;
        protected override void DynamicExecutor_OnExecute()
        {
            if (BoundsComponent != null)
            {
                _bounds = BoundsComponent;
            }
            else if(_bounds == null)
            {
                if (!gameObject.TryGetBoundsComponent(out _bounds))
                {
                    Debug.LogError("This gameobject doesn't contain Bounds_Element component!", gameObject);
                    return;
                }
            }

            Bounds bounds = _bounds.Bounds;

            float cameraHeight = Camera.main.orthographicSize * 2.0f;
            float cameraWidth = cameraHeight * Camera.main.aspect;
            float diff;

            if (!Relative) 
            {
                if (Stretch.HasFlag(ScaleType.MatchWidth))
                {
                    if (bounds.size.x <= 0) return;

                    diff = (cameraWidth / bounds.size.x * Multiply.x) + Offset.x;

                    transform.localScale = new(diff, transform.localScale.y, transform.localScale.z);
                }

                if (Stretch.HasFlag(ScaleType.MatchHeight))
                {
                    if (bounds.size.y <= 0) return;

                    diff = (cameraHeight / bounds.size.y * Multiply.y) + Offset.y;

                    transform.localScale = new(transform.localScale.x, diff, transform.localScale.z);
                }
            }
            else
            {
                if (Stretch.HasFlag(ScaleType.MatchWidth))
                {
                    if (bounds.size.x <= 0) return;

                    diff = (cameraWidth / bounds.size.x * Multiply.x) + Offset.x;

                    transform.localScale = new(diff, transform.localScale.y - (transform.localScale.x - diff), transform.localScale.z);
                }


                if (Stretch.HasFlag(ScaleType.MatchHeight))
                {
                    if (bounds.size.y <= 0) return;

                    diff = (cameraHeight / bounds.size.y * Multiply.y) + Offset.y;

                    transform.localScale = new(transform.localScale.x - (transform.localScale.y - diff), diff, transform.localScale.z);
                }
            }
        }

        protected override void DynamicExecutor_Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) return;

            if (Preview)
            {
                OnExecute();
            }
#endif
        }
    }
}
