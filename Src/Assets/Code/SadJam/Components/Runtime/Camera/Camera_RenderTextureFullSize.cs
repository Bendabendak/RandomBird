using System;
using UnityEngine;

namespace SadJam.Components
{
    [RequireComponent(typeof(Camera))]
    public class Camera_RenderTextureFullSize : MonoBehaviour
    {
        [field: SerializeField]
        public Vector2Int Border { get; private set; }

        [NonSerialized]
        private Camera _cam;

        protected virtual void Awake()
        {
            _cam = GetComponent<Camera>();
        }

        [NonSerialized]
        private float _lastAspect;
        protected virtual void Update()
        {
            if (_cam.targetTexture == null) return;

            float aspect = ((float)Screen.width - Border.x) / Screen.height - Border.y;

            if (_lastAspect != aspect)
            {
                _lastAspect = aspect;
                _cam.aspect = _lastAspect;

                _cam.targetTexture.Release();
                _cam.targetTexture.width = Screen.width - Border.x;
                _cam.targetTexture.height = Screen.height - Border.y;
                _cam.targetTexture.Create();
            }
        }
    }
}
