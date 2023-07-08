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
    ///     TODO: https://github.com/dolphin-emu/dolphin/blob/master/Source/Core/Core/HW/GCMemcard/GCMemcard.h#L240
    /// </example>
    public class GciHeader :
        IBinarySerializable
    {
        // Const / readonly
        public const int HeaderSize = 0x40; // FS entry?
        public const int BlockSize = 0x2000;
        public const int InternalFileNameLength = 32;
        public const int GameTitleLength = 32;
        public const int CommentLength = 60;
        public static readonly System.Text.Encoding Windows1252Encoding = System.Text.Encoding.GetEncoding(1252); // Windows-1252
        public static readonly System.Text.Encoding ShiftJisEncoding = System.Text.Encoding.GetEncoding(932); // shift-jis code page

        // Fields
        private GameID gameID;
        private byte const_0xFF;
        private BannerAndIconFlags bannerAndIconFlags;
        private string fileName = string.Empty;
        private uint modificationTime;
        private Offset imageDataOffset;
        private ImageFormat imageFormat;
        private AnimationSpeed animationSpeed;
        private PermissionFlags permissionFlags;
        private byte copyCount;
        private ushort firstBlockIndex; // on memory card AFAICT
        private ushort blockCount;
        private ushort const_0xFFFF; // FFFF
        private Offset commentOffset;
        // We're at 0x40
        private ushort checksum;
        private ushort uniqueID;
        private string gameTitle = string.Empty;
        private string comment = string.Empty;
        private MenuBanner banner = new();
        private MenuIcon[] icons = Array.Empty<MenuIcon>();

        // Accessors
        public GameID GameID { get => gameID; set => gameID = value; }
        public BannerAndIconFlags BannerAndIconFlags { get => bannerAndIconFlags; set => bannerAndIconFlags = value; }
        public string FileName { get => fileName; set => fileName = value; }
        /// <summary>
        ///     Time of file's last modification in seconds since 12am, January 1st, 2000
        /// </summary>
        public uint ModificationTime { get => modificationTime; set => modificationTime = value; }
        //public Offset ImageDataOffset { get => imageDataOffset; set => imageDataOffset = value; }
        public ImageFormat ImageFormat { get => imageFormat; set => imageFormat = value; }
        public AnimationSpeed AnimationSpeed { get => animationSpeed; set => animationSpeed = value; }
        public PermissionFlags PermissionFlags { get => permissionFlags; set => permissionFlags = value; }
        public byte CopyCount { get => copyCount; set => copyCount = value; }
        public ushort FirstBlockIndex { get => firstBlockIndex; set => firstBlockIndex = value; }
        public ushort BlockCount { get => blockCount; set => blockCount = value; }
        //public Offset CommentOffset { get => commentOffset; set => commentOffset = value; }
        public ushort Checksum { get => checksum; set => checksum = value; }
        public ushort UniqueID { get => uniqueID; set => uniqueID = value; }
        public string GameTitle { get => gameTitle; set => gameTitle = value; }
        public string Comment { get => comment; set => comment = value; }
        public MenuBanner Banner { get => banner; set => banner = value; }
        public MenuIcon[] Icons { get => icons; set => icons = value; }

        public Pointer ImageDataPtr { get; private set; }
        public Pointer CommentPtr { get; private set; }
        public int Size => blockCount * BlockSize + HeaderSize;

        // Methods
        public void Deserialize(EndianBinaryReader reader)
        {
            reader.Read(ref gameID);
            reader.Read(ref const_0xFF);
            reader.Read(ref bannerAndIconFlags);
            reader.Read(ref fileName, Windows1252Encoding, InternalFileNameLength);
            reader.Read(ref modificationTime);
            reader.Read(ref imageDataOffset);
            reader.Read(ref imageFormat);
            reader.Read(ref animationSpeed);
            reader.Read(ref permissionFlags);
            reader.Read(ref copyCount);
            reader.Read(ref firstBlockIndex);
            reader.Read(ref blockCount);
            reader.Read(ref const_0xFFFF);
            reader.Read(ref commentOffset);
            // Prepare pointers
            Pointer currentAddress = reader.GetPositionAsPointer();
            CommentPtr = currentAddress + commentOffset;
            ImageDataPtr = currentAddress + imageDataOffset;

            // GFZ HEADER?
            reader.Read(ref checksum);
            reader.Read(ref uniqueID);
            Assert.IsTrue(reader.GetPositionAsPointer() == CommentPtr);
            reader.Read(ref gameTitle, Windows1252Encoding, GameTitleLength);
            reader.Read(ref comment, Windows1252Encoding, CommentLength);
            Assert.IsTrue(reader.GetPositionAsPointer() == ImageDataPtr);
            reader.Read(ref banner);
            reader.Read(ref icons, 1);

            Assert.IsTrue(const_0xFF == 0xFF);
            Assert.IsTrue(const_0xFFFF == 0xFFFF);
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            System.Text.Encoding encoding = GetRegionEncoding(gameID);
            DateTime now = DateTime.Now;
            SetDefaultComment(now, false);
            SetTimestamp(now);
            checksum = ComputeCRC();

            Assert.IsTrue(fileName.Length <= InternalFileNameLength);
            Assert.IsTrue(gameTitle.Length <= GameTitleLength);
            Assert.IsTrue(comment.Length <= CommentLength);

            // TODO: ptrs

            writer.Write(gameID);
            writer.Write(0xFF);
            writer.Write(bannerAndIconFlags);
            writer.Write(fileName, encoding, false);
            writer.WritePadding(0x00, InternalFileNameLength - fileName.Length);
            writer.Write(modificationTime);
            writer.Write(imageDataOffset);
            writer.Write(imageFormat);
            writer.Write(animationSpeed);
            writer.Write(permissionFlags);
            writer.Write(copyCount);
            writer.Write(firstBlockIndex);
            writer.Write(blockCount);
            writer.Write(0xFFFF);
            writer.Write(commentOffset);

            // GFZ HEADER?
            writer.Write(checksum);
            writer.Write(uniqueID);
            writer.Write(gameTitle, encoding, false);
            writer.WritePadding(0x00, GameTitleLength - fileName.Length);
            writer.Write(comment, encoding, false);
            writer.WritePadding(0x00, CommentLength - comment.Length);
            writer.Write(banner);
            writer.Write(icons);
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
            DateTime epoch = new(2000, 01, 01);
            TimeSpan timeSpan = dateTime - epoch;
            uint secondsSince2000 = (uint)timeSpan.Seconds;
            modificationTime = secondsSince2000;
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
            FileName = internalFileName;

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
                    return Windows1252Encoding;

                default:
                    throw new NotImplementedException($"Unhandled region code '{gameID.RegionCode}'.");
            }
        }

        public byte[] GetAnimationFrameCount()
        {
            throw new NotImplementedException();
        }
    }
}
