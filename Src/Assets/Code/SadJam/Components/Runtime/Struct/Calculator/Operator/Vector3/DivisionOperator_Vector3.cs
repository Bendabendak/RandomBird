using UnityEngine;

namespace SadJam.Components
{
    public class DivisionOperator_Vector3 : SaveResultOperator_Vector3
    {
        public override string Symbol => ":";

        protected override Vector3 Result(Vector3 first, Vector3 second) => new(first.x / second.x, first.y / second.y, first.z / second.z);
    }
}
