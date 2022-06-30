using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.GX.Texture
{
    public class DirectTile : Tile
    {
        public TextureColor[] Colors { get; set; }

        public TextureColor this[int i] { get => Colors[i]; set => Colors[i] = value; }

        public DirectTile(byte width, byte height) : base(width, height)
        {
            Colors = new TextureColor[Width * Height];
        }
    }
}
