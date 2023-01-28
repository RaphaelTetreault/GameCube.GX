using Manifold.IO;

namespace GameCube.GX.Texture
{
    public abstract class FixedSizeTextureRGB5A3 :
        IBinarySerializable
    {
        public abstract int Width { get; }
        public abstract int Height { get; }

        public static readonly DirectEncoding DirectEncoding = DirectEncoding.GetEncoding(TextureFormat.RGB5A3);


        private Texture texture = new Texture();
        public Texture Texture => texture;

        public void Deserialize(EndianBinaryReader reader)
        {
            int blocksHorizontal = Width / DirectEncoding.BlockWidth;
            int blocksVertical = Height / DirectEncoding.BlockHeight;
            int blocksCount = blocksHorizontal * blocksVertical;
            var blocks = DirectEncoding.ReadBlocks<DirectBlock>(reader, DirectEncoding, blocksCount);
            texture = Texture.FromDirectBlocks(blocks, blocksHorizontal, blocksVertical);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            bool hasInvalidWidth = texture.Width != Width;
            bool hasInvalidHeight = texture.Height != Height;
            bool hasInvalidDimensions = hasInvalidWidth || hasInvalidHeight;
            if (hasInvalidDimensions)
            {
                string msg =
                    $"Banner has invalid dimensions ({texture.Width},{texture.Height})." +
                    $"Banner must have a dimension of exactly ({texture.Width}, {texture.Height}).";
                throw new ArgumentException(msg);
            }

            var blocks = Texture.CreateTextureDirectColorBlocks(texture, DirectEncoding);
            DirectEncoding.WriteBlocks(writer, blocks);
        }
    }
}