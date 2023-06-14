using Manifold.IO;
using System.Collections.Generic;

namespace GameCube.DiskImage
{
    public class DirectoryNode : FileSystemNode
    {
        public override FileSystemNodeType Type => FileSystemNodeType.Directory;

        public Pointer DirectoryParentOffset { get => filePtrOrDirectoryParentPtr; set => filePtrOrDirectoryParentPtr = value; }
        public int DirectoryLastChildIndex { get => fileLengthOrDirectoryLastChildIndex; set => fileLengthOrDirectoryLastChildIndex = value; }
        public int RootNodeCount { get => fileLengthOrDirectoryLastChildIndex; set => fileLengthOrDirectoryLastChildIndex = value; }

        public List<FileSystemNode> Children { get; set; } = new List<FileSystemNode>();

        public void AddChild(FileSystemNode node)
        {
            Children.Add(node);
            node.Parent = this;
        }
    }
}
