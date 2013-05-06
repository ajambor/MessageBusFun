using System.Collections.Generic;
using System.Linq;

namespace MessageBusFun
{
    public class MessageRouter : IMessageRouter
    {
        private HashSet<Channel> _channels = new HashSet<Channel>();
        private RouterTable _routerTable;

        public MessageRouter(RouterTable routerTable)
        {
            _routerTable = routerTable;
        }

        public MessageRouter():this(new RouterTable()){}
       
        public void Register(IProvider provider)
        {
            _routerTable.Register(provider);
        }

        public IEnumerable<Channel> AvailableChannels
        {
            get { return _routerTable.AvailableChannels; }
        }

        public void Register(ISubscriber subscriber)
        {
            _routerTable.Register(subscriber);
        }

        public IEnumerable<Channel> Channels
        {
            get { return _channels; }
        }
        
        public void Unregister(IProvider provider)
        {
            _routerTable.Unregister(provider);
        }
        
        public void Unregister(ISubscriber subscriber)
        {
            _routerTable.Unregister(subscriber);
        }

        public void Route(IProvider provider, Message message)
        {
            var channelsToNotify = _routerTable.AvailableChannels.Where(c => c.HasProvider(provider.Name));
            foreach (var channel in channelsToNotify)
            {
                message.Channel = channel.Name;
                channel.Notify(message);
            }
        }
    }
}