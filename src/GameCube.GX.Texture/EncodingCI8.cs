using Manifold.IO;

namespace GameCube.GX.Texture
{
    public sealed class EncodingCI8 : IndirectEncoding
    {
        public override byte BlockWidth => 8;
        public override byte BlockHeight => 4;
        public override byte BitsPerIndex => 8;
        public override ushort MaxPaletteSize => 1 << 8;
        public override TextureFormat Format => TextureFormat.CI8;


        public override Block ReadBlock(EndianBinaryReader reader)
        {
            var block = new IndirectBlock(BlockWidth, BlockHeight, Format);
            for (int i = 0; i < block.Indexes.Length; i++)
            {
                block.Indexes[i] = reader.ReadByte();
            }
            return block;
        }

        public override void WriteBlock(EndianBinaryWriter writer, Block block)
        {
            var indirectBlock = block as IndirectBlock;
            foreach (var index in indirectBlock.Indexes)
            {
                byte index8 = checked((byte)index);
                writer.Write(index8);
            }
        }
    }
}
