using Microsoft.VisualStudio.TestTools.UnitTesting;
using PubSub.Interfaces;
using System;
using System.Diagnostics;

namespace PubSub.Test
{
    [TestClass]
    public class ChannelConfigurationTest
    {
        IPubSubFactory<Channel1> factory1;
        IPubSubFactory<Channel2> factory2;
        IPubSubFactory<Channel3> factory3;
        [TestInitialize]
        public void Init()
        {
            factory1 = new PubSubFactory<Channel1>();
            factory2 = new PubSubFactory<Channel2>().IgnoreCallbackException();
            factory3 = new PubSubFactory<Channel3>().FireAndForgetCallback();

        }

        [TestMethod]
        //[ExpectedException(typeof(Exception), "excepting to raise an exception")]
        public void Test_Exception_Raise()
        {
            //var factory = new PubSubFactory();
            var subscriber = factory1.GetSubscriptionHandler();
            var publisher = factory1.GetPublisher();

            var exceptionThrown = false;
            try
            {
                subscriber.Subscribe<Event1>(item => throw new Exception(), item => item.Value == 1);

                publisher.Publish(new Event1(1));
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }
            finally
            {
                Assert.IsTrue(exceptionThrown);
            }
        }

        [TestMethod]
        public void Test_Exception_Ignore()
        {
            var subscriber = factory2.GetSubscriptionHandler();
            var publisher = factory2.GetPublisher();

            int result = 0;
            subscriber.Subscribe<Event1>(item =>
            {
                result = item.Value;
                throw new Exception();
            }, item => item.Value == 2);


            publisher.Publish(new Event1(2));

            Assert.AreEqual(result, 2);
        }

        [TestMethod]
        public void Test_WaitForCallbackResult()
        {
            var subscriber = factory1.GetSubscriptionHandler();
            var publisher = factory1.GetPublisher();

            string result = "";

            Stopwatch sw = new Stopwatch();
            sw.Start();
            subscriber.Subscribe<Event1>(item =>
            {
                result = item.Value.ToString();
                System.Threading.Thread.Sleep(2500);
                sw.Stop();
            }, item => item.Value == 3);

            publisher.Publish(new Event1(3));

            var elapsed = sw.ElapsedMilliseconds;
            Assert.IsTrue(elapsed >= 2500);
        }

        [TestMethod]
        public void Test_FireAndForgetCallback()
        {
            var subscriber = factory3.GetSubscriptionHandler();
            var publisher = factory3.GetPublisher();

            int result = 0;
            bool done = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            subscriber.Subscribe<Event1>(item =>
            {
                result = item.Value;
                System.Threading.Thread.Sleep(2500);
                sw.Stop();
                done = true;
            }, item => item.Value == 4);

            publisher.Publish(new Event1(4));

            while (result == 0) ;
            Assert.AreEqual(result, 4);
            Assert.IsFalse(done);
        }

        class Channel1 : BaseChannel { }
        class Channel2 : BaseChannel { }
        class Channel3 : BaseChannel { }

        class Event1
        {
            public int Value { get; set; }
            public Event1(int val)
            {
                Value = val;
            }
        }
    }
}
