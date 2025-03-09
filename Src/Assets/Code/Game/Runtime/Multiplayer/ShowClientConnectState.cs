using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class ShowClientConnectState : SadJam.Component
    {
        private enum ConnectState
        {
            None,
            Connecting,
            Connected,
        }

        [field: SerializeField]
        public TMP_Text Text { get; private set; }

        [field: Space, SerializeField]
        public UnityEvent OnConnecting { get; private set; }
        [field: SerializeField]
        public string ConnectingText { get;private set; }

        [field: Space, SerializeField]
        public UnityEvent OnConnected { get; private set; }
        [field: SerializeField]
        public string ConnectedText { get; private set; }

        [field: Space, SerializeField]
        public UnityEvent OnFailed { get; private set; }
        [field: SerializeField]
        public string FailedText { get; private set; }

        [NonSerialized]
        private ConnectState _connectState = ConnectState.None;
        protected virtual void Update()
        {
            if (NetworkClient.isConnecting)
            {
                if (_connectState != ConnectState.Connecting)
                {
                    _connectState = ConnectState.Connecting;

                    Text.text = ConnectingText;
                    OnConnecting.Invoke();
                }
            }
            else if (NetworkClient.isConnected)
            {
                if (_connectState != ConnectState.Connected)
                {
                    _connectState = ConnectState.Connected;

                    Text.text = ConnectedText;
                    OnConnected.Invoke();
                }
            }
            else
            {
                if (_connectState != ConnectState.None)
                {
                    _connectState = ConnectState.None;

                    Text.text = FailedText;
                    OnFailed.Invoke();
                }
            }
        }
    }
}

