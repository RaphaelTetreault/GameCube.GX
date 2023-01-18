using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Numerics;

namespace GameCube.GX.Texture
{
    /// <summary>
    ///     Utility class for DXT1 / Block Compression 1 algorithm.
    /// </summary>
    public static class DXT1
    {
        public static void RangeFitColors(in TextureColor[] pixelsBlock4x4, out ushort c0, out ushort c1, out uint packedIndexes)
        {
            bool isValid4x4Block = pixelsBlock4x4.Length == 4 * 4;
            if (!isValid4x4Block)
            {
                string msg = $"Argument {nameof(pixelsBlock4x4)}.Length is not exactly 16 (4x4).";
                throw new ArgumentException(msg);
            }

            // TODO: pass in function to get colour endpoints. This is a quick and dirty temp solution.
            // Compute endpoints using min/max color components.
            TextureColor min = new TextureColor(0xFFFFFFFF); // default is upper limit
            TextureColor max = new TextureColor(0x00000000); // default is lower limit
            foreach (TextureColor pixel in pixelsBlock4x4)
            {
                // Get minimum color of each component
                min.r = Math.Min(pixel.r, min.r);
                min.g = Math.Min(pixel.g, min.g);
                min.b = Math.Min(pixel.b, min.b);
                // Get maximum color of each component
                max.r = Math.Max(pixel.r, max.r);
                max.g = Math.Max(pixel.g, max.g);
                max.b = Math.Max(pixel.b, max.b);
            }

            ushort minRgb565 = TextureColor.ToRGB565(min);
            ushort maxRgb565 = TextureColor.ToRGB565(max);
            bool hasAlpha = ContainsTranslucidPixels(pixelsBlock4x4, 128);
            // c0 < c1 IF has alpha
            c0 = hasAlpha ? minRgb565 : maxRgb565;
            c1 = hasAlpha ? maxRgb565 : minRgb565;
            TextureColor[] colorPalette = EncodingCMPR.GetCmprPalette(c0, c1);

            // Convert color palette into Vector4 space
            Vector4[] vector4Palette = new Vector4[colorPalette.Length];
            for (int i = 0; i < vector4Palette.Length; i++)
                vector4Palette[i] = ColorToVector4(colorPalette[i]);

            // Get each pixel's nearest color value in palette
            byte[] unpackedIndexes = new byte[4 * 4];
            for (int i = 0; i < unpackedIndexes.Length; i++)
            {
                TextureColor color = pixelsBlock4x4[i];
                byte closestColorIndex = GetClosestIndex(color, vector4Palette);
                unpackedIndexes[i] = closestColorIndex;
            }

            packedIndexes = EncodingCMPR.PackIndexes(unpackedIndexes);

            // We're done! We have:
            // c0 and c1, properly ordered for DXT1 alpha lerp
            // packed indexes for palette lookup
        }

