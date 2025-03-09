using System;
using UnityEngine;

namespace SadJam
{
    [Serializable]
    public class StructCalculatorMember<T> : ISerializationCallbackReceiver where T : struct
    {
        public StructComponent<T> component;
        private StructCalculatorOperator<T> _operation;
        public StructCalculatorOperator<T> operation
        {
            get
            {
                return _operation;
            }
            set
            {
                _operation = value;

                if (operation == null) return;
                operationName = operation.GetType().FullName;
            }
        }
        [SerializeField]
        private string operationName;
        public string[] customData;

        public StructCalculatorMember()
        {
        }

        public StructCalculatorMember(StructComponent<T> component, StructCalculatorOperator<T> operation, string[] customData)
        {
            this.component = component;
            this.operation = operation;
            this.customData = customData;
        }

        private void Init()
        {
            if (operationName == null) return;

            operation = StructCalculatorComponent<T>.GetOperator(operationName);
        }

        public override int GetHashCode() => (component, component.Size, operation, customData).GetHashCode();

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            Init();
        }
    }
}
