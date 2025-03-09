using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

namespace SadJam
{
    public abstract class GetStructField<T> : StructComponent<T> where T : struct
    {
        [field: SerializeField]
        public UnityEngine.Component Target { get; private set; }
        [field: SerializeField]
        public Selection Field { get; private set; } = new();

        public PropertyInfo TargetProp { get; private set; }
        public FieldInfo TargetField { get; private set; }

        public override T Size => GetSize();

        private string _lastSelection;
        private bool _initialized = false;
        protected override void Start()
        {
            base.Start();

            _initialized = false;
            Init();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            TargetProp = null;
            TargetField = null;

            if (Target == null)
            {
                Field.ChangeCollection(null);
                return;
            }

            List<string> selection = new();
            Type t = Target.GetType();

            selection.AddRange(t.GetProperties().Where(p => p.PropertyType == typeof(T)).Select(m => m.Name));
            selection.AddRange(t.GetAllFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).Where(f => f.FieldType == typeof(T)).Select(m => m.Name));

            Field.ChangeCollection(selection);

            if(_lastSelection != Field.Selected)
            {
                _lastSelection = Field.Selected;
                Init();
                _initialized = false;
            }
        }

        private T GetSize()
        {
            Init();

            if (TargetProp == null)
            {
                return (T)TargetField.GetValue(Target);
            }

            return (T)TargetProp.GetValue(Target);
        }

        private void Init()
        {
            if (_initialized) return;
            _initialized = true;

            Type t = Target.GetType();

            TargetField = t.GetField(Field.Selected);

            if(TargetField == null)
            {
                TargetProp = t.GetProperty(Field.Selected);
            }
        }
    }
}
