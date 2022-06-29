using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.GX.Texture
{
    public abstract class Tile
    {
        public readonly byte Width;
        public readonly byte Height;
        public Tile(byte width, byte height)
        {
            Width = width;
            Height = height;
        }
    }
}
