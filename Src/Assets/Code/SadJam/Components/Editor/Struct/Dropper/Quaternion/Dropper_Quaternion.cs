using SadJam;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomDropper(typeof(StructComponent<Quaternion>))]
    public class Dropper_Quaternion : Dropper_StructComponent<Quaternion>
    {

    }
}
