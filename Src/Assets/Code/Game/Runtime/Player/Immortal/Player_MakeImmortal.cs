using SadJam;
using SadJam.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Player_MakeImmortal : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public List<Collider2D> Colliders { get; private set; }
        [field: SerializeField]
        public float Duration { get; private set; }
        [field: SerializeField]
        public AnimationClips ImmortalClip { get; private set; }

        [NonSerialized]
        private Coroutine _activeCoroutine = null;
        [NonSerialized]
        private List<Collider2D> _activeColliders = new();
        [NonSerialized]
        private float _duration = 0;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            _activeCoroutine = null;

            foreach (Collider2D coll in _activeColliders)
            {
                if (coll == null) continue;

                coll.enabled = true;
            }

            _activeColliders.Clear();
        }

        protected override void DynamicExecutor_OnExecute()
        {
            MakeImmortal(Duration);
        }

        public void MakeImmortal(float duration)
        {
            _duration = duration;

            if (_activeCoroutine == null)
            {
                ImmortalClip?.Play(this, 1);

                _activeCoroutine = StartCoroutine(ImmortalCoroutine(Colliders, () =>
                {
                    _activeCoroutine = null;
                }));
            }
        }

        private IEnumerator ImmortalCoroutine(List<Collider2D> colliders, Action done = null)
        {
            _activeColliders.Clear();

            foreach (Collider2D c in colliders)
            {
                if (c.enabled)
                {
                    _activeColliders.Add(c);

                    c.enabled = false;
                }
            }

            while (_duration > 0)
            {
                _duration -= Time.deltaTime;

                yield return null;
            }

            foreach (Collider2D coll in _activeColliders)
            {
                coll.enabled = true;
            }

            done?.Invoke();
        }
    }
}
