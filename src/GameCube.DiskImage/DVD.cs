using Manifold.IO;

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
        //
        private FileSystem fileSystem;

        // Pointers are temp for now
        public const Endianness endianness = Endianness.BigEndian;

        public Endianness Endianness => endianness;
        public string FileExtension => ".iso";
        public string FileName { get; set; } = string.Empty;



        public void Deserialize(EndianBinaryReader reader)
        {
            // Address jumps are temporary
            reader.JumpToAddress(0x0);
            reader.Read(ref diskHeader);
            //reader.JumpToAddress(0x440);
            //reader.Read(ref diskHeaderInformation);
            //reader.JumpToAddress(0x2440);
            //reader.Read(ref apploader);

            reader.JumpToAddress(diskHeader.FileSystemPointer);
            reader.Read(ref fileSystem);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}