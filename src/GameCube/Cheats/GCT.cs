using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameCube.Cheats
{
    public sealed class GCT :
        IBinarySerializable,
        IBinaryFileType
    {
        public const ulong magic = 0x00D0C0DE_00D0C0DE;
        public const ulong fileTerminator = 0xF0000000_00000000;

        public string gameCode;
        public GctCode[] codes;


        public Endianness Endianness => Endianness.BigEndian;
        public string FileExtension => throw new NotImplementedException();
        public string FileName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public void Deserialize(EndianBinaryReader reader)
        {
            var fileSize = (int)(reader.BaseStream.Length / 4);
            var isValidFile = (fileSize % 8) == 0;

            if (!isValidFile)
                throw new FileLoadException($"Not a valid GCT file (size not multiple of 8)");

            var header = reader.ReadUInt64();
            if (header != magic)
                throw new FileLoadException($"Not a valid GCT file (header is not {magic:x16})");

            var codes = new List<GctCode>();
            while (true)
            {
                // If end of file, break.
                // Do this first in case empty GCT
                var nextLine = reader.PeekUInt64();
                if (nextLine == fileTerminator)
                    break;

                // Instance code, deserialize, add to list of codes
                var code = new GctCode();
                reader.Read(ref code);
                codes.Add(code);
            }
            this.codes = codes.ToArray();
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(codes);
        }

    }
}
