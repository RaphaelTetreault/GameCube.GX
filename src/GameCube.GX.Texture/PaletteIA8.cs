using Manifold.IO;

namespace GameCube.GX.Texture
{
    [System.Serializable]
    public sealed class PaletteIA8 : Palette
    {
        public override TextureFormat Format => TextureFormat.IA8;


        public override void ReadPalette(EndianBinaryReader reader, IndirectEncoding indirectEncoding)
        {
            Colors = new TextureColor[indirectEncoding.MaxPaletteSize];
            for (int i = 0; i < Colors.Length; i++)
            {
                ushort ia8 = reader.ReadUInt16();
                Colors[i] = TextureColor.FromIA8(ia8);
            }
        }

        public override void WritePalette(EndianBinaryWriter writer, IndirectEncoding indirectEncoding)
        {
            Assert.IsTrue(Colors.Length == indirectEncoding.MaxPaletteSize);

            for (int i = 0; i < Colors.Length; i++)
            {
                ushort ia8 = TextureColor.ToIA8(Colors[i]);
                writer.Write(ia8);
            }
        }
    }
}
