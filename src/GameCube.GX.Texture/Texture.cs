using System;

namespace GameCube.GX.Texture
{
    /// <summary>
    ///     A GameCube GX texture.
    /// </summary>
    /// <remarks>
    ///     Invaluable resource: <see href="https://wiki.tockdom.com/wiki/Image_Formats"></see>
    /// </remarks>
    [Serializable]
    public class Texture
    {
        /// <summary>
        ///     The texture's format.
        /// </summary>
        public TextureFormat Format { get; set; } = TextureFormat.RGBA8;

        /// <summary>
        ///     The texture's pixel width.
        /// </summary>
        public int Width { get; private set; } = 0;

        /// <summary>
        ///     The texture's pixel height.
        /// </summary>
        public int Height { get; private set; } = 0;

        /// <summary>
        ///     The texture's pixels.
        /// </summary>
        /// <remarks>
        ///     Organized horizontally left-to-right with subsequent rows stacked vertically.
        /// </remarks>
        public TextureColor[] Pixels { get; private set; } = new TextureColor[0];

        /// <summary>
        ///     The texture's colour pallete, if texture uses a colour-indexed texture format.
        /// </summary>
        public Palette Palette { get; private set; } = null;

        /// <summary>
        ///     The texture's blocks.
        /// </summary>
        public Block[] Blocks { get; private set; } = new Block[0];

        /// <summary>
        ///     True if the texture's palette is not null.
        /// </summary>
        public bool IsPaletted => Palette is not null;


        /// <summary>
        ///     Indexer to get get/set a direct colour (pixel) within this texture.
        /// </summary>
        /// <param name="i">The direct colour (pixel) index in this texture.</param>
        /// <returns>
        ///     Direct colour (pixel) at the specified index within this block.
        /// </returns>
        public TextureColor this[int i]
        {
            get => Pixels[i];
            set => Pixels[i] = value;
        }

        /// <summary>
        ///     Indexer to get get/set a direct colour pixel within this texture.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the pixel in this texture.</param>
        /// <param name="y">The vertical coordinate of the pixel in this texture.</param>
        /// <returns>
        ///     Direct colour (pixel) at the specified coordinate within this block.
        /// </returns>
        public TextureColor this[int x, int y]
        {
            get => Pixels[x + y * Width];
            set => Pixels[x + y * Width] = value;
        }


        /// <summary>
        ///     Create a new empty texture.
        /// </summary>
        public Texture() { }

        /// <summary>
        ///     Create a new texture of <paramref name="width"/> by <paramref name="height"/> size
        ///     of the specified <paramref name="format"/>.
        /// </summary>
        /// <param name="width">The texture's pixel width.</param>
        /// <param name="height">The texture's pixel height.</param>
        /// <param name="format">The texture's format.</param>
        public Texture(int width, int height, TextureFormat format = TextureFormat.RGBA8)
        {
            Format = format;
            Width = width;
            Height = height;
            Pixels = new TextureColor[Width * Height];
        }

        /// <summary>
        ///     Create a new texture of <paramref name="width"/> by <paramref name="height"/> size
        ///     of the specified <paramref name="format"/> whose pixel contents are all <paramref name="color"/>.
        /// </summary>
        /// <param name="width">The texture's pixel width.</param>
        /// <param name="height">The texture's pixel height.</param>
        /// <param name="color">The default colour of all pixels for the texture.</param>
        /// <param name="format">The texture's format.</param>
        public Texture(int width, int height, TextureColor color, TextureFormat format = TextureFormat.RGBA8)
        {
            Format = format;
            Width = width;
            Height = height;
            Pixels = new TextureColor[Width * Height];

            // Set all colors as default
            for (int i = 0; i < Pixels.Length; i++)
                Pixels[i] = color;
        }


