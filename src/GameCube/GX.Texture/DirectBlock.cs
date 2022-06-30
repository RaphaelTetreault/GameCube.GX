namespace GameCube.GX.Texture
{
    public sealed class DirectBlock : Block
    {
        public TextureColor[] Colors { get; set; }

        public TextureColor this[int i] { get => Colors[i]; set => Colors[i] = value; }

        public DirectBlock(byte width, byte height) : base(width, height)
        {
            Colors = new TextureColor[Width * Height];
        }
    }
}
