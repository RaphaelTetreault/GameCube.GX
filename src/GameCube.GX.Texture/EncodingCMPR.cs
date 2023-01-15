using Manifold.IO;

namespace GameCube.GX.Texture
{
    /// <summary>
    /// Encoding format for 'compressed' (DXT1) texture.
    /// </summary>
    public sealed class EncodingCMPR : DirectEncoding
    {
        public override byte BlockWidth => 8;
        public override byte BlockHeight => 8;
        public override byte BitsPerColor => 4;
        public override TextureFormat Format => TextureFormat.CMPR;


        public override Block ReadBlock(EndianBinaryReader reader)
        {
            var block = new DirectBlock(BlockWidth, BlockHeight, Format);

            // CMPR 8x8 is split into 2x2, ie quadrants of 4x4
            for (int qy = 0; qy < 2; qy++)
            {
                for (int qx = 0; qx < 2; qx++)
                {
                    // Each subdivision is 4x4 with 2 leading RGB565 colors
                    ushort c0 = reader.ReadUInt16();
                    ushort c1 = reader.ReadUInt16();
                    TextureColor[] palette = GetCmprPalette(c0, c1);
                    // Then followed by 2-bit indexes packed into 32-bits
                    uint indexesPacked = reader.ReadUInt32();
                    byte[] indexes = UnpackIndexes(indexesPacked);

                    // Now that we have the data, get colors for each index
                    // and place it in the direct color block

                    // We must get the first index of the 2x2 grid for the pixel in the larger 8x8 grid.
                    // The following math gets the first pixel index for the quadrant.
                    int quadrantIndex2x2 = qx * 4 + qy * 32;
                    // Then we can iterate over the 4x4 subset.
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            // Get color
                            int blockIndex4x4 = x + (y * 4); // sub 4x4 index
                            byte paletteIndex = indexes[blockIndex4x4];
                            var color = palette[paletteIndex];

                            // Store color
                            int blockIndex8x8 = x + (y * 8) + quadrantIndex2x2; // true 8x8 index
                            block.Colors[blockIndex8x8] = color;
                        }
                    }
                }
            }
            return block;
        }

        public override void WriteBlock(EndianBinaryWriter writer, Block block)
        {
            var colorBlock = block as DirectBlock;

            // CMPR 8x8 is split into 2x2 quadrants of 4x4 pixels
            for (int qy = 0; qy < 2; qy++)
            {
                for (int qx = 0; qx < 2; qx++)
                {
                    // Get colors
                    var colors4x4 = new TextureColor[4*4];
                    // The following math gets the first pixel index for the quadrant.
                    int quadrantBaseIndex = qx * 4 + qy * 32 ;
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            int blockIndex4x4 = x + (y * 4); // 4x4
                            int blockIndex8x8 = x + (y * 8) + quadrantBaseIndex; // true 8x8 index
                            colors4x4[blockIndex4x4] = colorBlock.Colors[blockIndex8x8];
                        }
                    }

                    // Get color palette and indexes from compressor
                    DXT1.FastBadRmsCmprColors(colors4x4, out ushort c0, out ushort c1, out uint indexesPacked);
                    // write block
                    writer.Write(c0);
                    writer.Write(c1);
                    writer.Write(indexesPacked);
                }
            }
        }


        public static TextureColor[] GetCmprPalette(ushort c0, ushort c1)
        {
            var colors = new TextureColor[4];
            colors[0] = TextureColor.FromRGB565(c0);
            colors[1] = TextureColor.FromRGB565(c1);
            if (c0 > c1)
            {
                colors[2] = TextureColor.Lerp(colors[0], colors[1], 1f/3f);
                colors[3] = TextureColor.Lerp(colors[0], colors[1], 2f/3f);
            }
            else
            {
                colors[2] = TextureColor.Lerp(colors[0], colors[1], 1f/2f);
                colors[3] = new TextureColor(0x00000000);
            }
            return colors;
        }

        public static byte[] UnpackIndexes(uint indexesPacked)
        {
            byte[] indexes = new byte[4*4];
            for (int i = 0; i < indexes.Length; i++)
            {
                // left-most bits are index0, right-most bits are index15
                int rightShift = (15 - i) * 2;
                indexes[i] = (byte)((indexesPacked >> rightShift) & 0b_11);
            }
            return indexes;
        }

        public static uint PackIndexes(byte[] indexes)
        {
            bool isValidQuantity = indexes.Length == 4*4; // 16
            if (!isValidQuantity)
                throw new ArgumentException();

            uint packedIndexes = 0;
            for (int i = 0; i < indexes.Length; i++)
            {
                // left-most bits are index0, right-most bits are index15
                int leftShift = (15 - i) * 2;
                uint bits = (uint)((indexes[i] & 0b11) << leftShift);
                packedIndexes |= bits;
            }

            return packedIndexes;
        }

    }
}
