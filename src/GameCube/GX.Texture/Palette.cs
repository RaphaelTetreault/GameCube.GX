using Manifold.IO;

namespace GameCube.GX.Texture
{
    public abstract class Palette
    {
        public TextureColor[] Colors { get; protected set; }

        public abstract void ReadPalette(EndianBinaryReader reader, IndirectEncoding indirectEncoding);
        public abstract void WritePalette(EndianBinaryWriter writer, IndirectEncoding indirectEncoding);
    }
}
