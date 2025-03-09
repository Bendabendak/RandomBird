using UnityEngine;

namespace SadJam
{
    public static class FloatExtensions
    {
        public static Vector2 GetVectorFromAngle(this float angle)
        {
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
    }
}