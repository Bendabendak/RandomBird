using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    [Serializable]
    public class AnimationClip
    {  
        public UnityEngine.AnimationClip Clip;
        public Animation Animation;

        [Space]
        public float Speed = 1f;

        [Space]
        public float Delay = 0f;
        public bool PlayTogetherWithBefore = false;

        [Space]
        public int LoopCycles = -1;

        [Space]
        public bool ResetOnEnd = false;

        public bool IsPlaying { get; private set; }

        private static Dictionary<Animation, UnityEngine.AnimationClip> _autoClipsCache = new();

        public void Skip(float time)
        {
            if (Animation == null || Clip == null) return;

            if (!Animation.isActiveAndEnabled)
            {
                return;
            }

            if (Animation.clip != Clip)
            {
                Animation.clip = Clip;
            }

            AnimationState state = Animation[Clip.name];

            state.time = time;
            state.speed = 0f;

            Animation.Play();
        }

        public void SkipToEnd()
        {
            if (Animation == null || Clip == null) return;

            Skip(Clip.length);
        }

        [NonSerialized]
        private Coroutine _lastPlayCoroutine;
        [NonSerialized]
        private MonoBehaviour _lastPlayOn;
        public void Play(MonoBehaviour playOn) => Play(playOn, 1, null);
        public void Play(MonoBehaviour playOn, Action<bool> done) => Play(playOn, 1, done);
        public void Play(MonoBehaviour playOn, float speed) => Play(playOn, speed, null);
        public void Play(MonoBehaviour playOn, float speed, Action<bool> done)
        {
            if (IsPlaying)
            {
                Reset();
            }

            if (IsPlaying)
            {
                done?.Invoke(false);
                return;
            }

            _lastPlayOn = playOn;
            _lastDone = done;
            if (Animation == null || Clip == null || !Animation.isActiveAndEnabled)
            {
                Finish(true);
                return;
            }

            _lastPlayCoroutine = playOn.StartCoroutine(PlayCoroutine(speed, done));
        }

        public void Reset()
        {
            IsPlaying = false;

            ResetAnimation();

            if (_lastPlayCoroutine != null)
            {
                _lastPlayOn.StopCoroutine(_lastPlayCoroutine);
                _lastPlayOn = null;
                _lastPlayCoroutine = null;

                if (_lastDone != null)
                {
                    Action<bool> done = _lastDone;
                    _lastDone = null;

                    done.Invoke(false);
                }
            }
        }

        private void ResetAnimation()
        {
            Animation.Rewind();
            Animation.Play();
            Animation.Sample();
            Animation.Stop();
        }

        [NonSerialized]
        private Action<bool> _lastDone;
        private IEnumerator PlayCoroutine(float speed, Action<bool> done = null)
        {
            _lastDone = done;
            if (Animation.playAutomatically)
            {
                if (!_autoClipsCache.ContainsKey(Animation))
                {
                    _autoClipsCache[Animation] = Animation.clip;
                }
            }

            if (Animation.clip != Clip)
            {
                Animation.clip = Clip;
            }

            float s = Speed * speed;
            Animation[Clip.name].speed = s;

            if (Delay > 0)
            {
                float start = Time.time;
                while (Time.time < start + Delay)
                {
                    yield return null;
                }
            }

            Animation.Rewind();
            Animation.Play();

            if (Clip.wrapMode == WrapMode.Loop)
            {
                if (LoopCycles > -1)
                {
                    float time = 0;
                    int cycleCount = 0;

                    while (cycleCount < LoopCycles)
                    {
                        time += Time.deltaTime;

                        if (time >= Clip.length)
                        {
                            cycleCount++;
                            time = 0;
                        }

                        yield return null;
                    }

                    ResetAnimation();
                    Finish(true);

                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                if (s <= 0)
                {
                    ResetAnimation();
                    Finish(false);

                    yield break;
                }
                else
                {
                    float start = Time.time;
                    while (Time.time < start + Clip.length / s)
                    {
                        yield return null;
                    }
                }
            }

            if (Animation.playAutomatically)
            {
                Animation.clip = _autoClipsCache[Animation];
                Animation.Rewind();
                Animation.Play();
            }

            if (ResetOnEnd)
            {
                ResetAnimation();
            }

            Finish(true);
        }

        private void Finish(bool finished)
        {
            IsPlaying = false;

            if (_lastDone != null)
            {
                Action<bool> done = _lastDone;
                _lastDone = null;

                done.Invoke(finished);
            }
        }
    }
}
