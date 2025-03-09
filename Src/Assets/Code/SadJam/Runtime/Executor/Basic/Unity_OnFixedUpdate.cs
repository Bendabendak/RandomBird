using TypeReferences;
using UnityEngine;

namespace SadJam
{
    [DefaultExecutionOrder(1000)]
    [ClassTypeAddress("Executor/Unity/OnFixedUpdate")]
    [CustomStaticExecutor("K5I7nZEsc0G_p41xTEdoEg")]
    public class Unity_OnFixedUpdate : StaticExecutor
    {
        private void FixedUpdate() => Execute(Time.fixedDeltaTime);
    }
}
