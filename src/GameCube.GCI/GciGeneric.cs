using Manifold.IO;

namespace GameCube.GCI
{
    public abstract class Gci<TBinarySerializable> :
        IBinaryFileType,
        IBinarySerializable
        where TBinarySerializable : IBinarySerializable, new()
    {
        public const Endianness endianness = Endianness.BigEndian;
        public const string Extension = ".gci";
        public const int MinimumBlockSize = 0x2000; // 8192
        public const int MinimumHeaderSize = 0x40;  // 64

        public Endianness Endianness => endianness;
        public string FileExtension => Extension;
        public string FileName { get; set; } = string.Empty;
        public abstract ushort UniqueID { get; }


        public GciHeader header = new();
        public TBinarySerializable fileData = new();


        public void Deserialize(EndianBinaryReader reader)
        {
            reader.Read(ref header);
            reader.Read(ref fileData);

            bool isValidUniqueID = header.UniqueID == UniqueID;
            if (!isValidUniqueID)
            {
                string msg = $"Expected {nameof(UniqueID)} of {UniqueID:x4}, is {header.UniqueID:x4}.";
                throw new InvalidGciException(msg);
            }
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(header);
            writer.Write(fileData);

            // Pad to GC block alginment + "header" bytes.
            int unalignedBytes = (int)writer.BaseStream.Position % MinimumBlockSize;
            bool isAlgined = unalignedBytes == MinimumHeaderSize;
            if (!isAlgined)
            {
                writer.WritePadding(0x00, unalignedBytes);
            }
        }
    }
}
