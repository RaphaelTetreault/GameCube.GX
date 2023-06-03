using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.DiskImage
{
    public class FileSystem :
        IBinaryAddressable,
        IBinarySerializable
    {
        private FileSystemEntry root;
        private FileSystemEntry[] entries;

        public List<FileEntry> FileEntries { get; private set; } = new List<FileEntry>();
        public AddressRange AddressRange { get; set; }



        public void Deserialize(EndianBinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.Read(ref root);
                reader.Read(ref entries, root.RootEntries - 1);
            }
            // names start
            AsciiCString[] stringTableRaw = null;
            reader.Read(ref stringTableRaw, entries.Length);


            // Set default for paths
            string[] paths = new string[root.RootEntries];
            for (int i = 0; i < paths.Length; i++)
                paths[i] = "";


            for (int i = 1; i < paths.Length; i++)
            {
                var entry = entries[i - 1];
                var name = stringTableRaw[i - 1];

                // Append directory if necessary
                if (entry.Type == FileSystemEntryType.Directory)
                {
                    for (int j = i; j < entry.DirectoryStackCount; j++)
                    {
                        int index = j;
                        paths[index] += $"{name}/";
                    }
                }
                else
                {
                    paths[i] += name;
                }

                Console.WriteLine(paths[i]);
            }

            //
            FileEntries = new List<FileEntry>();
            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                if (entry.Type == FileSystemEntryType.Directory)
                    continue;

                var ptr = entry.FileOffset;
                var len = entry.FileLength;
                reader.JumpToAddress(ptr);
                var data = reader.ReadBytes(len);

                var x = new FileEntry()
                {
                    Name = paths[i],
                    data = data,
                };
            }
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
