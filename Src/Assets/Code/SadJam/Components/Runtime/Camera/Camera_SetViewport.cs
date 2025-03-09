using System;
using UnityEngine;

namespace SadJam.Components
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class Camera_SetViewport : MonoBehaviour
    {
        public enum ConstraintType { Landscape, Portrait }

        [field: SerializeField]
        public Camera Camera { get; private set; }

        [field: Space, SerializeField]
        public Color WireColor { get; private set; } = Color.white;
        [field: SerializeField]
        public float UnitsSize { get; private set; } = 1; // size of your scene in unity units
        [field: SerializeField]
        public ConstraintType Constraint { get; private set; } = ConstraintType.Portrait;

        [field: Space, Range(0f, 1f), SerializeField]
        public float Adapt { get; private set; } = 1;
        [field: SerializeField]
        public Vector2 MinMaxSize { get; private set; } = Vector2.zero;

        [field: Space, SerializeField]
        public bool Preview { get; private set; }

        protected virtual void Awake()
        {
            if (Camera == null) return;

            ComputeResolution();
        }

        private void ComputeResolution()
        {
            if (Constraint == ConstraintType.Landscape)
            {
                float s = Mathf.Lerp(UnitsSize / 2f, 1f / Camera.aspect * UnitsSize / 2f, Adapt);

                Camera.orthographicSize = s;

                if (MinMaxSize.x != 0 && Screen.width < MinMaxSize.x)
                {
                    s = Mathf.Lerp(UnitsSize / 2f, 1f / (MinMaxSize.x / Screen.height) * UnitsSize / 2f, Adapt);
                    Camera.orthographicSize = s;
                }

                Rect r = Camera.rect;
                if (MinMaxSize.y != 0 && Screen.width > MinMaxSize.y)
                {
                    r.width = MinMaxSize.y / Screen.width;
                }
                else
                {
                    r.width = 1f;
                }

                r.x = 0.5f - r.width / 2f;
                Camera.rect = r;
            }
            else
            {
                Camera.orthographicSize = UnitsSize / 2f;
            }
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (Preview)
            {
                if (Camera == null) return;

                ComputeResolution();
            }
#endif
        }

        protected virtual void OnDrawGizmos()
        {
            if (Camera == null) return;

            Gizmos.color = WireColor;

            Matrix4x4 temp = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            if (Camera.orthographic)
            {
                float spread = Camera.farClipPlane - Camera.nearClipPlane;
                float center = (Camera.farClipPlane + Camera.nearClipPlane) * 0.5f;
                Gizmos.DrawWireCube(new Vector3(0, 0, center), new Vector3(Camera.orthographicSize * 2 * Camera.aspect, Camera.orthographicSize * 2, spread));
            }
            else
            {
                Gizmos.DrawFrustum(Vector3.zero, Camera.fieldOfView, Camera.farClipPlane, Camera.nearClipPlane, Camera.aspect);
            }
            Gizmos.matrix = temp;
        }
    }
}