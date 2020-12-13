using PubSub;
using SnakeSampleProject.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace SnakeSampleProject
{
    public class Snake
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
            _snakeTimer = new Timer(100);
            _snakeTimer.AutoReset = false;
            _snakeTimer.Elapsed += _snakeTimer_Elapsed;
            _subscribable = subscribable;
        }

        private void _snakeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Move();

            var snakeMoveEvent = new SnakeMoved() { Size = SnakeTiles.Count(), Head = SnakeTiles[0], Direction = Direction };
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
            Tile previousTile = tile;
            SnakeTiles.Add(tile);
            for (int i = 1; i < size; i++)
            {

                var newTile = direction switch
                {
                    SnakeDirection.Up => new Tile(SnakeTiles[i - 1].PosX, SnakeTiles[i - 1].PosY + 1),
                    SnakeDirection.Down => new Tile(SnakeTiles[i - 1].PosX, SnakeTiles[i - 1].PosY - 1),
                    SnakeDirection.Left => new Tile(SnakeTiles[i - 1].PosX + 1, SnakeTiles[i - 1].PosY),
                    SnakeDirection.Right => new Tile(SnakeTiles[i - 1].PosX - 1, SnakeTiles[i - 1].PosY),
                    _ => throw new InvalidOperationException()
                };
                SnakeTiles.Add(newTile);
            }

            _subscribable.Subscribe<GameOver>(item => { _snakeTimer.Stop(); });
            _snakeTimer.Start();
        }

        public void GetBigger()
        {
            var newHead = Direction switch
            {
                SnakeDirection.Up => new Tile(Head.PosX, Head.PosY - 1),
                SnakeDirection.Down => new Tile(Head.PosX, Head.PosY + 1),
                SnakeDirection.Left => new Tile(Head.PosX - 1, Head.PosY),
                SnakeDirection.Right => new Tile(Head.PosX + 1, Head.PosY),
                _ => throw new InvalidOperationException()
            };
            SnakeTiles.Insert(0, newHead);
        }

        private void Move()
        {
            for (int i = SnakeTiles.Count - 1; i >= 1 ; i--)
            {
                SnakeTiles[i] = SnakeTiles[i - 1];
            }

            var head = SnakeTiles[0];
            var newHead = Direction switch
            {
                SnakeDirection.Up => new Tile(head.PosX, head.PosY-1),
                SnakeDirection.Down => new Tile(head.PosX, head.PosY+1),
                SnakeDirection.Left => new Tile(head.PosX-1, head.PosY),
                SnakeDirection.Right => new Tile(head.PosX+1, head.PosY),
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
