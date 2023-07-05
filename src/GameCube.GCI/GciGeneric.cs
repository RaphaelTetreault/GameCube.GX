using Manifold.IO;
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
    public abstract class Gci<TBinarySerializable> :
        IBinaryFileType,
        IBinarySerializable
        where TBinarySerializable : IBinarySerializable, IBinaryFileType, new()
    {
        public const Endianness endianness = Endianness.BigEndian;
        public const string Extension = ".gci";
        public const int MinimumBlockSize = 0x2000; // 8192
        public const int MinimumHeaderSize = 0x40;  // 64

        public Endianness Endianness => endianness;
        public string FileExtension => Extension;
        public string FileName { get; set; } = string.Empty;
        public abstract ushort[] UniqueIDs { get; }


        public GciHeader header = new();
        public TBinarySerializable fileData = new();


        public void Deserialize(EndianBinaryReader reader)
        {
            // Read header
            header.Deserialize(reader);

            // Validate header with regards to expected data
            bool isValidUniqueID = false;
            foreach (var uniqueID in UniqueIDs)
            {
                if (uniqueID == header.UniqueID)
                {
                    isValidUniqueID = true;
                    break;
                }
            }
            if (!isValidUniqueID)
            {
                string msg = $"{nameof(UniqueIDs)} did not match header value {header.UniqueID:x4}.";
                throw new InvalidGciException(msg);
            }

            // Read data
            fileData.Deserialize(reader);
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
