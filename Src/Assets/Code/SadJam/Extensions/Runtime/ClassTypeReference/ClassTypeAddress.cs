using System;

namespace TypeReferences
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClassTypeAddress : Attribute
    {
        private string _address;
        public ClassTypeAddress(string address)
        {
            _address = address;
        }

        public string Address => _address;
    }
}
