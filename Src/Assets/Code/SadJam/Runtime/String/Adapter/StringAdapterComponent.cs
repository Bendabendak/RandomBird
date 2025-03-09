using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SadJam
{
    [AddFromCodeOnly]
    public class StringAdapterComponent : StringComponent, ISerializationCallbackReceiver
    {
        [field: SerializeField, DebugOnly]
        public List<UnityEngine.Component> Inputs { get; set; }

        public StringConvertor stringConvertor;
        [SerializeField]
        private string stringConvertorName;

        public override string Content => stringConvertor.GetContent(Inputs);

        public override void Validate()
        {
            base.Validate();

            ChangeLabel(string.Join(" ", Inputs.Select
            ((UnityEngine.Component i) =>
            {
                if (i == null) return "";

                return ((IComponent)i).Label;
            })));
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            stringConvertor = GetStringConvertor(stringConvertorName);
        }

        public static Dictionary<string, StringConvertor> StringConvertors => GetStringConvertors();

        private static Dictionary<string, StringConvertor> _sizeConvertors;
        private static Dictionary<string, StringConvertor> GetStringConvertors()
        {
            if (_sizeConvertors == null)
            {
                _sizeConvertors = new(StaticBehaviourInitializer.GetBehaviours<StringConvertor>());
            }

            return _sizeConvertors;
        }

        public static StringConvertor GetStringConvertor<D>() => GetStringConvertor(typeof(D).FullName);
        public static StringConvertor GetStringConvertor(Type t) => GetStringConvertor(t.FullName);
        public static StringConvertor GetStringConvertor(string name) => StringConvertors[name];

        /// <typeparam name="A">Adapter</typeparam>
        /// <typeparam name="B">Size convertor</typeparam>
        public static StringAdapterComponent GetAdapter(GameObject creator, List<UnityEngine.Component> inputs)
        {
            GameObject garbage = GarbageCollector.Get(creator.transform);

            StringAdapterComponent c = garbage.AddComponent<StringAdapterComponent>();

            foreach (UnityEngine.Component input in inputs)
            {
                if (input is IComponent inputC)
                {
                    inputC.AddReference(c);
                }
            }

            c.Inputs = inputs;

            c.stringConvertorName = typeof(StringConvertor).FullName;
            c.stringConvertor = GetStringConvertor<StringConvertor>();

            return c;
        }
    }
}
