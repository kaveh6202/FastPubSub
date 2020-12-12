using PubSub;
using Serilog;
using Serilog.Events;
using SnakeSampleProject.Event;
using System;

namespace SnakeSampleProject
{
    class Program
    {

        private static IPublisher _publisher;
        private static ISubscriptionHandler _subscribable;
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
