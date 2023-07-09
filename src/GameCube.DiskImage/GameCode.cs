using Manifold.IO;
using System;
using System.Text;

namespace GameCube.DiskImage
{
    public struct GameID :
        IBinarySerializable
    {
        //
        private const int ByteLength = 6;
        private readonly Encoding encoding = Encoding.ASCII;

        //
        private byte[] characters;

        //
        public char ConsoleCode
        {
            get => this[0];
            set => this[0] = value;
        }
        public ushort GameCode
        {
            get => (ushort)(characters[1] << 8 | characters[2] << 0);
            set
            {
                characters[1] = (byte)((value >> 8) & 0xFF);
                characters[2] = (byte)((value >> 0) & 0xFF);
            }
        }
        public char RegionCode
        {
            get => this[3];
            set => this[3] = value;
        }
        public ushort DeveloperCode
        {
            get => (ushort)(characters[4] << 8 | characters[5] << 0);
            set
            {
                characters[4] = (byte)((value >> 8) & 0xFF);
                characters[5] = (byte)((value >> 0) & 0xFF);
            }
        }
        public byte[] CharactersRaw => characters;


        //
        public GameID()
        {
            characters = new byte[ByteLength];
        }


        //
        public char this[int i]
        {
            get => Convert.ToChar(characters[i]);
            set => characters[i] = Convert.ToByte(value);
        }

        //
        public void Deserialize(EndianBinaryReader reader)
        {
            reader.Read(ref characters, ByteLength);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(characters);
        }
        public string GetAsString()
        {
            var str = encoding.GetString(characters);
            return str;
        }

        public override string ToString()
        {
            return GetAsString();
        }

    }
}
