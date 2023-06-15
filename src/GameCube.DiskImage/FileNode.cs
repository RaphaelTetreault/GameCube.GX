using Manifold.IO;
using System;

namespace GameCube.DiskImage
{
    public class FileNode : FileSystemNode
    {
        public override FileSystemNodeType Type => FileSystemNodeType.File;

        public Pointer FilePointer { get => filePtrOrDirectoryParentPtr; set => filePtrOrDirectoryParentPtr = value; }
        public int FileLength { get => fileLengthOrDirectoryLastChildIndex; set => fileLengthOrDirectoryLastChildIndex = value; }

        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}
