using Manifold.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameCube.DiskImage
{
    public class FileSystem :
        IBinaryAddressable,
        IBinarySerializable
    {
        private byte[] raw = Array.Empty<byte>();
        private DirectoryNode root = new();
        private FileSystemNode[] nodes = Array.Empty<FileSystemNode>();
        private AddressRange fileSystemNodesAddressRange;
        private AddressRange namesAddressRange;

        public AddressRange AddressRange { get; set; }
        public AddressRange FileSystemNodesAddressRange { get => fileSystemNodesAddressRange; set => fileSystemNodesAddressRange = value; }
        public AddressRange NamesAddressRange { get => namesAddressRange; set => namesAddressRange = value; }
        public byte[] Raw => raw;
        public DirectoryNode RootNode { get => root; set => root = value; }
        public FileSystemNode[] Nodes { get => nodes; }
        public IEnumerable<FileNode> Files { get => nodes.OfType<FileNode>(); }
        public IEnumerable<DirectoryNode> Directories { get => nodes.OfType<DirectoryNode>(); }


        public void Deserialize(EndianBinaryReader reader)
        {
            this.RecordStartAddress(reader);
            fileSystemNodesAddressRange.RecordStartAddress(reader);
            {
                // Read root and reset stream position.
                // Loops later in code begin at index 1 to skip root.
                reader.Read(ref root);
                reader.JumpToAddress(AddressRange.startAddress);
                // Read all entries in one go
                reader.Read(ref nodes, root.RootNodeCount);
            }
            fileSystemNodesAddressRange.RecordEndAddress(reader);
            namesAddressRange.RecordStartAddress(reader);
            {
                var directoryStack = new Stack<DirectoryNode>();
                directoryStack.Push(root);

                // Get names for all elements
                for (int i = 1; i < nodes.Length; i++)
                {
                    // Clear directories from stack when last child index is met
                    while (i >= directoryStack.Peek().DirectoryLastChildIndex)
                        directoryStack.Pop();

                    // Interally cast node to FileNode or DirectoryNode
                    nodes[i] = nodes[i].GetNodeAsProperType();
                    var currentNode = nodes[i];

                    // Add current node to directory stack
                    var currentDirectory = directoryStack.Peek();
                    currentDirectory.AddChild(currentNode);

                    // Get name
                    AsciiCString entryName = string.Empty;
                    var namePtr = namesAddressRange.startAddress + currentNode.NameOffset;
                    reader.JumpToAddress(namePtr);
                    reader.Read(ref entryName);
                    currentNode.Name = entryName;

                    // If a directory, add to directory stack
                    bool isDirectory = currentNode.Type == FileSystemNodeType.Directory;
                    if (isDirectory)
                    {
                        var directory = currentNode as DirectoryNode;
                        Assert.IsTrue(directory != null);
                        directoryStack.Push(directory);
                    }
                }

                // Remove root node from final array
                nodes = nodes[1..];

                //// DEBUG
                //for (int i = 0; i < nodes.Length; i++)
                //{
                //    string fullName = nodes[i].GetResolvedPath();
                //    Console.WriteLine($"{i} {fullName}");
                //}
            }
            // Now that strings are read, we have the final address
            namesAddressRange.RecordEndAddress(reader);
            this.RecordEndAddress(reader);

            // TEMP? Read as block to save out for Dolphin (play via main.dol, etc)
            // Read FST as single block
            reader.JumpToAddress(AddressRange.startAddress);
            reader.Read(ref raw, AddressRange.Size);
            // The address should end where it was, otherwise wrong amount of data read.
            Assert.IsTrue(reader.BaseStream.Position == AddressRange.endAddress);


        }

        public void Serialize(EndianBinaryWriter writer)
        {
            // TODO:
            // When implementing, write 2 functions
            // One to write FS descriptions
            // Another to write file names.
            // Why? Because ARC file write needs to rewrite FS descs, not names.

            throw new NotImplementedException();
        }


    }
}
