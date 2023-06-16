using Manifold.IO;
using System.Collections.Generic;
using System.Linq;

namespace GameCube.DiskImage
{
    // TODO: rewrite recursive functions to use type-based switch?
    //       functional paradigm to reuse code?

    public class DirectoryNode : FileSystemNode
    {
        public override FileSystemNodeType Type => FileSystemNodeType.Directory;

        public Offset DirectoryParentOffset { get => filePtrOrDirectoryParentOffset; set => filePtrOrDirectoryParentOffset = value; }
        public int DirectoryLastChildIndex { get => fileLengthOrDirectoryLastChildIndex; set => fileLengthOrDirectoryLastChildIndex = value; }
        public int RootNodeCount { get => fileLengthOrDirectoryLastChildIndex; set => fileLengthOrDirectoryLastChildIndex = value; }

        public List<FileSystemNode> Children { get; set; } = new List<FileSystemNode>();

        public void AddChild(FileSystemNode node)
        {
            Children.Add(node);
            node.Parent = this;
        }

        internal void AlphabetizeChildrenRecursively()
        {
            // Alphabetize children
            Children = Children.OrderBy(child => child.Name).ToList();

            // Recursively alphabetive children's children
            foreach (var child in Children)
                if (child is DirectoryNode directoryNode)
                    directoryNode.AlphabetizeChildrenRecursively();
        }

        internal void SerializeFileSystemRecursively(EndianBinaryWriter writer)
        {
            Serialize(writer);
            foreach (var child in Children)
                if (child is DirectoryNode directoryNode)
                    directoryNode.SerializeFileSystemRecursively(writer);
                else // file
                    child.Serialize(writer);
        }

        internal void SerializeNamesRecursively(EndianBinaryWriter writer)
        {
            writer.Write<AsciiCString>(Name);
            foreach (var child in Children)
                if (child is DirectoryNode directoryNode)
                    directoryNode.SerializeNamesRecursively(writer);
                else // file
                    writer.Write<AsciiCString>(child.Name);
        }

        internal void PrepareFileSystemDataRecursively(Pointer nameTableBasePointer, int currentIndex)
        {
            currentIndex++;
            PrepareFileSystemData(nameTableBasePointer, currentIndex);

            foreach (var child in Children)
                if (child is DirectoryNode directoryNode)
                    directoryNode.PrepareFileSystemDataRecursively(nameTableBasePointer, currentIndex);
                else
                    child.PrepareFileSystemData(nameTableBasePointer, currentIndex);
        }

        internal override void PrepareFileSystemData(Pointer nameTableBasePointer, int currenIndex)
        {
            base.PrepareFileSystemData(nameTableBasePointer, currenIndex);
            //
            DirectoryParentOffset = (Parent is null) ? 0 : Parent.NameOffset;
            DirectoryLastChildIndex = GetChildCountRecursively() + currenIndex;
        }

        internal void GetFiles(List<FileNode> fileNodesList)
        {
            foreach (var child in Children)
                if (child is DirectoryNode directoryNode)
                    directoryNode.GetFiles(fileNodesList);
                else
                    fileNodesList.Add((FileNode)child);
        }
        internal void GetDirectories(List<DirectoryNode> directoryNodeList)
        {
            directoryNodeList.Add(this);

            foreach (var child in Children)
            {
                if (child is DirectoryNode directoryNode)
                {
                    directoryNode.GetDirectories(directoryNodeList);
                }
            }
        }
        internal int GetChildCountRecursively()
        {
            int count = 0;

            // Add child count recursively
            foreach (var child in Children)
                if (child is DirectoryNode directoryNode)
                    count += directoryNode.GetNodeCount();
                else
                    count++;

            return count;
        }
        internal int GetNodeCount()
        {
            int count = GetChildCountRecursively() + 1; // 1 for self
            return count;
        }
        internal int GetNameTableLength()
        {
            int runningLength = Name.GetSerializedLength();

            // Add child name length recursively
            foreach (var child in Children)
                if (child is DirectoryNode directoryNode)
                    runningLength += directoryNode.GetNameTableLength();

            return runningLength;
        }

        internal void SetAsRootNode()
        {
            RootNodeCount = GetNodeCount();
            DirectoryParentOffset = 0;
        }

    }
}
