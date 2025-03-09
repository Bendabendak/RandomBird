using UnityEngine;

namespace SadJam
{
    public static class QuaternionExtensions
    {
        public static Quaternion GetFrozen(this Quaternion target, QuaternionFreeze freezer, Quaternion defaultValues)
        {
            Quaternion result = target;
            if ((freezer & QuaternionFreeze.X) != 0)
            {
                result.x = defaultValues.x;
            }
            if ((freezer & QuaternionFreeze.Y) != 0)
            {
                result.y = defaultValues.y;
            }
            if ((freezer & QuaternionFreeze.Z) != 0)
            {
                result.z = defaultValues.z;
            }
            if ((freezer & QuaternionFreeze.W) != 0)
            {
                result.w = defaultValues.w;
            }

            return result;
        }
    }
}
