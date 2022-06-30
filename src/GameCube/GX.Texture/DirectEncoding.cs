namespace GameCube.GX.Texture
{
    public abstract class DirectEncoding : Encoding
    {
        public abstract byte BitsPerColor { get; }
        public int BytesPerPixel => BitsPerColor / 8;
        public override bool IsDirect => true;
        public override bool IsIndirect => false;
    }
}
