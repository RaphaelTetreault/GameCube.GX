using GameCube.GX.Texture;
using Manifold.IO;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Xml;

namespace GameCube.GCI
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TBinarySerializable"></typeparam>
    /// <remarks>
    ///     Override UniqueID (getter).
    ///     Add well-named accessor to <typeparamref name="TBinarySerializable"/>.
    /// </remarks>
    public abstract class Gci<TBinarySerializable> : //, TGciSubheader> :
        IBinaryFileType,
        IBinarySerializable
        where TBinarySerializable : IBinarySerializable, IBinaryFileType, new()
    {
        public const Endianness endianness = Endianness.BigEndian;
        public const string Extension = ".gci";
        public const int MinimumBlockSize = 0x2000; // 8192
        public const int MinimumHeaderSize = 0x40;  // 64
        public const int BannerWidth = 96;
        public const int BannerHeight = 32;
        public const int IconWidth = 32;
        public const int IconHeight = 32;


        public Endianness Endianness => endianness;
        public string FileExtension => Extension;
        public string FileName { get; set; } = string.Empty;


        public abstract string Comment { get; set; }
        public Texture Banner { get; protected set; } = new();
        public Texture[] Icons { get; protected set; } = Array.Empty<Texture>();


        protected GciHeader header = new();
        protected TBinarySerializable fileData = new();


        public abstract void DeserializeCommentAndImages(EndianBinaryReader reader);
        public abstract void SerializeCommentAndImages(EndianBinaryWriter writer);


        public void Deserialize(EndianBinaryReader reader)
        {
            // Read header
            header.Deserialize(reader);
            //
            DeserializeCommentAndImages(reader);
            // Read data
            var correctEndianReader = new EndianBinaryReader(reader.BaseStream, fileData.Endianness);
            fileData.Deserialize(correctEndianReader);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            // Write headder
            writer.Write(header);
            //
            SerializeCommentAndImages(writer);
            // Write file data using correct endianness
            var correctEndianWriter = new EndianBinaryWriter(writer.BaseStream, fileData.Endianness);
            correctEndianWriter.Write(fileData);

            // Pad to GC block alginment + "header" bytes.
            int unalignedBytes = (int)writer.BaseStream.Position % MinimumBlockSize;
            bool isAlgined = unalignedBytes == MinimumHeaderSize;
            if (!isAlgined)
            {
                writer.WritePadding(0x00, unalignedBytes);
            }
        }


        public bool IsTextureCI8 => GetTextureFormat() == TextureFormat.CI8;
        public bool IsTextureRGB565 => GetTextureFormat() == TextureFormat.RGB565;
        private TextureFormat GetTextureFormat()
        {
            bool isInvalid = (header.BannerAndIconFlags & BannerAndIconFlags.InvalidBanner) == BannerAndIconFlags.InvalidBanner;
            if (isInvalid)
            {
                string msg = $"Invalid banner and icon format detected.";
                throw new Exception(msg);
            }

            //bool isIndirectCI8 = (header.BannerAndIconFlags & BannerAndIconFlags.IndirectColorCI8) == BannerAndIconFlags.IndirectColorCI8;
            bool isDirectRGB565 = (header.BannerAndIconFlags & BannerAndIconFlags.DirectColorRGB565) == BannerAndIconFlags.DirectColorRGB565;
            TextureFormat textureFormat = isDirectRGB565 ? TextureFormat.RGB565 : TextureFormat.CI8;
            return textureFormat;
        }
        public Texture ReadDirectColorTexture(EndianBinaryReader reader, int width, int height)
        {
            Texture texture = Texture.ReadDirectColorTexture(reader, TextureFormat.RGB565, width, height);
            return texture;
        }
        public Texture ReadDirectColorBanner(EndianBinaryReader reader)
            => ReadDirectColorTexture(reader, BannerWidth, BannerHeight);
        public Texture ReadDirectColorIcon(EndianBinaryReader reader)
            => ReadDirectColorTexture(reader, IconWidth, IconHeight);
        public Palette ReadPalette(EndianBinaryReader reader)
        {
            Palette palette = Palette.ReadPalette(reader, TextureFormat.CI8, TextureFormat.RGB565);
            return palette;
        }
        public Texture ReadIndirectColorTexture(EndianBinaryReader reader, Palette palette, int width, int height)
        {
            Texture texture = Texture.ReadIndirectColorTexture(reader, palette, TextureFormat.CI8, width, height);
            return texture;
        }
        public Texture ReadIndirectColorBanner(EndianBinaryReader reader, Palette palette)
            => ReadIndirectColorTexture(reader, palette, BannerWidth, BannerHeight);
        public Texture ReadIndirectColorIcon(EndianBinaryReader reader, Palette palette)
            => ReadIndirectColorTexture(reader, palette, IconWidth, IconHeight);
    }
}
