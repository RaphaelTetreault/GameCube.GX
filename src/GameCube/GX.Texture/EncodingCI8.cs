using Manifold.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.GX.Texture
{
    public class EncodingCI8 : IndirectEncoding
    {
        public override byte TileWidth => 8;

        public override byte TileHeight => 4;

        public override byte BitsPerPixel => 8;

        public override Palette DecodePalette(EndianBinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override Tile DecodeTile(EndianBinaryReader reader)
        {
            var tile = new IndirectTile(TileWidth, TileHeight);
            for (int i = 0; i < tile.Indexes.Length; i++)
            {
                tile.Indexes[i] = reader.ReadByte();
            }
            return tile;
        }

        public override void EncodePalette(EndianBinaryWriter writer, Palette palette)
        {
            throw new NotImplementedException();
        }

        public override void EncodeTile(EndianBinaryWriter writerTile, Tile tile)
        {
            var indexTile = tile as IndirectTile;
            foreach (var index in indexTile.Indexes)
                writerTile.Write(index);
        }
    }
}
