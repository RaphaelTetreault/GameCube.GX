//using GameCube.GX.Texture;
//using Manifold.IO;
//using System.Reflection.PortableExecutable;

//namespace GameCube.GCI
//{
//    public abstract class GciSubheader :
//        IBinaryAddressable,
//        IBinarySerializable
//    {
//        public AddressRange AddressRange { get; set; }

//        public const byte Size = 0x60;
//        public virtual byte GameTitleLength => 32;
//        public virtual byte CommentLength => 64;

//        public abstract MenuBanner Banner { get;  }
//        public abstract MenuIcon[] Icons { get; }
//        public abstract string Comment { set; }


//        // Left to the implementor
//        protected abstract void Deserialize(EndianBinaryReader reader);
//        protected abstract void Serialize(EndianBinaryWriter writer);

//        void IBinarySerializable.Deserialize(EndianBinaryReader reader)
//        {
//            this.RecordStartAddress(reader);
//            Deserialize(reader);
//            this.RecordEndAddress(reader);
//        }

//        void IBinarySerializable.Serialize(EndianBinaryWriter writer)
//        {
//            this.RecordStartAddress(writer);
//            Serialize(writer);
//            this.RecordEndAddress(writer);
//        }
//    }
//}
