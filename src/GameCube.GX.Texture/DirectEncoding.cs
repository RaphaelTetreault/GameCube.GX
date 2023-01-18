namespace GameCube.GX.Texture
{
    /// <summary>
    ///     The base representation of a GameCube direct-colour texture format encoding.
    /// </summary>
    public abstract class DirectEncoding : Encoding
    {
        /// <summary>
        ///     The number of bits used by this encoding to represent a single colour.
        /// </summary>
        public abstract byte BitsPerColor { get; }

        /// <summary>
        ///     The number of bytes used by this encoding to represent a single colour.
        /// </summary>
        /// <remarks>
        ///     Will return 0 for encodings with less than 8 bits per colour.
        /// </remarks>
        public int BytesPerPixel => BitsPerColor / 8;

        public override bool IsDirect => true;
        public override bool IsIndirect => false;
        public override int BytesPerBlock => (int)MathF.Ceiling(BitsPerColor / 8f * BlockWidth * BlockHeight);
    }
}
