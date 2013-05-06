using System.Linq;

namespace MessageBusFun
{
    public class Router
    {
        private readonly RouterTable _routerTable;

        public Router(RouterTable routerTable)
        {
            _routerTable = routerTable;
        }

        public Router() : this(new RouterTable()){}

        public void RouteMessage(Message message)
        {
            var channel = _routerTable.AvailableChannels.First(c => c.Name.Equals(message.Channel));
            
            if (channel == null)
            {
                return;
            }

            channel.Notify(message);

        }
    }
}