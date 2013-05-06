using System.Collections.Generic;
using System.Linq;

namespace MessageBusFun
{
    public class Channel
    {
        private HashSet<IProvider> _providers = new HashSet<IProvider>();
        private HashSet<ISubscriber> _subscribers = new HashSet<ISubscriber>();

        public string Name { get; set; }

        public bool IsAvailable
        {
            get { return _providers.Count > 0; }
        }

        public void AddProvider(IProvider provider)
        {
            if(_providers.Count(p => p.Name.Equals(provider.Name)) == 0)
                _providers.Add(provider);
        }

        public int Providers
        {
            get { return _providers.Count(); }
        }

        public int Subscribers
        {
            get { return _subscribers.Count; }
        }

        public void AddSubscriber(ISubscriber subscriber)
        {
            if (_subscribers.Count(p => p.Name.Equals(subscriber.Name)) == 0)
            {
                
                _subscribers.Add(subscriber);
            }
        }

        public void RemoveProvider(IProvider provider)
        {
            _providers.RemoveWhere(p => p.Name.Equals(provider.Name));
            HandleRemovedProvider();
        }

        public void RemoveSubscriber(ISubscriber subscriber)
        {
            _subscribers.RemoveWhere(s => s.Name.Equals(subscriber.Name));
        }

        public void Notify(Message message)
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Notify(message);
            }
        }

        public void HandleRemovedProvider()
        {
            if(IsAvailable) return;
            var message = new Message {Channel = Name, Text = string.Format("Channel {0} is no longer available", Name)};
            Notify(message);
        }

        public bool HasProvider(string providerName)
        {
            return _providers.Any(p => p.Name.Equals(providerName));
        }

        public bool HasSubscriber(string subscriberName)
        {
            return _subscribers.Any(s => s.Name.Equals(subscriberName));
        }
    }
}