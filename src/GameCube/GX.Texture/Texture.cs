using System;

namespace GameCube.GX.Texture
{
    public class Texture
    {
        public TextureFormat Format { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TextureColor[] Pixels { get; private set; }
        public Palette Palette { get; private set; }
        public Block[] Blocks { get; private set; }
        public bool IsPaletted => Palette is not null;


        public TextureColor this[int i]
        {
            get => Pixels[i];
            set => Pixels[i] = value;
        }
        public TextureColor this[int x, int y]
        {
            get => Pixels[x + y * Width];
            set => Pixels[x + y * Width] = value;
        }


        public Texture() { }
        public Texture(int width, int height, TextureFormat format = TextureFormat.RGBA8)
        {
            Format = format;
            Width = width;
            Height = height;
            Pixels = new TextureColor[Width * Height];
        }


        public static Texture FromColors(TextureColor[] colors, int width, int height)
        {
            int numPixels = width * height;
            if (numPixels != colors.Length)
                throw new ArgumentException("Number of raw colors does not match length of width*height.");

            var texture = new Texture
            {
                Format = TextureFormat.RGBA8,
                Width = width,
                Height = height,
                Pixels = new TextureColor[numPixels]
            };

            // Copy pixels
            Array.Copy(colors, texture.Pixels, numPixels);

            return texture;
        }

        public static Texture FromDirectBlocks(DirectBlock[] directBlocks, int blocksWidth, int blocksHeight)
        {
            int subBlockWidth = directBlocks[0].Width;
            int subBlockHeight = directBlocks[0].Height;
            var texture = new Texture();
            texture.Format = directBlocks[0].Format;
            texture.Width = blocksWidth * subBlockWidth;
            texture.Height = blocksHeight * subBlockHeight;
            texture.Blocks = directBlocks;

            int pixelIndex = 0;
            // Linearize texture pixels
            for (int h = 0; h < blocksHeight; h++)
            {
                for (int y = 0; y < subBlockHeight; y++)
                {
                    for (int w = 0; w < blocksWidth; w++)
                    {
                        int blockIndex = h * blocksWidth + w;
                        for (int x = 0; x < subBlockWidth; x++)
                        {
                            int colorIndex = y * subBlockWidth + x;
                            var block = directBlocks[blockIndex];
                            var color = block.Colors[colorIndex];
                            texture.Pixels[pixelIndex] = color;
                        }
                    }
                }
            }
            return texture;
        }

        public static Texture FromIndexBlocksAndPalette(IndirectBlock[] indirectBlocks, int blocksWidth, int blocksHeight, Palette palette)
        {
            int subBlockWidth = indirectBlocks[0].Width;
            int subBlockHeight = indirectBlocks[0].Height;
            var texture = new Texture();
            texture.Format = indirectBlocks[0].Format;
            texture.Width = blocksWidth * subBlockWidth;
            texture.Height = blocksHeight * subBlockHeight;
            texture.Palette = palette;
            texture.Blocks = indirectBlocks;

            int pixelIndex = 0;
            // Linearize texture pixels
            for (int h = 0; h < blocksHeight; h++)
            {
                for (int y = 0; y < subBlockHeight; y++)
                {
                    for (int w = 0; w < blocksWidth; w++)
                    {
                        int blockIndex = h * blocksWidth + w;
                        for (int x = 0; x < subBlockWidth; x++)
                        {
                            int subBlockIndex = y * subBlockWidth + x;
                            var indirectBlock = indirectBlocks[blockIndex];
                            var indirectIndex = indirectBlock.Indexes[subBlockIndex];
                            var color = palette.Colors[indirectIndex];
                            texture.Pixels[pixelIndex] = color;
                        }
                    }
                }
            }
            return texture;
        }

        public static Texture FromRawColors(uint[] rawColors, int width, int height)
        {
            int numPixels = width * height;
            if (numPixels != rawColors.Length)
                throw new ArgumentException("Number of raw colors does not match length of width*height.");

            var texture = new Texture
            {
                Format = TextureFormat.RGBA8,
                Width = width,
                Height = height,
                Pixels = new TextureColor[numPixels]
            };

            // Copy in pixels
            for (int i = 0; i < rawColors.Length; i++)
                texture.Pixels[i] = new TextureColor(rawColors[i]);

            return texture;
        }

    }
}
