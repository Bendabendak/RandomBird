using System;
using UnityEngine;

namespace Game
{
    [ExecuteInEditMode]
    public class Shader_Liquid : MonoBehaviour
    {
        public enum UpdateMode { Normal, UnscaledTime }
        public enum Rotation { Up, Down, Right, Left }
        public UpdateMode updateMode;

        [SerializeField]
        float MaxWobble = 0.03f;
        [SerializeField]
        float WobbleSpeedMove = 1f;
        [SerializeField, Range(0, 1)]
        float fillAmount = 0.5f;
        [SerializeField]
        float Recovery = 1f;
        [SerializeField]
        float Thickness = 1f;
        [SerializeField]
        Vector3 MinVelocity = Vector3.zero;
        [SerializeField]
        Rotation rotationType = Rotation.Up;
        [SerializeField]
        Renderer rend;
        Vector3 lastPos;
        Vector3 velocity;
        Quaternion lastRot;
        Vector3 angularVelocity;
        float wobbleAmountX;
        float wobbleAmountZ;
        float wobbleAmountToAddX;
        float wobbleAmountToAddZ;
        float pulse;
        float sinewave;
        float time = 0.5f;

        [NonSerialized]
        private float? _lastFillAmount = null;
        void Update()
        {
            Bounds bounds = rend.bounds;
            Material mat;
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                mat = rend.material;
            }
            else
            {
                mat = rend.sharedMaterial;
            }
#else
            mat = rend.material;
#endif

            float deltaTime = 0;
            switch (updateMode)
            {
                case UpdateMode.Normal:
                    deltaTime = Time.deltaTime;
                    break;

                case UpdateMode.UnscaledTime:
                    deltaTime = Time.unscaledDeltaTime;
                    break;
            }

            time += deltaTime;

            if (deltaTime != 0)
            {
                // decrease wobble over time
                wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, (deltaTime * Recovery));
                wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, (deltaTime * Recovery));


                // make a sine wave of the decreasing wobble
                pulse = 2 * Mathf.PI * WobbleSpeedMove;
                sinewave = Mathf.Lerp(sinewave, Mathf.Sin(pulse * time), deltaTime * Mathf.Clamp(velocity.magnitude + angularVelocity.magnitude, Thickness, 10));

                wobbleAmountX = wobbleAmountToAddX * sinewave;
                wobbleAmountZ = wobbleAmountToAddZ * sinewave;


                // velocity
                velocity = (lastPos - transform.position) / deltaTime;
                velocity = Vector3.Max(MinVelocity, velocity);

                angularVelocity = GetAngularVelocity(lastRot, transform.rotation);

                // add clamped velocity to wobble
                wobbleAmountToAddX += Mathf.Clamp(velocity.x + (velocity.y * 0.2f) + angularVelocity.z + angularVelocity.y, -MaxWobble, MaxWobble);
                wobbleAmountToAddZ += Mathf.Clamp(velocity.z + (velocity.y * 0.2f) + angularVelocity.x + angularVelocity.y, -MaxWobble, MaxWobble);
            }

            // send it to the shader
            mat.SetFloat("_WobbleX", wobbleAmountX);
            mat.SetFloat("_WobbleZ", wobbleAmountZ);

            if (_lastFillAmount != fillAmount)
            {
                switch (rotationType)
                {
                    case Rotation.Up:
                        mat.SetFloat("_BaseRotation", 0);
                        mat.SetVector("_FillAmount", new Vector3(0, -0.5f - bounds.extents.y + (bounds.size.y * fillAmount), 0));
                        break;
                    case Rotation.Down:
                        mat.SetFloat("_BaseRotation", 180);
                        mat.SetVector("_FillAmount", new Vector3(0, -0.5f - bounds.extents.y + (bounds.size.y * fillAmount), 0));
                        break;
                    case Rotation.Right:
                        mat.SetFloat("_BaseRotation", 90);
                        mat.SetVector("_FillAmount", new Vector3(0, -0.5f - bounds.extents.x + (bounds.size.x * fillAmount), 0));
                        break;
                    case Rotation.Left:
                        mat.SetFloat("_BaseRotation", 270);
                        mat.SetVector("_FillAmount", new Vector3(0, -0.5f - bounds.extents.x + (bounds.size.x * fillAmount), 0));
                        break;
                }

                _lastFillAmount = fillAmount;
            }

            // keep last position
            lastPos = transform.position;
            lastRot = transform.rotation;
        }

        //https://forum.unity.com/threads/manually-calculate-angular-velocity-of-gameobject.289462/#post-4302796
        Vector3 GetAngularVelocity(Quaternion foreLastFrameRotation, Quaternion lastFrameRotation)
        {
            var q = lastFrameRotation * Quaternion.Inverse(foreLastFrameRotation);
            // no rotation?
            // You may want to increase this closer to 1 if you want to handle very small rotations.
            // Beware, if it is too close to one your answer will be Nan
            if (Mathf.Abs(q.w) > 1023.5f / 1024.0f)
                return Vector3.zero;
            float gain;
            // handle negatives, we could just flip it but this is faster
            if (q.w < 0.0f)
            {
                var angle = Mathf.Acos(-q.w);
                gain = -2.0f * angle / (Mathf.Sin(angle) * Time.deltaTime);
            }
            else
            {
                var angle = Mathf.Acos(q.w);
                gain = 2.0f * angle / (Mathf.Sin(angle) * Time.deltaTime);
            }
            Vector3 angularVelocity = new Vector3(q.x * gain, q.y * gain, q.z * gain);

            if (float.IsNaN(angularVelocity.z))
            {
                angularVelocity = Vector3.zero;
            }
            return angularVelocity;
        }
    }
}
