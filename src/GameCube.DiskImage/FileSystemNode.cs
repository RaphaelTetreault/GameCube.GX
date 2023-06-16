using Manifold.IO;

namespace GameCube.DiskImage
{
    /// <summary>
    ///     Represents a file or directory in the file system.
    /// </summary>
    public class FileSystemNode :
        IBinaryAddressable,
        IBinarySerializable
    {
        // CONST    
        public const int StructureSize = 12; // bytes

        // MEMBERS
        internal FileSystemNodeType type; // 1 byte
        internal Offset nameOffset; // 3 bytes
        internal int filePtrOrDirectoryParentOffset;
        internal int fileLengthOrDirectoryLastChildIndex;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public AsciiCString Name { get; set; } = string.Empty;
        public Offset NameOffset { get => nameOffset; set => nameOffset = value; }
        public DirectoryNode? Parent { get; set; } = null;
        public virtual FileSystemNodeType Type { get => type; }


        // METHODS
        public void Deserialize(EndianBinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                //
                uint typeAndFileNameOffset = reader.ReadUInt32();
                reader.Read(ref filePtrOrDirectoryParentOffset);
                reader.Read(ref fileLengthOrDirectoryLastChildIndex);

                // Unpack type and file name offset
                type = (FileSystemNodeType)((typeAndFileNameOffset >> 24) & 0xFF);
                nameOffset = typeAndFileNameOffset & 0x00FFFFFF;
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                //
                uint typePacked = (uint)Type << 24;
                uint fileNameOffsetPacked = (uint)nameOffset & 0x00FFFFFF;
                uint typeAndFileNameOffset = typePacked | fileNameOffsetPacked;

                //
                writer.Write(typeAndFileNameOffset);
                writer.Write(filePtrOrDirectoryParentOffset);
                writer.Write(fileLengthOrDirectoryLastChildIndex);
            }
            this.RecordEndAddress(writer);
        }


        public string GetResolvedPath()
        {
            if (Parent != null)
            {
                string parentName = Parent.GetResolvedPath();
                string name = $"{parentName}/{Name}";
                return name;
            }

            return Name;
        }

        public FileSystemNode GetNodeAsProperType()
        {
            FileSystemNode node;
            switch (type)
            {
                case FileSystemNodeType.File: node = new FileNode(); break;
                case FileSystemNodeType.Directory: node = new DirectoryNode(); break;
                default: throw new System.Exception();
            }

            // Copy values
            node.nameOffset = nameOffset;
            node.filePtrOrDirectoryParentOffset = filePtrOrDirectoryParentOffset;
            node.fileLengthOrDirectoryLastChildIndex = fileLengthOrDirectoryLastChildIndex;

            return node;
        }

        internal virtual void PrepareFileSystemData(Pointer nameTableBasePointer, int currentIndex)
        {
            type = Type;
            nameOffset = Name.GetPointer() - nameTableBasePointer;
        }

        public DirectoryNode GetRoot()
        {
            if (Parent is not null)
                return Parent.GetRoot();
            else
                return (DirectoryNode)this;
        }

        public override string ToString()
        {
            string name = Name == null ? "null" : Name;
            return $"{GetType().Name}({name})";
        }
    }
}
