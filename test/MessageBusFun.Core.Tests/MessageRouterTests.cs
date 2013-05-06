using MessageBusFun;
using Moq;
using NUnit.Framework;

namespace MessageBusFunTests
{
    [TestFixture]
    public class MessageRouterTests
    {
        [Test]
        public void Route_GivenProviderAndMessage_CorrectlyRoutesMessage()
        {
            var channel1Name = "Test Channel";
            var channel2Name = "Test Channel 2";
            var subscriber1 = CreateSubsciber(channel1Name, "Test Subscriber");
            var subscriber2 = CreateSubsciber(channel2Name, "Test Subscriber 2");
            var provider1 = CreateProvider(channel1Name, "Test Provider");
            var provider2 = CreateProvider(channel2Name, "Test Provider 2");

            var routerTable = new RouterTable();
            routerTable.Register(provider1);
            routerTable.Register(provider2);
            routerTable.Register(subscriber1.Object);
            routerTable.Register(subscriber2.Object);

            var router = new MessageRouter(routerTable);
            var message = new Message
                {
                    Text = "This is a crazy test!"
                };

            router.Route(provider1, message);

            subscriber1.Verify(s => s.Notify(message));
            Assert.That(message.Channel.Equals(channel1Name));
            subscriber2.Verify(s => s.Notify(message), Times.Never());

        }
        
        [Test]
        public void Route_GivenProviderAcrossMultipleChannelsAndMessage_CorrectlyRoutesMessage()
        {
            var channel1Name = "Test Channel";
            var channel2Name = "Test Channel 2";
            var subscriber1 = CreateSubsciber(channel1Name, "Test Subscriber");
            var subscriber2 = CreateSubsciber(channel2Name, "Test Subscriber 2");
            var provider1 = CreateProvider(channel1Name, "Test Provider");
            var provider2 = CreateProvider(channel2Name, "Test Provider 2");

            var routerTable = new RouterTable();
            routerTable.Register(provider1);
            provider1.Channel = channel2Name;
            routerTable.Register(provider1);
            routerTable.Register(provider2);
            routerTable.Register(subscriber1.Object);
            routerTable.Register(subscriber2.Object);

            var router = new MessageRouter(routerTable);
            var message = new Message
                {
                    Text = "This is a crazy test!"
                };

            router.Route(provider1, message);

            subscriber1.Verify(s => s.Notify(message));
            subscriber2.Verify(s => s.Notify(message));
            
        }

        private IProvider CreateProvider(string channel, string providerName)
        {
            return new TestProvider
                {
                    Channel = channel,
                    Name = providerName
                };
        }

        private Mock<ISubscriber> CreateSubsciber(string channel, string subscriberName)
        {
            var subscriber = new Mock<ISubscriber>();
            subscriber.SetupProperty(s => s.Channel, channel);
            subscriber.SetupProperty(s => s.Name, subscriberName);
            return subscriber;
        }
    }
}
