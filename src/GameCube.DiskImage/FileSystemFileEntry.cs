using Manifold.IO;

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
        public byte[] Data { get; set; }
    }
}
