using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubSub.Test
{
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void Test_Subscribe_To_Any()
        {
            var factory = new PubSubFactory();
            var subscriber = factory.GetSubscriptionHandler();
            var publisher = factory.GetPublisher();


            object result = null;
            subscriber.Subscribe(item => { result = item; });

            var publishValue = "Hello World!";
            publisher.Publish(publishValue);

            Assert.AreEqual(result.ToString(), publishValue);
        }

        [TestMethod]
        public void Test_Subscribe_To_Model()
        {
            var factory = new PubSubFactory();
            var subscriber = factory.GetSubscriptionHandler();
            var publisher = factory.GetPublisher();

            Event result = null;
            subscriber.Subscribe<Event>(e => { result = e; });

            var sEvent = new Event(10, 2);
            publisher.Publish(sEvent);

            Assert.AreEqual(result, sEvent);
        }

        [TestMethod]
        public void Test_Subscribe_To_Model_With_Filter_NoResult()
        {
            var factory = new PubSubFactory();
            var subscriber = factory.GetSubscriptionHandler();
            var publisher = factory.GetPublisher();

            Event result = null;
            subscriber.Subscribe<Event>(e => { result = e; }, f => { return f.X == 9; });

            var sEvent = new Event(10, 2);
            publisher.Publish(sEvent);

            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void Test_Subscribe_To_Model_With_Filter_HasResult()
        {
            var factory = new PubSubFactory();
            var subscriber = factory.GetSubscriptionHandler();
            var publisher = factory.GetPublisher();

            Event result = null;
            subscriber.Subscribe<Event>(e => { result = e; }, f => { return f.X == 10; });

            var sEvent = new Event(10, 2);
            publisher.Publish(sEvent);

            Assert.AreEqual(result, sEvent);
        }

        [TestMethod]
        public void Test_Channels_Not_Interfere_Together_1()
        {
            var sfactory = new PubSubFactory<Channel1>();
            var ssubscriber = sfactory.GetSubscriptionHandler();
            var spublisher = sfactory.GetPublisher();

            var bfactory = new PubSubFactory<Channel2>();
            var bsubscriber = bfactory.GetSubscriptionHandler();
            var bpublisher = bfactory.GetPublisher();

            Event channel1Result = null;
            ssubscriber.Subscribe<Event>(e => { channel1Result = e; });

            Event channel2Result = null;
            bsubscriber.Subscribe<Event>(e => { channel2Result = e; });

            var sEvent = new Event(10, 2);
            spublisher.Publish(sEvent);

            Assert.AreEqual(channel2Result, null);
            Assert.AreEqual(channel1Result, sEvent);
        }

        [TestMethod]
        public void Test_Channels_Not_Interfere_Together_2()
        {
            var sfactory = new PubSubFactory<Channel1>();
            var ssubscriber = sfactory.GetSubscriptionHandler();
            var spublisher = sfactory.GetPublisher();

            var bfactory = new PubSubFactory<Channel2>();
            var bsubscriber = bfactory.GetSubscriptionHandler();
            var bpublisher = bfactory.GetPublisher();

            Event channel1Result = null;
            ssubscriber.Subscribe<Event>(e => { channel1Result = e; });

            Event channel2Result = null;
            bsubscriber.Subscribe<Event>(e => { channel2Result = e; });

            var sEvent = new Event(10, 2);
            spublisher.Publish(sEvent);

            var bEvent = new Event(6, 15);
            bpublisher.Publish(bEvent);

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
