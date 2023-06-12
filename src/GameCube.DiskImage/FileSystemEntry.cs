using Manifold.IO;

namespace GameCube.DiskImage
{
    /// <summary>
    ///     Represents a file or folder in an ARC file. Can be either a file or directory.
    /// </summary>
    public class FileSystemEntry :
        IBinaryAddressable,
        IBinarySerializable
    {
        private FileSystemEntryType type; // 1 byte
        private Pointer entryNamePtr; // 3 bytes
        private Pointer filePtrOrDirectoryParentPtr;
        private int fileLengthOrDirectoryLastChildIndex;

        public AddressRange AddressRange { get; set; }
        public FileSystemEntryType Type { get => type; set => type = value; }
        public int DirectoryParentOffset { get => filePtrOrDirectoryParentPtr; set => filePtrOrDirectoryParentPtr =  value; }
        public int DirectoryLastChildIndex { get => fileLengthOrDirectoryLastChildIndex; set => fileLengthOrDirectoryLastChildIndex = value; }
        public Pointer FilePointer { get => filePtrOrDirectoryParentPtr; set => filePtrOrDirectoryParentPtr = value; }
        public Pointer NamePointer { get => entryNamePtr; set => entryNamePtr = value; }
        public int FileLength { get => fileLengthOrDirectoryLastChildIndex; set => fileLengthOrDirectoryLastChildIndex = value; }
        public int RootEntries { get => fileLengthOrDirectoryLastChildIndex; set => fileLengthOrDirectoryLastChildIndex = value; }


        public void Deserialize(EndianBinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                uint typeAndFileNameOffset = reader.ReadUInt32();
                reader.Read(ref filePtrOrDirectoryParentPtr);
                reader.Read(ref fileLengthOrDirectoryLastChildIndex);

                // Unpack type and file name offset
                type = (FileSystemEntryType)((typeAndFileNameOffset >> 24) & 0xFF);
                entryNamePtr = typeAndFileNameOffset & 0x00FFFFFF;
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            // TODO: ensure entry is either file or directory.
            filePtrOrDirectoryParentPtr = type == FileSystemEntryType.File ? FilePointer : DirectoryParentOffset;
            fileLengthOrDirectoryLastChildIndex = type == FileSystemEntryType.File ? FileLength : RootEntries;
            uint typePacked = (uint)type << 24;
            uint fileNameOffsetPacked = (uint)entryNamePtr & 0x00FFFFFF;
            uint typeAndFileNameOffset = typePacked + fileNameOffsetPacked;

            writer.Write(typeAndFileNameOffset);
            writer.Write(filePtrOrDirectoryParentPtr);
            writer.Write(fileLengthOrDirectoryLastChildIndex);

            // You have not implemented properly handling the above / getting addresses!
            throw new System.NotImplementedException();
        }
    }
}
