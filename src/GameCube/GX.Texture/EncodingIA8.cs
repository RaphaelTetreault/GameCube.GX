using Manifold.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.GX.Texture
{
    public class EncodingIA8 : DirectEncoding
    {
        public override byte TileWidth => throw new NotImplementedException();

        public override byte TileHeight => throw new NotImplementedException();

        public override byte BitsPerPixel => throw new NotImplementedException();

        public override Tile DecodeTile(EndianBinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void EncodeTile(EndianBinaryWriter writerTile, Tile tile)
        {
            throw new NotImplementedException();
        }
    }
}
