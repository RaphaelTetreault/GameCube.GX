using Manifold.IO;
using System.Runtime.InteropServices;

namespace GameCube.DiskImage
{
    [StructLayout(LayoutKind.Explicit)]
    public struct GameID :
        IBinarySerializable
    {
        [FieldOffset(0)] public byte ConsoleCode;
        [FieldOffset(1)] public ushort GameCode;
        [FieldOffset(3)] public byte CountryCode;
        // Mirror / helpers
        [FieldOffset(3)] public uint Code;

        public void Deserialize(EndianBinaryReader reader)
        {
            reader.Read(ref Code);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Code);
        }
    }
}