        /// <summary>
        ///     
        /// </summary>
        /// <param name="directEncoding"></param>
        /// <returns>
        ///     
        /// </returns>
        public static DirectBlock[] CreateTextureDirectColorBlocks(Texture sourceTexture, DirectEncoding directEncoding, out int blocksCountHorizontal, out int blocksCountVertical)
        {
            blocksCountHorizontal = sourceTexture.Width / directEncoding.BlockWidth;
            blocksCountVertical = sourceTexture.Height / directEncoding.BlockHeight;
            int blocksCount = blocksCountHorizontal * blocksCountVertical;
            DirectBlock[] blocks = new DirectBlock[blocksCount];

            for (int v = 0; v < blocksCountVertical; v++)
            {
                int originY = v * directEncoding.BlockHeight;
                for (int h = 0; h < blocksCountHorizontal; h++)
                {
                    int originX = h * directEncoding.BlockWidth;
                    int blockIndex = h + v * blocksCountHorizontal;
                    blocks[blockIndex] = RegionToDirectBlock(sourceTexture, directEncoding, originX, originY);
                }
            }

            return blocks;
        }

        public static DirectBlock[] CreateTextureDirectColorBlocks(Texture sourceTexture, DirectEncoding directEncoding)
            => CreateTextureDirectColorBlocks(sourceTexture, directEncoding, out _, out _);


        /// <summary>
        ///     Cretae a new texture block from a <paramref name="sourceTexture"/>.
        /// </summary>
        /// <param name="sourceTexture">The source texture to copy a region from.</param>
        /// <param name="directEncoding">The encoding to use use for the output block.</param>
        /// <param name="originX">The left edge of the copied region from <paramref name="sourceTexture"/>.</param>
        /// <param name="originY">The top edge of the copied region from <paramref name="sourceTexture"/>.</param>
        /// <returns>
        ///     A new block which contains the region sampled from <paramref name="sourceTexture"/>. The width
        ///     and height of the block depends on the <paramref name="directEncoding"/> used.
        /// </returns>
        public static DirectBlock RegionToDirectBlock(Texture sourceTexture, DirectEncoding directEncoding, int originX, int originY)
        {
            var block = new DirectBlock(directEncoding);

            // Figure out how many pixels to copy over. On smaller textures (eg: 4x2), outside region is black.
            int nPixelsX = Math.Min(directEncoding.BlockWidth, sourceTexture.Width - originX);
            int nPixelsY = Math.Min(directEncoding.BlockHeight, sourceTexture.Height - originY);

            // Copy texture region into block
            for (int y = 0; y < nPixelsY; y++)
            {
                int sourceY = originY + y;
                for (int x = 0; x < nPixelsX; x++)
                {
                    int sourceX = originX + x;
                    block[x, y] = sourceTexture[sourceX, sourceY];
                }
            }

            return block;
        }


