using Manifold.IO;

namespace GameCube.GX.Texture
{
    public abstract class Encoding
    {
        // Encodings for direct textures
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
        // Palettes for indirect textures
        public static readonly PaletteIA8 PaletteIA8 = new();
        public static readonly PaletteRGB565 PaletteRGB565 = new();
        public static readonly PaletteRGB5A3 PaletteRGB5A3 = new();

        public abstract byte BlockWidth { get; }
        public abstract byte BlockHeight { get; }


        public abstract Block ReadBlock(EndianBinaryReader reader);
        public Texture DecodeTexture(EndianBinaryReader reader, int width, int height)
        {
            throw new System.NotImplementedException();
        }

        public abstract void WriteBlock(EndianBinaryWriter writerBlock, Block block);
        public void EncodeTexture(EndianBinaryWriter writer, Texture texture)
        {
            throw new System.NotImplementedException();
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
                case TextureFormat.CMPR: return EncodingCMPR ;
                default: throw new System.Exception($"Unhandled texture format {textureFormat}.");
            }
        }

        public static Palette GetPalette(TextureFormat textureFormat)
        {
            switch (textureFormat)
            {
                case TextureFormat.IA8: return PaletteIA8;
                case TextureFormat.RGB565: return PaletteRGB565;
                case TextureFormat.RGB5A3: return PaletteRGB5A3;
                default: throw new System.Exception($"No palette defined for texture format {textureFormat}.");
            }
        }
    }
}
