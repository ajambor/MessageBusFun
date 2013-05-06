using System.Collections.Generic;

namespace MessageBusFun
{
    public class MessageBus
    {
        private IMessageRouter _messageRouter;

        public MessageBus(IMessageRouter messageRouter)
        {
            _messageRouter = messageRouter;
        }

        public MessageBus():this(new MessageRouter()){}

        public void RegisterProvider(IProvider provider)
        {
            _messageRouter.Register(provider);
        }

        public IEnumerable<Channel> GetAvailableChannels()
        {
            return _messageRouter.AvailableChannels;
        }

        public void RegisterSubscriber(ISubscriber subscriber)
        {
            _messageRouter.Register(subscriber);
        }

        public void UnregisterProvider(IProvider providerToRemove)
        {
            _messageRouter.Unregister(providerToRemove);
        }
        
        public void UnregisterSubscriber(ISubscriber subscriberToRemove)
        {
            _messageRouter.Unregister(subscriberToRemove);
        }

        public void Notify(IProvider provider, Message message)
        {
            _messageRouter.Route(provider, message);
        }
    }
}