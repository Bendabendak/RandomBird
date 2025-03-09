using UnityEngine;

namespace SadJam.Components
{
    public abstract class Log_Message_Base : DynamicExecutor
    {
        [field: SerializeField]
        public string Message { get; protected set; }
        [field: SerializeField]
        public Object Context { get; protected set; }
    }
}
