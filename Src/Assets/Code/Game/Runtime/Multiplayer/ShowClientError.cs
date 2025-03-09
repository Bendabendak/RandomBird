using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class ShowClientError : SadJam.Component
    {
        [field: SerializeField]
        public TMP_Text Text { get; private set; }
        [field: SerializeField]
        public string Prefix { get; private set; }
        [field: SerializeField]
        public string Suffix { get; private set; }

        [field: SerializeField, Space]
        public UnityEvent OnFailed { get; private set; }
        [field: SerializeField]
        public UnityEvent OnConnected { get; private set; }
        [field: Space, SerializeField]
        public string OnConnectedText { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            Transport.active.OnClientError -= OnClientError;
            Transport.active.OnClientError += OnClientError;

            Transport.active.OnClientConnected -= OnClientConnected;
            Transport.active.OnClientConnected += OnClientConnected;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Transport.active.OnClientConnected -= OnClientConnected;
            Transport.active.OnClientError -= OnClientError;
        }

        private void OnClientError(TransportError error, string reason)
        {
            Text.text = Prefix + reason + Suffix;

            OnFailed.Invoke();
        }

        private void OnClientConnected()
        {
            Text.text = OnConnectedText;

            OnConnected.Invoke();
        }
    }
}
