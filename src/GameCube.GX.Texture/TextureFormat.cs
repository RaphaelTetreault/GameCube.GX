using System.ComponentModel;

namespace GameCube.GX.Texture
{
    /// <summary>
    /// GameCube GX texture formats.
    /// </summary>
    public enum TextureFormat : byte
    {
        [Description("4-bit intensity")]
        I4 = 0,

        [Description("8-bit intensity")]
        I8 = 1,

        [Description("4-bit intensity and 4-bit alpha")]
        IA4 = 2,

        [Description("8-bit intensity and 8-bit alpha")]
        IA8 = 3,

        [Description("16-bit color")]
        RGB565 = 4,

        [Description("16-bit color with variable alpha")]
        RGB5A3 = 5,

        [Description("24-bit color and 8-bit alpha")]
        RGBA8 = 6,

        [Description("4-bit index into 16 color palette")]
        CI4 = 8,

        [Description("8-bit index into 256 color palette")]
        CI8 = 9,

        [Description("14-bit index into 16384 color palette")]
        CI14X2 = 10,

        [Description("Compressed 16-bit color (DXT1)")]
        CMPR = 14
    }
}
