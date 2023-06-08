using Manifold.IO;
using System;

namespace GameCube.DiskImage
{
    public class FileSystemEntry :
        IBinaryAddressable,
        IBinarySerializable
    {
        private FileSystemEntryType type; // 1 byte
        private Offset fileNameOffset; // 3 bytes
        private Offset fileOrDirectoryOffset; // if file OR directory
        private int fileLengthOrDirectoryEntries; // if file OR directory
        // References
        //private CString fileName = null;

        public AddressRange AddressRange { get; set; }
        public FileSystemEntryType Type { get => type; set => type = value; }
        public int DirectoryOffset { get; set; }
        public int DirectoryStackCount { get; set; }
        public int FileOffset { get; set; }
        public int FileNameOffset { get; set; }
        public int FileLength { get; set; }
        public int RootEntries { get; set; }


        public void Deserialize(EndianBinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.Read(ref type);

                // 3 bytes
                reader.BaseStream.Position--;
                reader.Read(ref fileNameOffset);
                fileNameOffset &= 0x00FFFFFF; // mask upper 8 bits
                // TODO: assert?

                reader.Read(ref fileOrDirectoryOffset);
                reader.Read(ref fileLengthOrDirectoryEntries);
            }
            this.RecordEndAddress(reader);
            {
                // Set properties and do a sanity check
                switch (type)
                {
                    case FileSystemEntryType.File:
                        FileNameOffset = fileNameOffset;
                        FileOffset = fileOrDirectoryOffset;
                        FileLength = fileLengthOrDirectoryEntries;
                        break;

                    case FileSystemEntryType.Directory:
                        DirectoryOffset = fileOrDirectoryOffset;
                        DirectoryStackCount = fileLengthOrDirectoryEntries;
                        RootEntries = fileLengthOrDirectoryEntries;
                        break;

                    default:
                        string msg = $"{nameof(FileSystemEntryType)} type is not defined. Value is: '{type}'.";
                        throw new ArgumentException(msg);
                }
            }
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            // TODO: ensure entry is either file or directory.
            fileOrDirectoryOffset = type == FileSystemEntryType.File ? FileOffset : DirectoryOffset;
            fileLengthOrDirectoryEntries = type == FileSystemEntryType.File ? FileLength : RootEntries;

            // Write 4 bytes for file name offset
            writer.Write(fileNameOffset);
            // Back up 4 bytes, overwrite first byte
            writer.BaseStream.Position -= 4;
            writer.Write(type);
            // forward 3 bytes back to correct stream position
            writer.Write(fileOrDirectoryOffset);
            writer.Write(fileLengthOrDirectoryEntries);
        }
    }
}
