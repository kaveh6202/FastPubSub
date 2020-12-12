using PubSub.Model;

namespace PubSub.Interfaces
{
    public interface IChannel : IPublisher, ISubscriptionHandler
    {
        ConfigurationModel Config { get; set; }
    }
}
