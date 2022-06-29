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
    }
}
