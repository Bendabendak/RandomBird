using UnityEngine;

namespace SadJam.Components
{
    public class MultiplicationOperator_Quaternion : SaveResultOperator_Quaternion
    {
        public override string Symbol => "*";

        protected override Quaternion Result(Quaternion first, Quaternion second) => first * second;
    }
}
