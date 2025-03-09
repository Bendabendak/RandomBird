using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SadJam
{
    public abstract class Component : MonoBehaviour, IILComponent
    {
        public virtual string Label => _label;

        [field: SerializeField, DebugOnly]
        public List<UnityEngine.Component> AssignedTo { get; set; } = new();

        [NonSerialized]
        private Action _onAwakeOnce;
        [NonSerialized]
        private Action _onDestroy;

        public void AddReference(UnityEngine.Component to)
        {
            if (!AssignedTo.Contains(to))
            {
                AssignedTo.Add(to);
            }
        }

        public void RemoveReference(UnityEngine.Component from)
        {
            Type t = from.GetType();

            foreach(FieldInfo f in t.GetAllFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                object val = f.GetValue(from);

                if (val is SadJam.Component c && c == this)
                {
                    return;
                }
            }

            AssignedTo.RemoveAll(c => c == from);
        }

        [SerializeField, DebugOnly]
        private string _label;
        
        [NonSerialized]
        private bool _awakedOnce = false;
        protected virtual void Awake() 
        {
            if (!_awakedOnce)
            {
                _awakedOnce = true;
                AwakeOnce();
            }
        }

        public void AddOnAwakeOnceReceiver(Action receiver)
        {
            if (receiver == null) return;

            _onAwakeOnce -= receiver;
            _onAwakeOnce += receiver;
        }

        public void RemoveOnAwakeOnceReceiver(Action receiver)
        {
            if (receiver == null) return;

            _onAwakeOnce -= receiver;
        }

        protected virtual void AwakeOnce()
        {
            _onAwakeOnce?.Invoke();
        }

        [NonSerialized]
        private bool _startedOnce = false;
        [NonSerialized]
        private bool _started = false;
        protected virtual void Start() 
        {
            _started = true;

            if (!_startedOnce)
            {
                _startedOnce = true;
                StartOnce();
            }

            OnStartAndEnable();
        }

        protected virtual void OnStartAndEnable()
        {

        }

        protected virtual void StartOnce()
        {

        }

        protected virtual void OnEnable() 
        {
            if (_started)
            {
                OnStartAndEnable();
            }
        }

        protected virtual void OnDisable() 
        {
            _started = false;
        }

        public void AddOnDestroyReceiver(Action receiver)
        {
            if (receiver == null) return;

            _onDestroy -= receiver;
            _onDestroy += receiver;
        }
        public void RemoveOnDestroyReceiver(Action receiver)
        {
            if (receiver == null) return;

            _onDestroy -= receiver;
        }

        protected virtual void OnDestroy() 
        {
            _onDestroy?.Invoke();
        }

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtilityExtensions.SafeOperation(() => 
                {
                    try
                    {
                        if (gameObject == null) return;
                    }
                    catch
                    {
                        return;
                    }

                    Validate(); 
                });
            }
#endif
        }

        public void ChangeLabel(string label)
        {
            _label = new(label);
        }

        /// <summary>
        /// Called on OnValidate (Editor only)
        /// </summary>
        public virtual void Validate()
        {
            if (Label == null || string.IsNullOrWhiteSpace(Label))
            {
                ChangeLabel(GetType().Name);
            }
        }
    }
}
