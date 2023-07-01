using GameCube.DiskImage;
using GameCube.GX.Texture;
using Manifold.IO;

namespace GameCube.GCI
{
    /// <summary>
    ///     
    /// </summary>
    /// <example>
    ///     Primary source of information:
    ///     https://docs.google.com/document/d/1c4a7d6xZ-rnK-E5p7d__6V1qhLxcdOHwAmv-FR8K50s/edit
    /// </example>
    public class GciHeader :
        IBinarySerializable
    {
        // Const / readonly
        public const int InternalFileNameLength = 32;
        public const int UnknownDataLength = 9;
        public const int GameTitleLength = 32;
        public const int CommentLength = 60;
        public readonly System.Text.Encoding AsciiEncoding = System.Text.Encoding.ASCII;
        public readonly System.Text.Encoding ShiftJisEncoding = System.Text.Encoding.GetEncoding(932); // shift-jis code page

        // Fields
        private GameID gameID;
        private byte const_0xFF;
        private byte regionCode;
        private string internalFileName = string.Empty;
        private uint timestamp;
        private byte[] unkData = new byte[9];
        private byte copyCount;
        private ushort memoryCardStartBlock;
        private int unk0;
        private int unk1;
        private ushort checksum;
        private ushort uniqueID;
        private string gameTitle = string.Empty;
        private string comment = string.Empty;
        private MenuBanner banner = new();
        private MenuIcon icon = new();

        // Accessors
        public GameID GameID { get => gameID; set => gameID = value; }
        public byte Const_0xFF { get => const_0xFF; set => const_0xFF = value; }
        public byte RegionCode { get => regionCode; set => regionCode = value; }
        public string InternalFileName { get => internalFileName; set => internalFileName = value; }
        public uint Timestamp { get => timestamp; set => timestamp = value; }
        public byte[] UnkData { get => unkData; set => unkData = value; }
        public byte CopyCount { get => copyCount; set => copyCount = value; }
        public ushort MemoryCardStartBlock { get => memoryCardStartBlock; set => memoryCardStartBlock = value; }
        public int Unk0 { get => unk0; set => unk0 = value; }
        public int Unk1 { get => unk1; set => unk1 = value; }
        public ushort Checksum { get => checksum; set => checksum = value; }
        public ushort UniqueID { get => uniqueID; set => uniqueID = value; }
        public string GameTitle { get => gameTitle; set => gameTitle = value; }
        public string Comment { get => comment; set => comment = value; }
        public MenuBanner Banner { get => banner; set => banner = value; }
        public MenuIcon Icon { get => icon; set => icon = value; }

        // Methods
        public void Deserialize(EndianBinaryReader reader)
        {
            reader.Read(ref gameID);
            reader.Read(ref const_0xFF);
            reader.Read(ref regionCode);
            reader.Read(ref internalFileName, AsciiEncoding, InternalFileNameLength);
            reader.Read(ref timestamp);
            reader.Read(ref unkData, UnknownDataLength);
            reader.Read(ref copyCount);
            reader.Read(ref memoryCardStartBlock);
            reader.Read(ref unk0);
            reader.Read(ref unk1);
            reader.Read(ref checksum);
            reader.Read(ref uniqueID);
            reader.Read(ref gameTitle, AsciiEncoding, GameTitleLength);
            reader.Read(ref comment, AsciiEncoding, CommentLength);
            reader.Read(ref banner);
            reader.Read(ref icon);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            System.Text.Encoding encoding = GetRegionEncoding(gameID);
            DateTime dateTime = DateTime.Now;
            SetDefaultComment(dateTime, false);
            SetTimestamp(dateTime);
            checksum = ComputeCRC();

            Assert.IsTrue(internalFileName.Length <= InternalFileNameLength);
            Assert.IsTrue(gameTitle.Length <= GameTitleLength);
            Assert.IsTrue(comment.Length <= CommentLength);
            Assert.IsTrue(unkData.Length == UnknownDataLength);

            writer.Write(gameID);
            writer.Write(0xFF);
            writer.Write(regionCode);
            writer.Write(internalFileName, encoding, false);
            writer.WritePadding(0x00, InternalFileNameLength - internalFileName.Length);
            writer.Write(timestamp);
            writer.Write(unkData);
            writer.Write(copyCount);
            writer.Write(memoryCardStartBlock);
            writer.Write(unk0);
            writer.Write(unk1);
            writer.Write(checksum);
            writer.Write(uniqueID);
            writer.Write(gameTitle, encoding, false);
            writer.WritePadding(0x00, GameTitleLength - internalFileName.Length);
            writer.Write(comment, encoding, false);
            writer.WritePadding(0x00, CommentLength - internalFileName.Length);
            writer.Write(banner);
            writer.Write(icon);
        }

        /// <summary>
        ///     Sets a default comment if the comment string is null or empty.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="overwriteComment"></param>
        private void SetDefaultComment(DateTime dateTime, bool overwriteComment = false)
        {
            bool hasComment = !string.IsNullOrEmpty(comment);
            if (hasComment || !overwriteComment)
                return;

            string time = dateTime.ToString("yyyy/MM/dd hh:mm.ss");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string name = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            name = name is null ? string.Empty : name;
            comment = $"Created by {name} at {time}.";
        }

        /// <summary>
        ///     Sets the header timestamp to the provided <paramref name="dateTime"/>.
        /// </summary>
        /// <param name="dateTime">The time to use.</param>
        private void SetTimestamp(DateTime dateTime)
        {
            DateTimeOffset dateTimeOffset = dateTime;
            uint unixTime = (uint)dateTimeOffset.ToUnixTimeSeconds();
            timestamp = unixTime;
        }

        /// <summary>
        ///     Compute file CRC
        /// </summary>
        /// <returns>
        ///     
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public ushort ComputeCRC()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Set filename and prevent file length overflow.
        /// </summary>
        /// <param name="internalFileName"></param>
        /// <returns>
        ///     True if <paramref name="internalFileName"/> fits in character limit, false otherwise.
        /// </returns>
        public bool SafeSetInternalFileName(string internalFileName)
        {
            // Trim file name if it is too long
            bool fileNameFits = internalFileName.Length <= InternalFileNameLength;
            if (!fileNameFits)
            {
                internalFileName = internalFileName.Substring(0, InternalFileNameLength);
            }

            // Set file name
            InternalFileName = internalFileName;

            // provide info
            return fileNameFits;
        }

        /// <summary>
        ///     Get the correct text endoing based on the region of <paramref name="gameID"/>.
        /// </summary>
        /// <param name="gameID">The Game ID for this file.</param>
        /// <returns>
        ///     
        /// </returns>
        /// <exception cref="NotImplementedException">
        ///     Thrown if <paramref name="gameID"/> region code is not implemented.
        /// </exception>
        private System.Text.Encoding GetRegionEncoding(GameID gameID)
        {
            switch (gameID.RegionCode)
            {
                case 'J':
                    return ShiftJisEncoding;

                case 'E':
                case 'P':
                    return AsciiEncoding;

                default:
                    throw new NotImplementedException($"Unhandled region code '{gameID.RegionCode}'.");
            }
        }

    }
}
