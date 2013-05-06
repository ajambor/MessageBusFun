using System.Collections.Generic;

namespace MessageBusFun
{
    public interface IMessageRouter
    {
        void Register(IProvider provider);
        IEnumerable<Channel> AvailableChannels  { get; }
        void Register(ISubscriber subscriber);
        void Unregister(IProvider providerToRemove);
        void Unregister(ISubscriber subscriberToRemove);
        void Route(IProvider provider, Message message);
    }
}