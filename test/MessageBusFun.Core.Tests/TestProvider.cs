using MessageBusFun;

namespace MessageBusFunTests
{
    public class TestProvider : IProvider
    {
        public string Channel { get; set; }

        public string Name { get; set; }
    }
}