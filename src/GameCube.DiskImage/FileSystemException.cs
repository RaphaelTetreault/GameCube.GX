using System;

namespace GameCube.DiskImage
{
    internal class FileSystemException : Exception
    {
        public FileSystemException()
        {
        }

        public FileSystemException(string message)
            : base(message)
        {
        }

        public FileSystemException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
