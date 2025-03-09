using System.Collections.Generic;

namespace SadJam
{
    public interface IExecutable
    {
        public List<string> StaticExecutors { get; set; }
        public List<DynamicExecutor> DynamicExecutors { get; set; }
        public float Delta { get; set; }
        public float DelayIn { get; set; }
        public float SequentialDelayIn { get; set; }
        public float DelayOut { get; set; }
        public float SequentialDelayOut { get; set; }
        public Dictionary<string, object> CustomData { get; set; }
        public int ExecutionOrder { get; set; }
        public void OnExecute();
    }
}
