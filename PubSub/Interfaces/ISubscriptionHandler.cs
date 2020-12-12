using System;

namespace PubSub
{
    public interface ISubscriptionHandler
    {
        /// <summary>
        /// subscribe to every thing is this channel
        /// </summary>
        /// <returns>Subscribtion Key - use to unsubcribe from channel</returns>
        string Subscribe(Action<object> callback);
        string Subscribe(object sender,Action<object> callback);

        /// <summary>
        /// subscribe to specific type in this channel
        /// </summary>
        /// <param name="action"></param>
        /// <returns>use to unsubcribe from channel</returns>
        string Subscribe<T>(Action<T> callback, Func<T, bool>? filter = null);
        string Subscribe<T>(object sender, Action<T> callback, Func<T, bool>? filter = null);

        void UnSubscribe<T>(string key);
    }
}
