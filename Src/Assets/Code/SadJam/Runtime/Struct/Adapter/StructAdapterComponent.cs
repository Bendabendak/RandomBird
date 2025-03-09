using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SadJam
{
    [AddFromCodeOnly]
    /// <typeparam name="T">Output</typeparam>
    public abstract class StructAdapterComponent<T> : StructComponent<T>, ISerializationCallbackReceiver where T : struct
    {
        public override string Label => GetLabel();

        [field: SerializeField, DebugOnly]
        public string[] CustomData { get; set; }

        [field: SerializeField, DebugOnly]
        public List<UnityEngine.Component> Inputs { get; set; }

        public SizeConvertor<T> sizeConvertor;
        [SerializeField]
        private string sizeConvertorName;

        public override T Size => sizeConvertor.ConvertSize(Inputs, CustomData);

        public virtual Label ConversionLabel => sizeConvertor.ConversionLabel;

        public override void Validate()
        {
            base.Validate();

            if (Inputs == null) return;

            ChangeLabel(string.Join(" ", Inputs.Select
            ((UnityEngine.Component i) =>
            {
                if (i == null) return "";

                return ((IComponent)i).Label;
            })));
        }

        private string GetLabel()
        {
            if (Inputs == null) return "";

            return string.Join(" ", Inputs.Select
            ((UnityEngine.Component i) =>
            {
                if (i == null) return "";

                return ((IComponent)i).Label + " (" + sizeConvertor.ConversionLabel.text + ")";
            }));
        }

        public IEnumerable<UnityEngine.Component> GetSourceInputs()
        {
            foreach(UnityEngine.Component c in Inputs)
            {
                if (c == null) continue;

                Type t = c.GetType();

                if (!t.IsAssignableToGenericType(typeof(StructAdapterComponent<>)))
                {
                    yield return c;
                    continue;
                }

                foreach(UnityEngine.Component c2 in (IEnumerable<UnityEngine.Component>)t.GetMethod(nameof(GetSourceInputs)).Invoke(c, null))
                {
                    yield return c2;
                }
            }
        }

        public static Dictionary<string, SizeConvertor<T>> SizeConvertors => GetSizeConvertors();

        private static Dictionary<string, SizeConvertor<T>> _sizeConvertors;
        private static Dictionary<string, SizeConvertor<T>> GetSizeConvertors()
        {
            if (_sizeConvertors == null)
            {
                _sizeConvertors = new(StaticBehaviourInitializer.GetBehaviours<SizeConvertor<T>>());
            }

            return _sizeConvertors;
        }

        public static SizeConvertor<T> GetSizeConvertor<D>() => GetSizeConvertor(typeof(D).FullName);
        public static SizeConvertor<T> GetSizeConvertor(Type t) => GetSizeConvertor(t.FullName);
        public static SizeConvertor<T> GetSizeConvertor(string name) => SizeConvertors[name];

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            sizeConvertor = GetSizeConvertor(sizeConvertorName);
        }

        public IEnumerable<UnityEngine.Component> GetInputs<D>() => GetInputs(typeof(D));

        public IEnumerable<UnityEngine.Component> GetInputs(Type t)
        {
            foreach(UnityEngine.Component c in Inputs)
            {
                if(c == null)
                {
                    yield return null;
                    continue;
                }

                if (t.IsAssignableFrom(c.GetType()))
                {
                    yield return c;
                }
            }
        }

        /// <typeparam name="A">Adapter</typeparam>
        /// <typeparam name="B">Size convertor</typeparam>
        public static A GetAdapter<A, B>(GameObject creator, List<UnityEngine.Component> inputs, params string[] customData)
            where A : StructAdapterComponent<T>, new() where B : SizeConvertor<T>, new()
        {
            GameObject garbage = GarbageCollector.Get(creator.transform);

            A c = garbage.AddComponent<A>();
            c.CustomData = customData;

            foreach (UnityEngine.Component input in inputs)
            {
                if (input is IComponent inputC)
                {
                    inputC.AddReference(c);
                }
            }

            c.Inputs = inputs;

            c.sizeConvertorName = typeof(B).FullName;
            c.sizeConvertor = GetSizeConvertor<B>();

            c.Validate();

            return c;
        }
    }
}
