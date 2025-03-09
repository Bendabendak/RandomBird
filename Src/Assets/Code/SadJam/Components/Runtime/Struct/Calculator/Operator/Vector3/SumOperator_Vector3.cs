using UnityEngine;

namespace SadJam.Components
{
    public class SumOperator_Vector3 : SaveResultOperator_Vector3
    {
        public override string Symbol => "+";
    
        protected override Vector3 Result(Vector3 first, Vector3 second) => first + second;
    }
}
