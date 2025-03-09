namespace SadJam.Components
{
    public class DivisionOperator_Float : SaveResultOperator_Float
    {
        public override string Symbol => ":";
    
        protected override float Result(float first, float second) => first / second;
    }
}
