namespace SnakeSampleProject.Event
{
    public class SnakeHitObstacle
    {
        public SnakeHitObstacle(int posX, int posY)
        {
            PosX = posX;
            PosY = posY;
        }

        public int PosX { get; }
        public int PosY { get; }
    }
}
