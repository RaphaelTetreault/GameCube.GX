using Manifold.IO;

namespace GameCube.GX.Texture
{
    public abstract class Palette
    {
        // Palettes for indirect textures
        public static readonly PaletteIA8 PaletteIA8 = new();
        public static readonly PaletteRGB565 PaletteRGB565 = new();
        public static readonly PaletteRGB5A3 PaletteRGB5A3 = new();

        public abstract TextureFormat Format { get; }
        public TextureColor[] Colors { get; protected set; }

        public abstract void ReadPalette(EndianBinaryReader reader, IndirectEncoding indirectEncoding);
        public abstract void WritePalette(EndianBinaryWriter writer, IndirectEncoding indirectEncoding);

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
