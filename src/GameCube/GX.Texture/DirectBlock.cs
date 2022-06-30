namespace GameCube.GX.Texture
{
    public sealed class DirectBlock : Block
    {
        public TextureColor[] Colors { get; set; }

        public TextureColor this[int i] { get => Colors[i]; set => Colors[i] = value; }

        public DirectBlock(byte width, byte height, TextureFormat format) : base(width, height, format)
        {
            Colors = new TextureColor[Width * Height];
        }
    }
}
