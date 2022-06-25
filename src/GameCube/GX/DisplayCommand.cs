using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GX
{
    [Serializable]
    public class DisplayCommand :
        IBinarySerializable
    {
        // FIELDS
        private byte command;
        private Primitive primitive;
        private VertexFormat vertexFormat;


        // PROPERTIES
        public Primitive Primitive
        {
            get => primitive;
            set
            {
                primitive = value;
                command &= 0b00000111;
                command |= (byte)primitive;
            }
        }
        public VertexFormat VertexFormat
        {
            get => vertexFormat;
            set
            {
                vertexFormat = value;
                command &= 0b11111000;
                command |= (byte)vertexFormat;
            }
        }
        public byte VertexFormatIndex => (byte)VertexFormat;


        // METHODS
        public void Deserialize(EndianBinaryReader reader)
        {
            reader.Read(ref command);
            primitive = (Primitive)(command & 0b11111000); // 5 highest bits
            vertexFormat = (VertexFormat)(command & 0b00000111); // 3 lowest bits
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(command);
        }

    }
}