using System.Collections.Generic;
using MessageBusFun;
using MessageBusFunTests;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace MessageBusTests
{
    [TestFixture]
    public class RouterTableTests
    {
        private const string testChannelName = "Test Channel";
        private const string testProviderName = "Test Provider";
        private const string testSubscriberName = "Test Subscriber";

        [Test]
        public void RegisterProvider_GivenNewChannelWithProvider_CreatesRouteTableEntryWithNewChannelAndProvider()
        {
            var provider = new TestProvider {Name = testProviderName, Channel = testChannelName};
            var routerTable = new RouterTable();
            routerTable.Register(provider);
            Assert.That(routerTable.AvailableChannels.Any());
            Assert.That(routerTable.AvailableChannels.First().HasProvider(testProviderName));
        }
        
        [Test]
        public void RegisterProvider_GivenExistingChannelWithNewProvider_CreatesRouteTableEntryWithNewProvider()
        {
            var provider = new TestProvider {Name = testProviderName, Channel = testChannelName};
            const string testProvider2Name = "Test Provider2";
            var newProvider = new TestProvider {Name = testProvider2Name, Channel = testChannelName};
            var routerTable = new RouterTable();
            routerTable.Register(provider);
            routerTable.Register(newProvider);
            Assert.That(routerTable.AvailableChannels.ToList()[0].Providers == 2);
            Assert.That(routerTable.AvailableChannels.Any(c => c.HasProvider(testProviderName)));
            Assert.That(routerTable.AvailableChannels.Any(c => c.HasProvider(testProvider2Name)));
        }
        
        [Test]
        public void RegisterProvider_GivenExistingChannelWithSameProvider_DoesNotCreateNewRouteTableEntry()
        {
            var provider = new TestProvider { Name = testProviderName, Channel = testChannelName };
            var newProvider = new TestProvider {Name = testProviderName, Channel = testChannelName};
            var routerTable = new RouterTable();
            routerTable.Register(provider);
            routerTable.Register(newProvider);
            Assert.That(routerTable.AvailableChannels.ToList()[0].Providers == 1);
        } 
        
        [Test]
        public void RegisterSubscriber_GivenAvailableProvider_CreatesRouteTableEntryWithSubscriber()
        {
            var provider = new TestProvider { Name = "Test Provider", Channel = testChannelName};
            var subscriber = new TestSubscriber {Name = testSubscriberName, Channel = testChannelName};
            var routerTable = new RouterTable();
            routerTable.Register(provider);
            routerTable.Register(subscriber);
           
            Assert.That(routerTable.AvailableChannels.ToList()[0].Subscribers == 1);
            Assert.That(routerTable.AvailableChannels.Any(c => c.HasSubscriber(testSubscriberName)));
        }
        
        [Test]
        public void RegisterSubscriber_GivenAvailableProviderWithMultipleSubscribers_CreatesRouteTableEntryWithSubscriber()
        {
            var provider = new TestProvider { Name = "Test Provider", Channel = testChannelName};
            var subscriber = new TestSubscriber {Name = testSubscriberName, Channel = testChannelName};
            const string testSubscriber2Name = "Test Subscriber 2";
            var subscriber2 = new TestSubscriber {Name = testSubscriber2Name, Channel = testChannelName};
            var routerTable = new RouterTable();
            routerTable.Register(provider);
            routerTable.Register(subscriber);
            routerTable.Register(subscriber2);
           
            Assert.That(routerTable.AvailableChannels.First().Subscribers == 2);
            Assert.That(routerTable.AvailableChannels.Any(c => c.HasSubscriber(testSubscriberName)));
            Assert.That(routerTable.AvailableChannels.Any(c => c.HasSubscriber(testSubscriber2Name)));
        }
        
        [Test]
        public void RegisterSubscriber_GivenAvailableProviderWithSameSubscriber_DoesNotCreatesRouteTableEntryWithSubscriber()
        {
            var provider = new TestProvider { Name = "Test Provider", Channel = testChannelName};
            var subscriber = new TestSubscriber {Name = testSubscriberName, Channel = testChannelName};
            var subscriber2 = new TestSubscriber {Name = testSubscriberName, Channel = testChannelName};
            var routerTable = new RouterTable();
            routerTable.Register(provider);
            routerTable.Register(subscriber);
            routerTable.Register(subscriber2);
           
            Assert.That(routerTable.AvailableChannels.Any(c => c.HasSubscriber(testSubscriberName)));
            Assert.That(routerTable.AvailableChannels.First().Subscribers == 1);
            
        }
        
        [Test]
        public void RegisterSubscriber_GivenUnAvailableProvider_DoesNotCreateRouteTableEntry()
        {
            var subscriber = new TestSubscriber {Name = testChannelName, Channel = testChannelName};
            var routerTable = new RouterTable();
            routerTable.Register(subscriber);
            Assert.That(!routerTable.AvailableChannels.Any());
        }

        [Test]
        public void UnregisterProvider_GivenExistingChannelWithOneExistingProviderAndNoSubscribers_RemovesProviderAndChannel()
        {
            var provider = new TestProvider { Name = testProviderName, Channel = testChannelName};
            var router = new RouterTable();
            router.Register(provider);
            router.Unregister(provider);
            Assert.That(!router.Channels.Any());
        }
        
        [Test]
        public void UnregisterProvider_GivenExistingChannelWithExistingProviderAndSubscribers_RemovesProviderAndKeepsChannelWithSubscribers(){
            var provider = new TestProvider { Name = testProviderName, Channel = testChannelName };
            var subscriber = new TestSubscriber {Name = testSubscriberName, Channel = testChannelName};
            var router = new RouterTable();
            router.Register(provider);
            router.Register(subscriber);
            router.Unregister(provider);
            Assert.That(router.Channels.Any());
            Assert.That(router.Channels.First().HasSubscriber(testSubscriberName));
        }
        
        [Test]
        public void UnregisterProvider_GivenExistingChannelWithExistingProviderAndSubscribers_MakesChannelUnavailableAndCallsNotify(){
            var provider = new TestProvider { Name = testProviderName, Channel = testChannelName };
            var subscriber = new Mock<ISubscriber>();
            subscriber.SetupProperty(s => s.Name, testSubscriberName);
            subscriber.SetupProperty(s => s.Channel, testChannelName);
            var router = new RouterTable();
            router.Register(provider);
            router.Register(subscriber.Object);
            router.Unregister(provider);
            Assert.That(!router.Channels.First().IsAvailable);
            subscriber.Verify(s => s.Notify(It.IsAny<Message>()));
        }

        [Test]
        public void UnregisterProvider_GivenUnknownChannel_DoesNotRemoveProvider()
        {
            const string providerName = "Test Provider";
            var provider = new TestProvider
            {
                Channel = "Test Channel",
                Name = providerName
            };

            var routerTable = new RouterTable();
            routerTable.Register(provider);
            var providerToRemove = new TestProvider
            {
                Channel = "Test Channelz",
                Name = providerName
            };
            routerTable.Unregister(providerToRemove);

            var availableChannels = routerTable.AvailableChannels;
            var availableChannelsList = availableChannels as IList<Channel> ?? availableChannels.ToList();
            Assert.That(availableChannelsList.Count == 1);
            Assert.That(availableChannelsList.First().HasProvider(providerName));
        }

        [Test]
        public void UnregisterSubscriber_GivenExistingChannel_RemovesSubscriber()
        {
            var provider = new TestProvider {Name = testProviderName, Channel = testChannelName};
            var subscriber = new TestSubscriber {Name = testSubscriberName, Channel = testChannelName};

            var router = new RouterTable();
            router.Register(provider);
            router.Register(subscriber);
            router.Unregister(subscriber);

            Assert.That(router.Channels.Any());
            Assert.That(!router.Channels.First().HasSubscriber(testSubscriberName));
        }

        
    }
}
