using UnityEngine;

namespace SadJam
{
    public static class Vector3Extensions
    {
        public static Vector3 GetFrozen(this Vector3 target, Vector3Freeze freezer, Vector3 defaultValues)
        {
            Vector3 result = target;
            if ((freezer & Vector3Freeze.X) != 0)
            {
                result.x = defaultValues.x;
            }
            if ((freezer & Vector3Freeze.Y) != 0)
            {
                result.y = defaultValues.y;
            }
            if ((freezer & Vector3Freeze.Z) != 0)
            {
                result.z = defaultValues.z;
            }

            return result;
        }

        public static bool IsNaN(this Vector3 vector)
        {
#pragma warning disable CS1718
            return vector != vector;
#pragma warning restore CS1718
        }

        public static Vector3 ReplaceNaN(this Vector3 vector, Vector3 replace)
        {
            Vector3 v = vector;

            if (float.IsNaN(v.x))
            {
                v.x = replace.x;
            }

            if (float.IsNaN(v.y))
            {
                v.y = replace.y;
            }

            if (float.IsNaN(v.z))
            {
                v.z = replace.z;
            }

            return v;
        }
    }
}
