using SadJam;
using System;
using TypeReferences;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Day/NightClock")]
    public class Day_NightClock : DynamicExecutor
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
            if (_count >= Config.NightInterval)
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
