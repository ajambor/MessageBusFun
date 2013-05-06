using System.Collections.Generic;
using System.Linq;

namespace MessageBusFun
{
    public class RouterTable
    {
        private HashSet<Channel> _channels = new HashSet<Channel>();

        public void Register(IProvider provider)
        {
            var channelName = provider.Channel;
            if (channelName == null)
            {
                return;
            }
            if (_channels.Any(c => c.Name.Equals(channelName) && c.HasProvider(provider.Name)))
            {
                AddProviderToChannel(provider);
            }
            else if (_channels.Any(c => c.Name.Equals(channelName)))
            {
                AddProviderToChannel(provider);
            }
            else
            {
                var channel = new Channel { Name = channelName };
                channel.AddProvider(provider);
                _channels.Add(channel);
            }

        }

        private void AddProviderToChannel(IProvider provider)
        {
            var existingChannel = _channels.First(c => c.Name.Equals(provider.Channel));
            existingChannel.AddProvider(provider);
        }

        public void Register(ISubscriber subscriber)
        {
            if (AvailableChannels.Count(c => c.Name.Equals(subscriber.Channel)) > 0)
            {
                var availableChannel = AvailableChannels.First(c => c.Name.Equals(subscriber.Channel));
                availableChannel.AddSubscriber(subscriber);
            }
            
        }

        public void Unregister(IProvider provider)
        {
            var channel = provider.Channel;
            if (!_channels.Any(c => c.Name.Equals(channel)))
                return;
            var existingChannel = _channels.First(c => c.Name.Equals(channel));
            existingChannel.RemoveProvider(provider);
            if (existingChannel.Providers == 0 && existingChannel.Subscribers == 0)
            {
                _channels.RemoveWhere(c => c.Name.Equals(channel));
            }
        }

        public void Unregister(ISubscriber subscriber)
        {
            var channel = subscriber.Channel;
            if (!_channels.Any(c => c.Name.Equals(channel)))
                return;
            var existingChannel = _channels.First(c => c.Name.Equals(channel));
            existingChannel.RemoveSubscriber(subscriber);
        }

        public IEnumerable<Channel> AvailableChannels
        {
            get { return _channels.Where(c => c.IsAvailable); }
        }

        public IEnumerable<Channel> Channels
        {
            get { return _channels; }
        }
    }
}