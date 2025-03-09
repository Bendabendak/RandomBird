using UnityEngine;

namespace SadJam
{
    public class DebugOnlyComponent : SadJam.Component
    {
        public override void Validate()
        {
            base.Validate();

            if ((bool)GlobalSettings.Get("Debug").Value)
            {
                gameObject.hideFlags = HideFlags.None;
            }
            else
            {
                gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
        }
    }
}
