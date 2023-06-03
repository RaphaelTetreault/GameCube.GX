using Manifold.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.DiskImage
{
    public class DiskHeader :
        IBinarySerializable
    {
        // NOTE: I used Offset to be consisten with YAGCD but unsure
        //  if their use of "offset" is the same as me (offset != pointer).

        private GameID id;
        private ushort makerCode;
        private byte diskID;
        private byte version;
        private byte audioStreaming;
        private byte streamBufferSize;
        private byte[] unused_0x0a; // size: 0x12
        private uint dvdMagicWord; // 0xc2339f3d
        private AsciiCString gameName; // Buffer size: 0x03e0
        private Offset debugMonitorOffset;
        private Pointer debugMonitorLoadAddress;
        private byte[] unused_0x0408; // size: 0x18
        private Offset mainExecutableOffset;
        private Pointer fileSystemPointer;
        private uint fileSystemSize;
        private uint fileSystemMaximumSize;
        private uint userPosition;
        private uint userLength;
        private uint unk_0x0438;
        private uint zero_0x043C;

        private const int unused_0x0a_size = 0x12;
        private const int unused_0x0408_size = 0x18;
        private const int endOfNameAddress = 0x0400;

        public AsciiCString GameName => gameName;
        public Pointer FileSystemPointer => fileSystemPointer;


        public void Deserialize(EndianBinaryReader reader)
        {
            reader.Read(ref id);
            reader.Read(ref makerCode);
            reader.Read(ref diskID);
            reader.Read(ref version);
            reader.Read(ref audioStreaming);
            reader.Read(ref streamBufferSize);
            reader.Read(ref unused_0x0a, unused_0x0a_size);
            reader.Read(ref dvdMagicWord);
            reader.Read(ref gameName);
            reader.JumpToAddress(endOfNameAddress);
            reader.Read(ref debugMonitorOffset);
            reader.Read(ref debugMonitorLoadAddress);
            reader.Read(ref unused_0x0408, unused_0x0408_size);
            reader.Read(ref mainExecutableOffset);
            reader.Read(ref fileSystemPointer);
            reader.Read(ref fileSystemSize);
            reader.Read(ref fileSystemMaximumSize);
            reader.Read(ref userPosition);
            reader.Read(ref userLength);
            reader.Read(ref unk_0x0438);
            reader.Read(ref zero_0x043C);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
