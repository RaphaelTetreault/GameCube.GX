namespace GameCube.GX.Texture
{
    public abstract class Block
    {
        public readonly byte Width;
        public readonly byte Height;
        public readonly TextureFormat Format;

        public Block(byte width, byte height, TextureFormat format)
        {
            Width = width;
            Height = height;
            Format = format;
        }
    }
}
