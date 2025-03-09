using SadJam;
using SadJam.Components;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Game
{
    public class Roulette_Timer : SadJam.Component
    {
        [GameConfigSerializeProperty]
        public Roulette_Config Config { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: Space, SerializeField]
        public TMP_Text Time { get; private set; }
        [field: SerializeField]
        public string TimePrefix { get; private set; } = "";
        [field: SerializeField]
        public string TimeSuffix { get; private set; } = "";

        [field: Space, SerializeField]
        public AnimationClips EnableClips { get; private set; }
        [field: SerializeField]
        public AnimationClips DisableClips { get; private set; }

        [OnGameConfigChanged(nameof(Owner))]
        [OnGameConfigChanged(nameof(Config))]
        public virtual void OnConfigChanged(string affected)
        {
            Enable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Enable();
        }

        [NonSerialized]
        private DateTime _lastRollTime;
        private void Enable()
        {
            Statistics.OnChanged -= OnChanged;
            Statistics.OnChanged += OnChanged;

            Config.GetLastRoll(Owner, out _lastRollTime);

            StopAllCoroutines();
            StartCoroutine(TimerCoroutine());
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Statistics.OnChanged -= OnChanged;

            StopAllCoroutines();
        }

        [NonSerialized]
        private WaitForSeconds _timerInterval = new(0.5f);
        private IEnumerator TimerCoroutine(Action done = null)
        {
            TimeSpan rouletteResetInterval = new(0, 0, Config.ResetInterval);
            DateTime targetTime = _lastRollTime.Add(rouletteResetInterval);
            DateTime now = DateTime.Now;

            if (targetTime < now)
            {
                EnableClips.SkipToEnd();

                done?.Invoke();
                yield break;
            }

            DisableClips.Play(this);

            while (true)
            {
                rouletteResetInterval = new(0, 0, Config.ResetInterval);
                targetTime = _lastRollTime.Add(rouletteResetInterval);
                now = DateTime.Now;

                if (targetTime < now)
                {
                    EnableClips.Play(this);
                    done?.Invoke();
                    break;
                }

                TimeSpan diff = targetTime - now;
                string diffString;

                if (diff.Days > 0)
                {
                    diffString = diff.ToString(@"dd\:hh\:mm\:ss");
                }
                else if(diff.Hours > 0)
                {
                    diffString = diff.ToString(@"hh\:mm\:ss");
                }
                else if(diff.Minutes > 0)
                {
                    diffString = diff.ToString(@"mm\:ss");
                }
                else
                {
                    diffString = diff.ToString(@"ss");
                }

                Time.text = TimePrefix + diffString + TimeSuffix;

                yield return _timerInterval;
            }
        }

        private void OnChanged(string ownerId, Statistics.DataEntry entry)
        {
            if (ownerId != Owner.Id || !entry.Verify(Config.SaveKey, out DateTime lastRoll)) return;

            _lastRollTime = lastRoll;
        }
    }
}
