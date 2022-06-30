using Manifold.IO;

namespace GameCube.GX.Texture
{
    public class EncodingRGB5A3 : DirectEncoding
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
                    ushort rgb5a3 = reader.ReadUInt16();
                    var color = TextureColor.FromRGB565(rgb5a3);
                    int index = x + (y * tile.Width);
                    tile[index] = color;
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
                    var color = colorTile[index];
                    ushort rgb5a3 = TextureColor.ToRGB5A3(color);
                    writer.Write(rgb5a3);
                }
            }
        }
    }
}
