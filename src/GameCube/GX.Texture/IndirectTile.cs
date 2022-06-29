using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.GX.Texture
{
    public class IndirectTile : Tile
    {
        public int[] Indexes { get; private set; }

        public int this[int i] { get => Indexes[i]; set => Indexes[i] = value; }

        public IndirectTile(byte width, byte height) : base(width, height)
        {
            Indexes = new int[Width * Height];
        }
    }
}
