using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubSub.Test
{
    [TestClass]
    public class IOCBasicTests
    {
        private ISubscriptionHandler _subscriber;
        private IPublisher _publisher;

        public IOCBasicTests()
        {
            var factory = new PubSubFactory();
            _subscriber = factory.GetSubscriptionHandler();
            _publisher = factory.GetPublisher();
        }

        [TestMethod]
        public void Test_Subscribe_To_Any()
        {
            object result = null;
            _subscriber.Subscribe(item => { result = item; });

            var publishValue = "Hello World!";
            _publisher.Publish(publishValue);

            Assert.AreEqual(result.ToString(), publishValue);
        }

        [TestMethod]
        public void Test_Subscribe_To_Model()
        {
            Event result = null;
            _subscriber.Subscribe<Event>(e => { result = e; });

            var sEvent = new Event(10, 2);
            _publisher.Publish(sEvent);

            Assert.AreEqual(result, sEvent);
        }

        [TestMethod]
        public void Test_Subscribe_To_Model_With_Filter_NoResult()
        {
            Event result = null;
            _subscriber.Subscribe<Event>(e => { result = e; }, f => { return f.X == 9; });

            var sEvent = new Event(10, 2);
            _publisher.Publish(sEvent);

            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void Test_Subscribe_To_Model_With_Filter_HasResult()
        {
            Event result = null;
            _subscriber.Subscribe<Event>(e => { result = e; }, f => { return f.X == 10; });

            var sEvent = new Event(10, 2);
            _publisher.Publish(sEvent);

            Assert.AreEqual(result, sEvent);
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

    }
}
