using PubSub;
using SnakeSampleProject.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace SnakeSampleProject
{
    class Snake
    {
        private readonly IPublisher _publisher;
        private readonly ISubscriptionHandler _subscribable;

        private SnakeDirection Direction;
        public List<Tile> SnakeTiles { get; private set; }
        public Tile Head => SnakeTiles?.FirstOrDefault();
        public int? Size => SnakeTiles?.Count();

        private readonly Timer _snakeTimer;

        public Snake(IPublisher publisher, ISubscriptionHandler subscribable)
        {
            _publisher = publisher;
            _snakeTimer = new Timer(250);
            _snakeTimer.AutoReset = false;
            _snakeTimer.Elapsed += _snakeTimer_Elapsed;
            _subscribable = subscribable;
        }

        private void _snakeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Move();

            var snakeMoveEvent = new SnakeMoved() { Size = SnakeTiles.Count(), Head = SnakeTiles[0] , Direction = Direction };
            _publisher.Publish(snakeMoveEvent);
            _snakeTimer.Start();
        }

        public void ChangeDirection(SnakeDirection direction)
        {
            Direction = direction;
        }

        public void Init(Tile head, int size, SnakeDirection direction)
        {
            Direction = direction;
            SnakeTiles = new List<Tile>();
            var tile = head;
            for (int i = 0; i < size; i++)
            {
                SnakeTiles.Add(tile);
                tile = direction switch
                {
                    SnakeDirection.Up => new Tile(tile.PosX, ++tile.PosY),
                    SnakeDirection.Down => new Tile(tile.PosX, --tile.PosY),
                    SnakeDirection.Left => new Tile(++tile.PosX, tile.PosY),
                    SnakeDirection.Right => new Tile(--tile.PosX, tile.PosY),
                    _ => throw new InvalidOperationException()
                };
            }

            _subscribable.Subscribe<GameOver>(item => { _snakeTimer.Stop(); });
            _snakeTimer.Start();
        }

        private void Move()
        {
            for (int i = 1; i < SnakeTiles.Count - 1; i++)
            {
                SnakeTiles[i] = SnakeTiles[i - 1];
            }

            var head = SnakeTiles[0];
            var newHead = Direction switch
            {
                SnakeDirection.Up => new Tile(head.PosX, --head.PosY),
                SnakeDirection.Down => new Tile(head.PosX, ++head.PosY),
                SnakeDirection.Left => new Tile(--head.PosX, head.PosY),
                SnakeDirection.Right => new Tile(++head.PosX, head.PosY),
                _ => throw new InvalidOperationException()
            };
            SnakeTiles[0] = newHead;
        }
    }

    public enum SnakeDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
