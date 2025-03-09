using System;
using UnityEngine;

namespace SadJam.Components
{
    [ExecuteInEditMode]
    public class Offset_GameObject : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Vector3 Offset { get; private set; }

        [field: Space, SerializeField]
        public Vector3 BoundsOffset { get; private set; }
        [field: SerializeField]
        public GameObjectBounds_Size BoundsComponent { get; private set; }

        [field: Space, SerializeField]
        public bool ParentLess { get; private set; } = false;
        [field: SerializeField]
        public bool DestroyWithParent { get; private set; } = false;

        [field: Space, SerializeField]
        public bool Preview { get; private set; }

        [NonSerialized]
        private Transform _parent;
        protected override void Awake()
        {
            if (!Application.isPlaying) return;

            if (DestroyWithParent)
            {
                if (transform.parent == null)
                {
                    _parent = transform;
                }
                else
                {
                    _parent = transform.parent;
                    transform.parent = null;
                }
            }

            if (ParentLess)
            {
                transform.parent = null;
            }

            base.Awake();
        }

        protected override void DynamicExecutor_Update()
        {
            if (Application.isPlaying && DestroyWithParent && _parent == null)
            {
                SpawnPool.Destroy(gameObject);
            }

#if UNITY_EDITOR
            if (Application.isPlaying) return;

            if (Preview)
            {
                OnExecute();
            }
#endif
        }

        [NonSerialized]
        private Bounds_Element _bounds;
        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            if (BoundsComponent != null)
            {
                _bounds = BoundsComponent;
            }

            Vector3 offset = Offset;
            if (BoundsOffset != Vector3.zero)
            {
                if (_bounds == null)
                {
                    if (!gameObject.TryGetBoundsComponent(out _bounds))
                    {
                        _bounds = null;
                        Debug.LogError(gameObject.name + " doesn't contain " + nameof(Bounds_Element) + "!", gameObject);
                        return;
                    }
                }

                Bounds b = _bounds.Bounds;
                offset += new Vector3(b.size.x * BoundsOffset.x, b.size.y * BoundsOffset.y, b.size.z * BoundsOffset.z);
            }

            transform.localPosition = offset;
        }
    }
}
