﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace SnakeSampleProject
{
    public class Tile : IEquatable<Tile>
    {
        public int PosX { get; set; }
        public int PosY { get; set; }

        public Tile()
        {

        }

        public Tile(int x,int y)
        {
            PosX = x;
            PosY = y;
        }

        public bool Equals([AllowNull] Tile other)
        {
            return this.PosX == other.PosX && this.PosY == other.PosY;
        }

        public override string ToString()
        {
            return $"X = {PosX} , Y = {PosY}";
        }
    }
}
