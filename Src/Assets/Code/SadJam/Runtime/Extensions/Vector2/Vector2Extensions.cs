using UnityEngine;

namespace SadJam
{
    public static class Vector2Extensions
    {
        public static Vector2 GetFrozen(this Vector2 target, Vector2Freeze freezer, Vector2 defaultValues)
        {
            Vector2 result = target;
            if ((freezer & Vector2Freeze.X) != 0)
            {
                result.x = defaultValues.x;
            }
            if ((freezer & Vector2Freeze.Y) != 0)
            {
                result.y = defaultValues.y;
            }

            return result;
        }
        public static bool IsNaN(this Vector2 vector)
        {
#pragma warning disable CS1718
            return vector != vector;
#pragma warning restore CS1718
        }

        public static Vector2 ReplaceNaN(this Vector2 vector, float replaceWith)
        {
            Vector2 v = vector;

            if (float.IsNaN(v.x))
            {
                v.x = replaceWith;
            }

            if (float.IsNaN(v.y))
            {
                v.y = replaceWith;
            }

            return v;
        }
    }
}
