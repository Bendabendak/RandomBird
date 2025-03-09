using UnityEngine;

namespace SadJam.Components
{
    public class SumOperator_Vector2 : SaveResultOperator_Vector2
    {
        public override string Symbol => "+";

        protected override Vector2 Result(Vector2 first, Vector2 second) => first + second;
    }
}
