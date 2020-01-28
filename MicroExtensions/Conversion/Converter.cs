
//namespace Extensions
//{
    using System;
    using System.Collections;
    using System.Text;

    public static class Converter
    {
        public static byte[] GetBytes(this sbyte value)
        {
            return new byte[1] { (byte)value };
        }

        public static byte[] GetBytes(this byte value)
        {
            return new byte[1] { value };
        }

        public static byte[] GetBytes(this bool value)
        {
            return new byte[1] { (value ? (byte)1 : (byte)0) };
        }

        public static byte[] GetBytes(this char value)
        {
            return BitConverter.GetBytes((short)value);
        }

        public static byte[] GetBytes(this short value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this ushort value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this uint value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this long value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this ulong value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this float value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(this double value)
        {
            return BitConverter.GetBytes(value);
        }

#if !MF && !TINYCLR
        public static byte[] GetBytes(this BitArray bits)
        {
            int numBytes = bits.Length / 8;
            if (bits.Length % 8 != 0) numBytes++;

            byte[] bytes = new byte[numBytes];
            int byteIndex = 0, bitIndex = 0;

            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                    bytes[byteIndex] |= (byte)(1 << (7 - bitIndex));

                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return bytes;
        }

        public static BitArray GetBits(this sbyte value)
        {
            return value.GetBytes().GetBits(0, 1);
        }

        public static BitArray GetBits(this byte value)
        {
            return value.GetBytes().GetBits(0, 1);
        }

        public static BitArray GetBits(this short value)
        {
            return value.GetBytes().GetBits(0, 4);
        }

        public static BitArray GetBits(this ushort value)
        {
            return value.GetBytes().GetBits(0, 4);
        }

        public static BitArray GetBits(this int value)
        {
            return value.GetBytes().GetBits(0, 8);
        }

        public static BitArray GetBits(this uint value)
        {
            return value.GetBytes().GetBits(0, 4);
        }

        public static BitArray GetBits(this long value)
        {
            return value.GetBytes().GetBits(0, 8);
        }

        public static BitArray GetBits(this ulong value)
        {
            return value.GetBytes().GetBits(0, 8);
        }

        public static BitArray GetBits(this byte[] value, int offset, int count)
        {
            byte[] buf = new byte[count];
            Array.Copy(value, offset, buf, 0, count);
            BitArray res = new BitArray(buf);

            return res;
        }
#endif

        public static char ToChar(this byte[] value, int offset)
        {
            return (char)BitConverter.ToInt16(value, offset);
        }

        public static short ToInt16(this byte[] value, int offset)
        {
            return BitConverter.ToInt16(value, offset);
        }

        public static ushort ToUInt16(this byte[] value, int offset)
        {
            return BitConverter.ToUInt16(value, offset);
        }

        public static int ToInt32(this byte[] value, int offset)
        {
            return BitConverter.ToInt32(value, offset);
        }

        public static uint ToUInt32(this byte[] value, int offset)
        {
            return BitConverter.ToUInt32(value, offset);
        }

        public static long ToInt64(this byte[] value, int offset)
        {
            return BitConverter.ToInt64(value, offset);
        }

        public static ulong ToUInt64(this byte[] value, int offset)
        {
            return BitConverter.ToUInt64(value, offset);
        }

        public static float ToSingle(this byte[] value, int offset)
        {
            return BitConverter.ToSingle(value, offset);
        }

        public static double ToDouble(this byte[] value, int offset)
        {
            return BitConverter.ToDouble(value, offset);
        }

        public static string ToByteString(this byte[] value)
        {
            return ToByteString(value, 0, value.Length);
        }

        public static string ToByteString(this byte[] value, int offset, int length)
        {
            char[] cbuf = new char[length];
            int j = 0;
            for (int i = offset; i < length + offset; i++)
            {
                cbuf[j] = (char)value[i];
                j++;
            }

            return new string(cbuf);
        }

        public static string ToHex(this byte value)
        {
            byte l = (byte)(value / 16);
            byte r = (byte)(value % 16);

            return Converter.InternalToHex(l) + Converter.InternalToHex(r);
        }

        public static string ToHex(this ushort value, string delimiter = "")
        {
            return value.GetBytes().SwapBytes().ToHex(delimiter);
        }

        public static string ToHex(this short value, string delimiter = "")
        {
            return value.GetBytes().SwapBytes().ToHex(delimiter);
        }

        public static string ToHex(this uint value, string delimiter = "")
        {
            return value.GetBytes().SwapWords().SwapBytes().ToHex(delimiter);
        }

        public static string ToHex(this int value, string delimiter = "")
        {
            return value.GetBytes().SwapWords().SwapBytes().ToHex(delimiter);
        }

        public static string ToHex(this ulong value, string delimiter = "")
        {
            return value.GetBytes().SwapBytes().ToHex(delimiter);
        }

        public static string ToHex(this long value, string delimiter = "")
        {
            return value.GetBytes().SwapDoubleWords().ToHex(delimiter);
        }

        public static string ToHex(this byte[] values, string delimiter = "")
        {
            return ToHex(values, 0, values.Length, delimiter);
        }

        public static string ToHex(this byte[] values, int offset, int length, string delimiter = "")
        {
#if MF_FRAMEWORK_VERSION_V4_0 || MF_FRAMEWORK_VERSION_V4_1
            string s = string.Empty;
            for (int i = offset; i < length; i++)
            {
                s += values[i].ToHex();
                s += delimiter;
            }

            if (delimiter != string.Empty)
            {
                return s.Substring(0, s.Length - 1);
            }

            return s;
#else
            StringBuilder s = new StringBuilder();
            for (int i = offset; i < length + offset; i++)
            {
                s.Append(values[i].ToHex());
                s.Append(delimiter);
            }

            if (delimiter != string.Empty)
            {
                s.Remove(s.Length - 1, 1);
            }

            return s.ToString();
#endif
        }

        public static byte[] Reverse(this byte[] value)
        {
            byte[] res = new byte[value.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = value[value.Length - i - 1];
            }

            return res;
        }

        public static byte[] SwapBytes(this byte[] values)
        {
            byte[] result = new byte[values.Length];
            values.CopyTo(result, 0);

            int i = 0;
            while (i < result.Length)
            {
                byte b = result[i];
                result[i] = result[i + 1];
                result[i + 1] = b;
                i += 2;
            }

            return result;
        }

        public static byte[] SwapWords(this byte[] values)
        {
            byte[] result = new byte[values.Length];
            values.CopyTo(result, 0);

            int i = 0;
            while (i < result.Length)
            {
                byte b1 = result[i];
                byte b2 = result[i + 1];

                result[i] = result[i + 2];
                result[i + 1] = result[i + 3];

                result[i + 2] = b1;
                result[i + 3] = b2;

                i += 4;
            }

            return result;
        }

        public static byte[] SwapDoubleWords(this byte[] values)
        {
            byte[] result = new byte[values.Length];
            values.CopyTo(result, 0);

            int i = 0;
            while (i < result.Length)
            {
                byte b1 = result[i];
                byte b2 = result[i + 1];
                byte b3 = result[i + 2];
                byte b4 = result[i + 3];

                result[i] = result[i + 4];
                result[i + 1] = result[i + 5];
                result[i + 2] = result[i + 6];
                result[i + 3] = result[i + 7];

                result[i + 4] = b1;
                result[i + 5] = b2;
                result[i + 6] = b1;
                result[i + 7] = b2;

                i += 8;
            }

            return result;
        }

        public static byte[] FromHex(this string value)
        {
            byte[] result = new byte[value.Length / 2];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)Convert.ToInt32(value.Substring(i * 2, 2), 16);
            }

            return result;
        }

        public static byte SetBit(this byte value, int bitNumber, bool bit = true)
        {
            switch (bitNumber)
            {
                case 0: if (bit) return (byte)(value | 1); else return (byte)(value ^ 1);
                case 1: if (bit) return (byte)(value | 2); else return (byte)(value ^ 1);
                case 2: if (bit) return (byte)(value | 4); else return (byte)(value ^ 1);
                case 3: if (bit) return (byte)(value | 8); else return (byte)(value ^ 1);
                case 4: if (bit) return (byte)(value | 16); else return (byte)(value ^ 1);
                case 5: if (bit) return (byte)(value | 32); else return (byte)(value ^ 1);
                case 6: if (bit) return (byte)(value | 64); else return (byte)(value ^ 1);
                case 7: if (bit) return (byte)(value | 128); else return (byte)(value ^ 1);
                default: throw new ArgumentOutOfRangeException(bitNumber.ToString());
            }
        }

        private static string InternalToHex(byte halfByte)
        {
            if (halfByte <= 9)
            {
                return halfByte.ToString();
            }
            else
            {
                switch (halfByte)
                {
                    case 10: return "A";
                    case 11: return "B";
                    case 12: return "C";
                    case 13: return "D";
                    case 14: return "E";
                    case 15: return "F";
                    default: return string.Empty;
                }
            }
        }
    }
//}
