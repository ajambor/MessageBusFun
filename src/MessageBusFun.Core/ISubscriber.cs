using System;

namespace MessageBusFun
{
    public interface ISubscriber
    {
        String Name { get; set; }
        String Channel { get; set; }
        void Notify(Message message);
    }
}