using Manifold.IO;

namespace GameCube.GX.Texture
{
    public class EncodingRGBA8 : DirectEncoding
    {
        public override byte TileWidth => 4;
        public override byte TileHeight => 4;
        public override byte BitsPerPixel => 32;

        public override Tile DecodeTile(EndianBinaryReader reader)
        {
            var directTile = new DirectTile(TileWidth, TileHeight);
            int size = TileWidth * TileHeight;
            var bytes = reader.ReadBytes(size);
            var colors = new TextureColor[size / 4];
            var a = ExtractBytes(bytes, 00, 2, 16);
            var r = ExtractBytes(bytes, 01, 2, 16);
            var g = ExtractBytes(bytes, 16, 2, 16);
            var b = ExtractBytes(bytes, 17, 2, 16);
            for (int i = 0; i < colors.Length; i++)
                colors[i] = new TextureColor(r[i], g[i], b[i], a[i]);
            directTile.Colors = colors;
            return directTile;
        }

        public override void EncodeTile(EndianBinaryWriter writer, Tile tile)
        {
            var directTile = tile as DirectTile;
            int nColors = directTile.Colors.Length;
            Assert.IsTrue(nColors == TileWidth * TileHeight);
            var a = new byte[nColors];
            var r = new byte[nColors];
            var g = new byte[nColors];
            var b = new byte[nColors];
            for (int i = 0; i < nColors; i++)
            {
                var color = directTile.Colors[i];
                a[i] = color.a;
                r[i] = color.r;
                g[i] = color.g;
                b[i] = color.b;
            }
            var bytes = new byte[TileWidth * TileHeight];
            InterleaveBytes(a, 00, 2, 16, ref bytes);
            InterleaveBytes(r, 01, 2, 16, ref bytes);
            InterleaveBytes(g, 16, 2, 16, ref bytes);
            InterleaveBytes(b, 17, 2, 16, ref bytes);
            writer.Write(bytes);
        }

        // TOOD: make generic on arrays, move elsewhere

        /// <summary>
        /// Iterate over <paramref name="bytes"/> a total of <paramref name="count"/> times, extracting each 
        /// value from position <paramref name="baseIndex"/> with successive separation of <paramref name="stride"/>.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="baseIndex"></param>
        /// <param name="stride"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] ExtractBytes(byte[] bytes, int baseIndex, int stride, int count)
        {
            byte[] values = new byte[count];
            int valuesIndex = 0;
            for (int i = baseIndex; i < baseIndex + count * stride; i += stride)
            {
                values[valuesIndex] = bytes[i];
                valuesIndex++;
            }
            return values;
        }

        /// <summary>
        /// Iterate over <paramref name="bytes"/> a total of <paramref name="count"/> times, interleaving
        /// each value to position <paramref name="baseIndex"/> with successive separation of <paramref name="stride"/>.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="baseIndex"></param>
        /// <param name="stride"></param>
        /// <param name="count"></param>
        /// <param name="destination"></param>
        public static void InterleaveBytes(byte[] bytes, int baseIndex, int stride, int count, ref byte[] destination)
        {
            int valuesIndex = 0;
            for (int i = baseIndex; i < baseIndex + count * stride; i += stride)
            {
                destination[i] = bytes[valuesIndex];
                valuesIndex++;
            }
        }

    }
}
