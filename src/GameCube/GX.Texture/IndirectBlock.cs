namespace GameCube.GX.Texture
{
    [System.Serializable]
    public sealed class IndirectBlock : Block
    {
        public ushort[] Indexes { get; private set; }

        public ushort this[int i] { get => Indexes[i]; set => Indexes[i] = value; }

        public IndirectBlock(byte width, byte height, TextureFormat format) : base(width, height, format)
        {
            Indexes = new ushort[Width * Height];
        }
    }
}
