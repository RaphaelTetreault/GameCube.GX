using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.GX.Texture
{
    internal class EncodingRGB565 : DirectEncoding
    {
        public override byte TileWidth => 4;
        public override byte TileHeight => 4;
        public override byte BitsPerPixel => 16;


        public override Tile DecodeTile(EndianBinaryReader reader)
        {
            var tile = new DirectTile(TileWidth, TileHeight);
            for (int y = 0; y < tile.Height; y++)
            {
                for (int x = 0; x < tile.Width; x++)
                {
                    int index = x + (y * tile.Width);
                    ushort rgb565 = reader.ReadUInt16();
                    byte r = (byte)(((rgb565 >> 11) & (0b_0001_1111)) * (1 << 3));
                    byte g = (byte)(((rgb565 >> 05) & (0b_0011_1111)) * (1 << 2));
                    byte b = (byte)(((rgb565 >> 00) & (0b_0001_1111)) * (1 << 3));
                    tile[index] = new TextureColor(r, g, b);
                }
            }
            return tile;
        }

        public override void EncodeTile(EndianBinaryWriter writer, Tile tile)
        {
            var colorTile = tile as DirectTile;
            for (int y = 0; y < tile.Height; y++)
            {
                for (int x = 0; x < tile.Width; x++)
                {
                    int index = x + (y * tile.Width);
                    var c = colorTile[index];
                    byte r5 = (byte)((c.r >> 3) & 0b_0001_1111);
                    byte g6 = (byte)((c.g >> 2) & 0b_0011_1111);
                    byte b5 = (byte)((c.b >> 3) & 0b_0001_1111);
                    ushort rgb565 = (ushort)(r5 << 11 + g6 << 05 + b5 << 00);
                    writer.Write(rgb565);
                }
            }
        }
    }
}
