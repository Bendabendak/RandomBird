using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SadJam
{
    public class GlobalSettings : StaticBehaviour
    {
        public static Action OnSave { get; set; }

        public static List<GlobalSettingsMember> Members => GetMembers();

        private static Dictionary<string, GlobalSettingsMember> _membersByName;
        private static List<GlobalSettingsMember> _members;
        public static GlobalSettingsMember Get(string name) => GetMembersByName()[name];
        public static void Set(string name, object val)
        {
            GetMembersByName()[name].Value = val;
        }

        private static Dictionary<string, GlobalSettingsMember> GetMembersByName()
        {
            if (_membersByName == null)
            {
                Load();
            }

            return _membersByName;
        }

        private static List<GlobalSettingsMember> GetMembers()
        {
            if(_members == null)
            {
                Load();
            }

            return _members;
        }

        public static void Load()
        {
#if UNITY_EDITOR
            _members = ScriptableObjectExtensions.GetAllInstances<GlobalSettingsMember>().ToList();
            _membersByName = _members.ToDictionary(x => x.name, x => x);

            foreach (GlobalSettingsMember m in _members)
            {
                switch (m.Type)
                {
                    case SettingsMemberType.Bool:
                        m.Value = EditorPrefs.GetBool(m.name);
                        break;
                    case SettingsMemberType.String:
                        m.Value = EditorPrefs.GetString(m.name);
                        break;
                    case SettingsMemberType.Float:
                        m.Value = EditorPrefs.GetFloat(m.name);
                        break;
                    case SettingsMemberType.Int:
                        m.Value = EditorPrefs.GetInt(m.name);
                        break;
                }
            }
#endif
        }
    }
}
