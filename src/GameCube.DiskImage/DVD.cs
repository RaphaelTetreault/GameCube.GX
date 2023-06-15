using Manifold.IO;
using System;

namespace GameCube.DiskImage
{
    /// <summary>
    /// Represents the root GameCube DVD file system.
    /// </summary>
    public class DVD :
        IBinaryFileType,
        IBinarySerializable
    {
        // Proper file structure
        private DiskHeader diskHeader;
        private DiskHeaderInformation diskHeaderInformation;
        private Apploader apploader;
        private FileSystem fileSystem;
        //private MainExecutable mainExecutable;
        private byte[] mainExecutableRaw;

        public const Endianness endianness = Endianness.BigEndian;


        public Endianness Endianness => endianness;
        public string FileExtension => ".iso";
        public string FileName { get; set; } = string.Empty;

        public Apploader Apploader { get => apploader; set => apploader = value; }
        public DiskHeader DiskHeader { get => diskHeader; set => diskHeader = value; }
        public DiskHeaderInformation DiskHeaderInformation { get => diskHeaderInformation; set => diskHeaderInformation = value; }
        public FileSystem FileSystem { get => fileSystem; set => fileSystem = value; }
        //public MainExecutable MainExecutable { get => mainExecutable; set => mainExecutable = value; }
        public byte[] MainExecutableRaw => mainExecutableRaw;

        public void Deserialize(EndianBinaryReader reader)
        {
            // Fixed addresses
            Assert.IsTrue(reader.BaseStream.Position == DiskHeader.Address);
            reader.JumpToAddress(DiskHeader.Address);
            reader.Read(ref diskHeader);
            // cont.
            Assert.IsTrue(reader.BaseStream.Position == DiskHeaderInformation.Address);
            reader.JumpToAddress(DiskHeaderInformation.Address);
            reader.Read(ref diskHeaderInformation);
            // cont.
            Assert.IsTrue(reader.BaseStream.Position == Apploader.Address);
            reader.JumpToAddress(Apploader.Address);
            reader.Read(ref apploader);
            
            // Non-fixed addresses
            // Read FS description
            reader.JumpToAddress(diskHeader.FileSystemPointer);
            reader.Read(ref fileSystem);
            // cont.
            reader.JumpToAddress(diskHeader.MainExecutablePtr);
            int size = diskHeader.FileSystemPointer - diskHeader.MainExecutablePtr;
            reader.Read(ref mainExecutableRaw, size);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}