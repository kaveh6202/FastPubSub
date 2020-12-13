using PubSub.Channels;

namespace PubSub.Interfaces
{
    public interface IPubSubFactory<T> where T : IChannel
    {
        IPublisher GetPublisher();
        ISubscriptionHandler GetSubscriptionHandler();


        IPubSubFactory<T> IgnoreCallbackException();
        IPubSubFactory<T> FireAndForgetCallback();
        IPubSubFactory<T> InvokeCallbackFunctionsSimultaneously();
    }

    public interface IPubSubFactory : IPubSubFactory<DefaultChannel> { }
}
