namespace SnakeSampleProject.Event
{
    public class BoardCreated
    {
        public BoardCreated(int height, int width, Tile snakeHead, int snakeSize, Tile reward)
        {
            Height = height;
            Width = width;
            SnakeHead = snakeHead;
            SnakeSize = snakeSize;
            Reward = reward;
        }

        public int Height { get; }
        public int Width { get; }
        public Tile SnakeHead { get;  }
        public int SnakeSize { get; }
        public Tile Reward { get;  }
    }
}
