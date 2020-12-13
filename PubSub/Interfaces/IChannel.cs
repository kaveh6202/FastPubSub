using PubSub.Model;
using System.Collections.Concurrent;

namespace PubSub.Interfaces
{
    public interface IChannel : IPublisher, ISubscriptionHandler
    {
        ConcurrentDictionary<string, Subscriber> Subscribers { get; }
        ConfigurationModel Config { get; set; }
    }
}
