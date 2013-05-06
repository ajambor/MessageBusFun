using System.Diagnostics;
using MessageBusFun;

namespace MessageBusFunTests
{
    public class TestSubscriber : ISubscriber
    {
        public string Name { get; set; }

        public string Channel { get; set; }

        public void Notify(Message message)
        {
            Debug.Print("Subcriber {0} has been notified with the following message:\"{1}\" from Channel: {2}", Name, message.Text, message.Channel); 
        }
    }
}