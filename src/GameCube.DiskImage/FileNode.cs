using Manifold.IO;
using System;

namespace GameCube.DiskImage
{
    public class FileNode : FileSystemNode
    {
        public override FileSystemNodeType Type => FileSystemNodeType.File;

        public Pointer FilePointer { get => filePtrOrDirectoryParentOffset; set => filePtrOrDirectoryParentOffset = value; }
        public int FileLength { get => fileLengthOrDirectoryLastChildIndex; set => fileLengthOrDirectoryLastChildIndex = value; }
        public byte[] Data { get; set; } = Array.Empty<byte>();

        public void ReadData(EndianBinaryReader reader)
        {
            //var ptr = reader.GetPositionAsPointer();
            reader.JumpToAddress(FilePointer);
            Data = reader.ReadBytes(FileLength);
            //reader.JumpToAddress(ptr);
        }

        public void WriteData(EndianBinaryWriter writer)
        {
            FilePointer = writer.GetPositionAsPointer();
            FileLength = Data.Length;
            writer.Write(Data);
        }

    }
}
