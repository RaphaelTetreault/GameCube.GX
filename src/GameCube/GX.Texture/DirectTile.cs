using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.GX.Texture
{
    public class DirectTile : Tile
    {
        public TextureColor[] Pixels { get; private set; }

        public TextureColor this[int i] { get => Pixels[i]; set => Pixels[i] = value; }

        public DirectTile(byte width, byte height) : base(width, height)
        {
            Pixels = new TextureColor[Width * Height];
        }
    }
}
