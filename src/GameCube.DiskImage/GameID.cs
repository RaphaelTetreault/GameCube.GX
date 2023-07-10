using Manifold.IO;
using System;
using System.Text;

namespace GameCube.DiskImage
{
    public struct GameID :
        IBinarySerializable
    {
        // CONSTANTS
        private const int ByteLength = 6;
        private readonly Encoding encoding = Encoding.ASCII;


        // FIELDS
        private byte[] characters;


        // PROPERTIES
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


        // CONSTRUCTORS
        public GameID()
        {
            characters = new byte[ByteLength];
        }


        // INDEXERS
        public char this[int i]
        {
            get => Convert.ToChar(characters[i]);
            set => characters[i] = Convert.ToByte(value);
        }


        // METHODS
        public void Deserialize(EndianBinaryReader reader)
        {
            reader.Read(ref characters, ByteLength);
            ThrowIfInvalidRegion();
        }
        public void Serialize(EndianBinaryWriter writer)
        {
            ThrowIfInvalidRegion();
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
        public static char GetRegionChar(Region region)
        {
            switch (region)
            {
                // TODO: Datel 'X', region free?
                case Region.NorthAmerica: return 'E';
                case Region.Europe: return 'P';
                case Region.Japan: return 'J';
                default: return '\0';
            }
        }
        private bool IsValidRegionChar()
        {
            switch (RegionCode)
            {
                case 'E':
                case 'J':
                case 'P':
                    return true;

                default:
                    return false;
            }
        }
        public void ThrowIfInvalidRegion()
        {
            bool isInvalidRegion = IsValidRegionChar();
            if (!isInvalidRegion)
            {
                string msg = $"Invalid region code '{RegionCode}'.";
                throw new NotImplementedException(msg);
            }
        }
    }
}
