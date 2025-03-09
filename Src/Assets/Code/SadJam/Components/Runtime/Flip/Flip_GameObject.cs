using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    [ExecuteInEditMode]
    public class Flip_GameObject : DynamicExecutor
    {
        public enum FlipType
        {
            None,
            Positive,
            Negative
        }

        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public FlipType HorizontalFlip { get; private set; }
        [field: SerializeField]
        public FlipType VerticalFlip { get; private set; }

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

        private static Dictionary<FlipType, Action<Flip_GameObject>> _flipVerticalMap = new(3)
        {
            {
                FlipType.None,
                (Flip_GameObject g)=>{}
            },
            {
                FlipType.Positive,
                (Flip_GameObject g)=>
                {
                    g.transform.localScale = new(g.transform.localScale.x, Mathf.Abs(g.transform.localScale.y), g.transform.localScale.z);
                }
            },
            {
                FlipType.Negative,
                (Flip_GameObject g)=>
                {
                    g.transform.localScale = new(g.transform.localScale.x, -Mathf.Abs(g.transform.localScale.y), g.transform.localScale.z);
                }
            }
        };

        private static Dictionary<FlipType, Action<Flip_GameObject>> _flipHorizontalMap = new(3)
        {
            {
                FlipType.None,
                (Flip_GameObject g)=>{}
            },
            {
                FlipType.Positive,
                (Flip_GameObject g)=>
                {
                    g.transform.localScale = new(Mathf.Abs(g.transform.localScale.x), g.transform.localScale.y, g.transform.localScale.z);
                }
            },
            {
                FlipType.Negative,
                (Flip_GameObject g)=>
                {
                    g.transform.localScale = new(-Mathf.Abs(g.transform.localScale.x), g.transform.localScale.y, g.transform.localScale.z);;
                }
            }
        };

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            _flipVerticalMap[VerticalFlip](this);
            _flipHorizontalMap[HorizontalFlip](this);
        }
    }
}
