using Manifold.IO;

namespace GameCube.GX.Texture
{
    public abstract class Encoding
    {
        // Encodings for textures
        public static readonly EncodingI4 EncodingI4 = new();
        public static readonly EncodingI8 EncodingI8 = new();
        public static readonly EncodingIA4 EncodingIA4 = new();
        public static readonly EncodingIA8 EncodingIA8 = new();
        public static readonly EncodingRGB565 EncodingRGB565 = new();
        public static readonly EncodingRGB5A3 EncodingRGB5A3 = new();
        public static readonly EncodingRGBA8 EncodingRGBA8 = new();
        public static readonly EncodingCI4 EncodingCI4 = new();
        public static readonly EncodingCI8 EncodingCI8 = new();
        public static readonly EncodingCI14X2 EncodingCI14X2 = new();
        public static readonly EncodingCMPR EncodingCMPR = new();

        public abstract byte BlockWidth { get; }
        public abstract byte BlockHeight { get; }
        public abstract bool IsDirect { get; }
        public abstract bool IsIndirect { get; }
        public abstract TextureFormat Format { get; }
        public abstract int BytesPerBlock { get; }

        public abstract Block ReadBlock(EndianBinaryReader reader);
        public TBlock[] ReadBlocks<TBlock>(EndianBinaryReader reader, int blocksWidth, int blocksHeight, Encoding encoding)
            where TBlock : Block
        {
            int blocksCount = blocksWidth * blocksHeight;
            var blocks = new TBlock[blocksCount];
            for (int i = 0; i < blocksCount; i++)
                blocks[i] = encoding.ReadBlock(reader) as TBlock;
            return blocks;
        }

        public abstract void WriteBlock(EndianBinaryWriter writer, Block block);
        public void WriteBlocks(EndianBinaryWriter writer, Block[] blocks, int blocksWidth, int blocksHeight)
        {
            int blocksCount = blocksWidth * blocksHeight;
            Assert.IsTrue(blocks.Length == blocksCount);

            for (int h = 0; h < blocksHeight; h++)
            {
                for (int w = 0; w < blocksWidth; w++)
                {
                    int index = w + h * blocksWidth;
                    var block = blocks[index];
                    WriteBlock(writer, block);
                }
            }
        }

        public static Encoding GetEncoding(TextureFormat textureFormat)
        {
            switch (textureFormat)
            {
                case TextureFormat.I4: return EncodingI4;
                case TextureFormat.I8: return EncodingI8;
                case TextureFormat.IA4: return EncodingIA4;
                case TextureFormat.IA8: return EncodingIA8;
                case TextureFormat.RGB565: return EncodingRGB565;
                case TextureFormat.RGB5A3: return EncodingRGB5A3;
                case TextureFormat.RGBA8: return EncodingRGBA8;
                case TextureFormat.CI4: return EncodingCI4;
                case TextureFormat.CI8: return EncodingCI8;
                case TextureFormat.CI14X2: return EncodingCI14X2;
                case TextureFormat.CMPR: return EncodingCMPR;
                default: throw new System.Exception($"Unhandled texture format {textureFormat}.");
            }
        }

    }
}
