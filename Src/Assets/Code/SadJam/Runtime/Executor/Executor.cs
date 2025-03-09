using System;
using System.Collections.Generic;

namespace SadJam
{
    public abstract class Executor : SadJam.Component
    {
        public abstract void Execute(float delta, params KeyValuePair<string, object>[] customData);

        public static Func<IExecutable, IExecutable, bool> ExecutionOrderComparer => (IExecutable t1, IExecutable t2) =>
        {
            return t1.ExecutionOrder < t2.ExecutionOrder;
        };
    }
}
