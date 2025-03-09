using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [DefaultExecutionOrder(1000)]
    [ClassTypeAddress("Executor/Unity/OnApplicationPause")]
    [CustomStaticExecutor("qXR074kZ2ESBOhc3TJduVg")]
    public class Unity_OnApplicationPause : StaticExecutor
    {
        protected virtual void OnApplicationPause() => Execute(Time.deltaTime);
    }
}
