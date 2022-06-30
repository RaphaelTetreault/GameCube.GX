namespace GameCube.GX.Texture
{
    public abstract class Block
    {
        public readonly byte Width;
        public readonly byte Height;
        public Block(byte width, byte height)
        {
            Width = width;
            Height = height;
        }
    }
}
