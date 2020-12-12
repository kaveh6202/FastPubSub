using PubSub;
using Serilog;
using Serilog.Events;
using SnakeSampleProject.Channel;
using SnakeSampleProject.Event;
using System;

namespace SnakeSampleProject
{
    class Program
    {

        private static IPublisher _publisher;
        private static ISubscriptionHandler _subscribable;

        private static IPublisher _snakePublishable;
        private static ISubscriptionHandler _snakeSubscribable;

        private static IPublisher _boardPublishable;
        private static ISubscriptionHandler _boardSubscribable;
        static void Main(string[] args)
        {
            InitLogger();
            InitializePubSubChannels();

            _subscribable.Subscribe<BoardCreated>(item => { 
                Log.Logger.Information(@$"
New Game Created -- Enjoy :)
Board Size - Width : {item.Width} ,Height : {item.Height}
Snake Head : {item.SnakeHead}
Snake Size : {item.SnakeSize}
Reward Location : {item.Reward}
-------------------------------
");
                //System.Threading.Thread.Sleep(5000);
                //throw new Exception("Oh No There is an exception!!!");
            });

            var snake = new Snake(_publisher, _subscribable);
            var board = new Board(_publisher, _subscribable, snake, Log.Logger);
            board.Init(20, 20);

            Console.ReadLine();
        }

        private static void InitializePubSubChannels()
        {
            var factory = new PubSubFactory().FireAndForgetCallback().IgnoreCallbackException();
            _publisher = factory.GetPublisher();
            _subscribable = factory.GetSubscriptionHandler();

            var snakeFactory = new PubSubFactory<SnakeChannel>().FireAndForgetCallback().IgnoreCallbackException();
            _snakePublishable = snakeFactory.GetPublisher();
            _snakeSubscribable = snakeFactory.GetSubscriptionHandler();

            var boardFactory = new PubSubFactory<BoardChannel>();
            _boardPublishable = boardFactory.GetPublisher();
            _boardSubscribable = boardFactory.GetSubscriptionHandler();
        }

        private static void InitLogger()
        {
            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .CreateLogger();
        }
    }
}
