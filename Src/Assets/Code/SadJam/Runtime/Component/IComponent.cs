using System.Collections.Generic;
namespace SadJam
{
    public interface IComponent
    {
        public string Label { get; }

        public List<UnityEngine.Component> AssignedTo { get; set; }
        public void AddReference(UnityEngine.Component to);
        public void RemoveReference(UnityEngine.Component from);
        public void Validate();
        public void ChangeLabel(string label);
    }
}