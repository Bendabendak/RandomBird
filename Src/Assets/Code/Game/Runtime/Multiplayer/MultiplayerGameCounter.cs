using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class MultiplayerGameCounter : SadJam.Component
    {
        [field: SerializeField]
        public TMP_Text Text { get; private set; }
        [field: SerializeField]
        public string Prefix { get; private set; }
        [field: SerializeField]
        public string Suffix { get; private set; }

        [field: Space, SerializeField]
        public bool ChangeTextCountFromAnimation { get; private set; } = false;

        [field: SerializeField, Space]
        public UnityEvent OnCounterStarted { get; private set; }
        [field: SerializeField]
        public UnityEvent OnChanged { get; private set; }
        [field: SerializeField]
        public UnityEvent OnCounterEnded { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            Game.NetworkManager.OnGameStarted -= OnGameStarted;
            Game.NetworkManager.OnGameStarted += OnGameStarted;

            if (Game.NetworkManager.StartGameCounter > 0)
            {
                OnGameStarted();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            StopAllCoroutines();

            Game.NetworkManager.OnGameStarted -= OnGameStarted;
        }

        private void OnGameStarted()
        {
            StopAllCoroutines();

            StartCoroutine(CounterCoroutine());
        }

        private IEnumerator CounterCoroutine()
        {
            int _lastValue = (int)Game.NetworkManager.StartGameCounter;

            if (!ChangeTextCountFromAnimation)
            {
                ChangeTextToCount();
            }

            OnCounterStarted.Invoke();

            while (Game.NetworkManager.StartGameCounter > 0)
            {
                int count = (int)Game.NetworkManager.StartGameCounter;
                if (_lastValue != count)
                {
                    if (!ChangeTextCountFromAnimation)
                    {
                        ChangeTextToCount();
                    }

                    OnChanged.Invoke();
                    _lastValue = count;
                }

                yield return null;
            }

            OnCounterEnded.Invoke();
        }

        public void ChangeTextToCount()
        {
            Text.text = Prefix + ((int)Game.NetworkManager.StartGameCounter + 1).ToString() + Suffix;
        }
    }
}
