using UnityEngine;

namespace SadJam.Components
{
    public class Log_Error : Log_Message_Base
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void DynamicExecutor_OnExecute()
        {
            Debug.LogWarning(Message, Context);
        }
    }
}
