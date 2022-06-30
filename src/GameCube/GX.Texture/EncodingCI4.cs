using Manifold.IO;

namespace GameCube.GX.Texture
{
    public sealed class EncodingCI4 : IndirectEncoding
    {
        public override byte BlockWidth => 8;
        public override byte BlockHeight => 8;
        public override byte BitsPerIndex => 4;
        public override ushort MaxPaletteSize => 1 << 4;
        public override TextureFormat Format => TextureFormat.CI4;


        public override Block ReadBlock(EndianBinaryReader reader)
        {
            var block = new IndirectBlock(BlockWidth, BlockHeight, Format);
            Assert.IsTrue(block.Indexes.Length % 2 == 0);

            // process 2 indexes at a time
            for (int i = 0; i < block.Indexes.Length; i += 2)
            {
                byte indexes01 = reader.ReadByte();
                byte index0 = (byte)((indexes01 >> 4) & 0b_0000_1111);
                byte index1 = (byte)((indexes01 >> 0) & 0b_0000_1111);
                block.Indexes[i + 0] = index0;
                block.Indexes[i + 1] = index1;
            }
            return block;
        }

        public override void WriteBlock(EndianBinaryWriter writer, Block block)
        {
            var indeirectBlock = block as IndirectBlock;
            Assert.IsTrue(indeirectBlock.Indexes.Length % 2 == 0);

            // Process 2 indexes at a time
            for (int i = 0; i < indeirectBlock.Indexes.Length; i += 2)
            {
                byte index0 = checked((byte)(i * 2));
                byte index1 = checked((byte)(index0 + 1));
                byte indexes01 = (byte)(index0 << 4 + index1 << 0);
                writer.Write(indexes01);
            }
        }
    }
}
