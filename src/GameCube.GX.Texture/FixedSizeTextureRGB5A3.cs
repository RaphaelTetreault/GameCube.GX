using Manifold.IO;

namespace GameCube.GX.Texture
{
    public abstract class FixedSizeTextureRGB5A3 :
        IBinarySerializable
    {
        public abstract int Width { get; }
        public abstract int Height { get; }

        public static readonly DirectEncoding DirectEncoding = DirectEncoding.GetEncoding(TextureFormat.RGB5A3);


        public Texture Texture { get; set; } = new Texture();

        public void Deserialize(EndianBinaryReader reader)
        {
            int blocksHorizontal = Width / DirectEncoding.BlockWidth;
            int blocksVertical = Height / DirectEncoding.BlockHeight;
            int blocksCount = blocksHorizontal * blocksVertical;
            var blocks = DirectEncoding.ReadBlocks<DirectBlock>(reader, DirectEncoding, blocksCount);
            Texture = Texture.FromDirectBlocks(blocks, blocksHorizontal, blocksVertical);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            bool hasInvalidWidth = Texture.Width != Width;
            bool hasInvalidHeight = Texture.Height != Height;
            bool hasInvalidDimensions = hasInvalidWidth || hasInvalidHeight;
            if (hasInvalidDimensions)
            {
                string msg =
                    $"{GetType().Name} has invalid dimensions ({Texture.Width},{Texture.Height}). " +
                    $"{GetType().Name} must have a dimension of exactly ({Width}, {Height}).";
                throw new ArgumentException(msg);
            }

            var blocks = Texture.CreateTextureDirectColorBlocks(Texture, DirectEncoding);
            DirectEncoding.WriteBlocks(writer, blocks);
        }
    }
}