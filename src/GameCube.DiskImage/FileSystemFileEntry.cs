using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameCube.DiskImage
{
    /// <summary>
    ///     File entry in ARC file system
    /// </summary>
    public class FileSystemFileEntry
    {
        public string Name { get; set; } = string.Empty;
        public Pointer Pointer { get; set; }
        public int Size { get; set; }


        public FileSystemFile ReadFile(EndianBinaryReader reader)
        {
            if (Pointer.IsNull)
            {
                string msg = "Pointer is null";
                throw new NullReferenceException(msg);
            }

            reader.JumpToAddress(Pointer);
            byte[] data = reader.ReadBytes(Size);

            FileSystemFile file = new FileSystemFile()
            {
                Path = Name,
                Data = data,
            };
            return file;
        }

        public static FileSystemFileEntry WriteFile(EndianBinaryWriter writer, FileSystemFile file)
        {
            return file.WriteFile(writer);
        }
        public static FileSystemFileEntry[] WriteFiles(EndianBinaryWriter writer, FileSystemFile[] file)
        {
            var fileEntries = new FileSystemFileEntry[file.Length];
            for (int i = 0; i < file.Length; i++)
            {
                fileEntries[i] = file[i].WriteFile(writer);
            }
            return fileEntries;
        }
    }
}