        /// <summary>
        ///     Create a new texture of <paramref name="width"/> by <paramref name="height"/> size.
        ///     The texture's pixels are <paramref name="colors"/>.
        /// </summary>
        /// <param name="colors">The source pixels to construct the texture with.</param>
        /// <param name="width">The texture's pixel width.</param>
        /// <param name="height">The texture's pixel height.</param>
        /// <returns>
        ///     A new texture created from the source <paramref name="colors"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if the size of the texture and number of <paramref name="colors"/> are not equal.
        /// </exception>
        public static Texture FromColors(TextureColor[] colors, int width, int height)
        {
            int numPixels = width * height;
            if (numPixels != colors.Length)
            {
                string msg = "Number of raw colors does not match length of width*height.";
                throw new ArgumentException(msg);
            }

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

        /// <summary>
        ///     Create a texture from an array of <paramref name="directBlocks"/> where <paramref name="blocksCountHorizontal"/>
        ///     defines the number of blocks across the texture width and <paramref name="blocksCountVertical"/> defines the number
        ///     of blocks across the texture height.
        /// </summary>
        /// <param name="directBlocks">The source texture blocks to construct the texture with.</param>
        /// <param name="blocksCountHorizontal">The number of blocks along the horizontal axis.</param>
        /// <param name="blocksCountVertical">The number of blocks along the vertical axis.</param>
        /// <returns>
        ///     A new texture created from the source <paramref name="directBlocks"/>.
        /// </returns>
        public static Texture FromDirectBlocks(DirectBlock[] directBlocks, int blocksCountHorizontal, int blocksCountVertical)
        {
            int numBlocks = blocksCountHorizontal * blocksCountVertical;
            if (numBlocks != directBlocks.Length)
            {
                string msg =
                    $"Number of {nameof(DirectBlock)} does not match length " +
                    $"{nameof(blocksCountHorizontal)}*{nameof(blocksCountVertical)}.";
                throw new ArgumentException(msg);
            }

            int subBlockWidth = directBlocks[0].Width;
            int subBlockHeight = directBlocks[0].Height;
            var format = directBlocks[0].Format;

            int pixelsWidth = blocksCountHorizontal * subBlockWidth;
            int pixelsHeight = blocksCountVertical * subBlockHeight;
            var texture = new Texture(pixelsWidth, pixelsHeight, format);
            texture.Blocks = directBlocks;

            int pixelIndex = 0;
            // Linearize texture pixels
            for (int v = 0; v < blocksCountVertical; v++)
            {
                for (int y = 0; y < subBlockHeight; y++)
                {
                    for (int h = 0; h < blocksCountHorizontal; h++)
                    {
                        // Which block we are sampling
                        int blockIndex = h + v * blocksCountHorizontal;
                        for (int x = 0; x < subBlockWidth; x++)
                        {
                            // Which sub-block we are sampling
                            int colorIndex = x + y * subBlockWidth;
                            var block = directBlocks[blockIndex];
                            var color = block.Colors[colorIndex];
                            texture.Pixels[pixelIndex++] = color;
                        }
                    }
                }
            }
            return texture;
        }

        /// <summary>
        ///     Create a texture from an array of <paramref name="indirectBlocks"/> where <paramref name="blocksCountHorizontal"/>
        ///     defines the number of blocks across the texture width and <paramref name="blocksCountVertical"/> defines the number
        ///     of blocks across the texture height.
        /// </summary>
        /// <param name="indirectBlocks">The source texture blocks to construct the texture with.</param>
        /// <param name="blocksCountHorizontal">The number of blocks along the horizontal axis.</param>
        /// <param name="blocksCountVertical">The number of blocks along the vertical axis.</param>
        /// <param name="palette">The colour palette for the <paramref name="indirectBlocks"/> indexes to sample from.</param>
        /// <returns>
        ///     A new texture created from the source <paramref name="indirectBlocks"/> and <paramref name="palette"/>.
        /// </returns>
        public static Texture FromIndirectBlocksAndPalette(IndirectBlock[] indirectBlocks, int blocksCountHorizontal, int blocksCountVertical, Palette palette)
        {
            int numBlocks = blocksCountHorizontal * blocksCountVertical;
            if (numBlocks != indirectBlocks.Length)
            {
                string msg =
                    $"Number of {nameof(IndirectBlock)} does not match length " +
                    $"{nameof(blocksCountHorizontal)}*{nameof(blocksCountVertical)}.";
                throw new ArgumentException(msg);
            }

            int blockWidth = indirectBlocks[0].Width;
            int blockHeight = indirectBlocks[0].Height;
            int pixelsCount = blocksCountHorizontal * blocksCountVertical * blockWidth * blockHeight;
            var texture = new Texture
            {
                Format = indirectBlocks[0].Format,
                Width = blocksCountHorizontal * blockWidth,
                Height = blocksCountVertical * blockHeight,
                Pixels = new TextureColor[pixelsCount],
                Palette = palette,
                Blocks = indirectBlocks,
            };

            int pixelIndex = 0;
            // Linearize texture pixels
            for (int v = 0; v < blocksCountVertical; v++)
            {
                for (int y = 0; y < blockHeight; y++)
                {
                    for (int h = 0; h < blocksCountHorizontal; h++)
                    {
                        int blockIndex = v * blocksCountHorizontal + h;
                        for (int x = 0; x < blockWidth; x++)
                        {
                            int subBlockIndex = y * blockWidth + x;
                            var indirectBlock = indirectBlocks[blockIndex];
                            var indirectIndex = indirectBlock.ColorIndexes[subBlockIndex];
                            var color = palette.Colors[indirectIndex];
                            texture.Pixels[pixelIndex++] = color;
                        }
                    }
                }
            }
            return texture;
        }

        /// <summary>
        ///     Create a new texture of <paramref name="width"/> by <paramref name="height"/> size.
        ///     The texture's pixels are <paramref name="rawColors"/> in RGBA format.
        /// </summary>
        /// <param name="rawColors">The raw pixels source to construct the texture with.</param>
        /// <param name="width">The texture's pixel width.</param>
        /// <param name="height">The texture's pixel height.</param>
        /// <returns>
        ///     A new texture created from the source <paramref name="rawColors"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if the size of the texture and number of <paramref name="rawColors"/> are not equal.
        /// </exception>
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

        /// <summary>
        ///     Create a new texture cropped from the a region of <paramref name="sourceTexture"/>.
        /// </summary>
        /// <param name="sourceTexture">The source texture to crop from.</param>
        /// <param name="pixelWidth">The pixel width of the cropped region.</param>
        /// <param name="pixelHeight">The pixel height of the cropped region.</param>
        /// <param name="originX">The horizontal origin point of the cropping region.</param>
        /// <param name="originY">The vertical origin point of the cropping region.</param>
        /// <returns>
        ///     A new texture instance with pixel contents cropped from the <paramref name="sourceTexture"/>.
        /// </returns>
        /// <remarks>
        ///     The new texture's format is the same as the source texture.
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="originX"/> or <paramref name="originY"/> are negative.
        ///     Thrown if desired crop region does not fit within the bounds of <paramref name="sourceTexture"/>.
        /// </exception>
        public static Texture Crop(Texture sourceTexture, int pixelWidth, int pixelHeight, int originX = 0, int originY = 0)
        {
            // Make sure origin indexes are not negative
            bool isNegative = originX < 0 || originY < 0;
            if (isNegative)
            {
                string msg = $"Neither argument {nameof(originX)} or {nameof(originY)} can be negative.";
                throw new ArgumentException(msg);
            }

            // Make sure desired crop region is within bounds of texture.
            bool isTooWide = (originX + pixelWidth) > sourceTexture.Width;
            bool isTooTall = (originY + pixelHeight) > sourceTexture.Height;
            bool isInvalidCropRegion = isTooWide || isTooTall;
            if (isInvalidCropRegion)
            {
                string msg =
                    $"Crop region is either too wide ({isTooWide}) " +
                    $"or too tall ({isTooTall}) for the {nameof(sourceTexture)}.";
                throw new ArgumentException(msg);
            }

            // Begin crop
            var cropped = new Texture(pixelWidth, pixelHeight, sourceTexture.Format);
            cropped.Blocks = sourceTexture.Blocks;

            for (int y = 0; y < cropped.Height; y++)
            {
                int sourceY = originY + y;
                for (int x = 0; x < cropped.Width; x++)
                {
                    int sourceX = originX + x;
                    cropped[x, y] = sourceTexture[sourceX, sourceY];
                }
            }

            return cropped;
        }

        public static void Copy(Texture sourceTexture, Texture destinationTexture, int destinationOriginX = 0, int destinationOriginY = 0)
        {
            bool canFitX = destinationOriginX + sourceTexture.Width <= destinationTexture.Width;
            bool canFitY = destinationOriginY + sourceTexture.Height <= destinationTexture.Height;
            bool cannotFitCopy = !canFitX || !canFitY;
            if (cannotFitCopy)
            {
                string msg =
                    $"Cannot copy texture contents. Destination texture ({destinationTexture.Width},{destinationTexture.Height}) " +
                    $"not large enough to fit source texture ({sourceTexture.Width},{sourceTexture.Height}) at origin point " +
                    $"({destinationOriginX},{destinationOriginY})";
                throw new ArgumentException(msg);
            }

            // Copy over data
            for (int y = 0; y < sourceTexture.Height; y++)
            {
                int dy = destinationOriginY + y;
                for (int x = 0; x < sourceTexture.Width; x++)
                {
                    int dx = destinationOriginX + x; ;
                    destinationTexture[dx,dy] = sourceTexture[x,y];
                }
            }
        }
    }
}
