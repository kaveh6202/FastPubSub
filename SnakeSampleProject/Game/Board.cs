using PubSub;
using Serilog;
using SnakeSampleProject.Event;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeSampleProject
{
    public class Board
    {
        public int Height { get; private set; }
        public int Width { get; private set; }

        public Tile Reward { get; private set; }
        public Snake Snake { get; private set; }

        private readonly IPublisher _publisher;
        private readonly ISubscriptionHandler _subscribable;
        private readonly ILogger _logger;

        public Board(IPublisher publisher, ISubscriptionHandler subscribable, Snake snake, ILogger logger)
        {
            _publisher = publisher;
            _subscribable = subscribable;
            Snake = snake;
            _logger = logger;
        }

        public void Init(int width, int height)
        {
            Width = width > 10 ? width : 10;
            Height = height > 10 ? height : 10;
            Snake.Init(new Tile(6, 1), 4, SnakeDirection.Right);
            NewReward();

            DisplayEngine.Draw(this);

            var e = new BoardCreated(Height, Width, Snake.Head, Snake.Size.Value, Reward);
            _publisher.Publish(e);

            _subscribable.Subscribe<SnakeMoved>(this, item =>
            {
                //_logger.Information($"Snaked Moved {item.Direction} - New Head : ({item.Head})");

                //check if snake eat a reward
                if (IntersectReward(item.Head))
                {
                    //_logger.Warning("Snake Eat a Reward ! Hooray");
                    NewReward();
                    Snake.GetBigger();
                }

                if (IntersectObstacle(item.Head))
                {
                    //_logger.Error("Snake Hit Obstacle !! You Lost :(");
                    _publisher.Publish(new GameOver());
                }

                DisplayEngine.Draw(this);
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
            var tiles = CreateTiles(this);
            while (true)
            {
                var rndx = new Random((int)DateTime.Now.Ticks).Next(1, Width - 1);
                var rndy = new Random((int)DateTime.Now.Ticks + 5).Next(1, Height - 1);

                var tile = tiles.FirstOrDefault(i => i.Equal(rndx, rndy));
                if (tile.TileObj == TileType.Blank) {
                    Reward = tile;
                    break;
                }

            }
        }

        private static List<Tile> CreateTiles(Board board)
        {
            var tiles = new List<Tile>();

            for (int i = 0; i < board.Width; i++)
            {
                tiles.Add(new Tile() { PosX = i, PosY = 0, TileObj = TileType.Obstacle });
                tiles.Add(new Tile() { PosX = i, PosY = board.Height - 1, TileObj = TileType.Obstacle });
            }
            for (int i = 0; i < board.Height; i++)
            {
                tiles.Add(new Tile() { PosX = 0, PosY = i, TileObj = TileType.Obstacle });
                tiles.Add(new Tile() { PosX = board.Width - 1, PosY = i, TileObj = TileType.Obstacle });
            }

            if (board.Reward != null)
                tiles.Add(new Tile() { PosX = board.Reward.PosX, PosY = board.Reward.PosY, TileObj = TileType.Reward });

            tiles.AddRange(board.Snake.SnakeTiles.Select(i => new Tile() { PosX = i.PosX, PosY = i.PosY, TileObj = TileType.Snake }));

            for (int i = 0; i < board.Width; i++)
            {
                for (int j = 0; j < board.Height; j++)
                {
                    var obj = tiles.FirstOrDefault(k => k.Equal(i, j));
                    if (obj == null)
                    {
                        tiles.Add(new Tile() { PosX = i, PosY = j, TileObj = TileType.Blank });
                    }
                }
            }

            return tiles;
        }

    }
}
