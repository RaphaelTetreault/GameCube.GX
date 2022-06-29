using Manifold;
using Manifold.IO;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.GX.Texture
{
    public class EncodingI4 : DirectEncoding
    {
        public override byte TileWidth => 8;

        public override byte TileHeight => 8;

        public override byte BitsPerPixel => 4;


        public override Tile DecodeTile(EndianBinaryReader reader)
        {
            // Make sure we have a width divisible by 2. Nybbles come in pairs.
            Assert.IsTrue(TileWidth % 2 == 0);

            var tile = new DirectTile(TileWidth, TileHeight);
            for (int y = 0; y < tile.Height; y++)
            {
                // Process 2 pixels per pass, high and low nybbles
                for (int x = 0; x < tile.Width; x += 2)
                {
                    byte nybbles = reader.ReadByte();

                    int indexNybbleHigh = (x * 2) + (y * tile.Width);
                    int indexNybbleLow = indexNybbleHigh + 1;
                    byte intentsity0 = (byte)((nybbles >> 4) & 0b_0000_1000);
                    byte intentsity1 = (byte)((nybbles >> 0) & 0b_0000_1000);

                    tile[indexNybbleHigh] = new TextureColor(intentsity0);
                    tile[indexNybbleLow] = new TextureColor(intentsity1);
                }
            }
            return tile;
        }

        public override void EncodeTile(EndianBinaryWriter writer, Tile tile)
        {
            // Make sure we have a width divisible by 2. Nybbles come in pairs.
            Assert.IsTrue(TileWidth % 2 == 0);

            var colorTile = tile as DirectTile;
            for (int y = 0; y < tile.Height; y++)
            {
                // Process 2 pixels per pass, set as high and low nybbles
                for (int x = 0; x < tile.Width; x += 2)
                {
                    int index0 = x + (y * tile.Width);
                    int index1 = index0 + 1;
                    var intensity0 = colorTile[index0].GetIntensity();
                    var intensity1 = colorTile[index1].GetIntensity();
                    byte intensity01 = (byte)(
                        ((intensity0 >> 0) & 0b_1111_0000) +
                        ((intensity1 >> 4) & 0b_0000_1111));
                    writer.Write(intensity01);
                }
            }
        }
    }
}