        /// <summary>
        ///     Check to see if any pixel in <paramref name="pixels"/> contains alpha lower than <paramref name="alphaThreshold"/>.
        /// </summary>
        /// <param name="pixels">The pixels to check against for alpha.</param>
        /// <param name="alphaThreshold">The maximum alpha value not considered to be translucid.</param>
        /// <returns>
        ///     True when any <paramref name="pixels"/>' alpha component is less than <paramref name="alphaThreshold"/>.
        /// </returns>
        private static bool ContainsTranslucidPixels(TextureColor[] pixels, byte alphaThreshold)
        {
            foreach (TextureColor pixel in pixels)
            {
                if (pixel.a < alphaThreshold)
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Get the index of the closest colour to <paramref name="color"/> within <paramref name="palette"/>.
        /// </summary>
        /// <param name="color">The colour to compare againts the palette.</param>
        /// <param name="palette">The colour palette used to represent colours.</param>
        /// <returns>
        ///     The index of the neared colour in <paramref name="palette"/> when representing
        ///     <paramref name="color"/> as a 4-dimensional vector.
        /// </returns>
        private static byte GetClosestIndex(TextureColor color, Vector4[] palette)
        {
            byte closesIndex = 0;
            float closestDistance = float.PositiveInfinity;
            Vector4 colorV4 = ColorToVector4(color);

            //
            for (byte i = 0; i < palette.Length; i++)
            {
                Vector4 paletteColor = palette[i];
                float distance = Vector4.Distance(colorV4, paletteColor);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closesIndex = i;
                }
            }
            return closesIndex;
        }

        private static Vector4 ColorToVector4(TextureColor color)
        {
            const float InvMax = 1f / 255f;
            Vector4 colorV4 = new Vector4(
                color.r * InvMax,
                color.g * InvMax,
                color.b * InvMax,
                color.a * InvMax);
            return colorV4;
        }

        #region TEST - WIP DXT1 Compression using LUT method. Did not work. :/
        public static void SingleColorLookupMethod(in TextureColor[] pixelsBlock4x4, out ushort c0, out ushort c1, out uint packedIndexes)
        {
            bool isValid4x4Block = pixelsBlock4x4.Length == 4 * 4;
            if (!isValid4x4Block)
            {
                string msg = $"Argument {nameof(pixelsBlock4x4)}.Length is not exactly 16 (4x4).";
                throw new ArgumentException(msg);
            }
            /// ABOVE IS GENERIC

            Compress3(pixelsBlock4x4, out Vector3 bestStart, out Vector3 bestEnd, out byte bestIndex);
            // Convert Vec3 to color
            TextureColor startColor = new TextureColor((byte)(bestStart.X * 255), (byte)(bestStart.Y * 255), (byte)(bestStart.Z * 255));
            TextureColor endColor = new TextureColor((byte)(bestEnd.X * 255), (byte)(bestEnd.Y * 255), (byte)(bestEnd.Z * 255));
            // Convert color to ushort
            ushort cmprStart = TextureColor.ToRGB565(startColor);
            ushort cmprEnd = TextureColor.ToRGB565(endColor);
            bool hasAlpha = ContainsTranslucidPixels(pixelsBlock4x4, 128);
            bool startGreaterThanEnd = cmprStart < cmprEnd;
            // c0 < c1 IF has alpha
            //c0 = hasAlpha && startGreaterThanEnd ? cmprStart : cmprEnd;
            //c1 = hasAlpha && startGreaterThanEnd ? cmprEnd : cmprStart;
            c0 = hasAlpha && startGreaterThanEnd ? cmprEnd : cmprStart;
            c1 = hasAlpha && startGreaterThanEnd ? cmprStart : cmprEnd;

            // BELOW IS GENERIC
            TextureColor[] colorPalette = EncodingCMPR.GetCmprPalette(c0, c1);

            // Convert color palette into Vector4 space
            Vector4[] vector4Palette = new Vector4[colorPalette.Length];
            for (int i = 0; i < vector4Palette.Length; i++)
                vector4Palette[i] = ColorToVector4(colorPalette[i]);

            // Get each pixel's nearest color value in palette
            byte[] unpackedIndexes = new byte[4 * 4];
            for (int i = 0; i < unpackedIndexes.Length; i++)
            {
                TextureColor color = pixelsBlock4x4[i];
                byte closestColorIndex = GetClosestIndex(color, vector4Palette);
                unpackedIndexes[i] = closestColorIndex;
            }

            packedIndexes = EncodingCMPR.PackIndexes(unpackedIndexes);

            // We're done! We have:
            // c0 and c1, properly ordered for DXT1 alpha lerp
            // packed indexes for palette lookup
        }

        // https://www.sjbrown.co.uk/posts/dxt-compression-techniques/
        public static void Compress3(in TextureColor[] pixelsBlock4x4, out Vector3 bestStart, out Vector3 bestEnd, out byte bestIndex)
        {
            // Get the lookups
            var LUTs = new DXT1LUT.SingleColourLookupTable[]
            {
                DXT1LUT.lookup_5_4,
                DXT1LUT.lookup_6_4,
                DXT1LUT.lookup_5_4,
            };

            ComputeEndpoints(LUTs, pixelsBlock4x4, out bestStart, out bestEnd, out bestIndex);

            // more to do
        }

        public static void ComputeEndpoints(DXT1LUT.SingleColourLookupTable[] LUTs, TextureColor[] colors, out Vector3 bestStart, out Vector3 bestEnd, out byte bestIndex)
        {
            int bestError = int.MaxValue;
            // Set defaults
            bestStart = new Vector3();
            bestEnd = new Vector3();
            bestIndex = 0;

            foreach (var color in colors)
            {
                byte[] m_colours = new byte[] { color.r, color.g, color.b };
                int nChannels = m_colours.Length;

                const int sourceBlocksPerLUT = 2;
                for (int sourceBlockIndex = 0; sourceBlockIndex < sourceBlocksPerLUT; sourceBlockIndex++)
                {
                    // Calculate how much error if using this channel index
                    int error = 0;
                    var sourceBlocks = new DXT1LUT.SourceBlock[nChannels]; // 3 - one for each channel
                    for (int colorChannelIndex = 0; colorChannelIndex < nChannels; colorChannelIndex++)
                    {
                        // Get lookup table and ... ???
                        var lookup = LUTs[colorChannelIndex];
                        int lookupIndex = m_colours[colorChannelIndex]; // for each colour component, get use byte as index into LUT

                        // Each iteration, copy single source block and add it to this array of 3
                        sourceBlocks[colorChannelIndex] = lookup.singleColourLookups[lookupIndex].sourceBlocks[sourceBlockIndex];

                        // Compute error, square it so more error is weighted heavier
                        int diff = sourceBlocks[colorChannelIndex].error;
                        error += diff * diff;
                    }

                    // keep it if the error is lower
                    if (error < bestError)
                    {
                        bestStart = new Vector3(
                            sourceBlocks[0].start / 31.0f,
                            sourceBlocks[1].start / 63.0f,
                            sourceBlocks[2].start / 31.0f
                        );
                        bestEnd = new Vector3(
                            sourceBlocks[0].end / 31.0f,
                            sourceBlocks[1].end / 63.0f,
                            sourceBlocks[2].end / 31.0f
                        );
                        bestIndex = (byte)(2 * sourceBlockIndex);
                        bestError = error;
                    }
                }
            }
        }
        #endregion

    }
}
