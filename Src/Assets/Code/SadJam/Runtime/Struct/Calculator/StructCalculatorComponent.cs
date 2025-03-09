using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam
{
    [ExecuteAlways]
    public abstract class StructCalculatorComponent<T> : StructComponent<T> where T : struct
    {
        public List<StructCalculatorMember<T>> members = new();

        [SerializeField, DebugOnly]
        private int _lastResultHash;
        [SerializeField, DebugOnly]
        private T _lastResult;
        public override T Size
        {
            get
            {
                int newHash = 0;

                foreach (StructCalculatorMember<T> m in members)
                {
                    newHash += m.GetHashCode();
                }

                if (newHash == _lastResultHash) return _lastResult;

                _lastResultHash = newHash;
                _lastResult = Calculate();

                return _lastResult;
            }
        }
        public static Dictionary<string, StructCalculatorOperator<T>> Operators => GetOperators();

        private static Dictionary<string, StructCalculatorOperator<T>> _operators;
        private static Dictionary<string, StructCalculatorOperator<T>> GetOperators()
        {
            if (_operators == null)
            {
                _operators = new(StaticBehaviourInitializer.GetBehaviours<StructCalculatorOperator<T>>());
            }

            return _operators;
        }

        public static StructCalculatorOperator<T> GetOperator<D>() => GetOperator(typeof(D).FullName);
        public static StructCalculatorOperator<T> GetOperator(Type t) => GetOperator(t.FullName);
        public static StructCalculatorOperator<T> GetOperator(string fullName) => Operators[fullName];

        public T Calculate()
        {
            if (members.Count <= 0) return new();

            if (members.Count == 1)
            {
                return members[0].component.Size;
            }

            T size = members[0].component.Size;

            int nextIndex = 0;
            foreach (StructCalculatorMember<T> member in members)
            {
                nextIndex++;

                if (nextIndex > members.Count - 1) break;

                size = member.operation.Calculate(size, members[nextIndex].component.Size);
            }

            return size;
        }
    }
}
