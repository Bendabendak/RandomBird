using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class ShowPort : SadJam.Component
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
        public UnityEvent OnSucceed { get; private set; }
        [field: Space, SerializeField]
        public string Error { get; private set; }

        [NonSerialized]
        private bool _errorExecuted = false;
        [NonSerialized]
        private bool _successExecuted = false;
        protected virtual void Update()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable && (NetworkServer.active || NetworkClient.active) && Transport.active is PortTransport portTransport)
            {
                _errorExecuted = false;

                if (!_successExecuted)
                {
                    _successExecuted = true;

                    if (!Text.enabled)
                    {
                        Text.enabled = true;
                    }

                    Text.text = Prefix + portTransport.Port + Suffix;

                    OnSucceed.Invoke();
                }
            }
            else
            {
                _successExecuted = false;

                if (!_errorExecuted)
                {
                    _errorExecuted = true;

                    Text.text = Error;

                    OnFailed.Invoke();
                }
            }
        }
    }
}
