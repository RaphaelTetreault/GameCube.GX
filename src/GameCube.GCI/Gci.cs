using GameCube.GX.Texture;
using Manifold.IO;

namespace GameCube.GCI
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TBinarySerializable"></typeparam>
    public abstract class Gci<TBinarySerializable> :
        IBinaryFileType,
        IBinarySerializable
        where TBinarySerializable : IBinarySerializable, IBinaryFileType, new()
    {
        public const TextureFormat DirectFormat = TextureFormat.RGB5A3;
        public const TextureFormat IndirectFormat = TextureFormat.CI8;

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


        public Texture Banner { get; protected set; } = new();
        public abstract string Comment { get; set; }
        public TBinarySerializable FileData { get; set; } = new();
        public GciHeader Header { get; protected set; } = new();
        public Texture[] Icons { get; protected set; } = Array.Empty<Texture>();


        public abstract void DeserializeCommentAndImages(EndianBinaryReader reader);
        public abstract void SerializeCommentAndImages(EndianBinaryWriter writer);

        public virtual void Deserialize(EndianBinaryReader reader)
        {
            // Read header
            Header.Deserialize(reader);
            //
            DeserializeCommentAndImages(reader);
            // Read data
            var correctEndianReader = new EndianBinaryReader(reader.BaseStream, FileData.Endianness);
            FileData.Deserialize(correctEndianReader);
        }
        public virtual void Serialize(EndianBinaryWriter writer)
        {
            // Write headder
            writer.Write(Header);
            //
            SerializeCommentAndImages(writer);
            // Write file data using correct endianness
            var correctEndianWriter = new EndianBinaryWriter(writer.BaseStream, FileData.Endianness);
            correctEndianWriter.Write(FileData);

            // Pad to GC block alginment + "header" bytes.
            int unalignedBytes = (int)writer.BaseStream.Position % MinimumBlockSize;
            bool isAlgined = unalignedBytes == MinimumHeaderSize;
            if (!isAlgined)
            {
                writer.WritePadding(0x00, unalignedBytes);
            }
        }


        public void SetFileData()
        {
            throw new NotImplementedException();
        }
        public void SetBanner()
        {
            throw new NotImplementedException();
        }
        public void SetIcons()
        {
            throw new NotImplementedException();
        }
        public void SetFileName()
        {
            throw new NotImplementedException();
        }


        public bool IsTextureCI8 => GetTextureFormat() == TextureFormat.CI8;
        public bool IsTextureRGB5A3 => GetTextureFormat() == TextureFormat.RGB5A3;

        private TextureFormat GetTextureFormat()
        {
            bool isInvalid = (Header.BannerAndIconFlags & BannerAndIconFlags.InvalidBanner) == BannerAndIconFlags.InvalidBanner;
            if (isInvalid)
            {
                string msg = $"Invalid banner and icon format detected.";
                throw new Exception(msg);
            }

            //bool isIndirectCI8 = (header.BannerAndIconFlags & BannerAndIconFlags.IndirectColorCI8) == BannerAndIconFlags.IndirectColorCI8;
            bool isDirectRGB5A3 = (Header.BannerAndIconFlags & BannerAndIconFlags.DirectColorRGB5A3) == BannerAndIconFlags.DirectColorRGB5A3;
            TextureFormat textureFormat = isDirectRGB5A3 ? DirectFormat : IndirectFormat;
            return textureFormat;
        }
        private Texture ReadDirectColorTexture(EndianBinaryReader reader, int width, int height)
        {
            Texture texture = Texture.ReadDirectColorTexture(reader, DirectFormat, width, height);
            return texture;
        }
        public Texture ReadDirectColorBanner(EndianBinaryReader reader)
            => ReadDirectColorTexture(reader, BannerWidth, BannerHeight);
        public Texture ReadDirectColorIcon(EndianBinaryReader reader)
            => ReadDirectColorTexture(reader, IconWidth, IconHeight);
        public Palette ReadPalette(EndianBinaryReader reader)
        {
            Palette palette = Palette.CreatePalette(DirectFormat);
            palette.ReadPaletteColors(reader, IndirectFormat);
            return palette;
        }
        private Texture ReadIndirectColorTexture(EndianBinaryReader reader, Palette palette, int width, int height)
        {
            Texture texture = Texture.ReadIndirectColorTexture(reader, palette, IndirectFormat, width, height);
            return texture;
        }
        public Texture ReadIndirectColorBanner(EndianBinaryReader reader, Palette palette)
            => ReadIndirectColorTexture(reader, palette, BannerWidth, BannerHeight);
        public Texture ReadIndirectColorIcon(EndianBinaryReader reader, Palette palette)
            => ReadIndirectColorTexture(reader, palette, IconWidth, IconHeight);

        // Direct color write
        private void WriteDirectColorTexture(EndianBinaryWriter writer, Texture texture)
        {
            Texture.WriteDirectColorTexture(writer, texture, DirectFormat);
        }
        public void WriteDirectColorBanner(EndianBinaryWriter writer)
            => WriteDirectColorTexture(writer, Banner);
        public void WriteDirectColorIcons(EndianBinaryWriter writer)
        {
            foreach (var icon in Icons)
            {
                WriteDirectColorTexture(writer, icon);
            }
        }
        // Indirect color write. Both as palette+indexes write, or as functions to get data if not in that order
        public void WriteSharedIndirectColorIcons(EndianBinaryWriter writer)
        {
            Texture combinedIcons = CreateSharedIcon();
            Texture.WriteIndirectColorTexture(writer, combinedIcons, IndirectFormat, DirectFormat);
        }
        public void WriteUniqueIndirectColorTexture(EndianBinaryWriter writer, Texture texture)
        {
            Texture.WriteIndirectColorTexture(writer, texture, IndirectFormat, DirectFormat);
        }
        private Texture CreateSharedIcon()
        {
            int count = Icons.Length;
            Assert.IsTrue(count <= 8);

            Texture combinedIcons = new Texture(IconWidth, IconHeight * count);
            for (int i = 0; i < count; i++)
            {
                int originY = i * IconHeight;
                Texture icon = Icons[i];
                Texture.Copy(icon, combinedIcons, 0, originY);
            }

            return combinedIcons;
        }
        public void GetSharedIconIndirectData(out IndirectBlock[] blocks, out Palette palette)
        {
            Texture combinedIcons = CreateSharedIcon();
            GetUniqueIndirectData(combinedIcons, out blocks, out palette);
        }
        public void GetUniqueIndirectData(Texture texture, out IndirectBlock[] blocks, out Palette palette)
        {
            Texture.CreateIndirectColorBlocksAndPaletteFromTexture(texture, IndirectFormat, out blocks, DirectFormat, out palette);
        }
    }
}
