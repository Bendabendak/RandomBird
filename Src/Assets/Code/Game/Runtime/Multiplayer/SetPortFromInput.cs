using Mirror;
using SadJam;
using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace Game
{
    public class SetPortFromInput : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public TMP_InputField Input { get; private set; }

        [NonSerialized]
        private StringBuilder _stringBuilder = new();
        protected override void DynamicExecutor_OnExecute()
        {
            string input = Input.text;

            if (string.IsNullOrEmpty(input)) return;

            _stringBuilder.Clear();
            foreach (char c in input)
            {
                if (char.IsNumber(c))
                {
                    _stringBuilder.Append(c);
                }
            }

            if (Transport.active is PortTransport portTransport && ushort.TryParse(_stringBuilder.ToString(), out ushort port))
            {
                portTransport.Port = port;
                Execute(Delta);
            }
        }
    }
}
