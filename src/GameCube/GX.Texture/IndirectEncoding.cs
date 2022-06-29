using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GameCube.GX.Texture
{
    public abstract class IndirectEncoding : Encoding
    {
        public byte BitsPerPaletteColor => throw new NotImplementedException();

        public abstract Palette DecodePalette(EndianBinaryReader reader);
        public abstract void EncodePalette(EndianBinaryWriter writer, Palette palette);
    }
}
