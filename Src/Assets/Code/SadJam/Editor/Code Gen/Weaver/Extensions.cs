using Mono.CecilX;
using Mono.CecilX.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SadJamEditor.Weaver
{
    public static class Extensions
    {
        public static bool Is(this TypeReference td, Type type) => type.IsGenericType ? td.GetElementType().FullName == type.FullName : td.FullName == type.FullName;
        public static bool Is<T>(this TypeReference td) => Is(td, typeof(T));

        public static bool IsDerivedFrom<T>(this TypeReference tr) => IsDerivedFrom(tr, typeof(T));
        public static bool IsDerivedFrom(this TypeReference tr, Type baseClass)
        {
            TypeDefinition td = tr.Resolve();
            if (!td.IsClass) return false;

            TypeReference parent = td.BaseType;

            if (parent == null) return false;

            if (parent.Is(baseClass)) return true;

            if (parent.CanBeResolved()) return IsDerivedFrom(parent.Resolve(), baseClass);

            return false;
        }

        public static bool CanBeResolved(this TypeReference parent)
        {
            while (parent != null)
            {
                if (parent.Scope.Name == "Windows")
                {
                    return false;
                }

                if (parent.Scope.Name == "mscorlib")
                {
                    TypeDefinition resolved = parent.Resolve();
                    return resolved != null;
                }

                try
                {
                    parent = parent.Resolve().BaseType;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static MethodReference MakeHostInstanceGeneric(this MethodReference self, ModuleDefinition module, GenericInstanceType instanceType)
        {
            MethodReference reference = new MethodReference(self.Name, self.ReturnType, instanceType)
            {
                CallingConvention = self.CallingConvention,
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis
            };

            foreach (ParameterDefinition parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            foreach (GenericParameter generic_parameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(generic_parameter.Name, reference));

            return module.ImportReference(reference);
        }

        public static FieldReference SpecializeField(this FieldReference self, ModuleDefinition module, GenericInstanceType instanceType)
        {
            FieldReference reference = new FieldReference(self.Name, self.FieldType, instanceType);
            return module.ImportReference(reference);
        }

        public static MethodDefinition GetMethod(this TypeDefinition td, string methodName)
        {
            return td.Methods.FirstOrDefault(method => method.Name == methodName);
        }

        public static List<MethodDefinition> GetMethods(this TypeDefinition td, string methodName)
        {
            return td.Methods.Where(method => method.Name == methodName).ToList();
        }

        public static TypeDefinition GetParentWithInterface(this TypeDefinition type, string interfaceFullName)
        {
            if (type.BaseType != null)
            {
                TypeDefinition baseTypeResolved = type.BaseType.Resolve();
                if (baseTypeResolved.Interfaces.Any(i => i.InterfaceType.FullName == interfaceFullName))
                {
                    return baseTypeResolved;
                }

                return GetParentWithInterface(type.BaseType.Resolve(), interfaceFullName);
            }

            return null;
        }

        public static IEnumerable<TypeDefinition> GetAllParents(this TypeDefinition td)
        {
            TypeReference baseType = td.BaseType;
            if (baseType == null)
            {
                yield break;
            }

            TypeDefinition baseTypeResolved = baseType.Resolve();
            yield return baseTypeResolved;

            foreach (TypeDefinition bt in GetAllParents(baseTypeResolved))
            {
                yield return bt;
            }
        }

        public static T GetField<T>(this CustomAttribute ca, string field, T defaultValue)
        {
            foreach (CustomAttributeNamedArgument customField in ca.Fields)
            {
                if (customField.Name == field)
                {
                    return (T)customField.Argument.Value;
                }
            }

            return defaultValue;
        }

        public static T GetProperty<T>(this CustomAttribute ca, string property, T defaultValue)
        {
            foreach (CustomAttributeNamedArgument customField in ca.Properties)
            {
                if (customField.Name == property)
                {
                    return (T)customField.Argument.Value;
                }
            }

            return defaultValue;
        }

        public static List<PropertyDefinition> GetAllProperties(this TypeDefinition td) => GetAllPropertiesHeleper(td, new());
        private static List<PropertyDefinition> GetAllPropertiesHeleper(TypeDefinition td, List<PropertyDefinition> list)
        {
            if (td.HasProperties)
            {
                list.AddRange(td.Properties);
            }

            if (td.BaseType != null)
            {
                TypeDefinition baseType = td.BaseType.Resolve();
                GetAllPropertiesHeleper(baseType, list);
            }

            return list;
        }

        public static bool RemoveFinalRetInstruction(this MethodDefinition method)
        {
            if (method.Body.Instructions.Count != 0)
            {
                Instruction retInstr = method.Body.Instructions[method.Body.Instructions.Count - 1];
                if (retInstr.OpCode == OpCodes.Ret)
                {
                    method.Body.Instructions.RemoveAt(method.Body.Instructions.Count - 1);
                    return true;
                }
                return false;
            }

            return true;
        }
    }
}
