using SadJam;
using SadJam.Components;
using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Game
{
    public class Spike_VerticalMovement : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Spike_Config Config { get; }
        [GameConfigSerializeProperty]
        public Progress_Config ProgressConfig { get; }

        [field: SerializeField, Space]
        public GameObject SpikeTop { get; private set; }
        [field: SerializeField]
        public GameObject SpikeBottom { get; private set; }
        [field: Space, SerializeField]
        public StructComponent<Vector3> ShowRange { get; private set; }

        [field: Space, SerializeField]
        public StructComponent<Vector3> ShowPoint { get; private set; }

        [field: Space, SerializeField]
        public MinMaxCurve ShowLerpCurve { get; private set; }
        [field: SerializeField]
        public float ShowSpeed { get; private set; } = 1f;

        [field: Space, SerializeField]
        public MinMaxCurve HideLerpCurve { get; private set; }
        [field: SerializeField]
        public float HideSpeed { get; private set; } = 1f;

        [field: Space, SerializeField]
        public MinMaxCurve MoveLerpCurve { get; private set; }

        public float Height { get; private set; } = 0;
        public Coroutine CurrentHeightCoroutine { get; private set; }
        public Action OnShown { get; set; }
        public Action OnHidden { get; set; }
        public Action OnMovementReset { get; set; }

        [NonSerialized]
        private Bounds_Element _spikeTopBounds;
        [NonSerialized]
        private Bounds_Element _spikeBottomBounds;
        [NonSerialized]
        private int _direction = 0;
        [NonSerialized]
        private Game.Random.Identifier _rndId;

        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            _rndId = Game.Random.GetIdentifier();
        }

        protected override void StartOnce()
        {
            base.StartOnce();

            if (!SpikeBottom.TryGetBoundsComponent(out _spikeTopBounds))
            {
                Debug.LogError("SpikeBottom doesn't contain any Bounds_Element component!", SpikeBottom);
            }

            if (!SpikeTop.TryGetBoundsComponent(out _spikeBottomBounds))
            {
                Debug.LogError("SpikeTop doesn't contain any Bounds_Element component!", SpikeTop);
            }
        }

        protected override void Start()
        {
            base.Start();

            ResetMovement();
        }

        public void ResetMovement()
        {
            OnMovementReset?.Invoke();

            float e = ShowRange.Size.y / 2f;
            float top = transform.position.y + e;
            float bottom = transform.position.y - e;
            Bounds topBounds = _spikeTopBounds.Bounds;
            Bounds bottomBounds = _spikeBottomBounds.Bounds;

            SpikeTop.transform.position = new(SpikeTop.transform.position.x, top + topBounds.extents.y);

            SpikeBottom.transform.position = new(SpikeBottom.transform.position.x, bottom - bottomBounds.extents.y);

            _shown = false;
            _showing = false;
            _heightToAddUp = 0.25f;
            _movingVertically = false;
            _direction = Config.HorizontalDirection;
            _frozen = false;
        }

        public void FreezeMovement()
        {
            _frozen = true;

            if (CurrentHeightCoroutine != null)
            {
                StopCoroutine(CurrentHeightCoroutine);

                if (_lastDone != null)
                {
                    _lastDone.Invoke(false);
                    _lastDone = null;
                }
            }
        }

        [NonSerialized]
        private bool _shown = false;
        [NonSerialized]
        private bool _showing = false;
        [NonSerialized]
        private float _heightToAddUp = 0.25f;
        [NonSerialized]
        private bool _movingVertically = false;
        [NonSerialized]
        private bool _frozen = false;
        protected override void DynamicExecutor_OnExecute()
        {
            if (!Config.Enabled || _frozen) return;
            if (_shown)
            {
                if (!_movingVertically && Height > 0 && Config.VerticalSpeed > 0)
                {
                    _movingVertically = true;
                    float targetHeight = Height + _heightToAddUp;
                    if (targetHeight > ShowRange.Size.y || targetHeight < 0)
                    {
                        targetHeight = Height;
                    }
                    ChangeHeight(targetHeight, Config.VerticalSpeed, MoveLerpCurve, (bool completed) =>
                    {
                        if (completed && Config.VerticalSpeed > 0)
                        {
                            targetHeight = Height - _heightToAddUp;
                            if (targetHeight > ShowRange.Size.y || targetHeight < 0)
                            {
                                targetHeight = Height;
                            }

                            ChangeHeight(targetHeight, Config.VerticalSpeed, MoveLerpCurve, (completed) =>
                            {
                                _heightToAddUp = -_heightToAddUp;
                                _movingVertically = false;
                            });
                        }
                    });
                }

                return;
            }

            if (!_showing && ShowPointReached(transform.position.x))
            { 
                ShowSpike();
            }
        }

        private bool ShowPointReached(float pos)
        {
            if (_direction < 0)
            {
                if (pos >= ShowPoint.Size.x)
                {
                    return true;
                }
            }
            else
            {
                if (pos <= ShowPoint.Size.x)
                {
                    return true;
                }
            }

            return false;
        }

        public void ShowSpike(Action<bool> done = null) 
        {
            _showing = true;
            _movingVertically = true;

            ChangeHeight(_rndId.Range(0, ShowRange.Size.y - Config.Gap - Config.VerticalSpeed), ShowSpeed, ShowLerpCurve, (bool completed) =>
            {
                _showing = false;
                _shown = true;
                _movingVertically = false;

                OnShown?.Invoke();

                done?.Invoke(completed);
            });
        }

        public void HideSpike(Action<bool> done = null) 
        {
            _movingVertically = true;

            ChangeHeight(-1, HideSpeed, HideLerpCurve, (bool completed) =>
            {
                _movingVertically = false;

                OnHidden?.Invoke();

                done?.Invoke(completed);
            });
        }

        [NonSerialized]
        private Action<bool> _lastDone;
        private void ChangeHeight(float height, float speed, MinMaxCurve lerpCurve, Action<bool> done = null) 
        {
            if (Height == height)
            {
                Done(true);
                return;
            }

            if (height < 0 && Height < 0) 
            {
                Done(true);
                return;
            }

            if (CurrentHeightCoroutine != null)
            {
                StopCoroutine(CurrentHeightCoroutine);
                CurrentHeightCoroutine = null;

                _lastDone?.Invoke(false);
            }

            _lastDone = done;

            CurrentHeightCoroutine = StartCoroutine(ChangeHeightCor());

            IEnumerator ChangeHeightCor()
            {
                float currentSpikeUpHeight = SpikeTop.transform.position.y;
                float currentSpikeDownHeight = SpikeBottom.transform.position.y;

                float e = ShowRange.Size.y / 2f;
                float top = transform.position.y + e;
                float bottom = transform.position.y - e;
                float rnd = _rndId.Value();
                float t = 0;
                Bounds topBounds = _spikeTopBounds.Bounds;
                Bounds bottomBounds = _spikeBottomBounds.Bounds;
                float topDiff = top + topBounds.extents.y;

                float targetBottomHeight;
                float targetTopHeight;
                if (height <= 0)
                {
                    targetBottomHeight = bottom - bottomBounds.extents.y;
                    targetTopHeight = top + topBounds.extents.y;
                }
                else
                {
                    targetBottomHeight = top - height - Config.Gap - bottomBounds.extents.y;
                    targetTopHeight = top - height + topBounds.extents.y;
                }

                while (t < 1)
                {
                    if (speed <= 0) yield break;

                    SpikeTop.transform.position = new(SpikeTop.transform.position.x, Mathf.LerpUnclamped(currentSpikeUpHeight, targetTopHeight, lerpCurve.Evaluate(t, rnd)));
                    SpikeBottom.transform.position = new(SpikeBottom.transform.position.x, Mathf.LerpUnclamped(currentSpikeDownHeight, targetBottomHeight, lerpCurve.Evaluate(t, rnd)));

                    Height = topDiff - SpikeTop.transform.position.y;

                    t += Time.deltaTime * speed;

                    yield return null;
                }

                SpikeTop.transform.position = new Vector2(SpikeTop.transform.position.x, targetTopHeight);
                SpikeBottom.transform.position = new Vector2(SpikeBottom.transform.position.x, targetBottomHeight);

                Height = height;

                Done(true);
            }

            void Done(bool ok)
            {
                CurrentHeightCoroutine = null;
                _lastDone = null;
                done?.Invoke(ok);
            }
        }
    }
}
