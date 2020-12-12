namespace PubSub
{
    public interface IPublisher
    {
        /// <summary>
        /// publish to channel and wait for handlers to finish their jobs
        /// </summary>
        void Publish<T>(T input);
    }
}
