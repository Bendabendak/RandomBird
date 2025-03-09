using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components 
{
    [Serializable]
    public class AnimationClips
    {
        public List<SadJam.Components.AnimationClip> Clips;
        
        public bool IsPlaying { get; private set; } 

        public void Reset()
        {
            IsPlaying = false;

            foreach (SadJam.Components.AnimationClip c in Clips)
            {
                c.Reset();
            }
        }

        public void Skip(float time)
        {
            float t = 0;

            foreach(SadJam.Components.AnimationClip c in Clips)
            {
                t += c.Clip.length;

                if (t >= time)
                {
                    c.Skip(c.Clip.length - (t - time));
                    return;
                }
                else
                {
                    c.SkipToEnd();
                }
            }
        }

        public void SkipToEnd()
        {
            foreach (SadJam.Components.AnimationClip c in Clips)
            {
                c.SkipToEnd();
            }
        }

        [NonSerialized]
        private int _clipsFinishedCount = 0;
        [NonSerialized]
        private int _clipsCount = 0;
        [NonSerialized]
        private Action<bool> _lastDone;
        [NonSerialized]
        private SadJam.Components.AnimationClip _nextClip;
        [NonSerialized]
        private int _nextClipExecutorIndex = -1;
        [NonSerialized]
        private MonoBehaviour _lastPlayOn;
        [NonSerialized]
        private float _lastSpeed;
        public void Play(MonoBehaviour playOn) => Play(playOn, 1, null);
        public void Play(MonoBehaviour playOn, Action<bool> done) => Play(playOn, 1, done);
        public void Play(MonoBehaviour playOn, float speed, Action<bool> done = null) 
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

            _lastDone = done;
            if (Clips.Count <= 0)
            {
                Finish(true);
                return;
            }

            IsPlaying = true;
            _lastSpeed = speed;
            _lastPlayOn = playOn;
            _clipsCount = Clips.Count;
            _clipsFinishedCount = 0;
            _nextClip = null;
            _nextClipExecutorIndex = -1;

            _nextClip = null;
            _nextClipExecutorIndex = -1;
            for (int i = 1; i < _clipsCount; i++)
            {
                SadJam.Components.AnimationClip clip = Clips[i];

                if (!clip.PlayTogetherWithBefore)
                {
                    _nextClipExecutorIndex = i - 1;
                    _nextClip = clip;
                    break;
                }
            }

            for (int i = 1; i < _clipsCount; i++)
            {
                SadJam.Components.AnimationClip clip = Clips[i];

                if (!clip.PlayTogetherWithBefore) break;

                clip.Play(playOn, speed, ClipFinished);
            }

            Clips[0].Play(playOn, speed, ClipFinished);
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

        private void ClipFinished(bool finished)
        {
            if (!finished)
            {
                Finish(finished);

                return;
            }

            _clipsFinishedCount++;

            if (_clipsFinishedCount >= _clipsCount)
            {
                Finish(finished);

                return;
            }

            if (_clipsFinishedCount - 1 == _nextClipExecutorIndex && _nextClip != null)
            {
                SadJam.Components.AnimationClip nextClip = _nextClip;
                int index = _nextClipExecutorIndex;

                _nextClip = null;
                _nextClipExecutorIndex = -1;
                for (int y = index + 1; y < _clipsCount; y++)
                {
                    SadJam.Components.AnimationClip clip = Clips[y];

                    if (!clip.PlayTogetherWithBefore)
                    {
                        _nextClipExecutorIndex = y - 1;
                        _nextClip = clip;
                        break;
                    }
                }

                for (int y = index + 1; y < _clipsCount; y++)
                {
                    SadJam.Components.AnimationClip clip = Clips[y];

                    if (!clip.PlayTogetherWithBefore) break;

                    clip.Play(_lastPlayOn, _lastSpeed, ClipFinished);
                }

                nextClip.Play(_lastPlayOn, _lastSpeed, ClipFinished);
            }
        }
    }
}