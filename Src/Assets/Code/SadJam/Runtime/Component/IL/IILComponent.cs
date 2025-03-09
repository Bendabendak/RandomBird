using System;

namespace SadJam
{
    public interface IILComponent : IComponent
    {
        public void AddOnAwakeOnceReceiver(Action receiver);
        public void RemoveOnAwakeOnceReceiver(Action receiver);

        public void AddOnDestroyReceiver(Action receiver);
        public void RemoveOnDestroyReceiver(Action receiver);
    }
}
