using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubSub.Test
{
    [TestClass]
    public class IOCChannelTests
    {
        private ISubscriptionHandler _subscriber1;
        private IPublisher _publisher1;
        
        private ISubscriptionHandler _subscriber2;
        private IPublisher _publisher2;

        public IOCChannelTests()
        {
            var factory1 = new PubSubFactory<Channel1>();
            var factory2 = new PubSubFactory<Channel2>();
            _subscriber1 = factory1.GetSubscriptionHandler();
            _publisher1 = factory1.GetPublisher();

            _subscriber2 = factory2.GetSubscriptionHandler();
            _publisher2 = factory2.GetPublisher();
        }

        [TestMethod]
        public void Test_Channels_Not_Interfere_Together_1()
        {
            Event channel1Result = null;
            _subscriber1.Subscribe<Event>(e => { channel1Result = e; });

            Event channel2Result = null;
            _subscriber2.Subscribe<Event>(e => { channel2Result = e; });

            var sEvent = new Event(10, 2);
            _publisher1.Publish(sEvent);

            Assert.AreEqual(channel2Result, null);
            Assert.AreEqual(channel1Result, sEvent);
        }

        [TestMethod]
        public void Test_Channels_Not_Interfere_Together_2()
        {
            Event channel1Result = null;
            _subscriber1.Subscribe<Event>(e => { channel1Result = e; });

            Event channel2Result = null;
            _subscriber2.Subscribe<Event>(e => { channel2Result = e; });

            var sEvent = new Event(10, 2);
            _publisher1.Publish(sEvent);

            var bEvent = new Event(6, 15);
            _publisher2.Publish(bEvent);

            Assert.AreEqual(channel2Result, bEvent);
            Assert.AreEqual(channel1Result, sEvent);
        }
        class Event
        {
            public int X { get; }
            public int Y { get; }
            public Event(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        class Channel1 : BaseChannel { }
        class Channel2 : BaseChannel { }
    }

}
