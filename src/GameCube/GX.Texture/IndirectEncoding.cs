namespace GameCube.GX.Texture
{
    public abstract class IndirectEncoding : Encoding
    {
        public abstract byte BitsPerIndex { get; }
        public int BytesPerIndex => BitsPerIndex / 8;
        public abstract ushort MaxPaletteSize { get; }
    }
}
