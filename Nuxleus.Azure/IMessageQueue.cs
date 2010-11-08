using System;

namespace Azure.Toolkit
{
    public interface IMessageQueue<T>
    {
        void AddMessage(T message);
        void DeleteMessage();
        T GetMessage();
    }
}
