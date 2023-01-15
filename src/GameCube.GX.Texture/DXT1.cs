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
    /// Utility class for DXT1 / Block Compression 1 algorithm.
    /// </summary>
    public static class DXT1
    {
        public static void FastBadRmsCmprColors(in TextureColor[] pixels, out ushort c0, out ushort c1, out uint packedIndexes)
        {
            bool isValid4x4Block = pixels.Length == 4*4;
            if (!isValid4x4Block)
                throw new Exception("bad length");

            TextureColor min = new TextureColor(0xFFFFFFFF);
            TextureColor max = new TextureColor(0x00000000);
            foreach (TextureColor pixel in pixels)
            {
                // Get minimum color of each component
                min.r = pixel.r < min.r ? pixel.r : min.r;
                min.g = pixel.g < min.g ? pixel.g : min.g;
                min.b = pixel.b < min.b ? pixel.b : min.b;
                // Get maximum color of each component
                max.r = pixel.r > max.r ? pixel.r : max.r;
                max.g = pixel.g > max.g ? pixel.g : max.g;
                max.b = pixel.b > max.b ? pixel.b : max.b;
            }

            ushort minRgb565 = TextureColor.ToRGB565(min);
            ushort maxRgb565 = TextureColor.ToRGB565(max);
            bool hasAlpha = StrictContainsAlpha(pixels);
            // c0 < c1 IF has alpha
            c0 = hasAlpha ? minRgb565 : maxRgb565;
            c1 = hasAlpha ? maxRgb565 : minRgb565;
            TextureColor[] colorPalette = EncodingCMPR.GetCmprPalette(c0, c1);

            // Convert color palette into Vector4 space
            Vector4[] vector4Palette = new Vector4[colorPalette.Length];
            for (int i = 0; i < vector4Palette.Length; i++)
                vector4Palette[i] = ColorToVector4(colorPalette[i]);

            // Get each pixel's nearest color value in palette
            byte[] unpackedIndexes = new byte[4*4]; 
            for (int i = 0; i < unpackedIndexes.Length; i++)
            {
                TextureColor color = pixels[i];
                byte closestColorIndex = GetClosestIndex(color, vector4Palette);
                unpackedIndexes[i] = closestColorIndex;
            }

            packedIndexes = EncodingCMPR.PackIndexes(unpackedIndexes);

            // We're done! We have:
            // c0 and c1, properly ordered for DXT1 alpha lerp
            // packed indexes for palette lookup
        }


        private static bool StrictContainsAlpha(TextureColor[] pixels)
        {
            foreach (TextureColor pixel in pixels)
            {
                if (pixel.a != byte.MaxValue)
                    return true;
            }
            return false;
        }



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

    }
}
