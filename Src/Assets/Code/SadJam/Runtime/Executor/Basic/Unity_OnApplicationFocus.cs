using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [DefaultExecutionOrder(1000)]
    [ClassTypeAddress("Executor/Unity/OnApplicationFocus")]
    [CustomStaticExecutor("pjYc9I5SGUu8IgHyz3U30g")]
    public class Unity_OnApplicationFocus : StaticExecutor
    {
        protected virtual void OnApplicationFocus() => Execute(Time.deltaTime);
    }
}
