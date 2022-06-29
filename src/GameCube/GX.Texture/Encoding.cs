using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GameCube.GX.Texture
{
    public abstract class Encoding
    {
        public abstract byte TileWidth { get; }
        public abstract byte TileHeight { get; }
        public abstract byte BitsPerPixel { get; }


        public abstract Tile DecodeTile(EndianBinaryReader reader);
        public Texture DecodeTexture(EndianBinaryReader reader, int width, int height)
        {
            throw new NotImplementedException();
        }

        public abstract void EncodeTile(EndianBinaryWriter writerTile, Tile tile);
        public void EncodeTexture(EndianBinaryWriter writer, Texture texture)
        {
            throw new NotImplementedException();
        }


        private static readonly EncodingI4 encodingI4 = new EncodingI4();
        private static readonly EncodingRGB565 encodingRGB565 = new EncodingRGB565();
        private static readonly EncodingCI8 encodingCI8 = new EncodingCI8();
        public static Encoding GetEncoding(TextureFormat textureFormat)
        {
            switch (textureFormat)
            {
                case TextureFormat.I4: return encodingI4;
                case TextureFormat.I8: throw new NotImplementedException();
                case TextureFormat.IA4: throw new NotImplementedException();
                case TextureFormat.IA8: throw new NotImplementedException();
                case TextureFormat.RGB565: return encodingRGB565;
                case TextureFormat.RGB5A3: throw new NotImplementedException();
                case TextureFormat.RGBA8: throw new NotImplementedException();
                case TextureFormat.CI4: throw new NotImplementedException();
                case TextureFormat.CI8: return encodingCI8;
                case TextureFormat.CI14X2: throw new NotImplementedException();
                case TextureFormat.CMPR: throw new NotImplementedException();
                default: throw new Exception($"Unhandled texture format {textureFormat}.");
            }
        }
    }
}
