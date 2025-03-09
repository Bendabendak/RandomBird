using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [DefaultExecutionOrder(1000)]
    [ClassTypeAddress("Executor/Unity/OnUpdate")]
    [CustomStaticExecutor("QAK5nwguz0muAyE4EzXthw")]
    public class Unity_OnUpdate : StaticExecutor
    {
        protected override void StaticExecutor_Update() => Execute(Time.deltaTime);
    }
}
