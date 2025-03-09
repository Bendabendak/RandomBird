using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [DefaultExecutionOrder(1000)]
    [ClassTypeAddress("Executor/Unity/OnApplicationQuit")]
    [CustomStaticExecutor("dptfMNZuuUWMqrTgyqNEdw")]
    public class Unity_OnApplicationQuit : StaticExecutor
    {
        protected virtual void OnApplicationQuit() => Execute(Time.deltaTime);
    }
}
