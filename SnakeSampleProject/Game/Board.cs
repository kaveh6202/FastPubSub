using PubSub;
using Serilog;
using SnakeSampleProject.Event;

namespace SnakeSampleProject
{
    class Board
    {
        public int Height { get; private set; }
        public int Width { get; private set; }

        public Tile Reward { get; private set; }
        public Snake Snake { get; private set; }

        private readonly IPublisher _publisher;
        private readonly ISubscriptionHandler _subscribable;
        private readonly ILogger _logger;

        public Board(IPublisher publisher, ISubscriptionHandler subscribable, Snake snake,ILogger logger)
        {
            _publisher = publisher;
            _subscribable = subscribable;
            Snake = snake;
            _logger = logger;
        }

        public void Init(int width,int height)
        {
            Width = width > 10 ? width : 10;
            Height = height > 10 ? height : 10;
            Snake.Init(new Tile(6, 1), 4, SnakeDirection.Right);
            NewReward();

            var e = new BoardCreated(Height, Width, Snake.Head, Snake.Size.Value, Reward);
            _publisher.Publish(e);

            _subscribable.Subscribe<SnakeMoved>(this, item => 
            {
                _logger.Information($"Snaked Moved {item.Direction} - New Head : ({item.Head})");

                //check if snake eat a reward
                if (IntersectReward(item.Head))
                {
                    _logger.Warning("Snake Eat a Reward ! Hooray");
                    //NewReward();
                }

                if (IntersectObstacle(item.Head))
                {
                    _logger.Error("Snake Hit Obstacle !! You Lost :(");
                    _publisher.Publish(new GameOver());
                }
            });
        }

        private bool IntersectReward(Tile head)
        {
            return head.Equals(Reward);
        }

        private bool IntersectObstacle(Tile head)
        {
            return (head.PosX == 0 || head.PosX == Width || head.PosY == 0 || head.PosY == Height);
        }

        private void NewReward()
        {
            Reward = new Tile(15, 1);
        }
    }
}
