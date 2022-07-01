using Manifold.IO;

namespace GameCube.GX.Texture
{
    public sealed class EncodingCMPR : DirectEncoding
    {
        public override byte BlockWidth => 8;
        public override byte BlockHeight => 8;
        public override byte BitsPerColor => 4;
        public override TextureFormat Format => TextureFormat.CMPR;


        public override Block ReadBlock(EndianBinaryReader reader)
        {
            var block = new DirectBlock(BlockWidth, BlockHeight, Format);

            // CMPR 8x8 is split into 2x2, ie quadrants
            for (int qy = 0; qy < 2; qy++)
            {
                for (int qx = 0; qx < 2; qx++)
                {
                    // Each subdivision is 4x4 with 2 leading RGB565 colors
                    ushort c0 = reader.ReadUInt16();
                    ushort c1 = reader.ReadUInt16();
                    var palette = GetCmprPalette(c0, c1);
                    // Then followed by 2-bit indexes packed into 32-bits
                    uint indexesPacked = reader.ReadUInt32();
                    byte[] indexes = UnpackIndexes(indexesPacked);

                    // Now that we have the data, get colors for each index
                    // and place it in the direct color block

                    // 2x2 mapped to 8x8: this index is the first for each quadrant
                    int quadrantIndex2x2 = qx * 4 + qy * 32; 
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

            // CMPR 8x8 is split into 2x2, ie quadrants
            for (int qy = 0; qy < 2; qy++)
            {
                for (int qx = 0; qx < 2; qx++)
                {
                    // Get colors
                    var colors4x4 = new TextureColor[4*4];
                    int quadrantBaseIndex = qy * 32 + qx * 4; // 2x2 mapped to 8x8
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            int subdivisionIndex = y * 4 + x; // 4x4
                            int blockColorIndex = quadrantBaseIndex + subdivisionIndex; // true 8x8 index
                            colors4x4[subdivisionIndex] = colorBlock.Colors[blockColorIndex];
                        }
                    }
                    // pass to DTX compressor
                    ushort c0 = 0xDEAD;
                    ushort c1 = 0xBEEF;
                    uint indexesPacked = 0xF0F0F0F0;

                    // write block
                    writer.Write(c0);
                    writer.Write(c1);
                    writer.Write(indexesPacked);

                    // TODO: actually implement DTX compresion :S
                    throw new System.NotImplementedException();
                }
            }
        }


        private TextureColor[] GetCmprPalette(ushort c0, ushort c1)
        {
            var colors = new TextureColor[4];
            colors[0] = TextureColor.FromRGB565(c0);
            colors[1] = TextureColor.FromRGB565(c1);
            if (c0 > c1)
            {
                colors[2] = TextureColor.Mix(colors[0], colors[1], 2f/3f);
                colors[3] = TextureColor.Mix(colors[0], colors[1], 1f/3f);
            }
            else
            {
                colors[2] = TextureColor.Mix(colors[0], colors[1], 1f/2f);
                colors[3] = new TextureColor(0x00000000);
            }
            return colors;
        }

        private byte[] UnpackIndexes(uint indexesPacked)
        {
            byte[] indexes = new byte[4*4];
            for (int i = 0; i < indexes.Length; i++)
            {
                // left-most bits are index0, rightmost are index15
                int rightShift = (15 - i) * 2;
                indexes[i] = (byte)((indexesPacked >> rightShift) & 0b_11);
            }
            return indexes;
        }

    }
}
