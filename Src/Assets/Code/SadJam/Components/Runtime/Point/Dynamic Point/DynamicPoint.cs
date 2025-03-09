using System;
using UnityEngine;

namespace SadJam.Components
{
    public abstract class DynamicPoint : StructComponent<Vector3>
    {
        [field: SerializeField]
        public Vector3 Offset { get; private set; }
        [field: Space, SerializeField]
        public bool ParentLess { get; private set; } = true;
        [field: SerializeField]
        public bool DestroyWithParent { get; private set; } = true;

        public override Vector3 Size 
        { 
            get 
            {
                if (!_destroyed)
                {
                    return Point_OnExecute();
                }

                return transform.position;
            }
        }

        [NonSerialized]
        private Transform _parent;
        protected override void Awake()
        {
            _destroyed = false;

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

        [NonSerialized]
        private bool _destroyed = true;
        protected virtual void Update()
        {
            if (DestroyWithParent && _parent == null)
            {
                SpawnPool.Destroy(gameObject);
                _destroyed = true;
            }
        }

        public abstract Vector3 Point_OnExecute();
    }
}
