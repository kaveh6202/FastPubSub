using PubSub.Interfaces;
using PubSub.Model;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace PubSub
{

    public abstract class BaseChannel : IChannel
    {
        private ConcurrentDictionary<string, Subscriber> _subscribers = new ConcurrentDictionary<string, Subscriber>();

        public ConfigurationModel Config { get; set; }

        protected BaseChannel()
        {
            Config = new ConfigurationModel();
        }

        #region Subscription

        public virtual string Subscribe<T>(Action<T> callback, Func<T, bool> filter = null) => Subscription(this, callback, typeof(T), filter);
        public virtual string Subscribe<T>(object sender, Action<T> callback, Func<T, bool>? filter = null) => Subscription(sender, callback, typeof(T), filter);

        public virtual string Subscribe(Action<object> callback) => Subscription(this, callback);
        public virtual string Subscribe(object sender, Action<object> callback) => Subscription(sender, callback);


        public virtual void UnSubscribe<T>(string key)
        {
            while (!_subscribers.TryRemove(key, out _)) { }
        }

        #endregion

        #region Publish

        public virtual void Publish<T>(T input = default(T))
        {

            RefreshSubsribers();

            //get all handlers
            var handlers = _subscribers.Select(i => i.Value).Where(i => i.Type == null || i.Type.Equals(typeof(T)));

            if (Config.InvokeCallbackFunctionsSimultaneously)
            {
                Parallel.ForEach(handlers, h =>
                {
                    InvokeCallback(h);
                });
            }
            else
            {
                foreach (var h in handlers)
                {
                    InvokeCallback(h);
                }
            }


            void InvokeCallback(Subscriber h)
            {
                try
                {
                    if (h.Type == null)
                    {
                        InvokceObj<T>(h, input);
                        return;
                    }

                    if (h.Filter != null)
                    {
                        var filter = (Func<T, bool>)h.Filter;
                        if (!filter(input))
                        {
                            return;
                        }

                    }

                    InvokeGeneric<T>(h, input);
                }
                catch
                {
                    if (!Config.IgnoreCallbackException)
                        throw;
                }
            }
        }
        #endregion

        #region private methods
        private string Subscription(object sender, Delegate callback, Type? type = null, Delegate? filter = null)
        {
            var sub = new Subscriber()
            {
                Action = callback,
                Filter = filter,
                Type = type,
                Sender = new WeakReference(sender)
            };
            var key = Guid.NewGuid().ToString();
            while (!_subscribers.TryAdd(key, sub)) { }
            return key;
        }

        private void RefreshSubsribers()
        {
            while (!_subscribers.All(i => i.Value.Sender.IsAlive))
            {
                var key = _subscribers.FirstOrDefault(i => !i.Value.Sender.IsAlive).Key;
                _subscribers.TryRemove(key, out _);
            }

        }

        private void InvokceObj<T>(Subscriber h, T input)
        {
            var action = (Action<object>)h.Action;
            if (Config.FireAndForgetCallback)
                Task.Run(() => action(input));
            else
            {
                action(input);
            }
        }

        private void InvokeGeneric<T>(Subscriber h, T input)
        {
            var action = (Action<T>)h.Action;
            if (Config.FireAndForgetCallback)
                Task.Run(() => action(input));
            else
            {
                action(input);
            }
        }


        #endregion
    }

}
