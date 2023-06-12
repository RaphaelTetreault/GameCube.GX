using Manifold.IO;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GameCube.DiskImage
{
    public class FileSystem :
        IBinaryAddressable,
        IBinarySerializable
    {
        private byte[] raw = Array.Empty<byte>();
        private FileSystemEntry root = new FileSystemEntry();
        private FileSystemEntry[] entries = Array.Empty<FileSystemEntry>();

        public AddressRange AddressRange { get; set; }
        public string[] Paths { get; private set; } = Array.Empty<string>();
        public string[] Directories { get; private set; } = Array.Empty<string>();
        public FileSystemFileEntry[] Files { get; private set; } = Array.Empty<FileSystemFileEntry>();
        public byte[] Raw => raw;


        public void Deserialize(EndianBinaryReader reader)
        {
            this.RecordStartAddress(reader);

            // Read root and reset stream position.
            // Loops later in code begin at index 1 to skip root.
            reader.Read(ref root);
            reader.JumpToAddress(AddressRange.startAddress);
            // Read all entries in one go
            reader.Read(ref entries, root.RootEntries);

            // Count files and folders
            int dirCount = 0;
            int fileCount = 0;
            for (int i = 1; i < entries.Length; i++)
                if (entries[i].Type == FileSystemEntryType.File)
                    fileCount++;
                else
                    dirCount++;

            // Create file and folder paths from source
            Files = new FileSystemFileEntry[fileCount];
            Directories = new string[dirCount];
            Paths = new string[entries.Length];
            Paths[0] = string.Empty; // Root path
            //
            int dirIndex = 0;
            int fileIndex = 0;
            for (int i = 1; i < entries.Length; i++)
            {
                // Get entry and get entries' name
                var entry = entries[i];
                AsciiCString entryName = "";
                reader.Read(ref entryName);

                bool isDirectory = entry.Type == FileSystemEntryType.Directory;
                if (isDirectory)
                {
                    // Append directory to relevant paths
                    for (int pathIndex = i; pathIndex < entry.DirectoryLastChildIndex; pathIndex++)
                        Paths[pathIndex] += $"{entryName}/";
                    // Register directory
                    Directories[dirIndex] = Paths[i];
                    dirIndex++;
                }
                else
                {
                    // Append file name to path
                    Paths[i] += entryName;
                    // Register file path
                    Files[fileIndex] = new FileSystemFileEntry()
                    {
                        Name = Paths[i],
                        Pointer = entry.FilePointer,
                        Size = entry.FileLength,
                    };
                    fileIndex++;
                }
            }

            // Now that strings are read, we have the final address
            this.RecordEndAddress(reader);

            // TEMP? Read as block to save out for Dolphin (play via main.dol, etc)
            // Read FST as single block
            reader.JumpToAddress(AddressRange.startAddress);
            reader.Read(ref raw, AddressRange.Size);
            // The address should end where it was, otherwise wrong amount of data read.
            Assert.IsTrue(reader.BaseStream.Position == AddressRange.endAddress);

            for (int i = 0; i < Files.Length; i++)
            {
                var file = Files[i];

                var ptr = file.Pointer;
                var len = file.Size;
                reader.JumpToAddress(ptr);
                var data = reader.ReadBytes(len);

                file.Data = data;
            }
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        private void ReadFilesMultithreaded(EndianBinaryReader reader, FileSystemFileEntry[] fileEntries)
        {
            Task[] readTasks = new Task[fileEntries.Length];

            for (int i = 0; i < fileEntries.Length; i++)
            {
                var fileEntry = fileEntries[i];

                var readFile = () =>
                {
                    var readerX = new EndianBinaryReader(reader.BaseStream, reader.Endianness);

                    var ptr = fileEntry.Pointer;
                    var len = fileEntry.Size;
                    // ERROR HERE
                    readerX.JumpToAddress(ptr);
                    var data = readerX.ReadBytes(len);

                    fileEntry.Data = data;
                };
                var readTask = Task.Factory.StartNew(readFile);
                readTasks[i] = readTask;
            }

            Task.WaitAll(readTasks);
        }

    }
}
