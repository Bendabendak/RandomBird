using System;
using System.Collections.Generic;
using System.Reflection;

namespace SadJam
{
    public static class TypeExtensions
	{
        /// <summary>
        /// Split names by /
        /// </summary>
        public static void GetDeepMember(this Type t, object target, string address, out FieldInfo field, out PropertyInfo property, out object fieldTarget)
        {
            string[] split = address.Split('/');

            FieldInfo fI = t.GetField(split[0], BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo pI = null;
            if(fI == null)
            {
                pI = t.GetProperty(split[0], BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            }

            if (split.Length <= 1 || (fI == null && pI == null))
            {
                fieldTarget = target;
                field = fI;
                property = pI;
                return;
            }

            string newString = address.Substring(address.IndexOf('/') + 1);

            if(fI == null)
            {
                GetDeepMember(pI.PropertyType, pI.GetValue(target), newString, out field, out property, out fieldTarget);
            }
            else
            {
                GetDeepMember(fI.FieldType, fI.GetValue(target), newString, out field, out property, out fieldTarget);
            }
        }

        /// <summary>
        /// Split names by /
        /// </summary>
        public static PropertyInfo GetDeepProperty(this Type t, string address)
        {
            string[] split = address.Split('/');

            PropertyInfo info = t.GetProperty(split[0], BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            if (split.Length <= 1 || info == null)
            {
                return info;
            }

            string newString = address.Substring(0, address.IndexOf('/'));
            return GetDeepProperty(info.PropertyType, newString);
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type t)
        {
            while (t != null)
            {
                foreach (FieldInfo i in t.GetFields())
                {
                    yield return i;
                }

                t = t.BaseType;
            }
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type t, BindingFlags flags)
        {
            while (t != null)
            {
                foreach(FieldInfo i in t.GetFields(flags))
                {
                    yield return i;
                }

                t = t.BaseType;
            }
        }

        public static string GetBackingFieldName(string propertyName) => string.Format("<{0}>k__BackingField", propertyName);

        public static FieldInfo GetFieldViaPath(this Type type, string path)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var parent = type;
            var fi = parent.GetField(path, flags);
            var paths = path.Split('.');

            if (fi != null) return fi;

            for (int i = 0; i < paths.Length; i++)
            {
                fi = parent.GetField(paths[i], flags);
                if (fi != null)
                {
                    if (fi.FieldType.IsArray)
                    {
                        parent = fi.FieldType.GetElementType();
                        i += 2;
                        continue;
                    }

                    if (fi.FieldType.IsGenericType)
                    {
                        parent = fi.FieldType.GetGenericArguments()[0];
                        i += 2;
                        continue;
                    }
                    parent = fi.FieldType;
                }
                else
                {
                    break;
                }

            }

            if (fi == null)
            {
                if (type.BaseType != null)
                {
                    return GetFieldViaPath(type.BaseType, path);
                }
                else
                {
                    return null;
                }
            }

            return fi;
        }

        public static Type GetGenericBaseType(this Type t, Type targetType)
		{
			if (t.IsGenericType && t.GetGenericTypeDefinition() == targetType) return t;

			if (t == typeof(object)) return null;

			return GetGenericBaseType(t.BaseType, targetType);
		}

		public static IEnumerable<Type> GetInheritanceHierarchy(this Type type)
		{
			for (var current = type; current != null; current = current.BaseType)
				yield return current;
		}

		public static bool OverridesMethod(this Type type, string methodName)
		{
			MethodInfo mi = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			if (mi == null) return false;

			return mi.DeclaringType.FullName.Equals(type.FullName, StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
		{
			if (!genericType.IsGenericType)
			{
				return genericType.IsAssignableFrom(givenType);
			}

			var interfaceTypes = givenType.GetInterfaces();

			foreach (var it in interfaceTypes)
			{
				if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
					return true;
			}

			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
				return true;

			Type baseType = givenType.BaseType;
			if (baseType == null) return false;

			return IsAssignableToGenericType(baseType, genericType);
		}
	}
}
