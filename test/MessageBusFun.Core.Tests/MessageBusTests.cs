
using MessageBusFun;
using NUnit.Framework;

namespace MessageBusFunTests

{
    [TestFixture]
    public class MessageBusTests
    {
        [Test]
        public void RunIntegrationTest()
        {
            var testChannel = "Test Channel";
            var provider = new TestProvider {Channel = testChannel, Name = "Test Provider"};
            var subscriber = new TestSubscriber {Channel = testChannel, Name = "Test Provider"};

            var messageBus = new MessageBus();
            messageBus.RegisterProvider(provider);
            messageBus.RegisterSubscriber(subscriber);

            var message = new Message {Text = "My cool test message"};

            messageBus.Notify(provider, message);
        }


    }
}
