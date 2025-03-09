using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace SadJam.Components
{
    public class Input_AddPrefixSufix : MonoBehaviour
    {
        [field: SerializeField]
        public TMP_InputField Input { get; private set; }

        [field: Space, SerializeField]
        public string Prefix { get; private set; }
        [field: SerializeField]
        public bool VerifyPrefix { get; private set; }

        [field: Space, SerializeField]
        public string Sufix { get; private set; }
        [field: SerializeField]
        public bool VerifySufix { get; private set; }

        [NonSerialized]
        private StringBuilder _stringBuilder = new();
        public void AddPrefixSufix(string value)
        {
            _stringBuilder.Clear();

            if (!value.StartsWith(Prefix))
            {
                _stringBuilder.Append(Prefix);
            }

            _stringBuilder.Append(value);

            if (!value.EndsWith(Sufix))
            {
                _stringBuilder.Append(Sufix);
            }

            Input.text = _stringBuilder.ToString();
        }
    }
}
