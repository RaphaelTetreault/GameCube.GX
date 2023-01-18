namespace GameCube.GX.Texture
{
    /// <summary>
    ///     The base representation of a GameCube indirect-colour texture format encoding.
    /// </summary>
    public abstract class IndirectEncoding : Encoding
    {
        /// <summary>
        ///     The number of bits used by this encoding to represent a single colour index.
        /// </summary>
        public abstract byte BitsPerIndex { get; }

        /// <summary>
        ///     The number of bytes used by this encoding to represent a single colour index.
        /// </summary>
        /// <remarks>
        ///     Will return 0 for encodings with less than 8 bits per index.
        /// </remarks>
        public int BytesPerIndex => BitsPerIndex / 8;

        /// <summary>
        ///     The maximum number of colours that can be represented using this encoding.
        /// </summary>
        public abstract ushort MaxPaletteSize { get; }

        public override bool IsDirect => false;
        public override bool IsIndirect => true;
        public override int BytesPerBlock => (int)MathF.Ceiling(BitsPerIndex / 8f * BlockWidth * BlockHeight);
    }
}
