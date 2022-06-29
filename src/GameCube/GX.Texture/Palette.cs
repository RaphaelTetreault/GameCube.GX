using Manifold;
using Manifold.IO;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.GX.Texture
{
    public class Palette
    {
        public int Size { get; private set; }
        public TextureColor[] colors { get; private set; }

        public void DecodePalette(EndianBinaryReader endianBinaryReader, DirectEncoding directEncoding)
        {
            bool isNotIA8 = directEncoding.GetType() != typeof(EncodingIA8);
            bool isNotRGB565 = directEncoding.GetType() != typeof(EncodingRGB565);
            bool isNotRGB5A3 = directEncoding.GetType() != typeof(EncodingRGB5A3);
            if (isNotIA8 || isNotRGB565 || isNotRGB5A3)
                throw new ArgumentException($"Invalid encoding provided. Use IA8, RGB565, or RGB53A encodings.");

            throw new NotImplementedException();
        }
    }
}
