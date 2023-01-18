using Manifold.IO;

namespace GameCube.GX.Texture
{
    /// <summary>
    ///     The base representation for a GameCube indexed-colour palette.
    /// </summary>
    public abstract class Palette
    {
        // Palettes for indirect textures
        public static readonly PaletteIA8 PaletteIA8 = new();
        public static readonly PaletteRGB565 PaletteRGB565 = new();
        public static readonly PaletteRGB5A3 PaletteRGB5A3 = new();

        /// <summary>
        ///     The texture format used by this palette.
        /// </summary>
        public abstract TextureFormat Format { get; }
        
        /// <summary>
        ///     The colours used by this palette.
        /// </summary>
        public TextureColor[] Colors { get; protected set; } = new TextureColor[0];

        /// <summary>
        ///     Read a palette using the specified <paramref name="indirectEncoding"/> encoding.
        /// </summary>
        /// <param name="reader">The stream to read the palette from.</param>
        /// <param name="indirectEncoding">The indirect encoding format used to deserialize the target palette.</param>
        public abstract void ReadPalette(EndianBinaryReader reader, IndirectEncoding indirectEncoding);

        /// <summary>
        ///     Write a palette using the specified <paramref name="indirectEncoding"/> encoding.
        /// </summary>
        /// <param name="writer">The stream to write the palette to.</param>
        /// <param name="indirectEncoding">The indirect encoding to use (limits number of indexes).</param>
        public abstract void WritePalette(EndianBinaryWriter writer, IndirectEncoding indirectEncoding);

        /// <summary>
        ///     Fetch a shared palette instance for the provided <paramref name="textureFormat"/> format.
        /// </summary>
        /// <param name="textureFormat">The texture format of the block.</param>
        /// <returns>
        ///     A texture palette of the specified texture format.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     Thrown if the <paramref name="textureFormat"/> is not supported by the GameCube hardware
        ///     for use as a colour-indexed palette.
        /// </exception>
        public static Palette GetPalette(TextureFormat textureFormat)
        {
            switch (textureFormat)
            {
                case TextureFormat.IA8: return PaletteIA8;
                case TextureFormat.RGB565: return PaletteRGB565;
                case TextureFormat.RGB5A3: return PaletteRGB5A3;
                default: throw new System.ArgumentException($"No palette defined for texture format {textureFormat}.");
            }
        }
    }
}
