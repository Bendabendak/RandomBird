using UnityEngine;
using TypeReferences;

namespace SadJam
{
    [DefaultExecutionOrder(1000)]
    [ClassTypeAddress("Executor/Unity/OnLateUpdate")]
    [CustomStaticExecutor("1gu5_MhIhUmUY8HiyRgkPg")]
    public class Unity_OnLateUpdate : StaticExecutor
    {
        private void LateUpdate() => Execute(Time.deltaTime);
    }
}
