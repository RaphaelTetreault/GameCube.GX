using Manifold.IO;

namespace GameCube.GX.Texture
{
    public sealed class PaletteRGB5A3 : Palette
    {
        public override void ReadPalette(EndianBinaryReader reader, IndirectEncoding indirectEncoding)
        {
            Colors = new TextureColor[indirectEncoding.MaxPaletteSize];
            for (int i = 0; i < Colors.Length; i++)
            {
                ushort rgb5a3 = reader.ReadUInt16();
                Colors[i] = TextureColor.FromRGB5A3(rgb5a3);
            }
        }

        public override void WritePalette(EndianBinaryWriter writer, IndirectEncoding indirectEncoding)
        {
            Assert.IsTrue(Colors.Length == indirectEncoding.MaxPaletteSize);

            for (int i = 0; i < Colors.Length; i++)
            {
                ushort rgb5a3 = TextureColor.ToRGB5A3(Colors[i]);
                writer.Write(rgb5a3);
            }
        }
    }
}
