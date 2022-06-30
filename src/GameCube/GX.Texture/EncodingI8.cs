using Manifold.IO;

namespace GameCube.GX.Texture
{
    public sealed class EncodingI8 : DirectEncoding
    {
        public override byte BlockWidth => 8;
        public override byte BlockHeight => 4;
        public override byte BitsPerColor => 8;

        public override Block ReadBlock(EndianBinaryReader reader)
        {
            var block = new DirectBlock(BlockWidth, BlockHeight);
            for (int y = 0; y < block.Height; y++)
            {
                for (int x = 0; x < block.Width; x++)
                {
                    byte i8 = reader.ReadByte();
                    var color = new TextureColor(i8);
                    int index = x + (y * block.Width);
                    block[index] = color;
                }
            }
            return block;
        }

        public override void WriteBlock(EndianBinaryWriter writer, Block block)
        {
            var colorBlock = block as DirectBlock;
            for (int y = 0; y < block.Height; y++)
            {
                for (int x = 0; x < block.Width; x++)
                {
                    int index = x + (y * block.Width);
                    var color = colorBlock[index];
                    byte i8 = color.GetIntensity();
                    writer.Write(i8);
                }
            }
        }
    }
}
