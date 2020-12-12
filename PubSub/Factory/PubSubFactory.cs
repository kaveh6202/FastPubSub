using PubSub.Channels;
using PubSub.Interfaces;
using PubSub.Model;
using System;
using System.Collections.Generic;

namespace PubSub
{
    public class PubSubFactory<T> : IPubSubFactory<T> where T : IChannel, new()
    {
        private readonly static Dictionary<Type, IChannel> _channels = new Dictionary<Type, IChannel>();
        public PubSubFactory()
        {
            GetChannel();
        }

        public IPublisher GetPublisher() => GetChannel();
        public ISubscriptionHandler GetSubscriptionHandler() => GetChannel();


        public IPubSubFactory<T> IgnoreCallbackException()
        {
            ((BaseChannel)_channels[typeof(T)]).Config.IgnoreCallbackException = true;
            return this;
        }
        public IPubSubFactory<T> FireAndForgetCallback()
        {
            ((BaseChannel)_channels[typeof(T)]).Config.FireAndForgetCallback = true;
            return this;
        }

        private IChannel GetChannel()
        {
            if (!_channels.ContainsKey(typeof(T)))
            {
                var channel = new T();
                _channels.Add(typeof(T), channel);
            }
            return _channels[typeof(T)];
        }
    }

    public class PubSubFactory : PubSubFactory<DefaultChannel>
    {

    }
}
