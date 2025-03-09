using SadJam;
using System;
using TypeReferences;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Day/DayClock")]
    public class Day_DayClock : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Day_Config Config { get; }

        [NonSerialized]
        private float _count = 0;
        protected override void DynamicExecutor_OnExecute()
        {
            if (_count >= Config.DayInterval)
            {
                _count = Delta;
                Execute(Delta);
            }
            else
            {
                _count += Delta;
            }
        }
    }
}
