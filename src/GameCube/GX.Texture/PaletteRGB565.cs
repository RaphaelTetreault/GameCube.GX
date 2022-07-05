using Manifold.IO;

namespace GameCube.GX.Texture
{
    [System.Serializable]
    public sealed class PaletteRGB565 : Palette
    {
        public override TextureFormat Format => TextureFormat.RGB565;


        public override void ReadPalette(EndianBinaryReader reader, IndirectEncoding indirectEncoding)
        {
            Colors = new TextureColor[indirectEncoding.MaxPaletteSize];
            for (int i = 0; i < Colors.Length; i++)
            {
                ushort rgb565 = reader.ReadUInt16();
                Colors[i] = TextureColor.FromRGB565(rgb565);
            }
        }

        public override void WritePalette(EndianBinaryWriter writer, IndirectEncoding indirectEncoding)
        {
            Assert.IsTrue(Colors.Length == indirectEncoding.MaxPaletteSize);

            for (int i = 0; i < Colors.Length; i++)
            {
                ushort rgb565 = TextureColor.ToRGB565(Colors[i]);
                writer.Write(rgb565);
            }
        }
    }
}
