using SadJam;

namespace Game
{
    public class Day_Transform_ToNight : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void DynamicExecutor_OnExecute()
        {
            Day_Transformer.GetTransformer().ChangeTransformStatus(Day_TransformStatus.Night);
        }
    }
}
