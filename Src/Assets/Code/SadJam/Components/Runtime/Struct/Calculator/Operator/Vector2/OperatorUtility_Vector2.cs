using UnityEngine;

namespace SadJam.Components
{
    public static class OperatorUtility_Vector2
    {
        public static Vector2 SkipNaN(Vector2 result, Vector2 first, Vector2 second)
        {
            if (result.IsNaN())
            {
                if (float.IsNaN(result.x))
                {
                    if (!float.IsNaN(first.x))
                    {
                        result.x = first.x;
                    }
                    else
                    {
                        result.x = second.x;
                    }
                }

                if (float.IsNaN(result.y))
                {
                    if (!float.IsNaN(first.y))
                    {
                        result.y = first.y;
                    }
                    else
                    {
                        result.y = second.y;
                    }
                }
            }

            return result;
        }
    }
}
