namespace SnakeSampleProject.Event
{
    public class SnakeEatReward
    {
        public SnakeEatReward(int posX, int posY)
        {
            PosX = posX;
            PosY = posY;
        }

        public int PosX { get; }
        public int PosY { get; }
    }
}
