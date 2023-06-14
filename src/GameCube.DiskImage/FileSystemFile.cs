using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameCube.DiskImage
{
    public class FileSystemFile
    {
        public string Path { get; set; } = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();


        /// <summary>
        ///     Write file to <paramref name="writer"/> and return a descriptor of file.
        /// </summary>
        /// <param name="writer">The writer to write the file to.</param>
        /// <returns>
        ///     Returns a descriptor of file for file system.
        /// </returns>
        public FileSystemFileEntry WriteFile(EndianBinaryWriter writer)
        {
            AddressRange addressRange = new AddressRange();

            addressRange.RecordStartAddress(writer);
            writer.Write(Data);
            addressRange.RecordEndAddress(writer);

            var fileSystemEntry = new FileSystemFileEntry()
            {
                Name = Path,
                Pointer = addressRange.startAddress,
                Size = addressRange.Size,
            };
            return fileSystemEntry;
        }

        public static FileSystemFile ReadFile(EndianBinaryReader reader, FileSystemFileEntry fileEntry)
        {
            return fileEntry.ReadFile(reader);
        }
        public static FileSystemFile[] ReadFiles(EndianBinaryReader reader, FileSystemFileEntry[] fileEntries)
        {
            var files = new FileSystemFile[fileEntries.Length];
            for (int i = 0; i < fileEntries.Length; i++)
            {
                files[i] = fileEntries[i].ReadFile(reader);
            }
            return files;
        }
        public static FileSystemFile[] ReadFiles(EndianBinaryReader reader, FileSystem fileSystem)
        {
            return ReadFiles(reader, fileSystem.FilesEntries);
        }


        public static FileSystemFile[] GetFiles(string rootPath, params string[] paths)
        {
            // Alphabetize. Needed for directory stack.
            Array.Sort(paths);
            List<string> activeDirectories = new List<string>();
            rootPath.Replace('\\', '/');

            //
            var files = new List<FileSystemFile>();
            for (int i = 0; i < paths.Length; i++)
            {
                // Ensure path is part of root
                string fullPath = paths[i];
                string path = paths[i];
                path.Replace('\\', '/');
                bool isChildPath = path.StartsWith(rootPath);
                if (!isChildPath)
                {
                    string msg = $"Path \"{path}\" is not child of root path \"{rootPath}\".";
                    throw new ArgumentException(msg);
                }

                // Ensure file or folder exists...
                bool isFile = File.Exists(path);
                bool isDir = Directory.Exists(path);
                bool isValid = isFile ^ isDir;
                if (!isValid)
                {
                    string msg = $"Path \"{path}\" is neither a valid file or directory.";
                    throw new ArgumentException(msg);
                }

                //
                string relativePath = path.Remove(0, rootPath.Length);
                string[] pathSegments = relativePath.Split('/');

                // TODO: maybe only process files?
                // -1 so not to process file as directory
                int offset = isFile ? -1 : 0;
                int directoryCount = pathSegments.Length + offset;
                for (int dirIndex = 0; dirIndex < directoryCount; dirIndex++)
                {
                    string currentDirectory = pathSegments[dirIndex];
                    bool doesDirExistAtIndex = dirIndex < activeDirectories.Count;
                    if (doesDirExistAtIndex)
                    {
                        // check if same
                        string activeDirectory = activeDirectories[dirIndex];
                        bool isSameDirectory = currentDirectory == activeDirectory;
                        if (isSameDirectory)
                        {
                            // we're good, continue
                            continue;
                        }
                        else
                        {
                            // Clear stack until base dir is same
                            for (int k = activeDirectories.Count - 1; k > dirIndex; k--)
                                activeDirectories.RemoveAt(k);
                            // Add next dir
                            activeDirectories.Add(currentDirectory);
                        }
                    }
                    else
                    {
                        // add to stack
                        activeDirectories.Add(currentDirectory);
                    }
                }

                // Construct file...?????????????????
                files[i] = new FileSystemFile()
                {
                    Path = pathSegments[pathSegments.Length - 1],
                    Data = File.ReadAllBytes(fullPath),
                };
            }

            return files.ToArray();
        }
    }
}
