using MessageBusFun;
using MessageBusTests;
using Moq;
using NUnit.Framework;

namespace MessageBusFunTests
{
    [TestFixture]
    public class ChannelTests
    {
        [Test]
        public void IsAvailable_WhenAtLeastOneProviderIsPresent_ReturnsTrue()
        {
            var channel = new Channel();
            var provider = new TestProvider()
                {
                    Channel = "Test Channel",
                    Name = "Test Provider"
                };
            channel.AddProvider(provider);
            
            Assert.That(channel.IsAvailable, Is.True);
        }

        [Test]
        public void IsAvailable_WhenNoProvidersArePresent_ReturnsTrue()
        {
            var channel = new Channel();
            Assert.That(channel.IsAvailable, Is.False);
        }
        
        [Test]
        public void AddProvider_WhenGivenSameProviderMoreThanOnce_OnlyAddsOneProvider()
        {
            var channel = new Channel();
            const string providerName = "yo provider!";
            var provider = new TestProvider{Name = providerName};
            var provider2 = new TestProvider{Name = providerName};
            channel.AddProvider(provider);
            channel.AddProvider(provider2);
            Assert.That(channel.Providers == 1);
        }

        [Test]
        public void AddSubscriber_WhenGivenSameSubscriberMoreThanOnce_OnlyAddsOneSubscriber()
        {
            var channel = new Channel();
            const string subscriberName = "crazy subscriber";
            var subscriber = new TestSubscriber{Name = subscriberName};
            var subscriber2 = new TestSubscriber{Name = subscriberName};
            channel.AddSubscriber(subscriber);
            channel.AddSubscriber(subscriber2);
            Assert.That(channel.Subscribers == 1);
            
        }

        [Test]
        public void RemoveProvider_WhenGivenSameProviderMoreThanOnce_DoesNotFail()
        {
            var channel = new Channel{Name = "Test Channel"};
            var provider = new TestProvider{Name = "cool provider"};
            channel.AddProvider(provider);
            channel.RemoveProvider(provider);
            channel.RemoveProvider(provider);
            Assert.That(channel.Providers == 0);
        }
        
        [Test]
        public void RemoveSubscriber_WhenGivenSameSubsciberMoreThanOnce_DoesNotFail()
        {
            var channel = new Channel{Name = "Test Channel"};
            var subscriber = new TestSubscriber{Name = "cool subscriber"};
            channel.AddSubscriber(subscriber);
            channel.RemoveSubscriber(subscriber);
            channel.RemoveSubscriber(subscriber);
            Assert.That(channel.Providers == 0);
        }
       
        [Test]
        public void Notify_GivenMessage_CallsNotifyOnAllSubscribers()
        {
            var channel = new Channel { Name = "Test Channel" };
            var subscriber = new Mock<ISubscriber>();
            subscriber.SetupProperty(s => s.Name, "subscriber1");
            var subscriber2 = new Mock<ISubscriber>();
            subscriber2.SetupProperty(s => s.Name, "subscriber2");
            channel.AddSubscriber(subscriber.Object);
            channel.AddSubscriber(subscriber2.Object);
            var message = new Message{Channel = channel.Name, Text = "my awesome message!"};
            channel.Notify(message);
            subscriber.Verify(s=>s.Notify(message));
            subscriber2.Verify(s=>s.Notify(message));
        }
        
        [Test]
        public void HandleRemovedProvider_NoAvailableProviders_CallsNotifyOnAllSubscribers()
        {
            var channel = new Channel { Name = "Test Channel" };
            var subscriber = new Mock<ISubscriber>();
            subscriber.SetupProperty(s => s.Name, "subscriber1");
            var subscriber2 = new Mock<ISubscriber>();
            subscriber2.SetupProperty(s => s.Name, "subscriber2");
            channel.AddSubscriber(subscriber.Object);
            channel.AddSubscriber(subscriber2.Object);
            channel.HandleRemovedProvider();
            subscriber.Verify(s=>s.Notify(It.IsAny<Message>()));
            subscriber2.Verify(s => s.Notify(It.IsAny<Message>()));
        }
        
        [Test]
        public void HandleRemovedProvider_HasAvailableProviders_DoesNotCallNotifyOnAllSubscribers()
        {
            var channel = new Channel { Name = "Test Channel" };
            var provider = new TestProvider {Name = "Test Provider"};
            channel.AddProvider(provider);
            var subscriber = new Mock<ISubscriber>();
            subscriber.SetupProperty(s => s.Name, "subscriber1");
            var subscriber2 = new Mock<ISubscriber>();
            subscriber2.SetupProperty(s => s.Name, "subscriber2");
            channel.AddSubscriber(subscriber.Object);
            channel.AddSubscriber(subscriber2.Object);
            channel.HandleRemovedProvider();
            subscriber.Verify(s=>s.Notify(It.IsAny<Message>()), Times.Never());
            subscriber2.Verify(s => s.Notify(It.IsAny<Message>()), Times.Never());
        }

    }
}
