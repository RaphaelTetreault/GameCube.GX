using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GameCube.GX.Texture
{
    public abstract class DirectEncoding : Encoding
    {
        public int BytesPerPixel => BitsPerPixel / 8;
    }
}
