using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeSampleProject
{
    public static class DisplayEngine
    {
        private static object _locker = new object();
        private static List<Tile> LastBoardTiles;
        public static void Draw(Board board)
        {
            lock (_locker)
            {

                if (LastBoardTiles != null)
                {
                    //draw diff 

                    var newTiles = CreateTiles(board);


                    var diff = new List<Tile>();
                    foreach (var item in newTiles)
                    {
                        var oldTile = LastBoardTiles.FirstOrDefault(i => i.Equal(item.X, item.Y));
                        if (oldTile == null) continue;
                        if (oldTile.TileObj != item.TileObj)
                        {
                            diff.Add(item);
                        }
                    }

                    foreach (var item in diff)
                    {
                        Console.SetCursorPosition(item.X, item.Y);
                        WriteSymbol(item.TileObj);
                    }

                    LastBoardTiles = newTiles.ToList();

                    return;
                }


                //lock (_locker)
                //{
                Console.SetCursorPosition(0, 0);

                var tiles = CreateTiles(board);
                for (int y = 0; y < board.Height; y++)
                {
                    for (int x = 0; x < board.Width; x++)
                    {
                        var obj = tiles.FirstOrDefault(k => k.Equal(x, y));
                        WriteSymbol(obj.TileObj);
                    }
                    Console.WriteLine();
                }

                LastBoardTiles = tiles.ToList() ;
            }
            //}
        }

        private static void WriteSymbol(TileObj obj)
        {
            Console.Write(obj switch { TileObj.Obstacle => "^", TileObj.Reward => "$", TileObj.Snake => "*", TileObj.Blank => " " });
        }

        private static List<Tile> CreateTiles(Board board)
        {
            var tiles = new List<Tile>();

            for (int i = 0; i < board.Width; i++)
            {
                tiles.Add(new Tile() { X = i, Y = 0, TileObj = TileObj.Obstacle });
                tiles.Add(new Tile() { X = i, Y = board.Height - 1, TileObj = TileObj.Obstacle });
            }
            for (int i = 0; i < board.Height; i++)
            {
                tiles.Add(new Tile() { X = 0, Y = i, TileObj = TileObj.Obstacle });
                tiles.Add(new Tile() { X = board.Width - 1, Y = i, TileObj = TileObj.Obstacle });
            }

            tiles.Add(new Tile() { X = board.Reward.PosX, Y = board.Reward.PosY, TileObj = TileObj.Reward });

            tiles.AddRange(board.Snake.SnakeTiles.Select(i => new Tile() { X = i.PosX, Y = i.PosY, TileObj = TileObj.Snake }));

            for (int i = 0; i < board.Width; i++)
            {
                for (int j = 0; j < board.Height; j++)
                {
                    var obj = tiles.FirstOrDefault(k => k.Equal(i, j));
                    if (obj == null)
                    {
                        tiles.Add(new Tile() { X = i, Y = j, TileObj = TileObj.Blank });
                    }
                }
            }

            return tiles;
        }

        enum TileObj
        {
            Obstacle,
            Snake,
            Reward,
            Blank
        }

        class Tile
        {
            public int X { get; set; }
            public int Y { get; set; }
            public TileObj TileObj { get; set; }

            public bool Equal(int x, int y)
            {
                return X == x && Y == y;
            }
        }
    }
}
