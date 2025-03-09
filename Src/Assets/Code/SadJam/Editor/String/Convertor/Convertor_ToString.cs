using SadJam;
using System.Collections.Generic;
using UnityEngine;

namespace SadJamEditor
{
    public static class Convertor_ToString
    {
        public static StringComponent Convert(GameObject target, UnityEngine.Component input, object before)
        {
            List<UnityEngine.Component> newInputs = new();

            if (before is StringAdapterComponent adapterBefore)
            {
                newInputs.AddRange(adapterBefore.Inputs);
            }
            else if (before is StringComponent stringBefore)
            {
                newInputs.Add(stringBefore);
            }

            newInputs.Add(input);

            if (newInputs.Count <= 1 && input is StringComponent stringComponent)
            {
                return stringComponent;
            }

            StringAdapterComponent adapter = StringAdapterComponent.GetAdapter(target, newInputs);

            return adapter;
        }
    }
}
