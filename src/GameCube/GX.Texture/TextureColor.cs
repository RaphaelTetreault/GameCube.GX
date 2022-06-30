using Manifold.IO;
using System.Runtime.InteropServices;

namespace GameCube.GX.Texture
{
    [StructLayout(LayoutKind.Explicit)]
    public struct TextureColor
    {
        [FieldOffset(0x00)] public uint raw;
        [FieldOffset(0x00)] public byte r;
        [FieldOffset(0x01)] public byte g;
        [FieldOffset(0x02)] public byte b;
        [FieldOffset(0x03)] public byte a;

        public TextureColor(int raw)
        {
            r = g = b = a = 0;
            this.raw = (uint)raw;
        }

        public TextureColor(uint raw)
        {
            r = g = b = a = 0;
            this.raw = raw;
        }

        public TextureColor(byte r, byte g, byte b, byte a = 0xFF)
        {
            raw = 0;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public TextureColor(byte intensity, byte a = 0xFF)
        {
            raw = 0;
            r = g = b = intensity;
            this.a = a;
        }

        public byte GetIntensity()
        {
            return (byte)(
                (r * 0.30f) +
                (g * 0.59f) +
                (b * 0.11f));
        }

        #region RGB565
        public static TextureColor FromRGB565(ushort rgb565)
        {
            byte r = (byte)(((rgb565 >> 11) & (0b_0001_1111)) * (1 << 3));
            byte g = (byte)(((rgb565 >> 05) & (0b_0011_1111)) * (1 << 2));
            byte b = (byte)(((rgb565 >> 00) & (0b_0001_1111)) * (1 << 3));
            var color = new TextureColor(r, g, b);
            return color;
        }
        public static ushort ToRGB565(TextureColor c)
        {
            byte r5 = (byte)((c.r >> 3) & 0b_0001_1111);
            byte g6 = (byte)((c.g >> 2) & 0b_0011_1111);
            byte b5 = (byte)((c.b >> 3) & 0b_0001_1111);
            ushort rgb565 = (ushort)(r5 << 11 + g6 << 05 + b5 << 00);
            return rgb565;
        }
        #endregion

        #region RGB5A3
        public static TextureColor FromRGB5A3(ushort rgb5a3)
        {
            byte r, g, b, a;
            bool hasAlpha = (rgb5a3 & 0x8000) == 0;
            if (hasAlpha)
            {
                a = (byte)(((rgb5a3 >> 12) & (0b_0000_0111)) * (1 << 5));
                r = (byte)(((rgb5a3 >> 08) & (0b_0000_1111)) * (1 << 4 + 1));
                g = (byte)(((rgb5a3 >> 04) & (0b_0000_1111)) * (1 << 4 + 1));
                b = (byte)(((rgb5a3 >> 00) & (0b_0000_1111)) * (1 << 4 + 1));
            }
            else
            {
                r = (byte)(((rgb5a3 >> 10) & (0b_0001_1111)) * (1 << 4));
                g = (byte)(((rgb5a3 >> 05) & (0b_0001_1111)) * (1 << 4));
                b = (byte)(((rgb5a3 >> 00) & (0b_0001_1111)) * (1 << 4));
                a = 0xFF;
            }
            var color = new TextureColor(r, g, b, a);
            return color;
        }
        public static ushort ToRGB5A3(TextureColor c)
        {
            byte r, g, b, a;
            ushort rgb5a3;

            // If alpha is mostly opaque, consider it fully opaque so that
            // we can use the format with more color depth.
            bool isVeryOpaque = (c.a >> 4) > 0b_0000_0111;
            if (isVeryOpaque)
            {
                // a is const 1 in bit position 15, 0x8000
                r = (byte)((c.r >> 3) & 0b_0001_1111);
                g = (byte)((c.g >> 3) & 0b_0001_1111);
                b = (byte)((c.b >> 3) & 0b_0001_1111);
                rgb5a3 = (ushort)(0x8000 + r << 10 + g << 5 + b << 0);
            }
            else
            {
                a = (byte)((c.a >> 5) & 0b_0000_0111); // 3 bits
                r = (byte)((c.r >> 4) & 0b_0000_1111); // 4 bits
                g = (byte)((c.g >> 4) & 0b_0000_1111); // 4 bits
                b = (byte)((c.b >> 4) & 0b_0000_1111); // 4 bits
                rgb5a3 = (ushort)(a << 12 + r << 8 + g << 4 + b << 0);
            }
            return rgb5a3;
        }
        #endregion

    }
}
