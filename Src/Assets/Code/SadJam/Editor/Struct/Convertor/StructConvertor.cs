using SadJam;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    /// <typeparam name="T">Input type</typeparam>
    public abstract class StructConvertor<T> : StaticBehaviour
    {
        public static Dictionary<string, StructConvertor<T>> Convertors => GetConvertors();

        private static Dictionary<string, StructConvertor<T>> _convertors;
        private static Dictionary<string, StructConvertor<T>> GetConvertors()
        {
            if(_convertors == null)
            {
                _convertors = new(StaticBehaviourInitializer.GetBehaviours<StructConvertor<T>>());
            }

            return _convertors;
        }

        /// <typeparam name="D">Return type</typeparam>
        public static StructConvertor<T> GetConvertor<D>() => GetConvertor(typeof(D).FullName);

        public static StructConvertor<T> GetConvertor(Type returnType)
        {
            Dictionary<int, StructConvertor<T>> pos = new();
            foreach (KeyValuePair<string, StructConvertor<T>> d in Convertors.Where((KeyValuePair<string, StructConvertor<T>> d) => 
            {
                object[] atts = d.Value.type.GetCustomAttributes(typeof(CustomStructConvertor), false);

                if (atts == null || atts.Length <= 0) return false;

                Type t = ((CustomStructConvertor)atts[0]).targetType;

                if (returnType.IsAssignableToGenericType(t)) return true;

                return t.IsAssignableFrom(returnType);
            }))
            {
                pos.Add(d.Value.type.GetInheritanceHierarchy().Count(), d.Value);
            }

            if (pos.Count <= 0)
            {
                return null;
            }

            return pos[pos.Keys.Max()];
        }

        public static StructConvertor<T> GetConvertor(string fullName) => Convertors[fullName];

        public abstract void ConvertMeAs(GameObject target, Type targetType, T input, object before, Action<object> done, params object[] customData);

        /// <param name="selected">Address, Result</param>
        public static void ConversionTypeSelection(T input, int numOfSplits, Action<string, List<object>> selected = null) => Selection(input, numOfSplits, ConversionType.None, selected);
        /// <param name="selected">Address, Result</param>
        public static void SplitSelection(T input, int numOfSplits, Action<string, List<object>> selected = null)
            => Selection(input, numOfSplits, ConversionType.Split, selected);
        /// <param name="selected">Address, Result</param>
        public static void NormalSelection(T input, Action<string, List<object>> selected = null)
            => Selection(input, 0, ConversionType.Normal, selected);

        public static void Selection(T input, int numOfSplits, ConversionType type, Action<string, List<object>> selected = null)
        {   
            GenericMenu menu = new();

            GUIContent none = new("None");

            List<GUIContent> splitAddresses = new() { none };
            List<GUIContent> splitSelection = new() { new GUIContent("Split/" + none.text) };
            List<GUIContent> normalSelection = new();
            List<ConversionType<T>> conversions = new();

            foreach (KeyValuePair<string, ConversionType<T>> c in StaticBehaviourInitializer.GetBehaviours<ConversionType<T>>())
            {
                conversions.Add(c.Value);

                if (type != ConversionType.None)
                {
                    if (c.Value.conversionType != type) continue;
                }

                switch (c.Value.conversionType)
                {
                    case ConversionType.None:
                        normalSelection.Add(new("Normal/" + c.Value.uniqueAddress));
                        splitSelection.Add(new("Split/" + c.Value.uniqueAddress));
                        splitAddresses.Add(new(c.Value.uniqueAddress));
                        break;
                    case ConversionType.Normal:
                        normalSelection.Add(new("Normal/" + c.Value.uniqueAddress));
                        break;
                    case ConversionType.Split:
                        splitSelection.Add(new("Split/" + c.Value.uniqueAddress));
                        splitAddresses.Add(new(c.Value.uniqueAddress));
                        break;
                }
            }

            List<List<GUIContent>> splits = new();

            IEnumerable<GUIContent> selection = normalSelection;

            if (splitSelection.Count > 1)
            {
                splits.Insert(0, splitSelection);

                for (int i = 0; i < numOfSplits - 1; i++)
                {
                    splits.Add(splitAddresses);
                }

                selection = normalSelection.Concat(GenericMenuExtensions.ConcatMenus(splits.ToArray()).ToList());
            }

            foreach (GUIContent g in selection)
            {
                menu.AddItem(new GUIContent(g), false, () =>
                {
                    string[] addresses = g.text.Substring(1).Split('/');

                    List<object> result = new();

                    foreach (string a in addresses)
                    {
                        if (a == none.text)
                        {
                            result.Add(null);
                            continue;
                        }

                        ConversionType<T> conversion = conversions.Where(c => c.uniqueAddress.Split('/').
                            Last() == a).FirstOrDefault();

                        if (conversion == null) continue;

                        result.Add(conversion.OnSelection(input));
                    }

                    selected?.Invoke(g.text, result);
                });
            }

            GenericMenuExtensions.Show(menu, "Select Conversion", Event.current.mousePosition);
        }
    }
}
