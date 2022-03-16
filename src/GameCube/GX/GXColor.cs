﻿using Manifold;
using Manifold.IO;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GameCube.GX
{
    [StructLayout(LayoutKind.Explicit)]
    public struct GXColor :
        IBinarySerializable
    {
        [FieldOffset(0)] private uint raw;
        [FieldOffset(0)] private byte r;
        [FieldOffset(1)] private byte g;
        [FieldOffset(2)] private byte b;
        [FieldOffset(3)] private byte a;
        [FieldOffset(4)] private ComponentType componentType;
 
        public byte R { get => r; set => r = value; }
        public byte G { get => g; set => g = value; }
        public byte B { get => b; set => b = value; }
        public byte A { get => a; set => a = value; }
        public uint Raw { get => raw; set => raw = value; }
        public ComponentType ComponentType { get => componentType; set => componentType = value; }


        //public GXColor()
        //{
        //    raw = 0;
        //    r = g = b = a = 0;
        //    componentType = ComponentType.GX_RGBA8;
        //}

        public GXColor(ComponentType componentType)
        {
            raw = 0;
            r = g = b = a = 0;
            this.componentType = componentType;
        }

        public GXColor(uint raw)
        {
            this.raw = raw;
            r = g = b = a = 0;
            componentType = ComponentType.GX_RGBA8;
        }

        public GXColor(byte r, byte g, byte b, byte a = 255)
        {
            raw = 0;
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
            r = reader.ReadUInt8();
            g = reader.ReadUInt8();
            b = reader.ReadUInt8();
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
            // Reconstruct the 24bit color as uint32
            var upper16 = reader.ReadUInt16();
            var lower8 = reader.ReadUInt8();
            var rgba6 = (uint)(upper16 << 8) | (lower8);

            r = (byte)(((rgba6 >> 18) & (0b_0011_1111)) * (1 << 2));
            g = (byte)(((rgba6 >> 12) & (0b_0011_1111)) * (1 << 2));
            b = (byte)(((rgba6 >> 06) & (0b_0011_1111)) * (1 << 2));
            a = (byte)(((rgba6 >> 00) & (0b_0011_1111)) * (1 << 2));
        }
        private void ReadRGBA8(EndianBinaryReader reader)
        {
            r = reader.ReadUInt8();
            g = reader.ReadUInt8();
            b = reader.ReadUInt8();
            a = reader.ReadUInt8();
        }
        private void ReadRGBX8(EndianBinaryReader reader)
        {
            r = reader.ReadUInt8();
            g = reader.ReadUInt8();
            b = reader.ReadUInt8();
            var _ = reader.ReadUInt8(); // discarded
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
            writer.Write(r);
            writer.Write(g);
            writer.Write(b);
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
            byte rgba6_hi = (byte)((rgba6 >> 16) & 0b_1111_1111);
            byte rgba6_mi = (byte)((rgba6 >> 08) & 0b_1111_1111);
            byte rgba6_lo = (byte)((rgba6 >> 00) & 0b_1111_1111);
            writer.Write(rgba6_hi);
            writer.Write(rgba6_mi);
            writer.Write(rgba6_lo);
        }
        private void WriteRGBA8(EndianBinaryWriter writer)
        {
            writer.Write(r);
            writer.Write(g);
            writer.Write(b);
            writer.Write(a);
        }
        private void WriteRGBX8(EndianBinaryWriter writer)
        {
            writer.Write(r);
            writer.Write(g);
            writer.Write(b);
            writer.Write((byte)0xFF);
        }


    }
}
