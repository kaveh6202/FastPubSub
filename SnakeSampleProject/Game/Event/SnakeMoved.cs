namespace SnakeSampleProject.Event
{
    class SnakeMoved
    {
        public Tile Head { get; set; }
        public int Size { get; set; }
        public SnakeDirection Direction { get; set; }
    }
}
