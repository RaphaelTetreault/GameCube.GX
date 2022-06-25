using Manifold;
using Manifold.IO;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GameCube.GX
{
    //[StructLayout(LayoutKind.Explicit)]
    public struct GXColor :
        IBinarySerializable
    {
        private byte r;
        private byte g;
        private byte b;
        private byte a;
        private ComponentType componentType;

        public byte R { get => r; set => r = value; }
        public byte G { get => g; set => g = value; }
        public byte B { get => b; set => b = value; }
        public byte A { get => a; set => a = value; }
        public ComponentType ComponentType { get => componentType; set => componentType = value; }


        public GXColor(ComponentType componentType)
        {
            r = g = b = a = 0;
            this.componentType = componentType;
        }

        public GXColor(int raw)
        {
            r = (byte)((raw >> 24) & 0b11111111);
            g = (byte)((raw >> 16) & 0b11111111);
            b = (byte)((raw >> 08) & 0b11111111);
            a = (byte)((raw >> 00) & 0b11111111);
            componentType = ComponentType.GX_RGBA8;
        }
        public GXColor(uint raw)
        {
            r = (byte)((raw >> 24) & 0b11111111);
            g = (byte)((raw >> 16) & 0b11111111);
            b = (byte)((raw >> 08) & 0b11111111);
            a = (byte)((raw >> 00) & 0b11111111);
            componentType = ComponentType.GX_RGBA8;
        }

        private void GetRGBA8(uint raw)
        {
            r = (byte)((raw >> 24) & 0b11111111);
            g = (byte)((raw >> 16) & 0b11111111);
            b = (byte)((raw >> 08) & 0b11111111);
            a = (byte)((raw >> 00) & 0b11111111);
        }

        public GXColor(byte r, byte g, byte b, byte a = 255)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            componentType = ComponentType.GX_RGBA8;
        }

        public void Deserialize(EndianBinaryReader reader)
        {
            switch (componentType)
            {
                case ComponentType.GX_RGB565: ReadRGBA565(reader); break;
                case ComponentType.GX_RGB8: ReadRGB8(reader); break;
                case ComponentType.GX_RGBA4: ReadRGBA4(reader); break;
                case ComponentType.GX_RGBA6: ReadRGBA6(reader); break;
                case ComponentType.GX_RGBA8: ReadRGBA8(reader); break;
                case ComponentType.GX_RGBX8: ReadRGBX8(reader); break;

                default:
                    throw new ArgumentException("Invalid GXColor type");
            }
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            switch (componentType)
            {
                case ComponentType.GX_RGB565: WriteRGBA565(writer); break;
                case ComponentType.GX_RGB8: WriteRGB8(writer); break;
                case ComponentType.GX_RGBA4: WriteRGBA4(writer); break;
                case ComponentType.GX_RGBA6: WriteRGBA6(writer); break;
                case ComponentType.GX_RGBA8: WriteRGBA8(writer); break;
                case ComponentType.GX_RGBX8: WriteRGBX8(writer); break;

                default:
                    throw new ArgumentException("Invalid GXColor type");
            }
        }


        private void ReadRGBA565(EndianBinaryReader reader)
        {
            // Left >> & = mask bits you want, right << to make bits max at 255
            var rgb565 = reader.ReadUInt16();
            r = (byte)(((rgb565 >> 11) & (0b_0001_1111)) * (1 << 3));
            g = (byte)(((rgb565 >> 05) & (0b_0011_1111)) * (1 << 2));
            b = (byte)(((rgb565 >> 00) & (0b_0001_1111)) * (1 << 3));
        }
        private void ReadRGB8(EndianBinaryReader reader)
        {
            var rgba8 = Read3BytesCorrectEndianness(reader);
            r = (byte)((rgba8 >> 16) & (0b_1111_1111));
            r = (byte)((rgba8 >> 08) & (0b_1111_1111));
            r = (byte)((rgba8 >> 00) & (0b_1111_1111));
        }
        private void ReadRGBA4(EndianBinaryReader reader)
        {
            var rgba4 = reader.ReadUInt16();
            r = (byte)(((rgba4 >> 12) & (0b_0000_1111)) * (1 << 4));
            g = (byte)(((rgba4 >> 08) & (0b_0000_1111)) * (1 << 4));
            b = (byte)(((rgba4 >> 04) & (0b_0000_1111)) * (1 << 4));
            a = (byte)(((rgba4 >> 00) & (0b_0000_1111)) * (1 << 4));
        }
        private void ReadRGBA6(EndianBinaryReader reader)
        {
            var rgba6 = Read3BytesCorrectEndianness(reader);
            r = (byte)(((rgba6 >> 18) & (0b_0011_1111)) * (1 << 2));
            g = (byte)(((rgba6 >> 12) & (0b_0011_1111)) * (1 << 2));
            b = (byte)(((rgba6 >> 06) & (0b_0011_1111)) * (1 << 2));
            a = (byte)(((rgba6 >> 00) & (0b_0011_1111)) * (1 << 2));
        }
        private void ReadRGBA8(EndianBinaryReader reader)
        {
            uint raw = reader.ReadUInt32();
            GetRGBA8(raw);
        }
        private void ReadRGBX8(EndianBinaryReader reader)
        {
            ReadRGBA8(reader);
            a = 0xFF; // discard alpha
        }

        private uint Read3BytesCorrectEndianness(EndianBinaryReader reader)
        {
            // Reconstruct the 24bit color as uint32
            var bytes = reader.ReadBytes(3);
            if (reader.IsLittleEndian ^ BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            uint color32 = BitConverter.ToUInt32(bytes);
            uint color24 = color32 & 0x00FFFFFF; // only 3 bytes
            return color24;
        }

        private void Write3BytesCorrectEndianness(EndianBinaryWriter writer, uint color24)
        {
            var bytes32 = BitConverter.GetBytes(color24);
            var bytes24 = new byte[3];
            bytes32.CopyTo(bytes24, 1);

            if (writer.IsLittleEndian ^ BitConverter.IsLittleEndian)
                Array.Reverse(bytes24);

            writer.Write(bytes24);
        }

        private void WriteRGBA565(EndianBinaryWriter writer)
        {
            byte r5 = (byte)((r >> 3) & 0b_0001_1111);
            byte g6 = (byte)((g >> 2) & 0b_0011_1111);
            byte b5 = (byte)((b >> 3) & 0b_0001_1111);
            ushort rgb565 = (ushort)(r5 << 11 + g6 << 05 + b5 << 00);
            writer.Write(rgb565);
        }
        private void WriteRGB8(EndianBinaryWriter writer)
        {
            uint raw = GetRawRGBA8();
            Write3BytesCorrectEndianness(writer, raw);
        }
        private void WriteRGBA4(EndianBinaryWriter writer)
        {
            byte r4 = (byte)((r >> 4) & 0b_0000_1111);
            byte g4 = (byte)((g >> 4) & 0b_0000_1111);
            byte b4 = (byte)((b >> 4) & 0b_0000_1111);
            byte a4 = (byte)((a >> 4) & 0b_0000_1111);
            ushort rgba4 = (ushort)(r4 << 12 + g4 << 08 + b4 << 04 + a4 << 00);
            writer.Write(rgba4);
        }
        private void WriteRGBA6(EndianBinaryWriter writer)
        {
            byte r6 = (byte)((r >> 6) & 0b_0011_1111);
            byte g6 = (byte)((g >> 6) & 0b_0011_1111);
            byte b6 = (byte)((b >> 6) & 0b_0011_1111);
            byte a6 = (byte)((a >> 6) & 0b_0011_1111);
            uint rgba6 = (uint)(r6 << 18 + g6 << 12 + b6 << 06 + a6 << 00);
            Write3BytesCorrectEndianness(writer, rgba6);
        }
        private void WriteRGBA8(EndianBinaryWriter writer)
        {
            uint raw = GetRawRGBA8();
            writer.Write(raw);
        }
        private void WriteRGBX8(EndianBinaryWriter writer)
        {
            // Write color with fixed alpha
            uint raw = GetRawRGBA8();
            var color = raw & 0x000000FF;
            writer.Write(color);
        }

        public override string ToString()
        {
            uint raw = GetRawRGBA8();
            return $"#{raw:x8}";
        }

        private uint GetRawRGBA8()
        {
            return (uint)(
                (r << 24) |
                (g << 16) |
                (b << 08) |
                (a << 00));
        }

    }
}
