using Manifold.IO;

namespace GameCube.DiskImage
{
    public class FileNode : FileSystemNode
    {
        public override FileSystemNodeType Type => FileSystemNodeType.File;

        public Pointer FilePointer { get => filePtrOrDirectoryParentPtr; set => filePtrOrDirectoryParentPtr = value; }
        public int FileLength { get => fileLengthOrDirectoryLastChildIndex; set => fileLengthOrDirectoryLastChildIndex = value; }
    }
}
