
//namespace Extensions
//{
    using System;
    using System.Reflection;
    using System.Collections;
#if !MF && !TINYCLR
    using System.Collections.Generic;
#endif
    using System.Text;
    using System.Runtime.CompilerServices;
using System.Diagnostics;

public static class BinarySerializer
{
    public enum SupportedTypes : byte
    {
        Unsupported,
        SByte,
        Byte,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        UInt64,
        Single,
        Double,
        Boolean,
        Char,
        DateTime,
        TimeSpan,
        String,
        Guid
    }

    /// <summary>
    /// Serialize any object as byte array.
    /// Can't serialize Enums
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static byte[] Serialize(this object obj)
    {
        if (obj == null)
        {
            return null;
        }

        //#if MF || NET40
        //            if (!obj.GetType().IsSerializable)
        //#else
        //            if (!obj.GetType().GetTypeInfo().IsSerializable)
        //#endif

        //if (!obj.GetType().IsSerializable)
        //{
        //    throw new InvalidOperationException();
        //}

        int size = 0;

        ArrayList list = RecursiveSerialize(obj, ref size);
        /// TODO: Has different hash algo in MF and .Net Standart
        // list.Insert(0, obj.GetType().GetHashCode().GetBytes()); 

        byte[] res = new byte[size];// + 4];

        int index = 0;
        foreach (var item in list)
        {
            ((byte[])item).CopyTo(res, index);
            index += ((byte[])item).Length;
        }

        return res;
    }

    public static object Deserialize(this byte[] buf, int offset, Type type)
    {
        object instance = CreateInstance(type);

        Deserialize(buf, offset, ref instance);

        return instance;
    }

    public static object Deserialize(this byte[] buf, Type type)
    {
        return Deserialize(buf, 0, type);
    }

    public static void Deserialize(byte[] buf, int offset, ref object obj)
    {
        if (buf == null || obj == null)
        {
            throw new ArgumentNullException();
        }

        //if (obj.GetType().GetHashCode() != buf.ToInt32(offset))
        //{
        //    throw new InvalidCastException();
        //}

        int index = offset; //+ 4;

        RecursiveDeserialize(ref obj, buf, ref index);
    }

    public static void Deserialize(byte[] buf, ref object obj)
    {
        Deserialize(buf, 0, ref obj);
    }

    private static ArrayList RecursiveSerialize(object obj, ref int size)
    {
        ArrayList res = new ArrayList();

        if (obj == null)
        {
            return res;
        }

        Type t = obj.GetType();

        /// TODO Can't deserialize enums on NETMF (CLR_E_WRONG_TYPE)
        if (t.IsEnum)
        {
            return res;
        }

        SupportedTypes st = ToSupportedType(t);

        /// Serialize primitives
        if (st != SupportedTypes.Unsupported)
        {
            res.Add(SerializeSupportedType(obj, st, ref size));
            return res;
        }

        FieldInfo[] fields = GetSerializableFileds(obj);

        foreach (var item in fields)
        {
            t = item.FieldType;

            /// TODO Can't deserialize enums on NETMF (CLR_E_WRONG_TYPE)
            if (t.IsEnum)
            {
                continue;
            }

            st = ToSupportedType(t);

            object o = item.GetValue(obj);

            if (o == null)
            {
                o = CreateInstance(t);
            }

            if (o == null)
            {
                return res;
            }

            st = ToSupportedType(o.GetType());

            if (st != SupportedTypes.Unsupported)
            {
                res.Add(SerializeSupportedType(o, st, ref size));
            }
            else
            {
                if (o is IList)
                {
                    IList l = (o as IList);

                    //res.Add(l.Count.GetBytes());
                    //size += 4;

                    if (l.Count > 0)
                    {
                        st = ToSupportedType(l[0].GetType());
                        if (st != SupportedTypes.Unsupported)
                        {
                            res.Add(((byte)st).GetBytes());
                            size += 1;
                        }

                        for (int i = 0; i < l.Count; i++)
                        {
                            res.AddRange(RecursiveSerialize(l[i], ref size));
                        }
                    }

                    continue;
                }

                res.AddRange(RecursiveSerialize(o, ref size));
            }
        }

        return res;
    }

    /// <summary>
    /// There is some problems in MF with enums
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="buf"></param>
    /// <param name="index"></param>
    private static void RecursiveDeserialize(ref object obj, byte[] buf, ref int index)
    {
        if (obj == null)
        {
            return;
        }

        Type t = obj.GetType();

        /// TODO Can't deserialize enums on NETMF (CLR_E_WRONG_TYPE)
        if (t.IsEnum)
        {
            return;
        }

        SupportedTypes st = ToSupportedType(t);

        /// Deserialize primitives
        if (st != SupportedTypes.Unsupported)
        {
            obj = DeserializeSupportedType(st, buf, ref index);
            return;
        }

        FieldInfo[] fields = GetSerializableFileds(obj);

        foreach (var item in fields)
        {
            t = item.FieldType;

            /// TODO Can't deserialize enums on NETMF (CLR_E_WRONG_TYPE)
            if (t.IsEnum)
            {
                continue;
            }

            st = ToSupportedType(t);

            object o = item.GetValue(obj);

            if (o == null)
            {
                o = CreateInstance(t);
            }

            if (o == null)
            {
                return;
            }

            st = ToSupportedType(o.GetType());

            if (st != SupportedTypes.Unsupported)
            {
                item.SetValue(obj, DeserializeSupportedType(st, buf, ref index));
            }
            else
            {
                if (o is IList)
                {
                    IList l = (o as IList);

                    //int len = buf.ToInt32(index);
                    //index += 4;

                    st = (SupportedTypes)buf[index];
                    index++;

                    for (int i = 0; i < l.Count; i++)
                    {
                        object to = l[i];
                        RecursiveDeserialize(ref to, buf, ref index);
                        l[i] = to;
                    }

                    continue;
                }

                if (o is IList)
                {
                    foreach (var enumerableItem in (o as IEnumerable))
                    {
                        object to = enumerableItem;
                        RecursiveDeserialize(ref to, buf, ref index);
                        item.SetValue(o, to);
                    }

                    continue;
                }

#if MF
                //if (t.IsEnum)
                //{
                //    continue;
                //}
#endif

                RecursiveDeserialize(ref o, buf, ref index);
                item.SetValue(obj, o);
            }
        }
    }

    private static byte[] SerializeSupportedType(object obj, SupportedTypes type, ref int size)
    {
        byte[] result;

        switch (type)
        {
            case SupportedTypes.SByte:
                {
                    result = ((SByte)obj).GetBytes();
                    size += 1;
                    break;
                }
            case SupportedTypes.Byte:
                {
                    result = ((Byte)obj).GetBytes();
                    size += 1;
                    break;
                }
            case SupportedTypes.Int16:
                {
                    result = ((Int16)obj).GetBytes();
                    size += 2;
                    break;
                }
            case SupportedTypes.UInt16:
                {
                    result = ((UInt16)obj).GetBytes();
                    size += 2;
                    break;
                }
            case SupportedTypes.Int32:
                {
                    result = ((Int32)obj).GetBytes();
                    size += 4;
                    break;
                }
            case SupportedTypes.UInt32:
                {
                    result = ((UInt32)obj).GetBytes();
                    size += 4;
                    break;
                }
            case SupportedTypes.Int64:
                {
                    result = ((Int64)obj).GetBytes();
                    size += 8;
                    break;
                }
            case SupportedTypes.UInt64:
                {
                    result = ((UInt64)obj).GetBytes();
                    size += 8;
                    break;
                }
            case SupportedTypes.Single:
                {
                    result = ((Single)obj).GetBytes();
                    size += 4;
                    break;
                }
            case SupportedTypes.Double:
                {
                    result = ((Double)obj).GetBytes();
                    size += 8;
                    break;
                }
            case SupportedTypes.Boolean:
                {
                    result = ((Boolean)obj).GetBytes();
                    size += 1;
                    break;
                }
            case SupportedTypes.Char:
                {
                    result = ((Char)obj).GetBytes();
                    size += 2;
                    break;
                }
            case SupportedTypes.DateTime:
                {
                    //result = ((DateTime)obj).Ticks.GetBytes();
                    var o = (DateTime)obj;

                    result = new byte[7];
                    int i = 0;
                    ((ushort)o.Year).GetBytes().CopyTo(result, i);
                    i += 2;

                    result[i++] = (byte)o.Month;
                    result[i++] = (byte)o.Day;
                    result[i++] = (byte)o.Hour;
                    result[i++] = (byte)o.Minute;
                    result[i++] = (byte)o.Second;

                    size += result.Length;
                    break;
                }
            case SupportedTypes.TimeSpan:
                {
                    result = ((TimeSpan)obj).Ticks.GetBytes();
                    size += 8;
                    break;
                }
            case SupportedTypes.String:
                {
                    string s = (obj as string);
                    int len = s.Length * 2 + 4;
                    result = new byte[len];
                    s.Length.GetBytes().CopyTo(result, 0);
                    for (int i = 0; i < s.Length; i++)
                    {
                        s[i].GetBytes().CopyTo(result, 4 + (i * 2));
                    }

                    size += len;
                    break;
                }
            case SupportedTypes.Guid:
                {
                    result = ((Guid)obj).ToByteArray();
                    size += 16;
                    break;
                }

            default: return null;
        }

        return result;
    }

    private static object DeserializeSupportedType(SupportedTypes type, byte[] buf, ref int index)
    {
        object result;

        switch (type)
        {
            case SupportedTypes.SByte:
                {
                    result = (SByte)buf[index];
                    index += 1;
                    break;
                }
            case SupportedTypes.Byte:
                {
                    result = buf[index];
                    index += 1;
                    break;
                }
            case SupportedTypes.Int16:
                {
                    result = buf.ToInt16(index);
                    index += 2;
                    break;
                }
            case SupportedTypes.UInt16:
                {
                    result = buf.ToUInt16(index);
                    index += 2;
                    break;
                }
            case SupportedTypes.Int32:
                {
                    result = buf.ToInt32(index);
                    index += 4;
                    break;
                }
            case SupportedTypes.UInt32:
                {
                    result = buf.ToUInt32(index);
                    index += 4;
                    break;
                }
            case SupportedTypes.Int64:
                {
                    result = buf.ToInt64(index);
                    index += 8;
                    break;
                }
            case SupportedTypes.UInt64:
                {
                    result = buf.ToUInt64(index);
                    index += 8;
                    break;
                }
            case SupportedTypes.Single:
                {
                    result = buf.ToSingle(index);
                    index += 4;
                    break;
                }
            case SupportedTypes.Double:
                {
                    if (index != 0) // crutch: .NET Framework cruhing in other case on G120
                    {
                        byte[] tempBuf = new byte[8];
                        Array.Copy(buf, index, tempBuf, 0, 8);
                        result = tempBuf.ToDouble(0);
                    }
                    else
                    {
                        result = buf.ToDouble(index);
                    }

                    index += 8;
                    break;
                }
            case SupportedTypes.Boolean:
                {
                    result = (buf[index] == 1) ? true : false;
                    index += 1;
                    break;
                }
            case SupportedTypes.Char:
                {
                    result = (Char)(buf.ToInt16(index));
                    index += 2;
                    break;
                }
            case SupportedTypes.DateTime:
                {
                    //result = new DateTime(buf.ToInt64(index));
                    ushort year = buf.ToUInt16(index);
                    index += 2;
                    byte month = buf[index++];
                    byte day = buf[index++];
                    byte hour = buf[index++];
                    byte min = buf[index++];
                    byte sec = buf[index++];

                    result = new DateTime(year, month, day, hour, min, sec);

                    //index += 7;
                    break;
                }
            case SupportedTypes.TimeSpan:
                {
                    result = new TimeSpan(buf.ToInt64(index));
                    index += 8;
                    break;
                }
            case SupportedTypes.String:
                {
                    int len = buf.ToInt32(index);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < len; i++)
                    {
                        sb.Append((char)(buf.ToInt16(index + 4 + (i * 2))));
                    }

                    result = sb.ToString();
                    index += (len * 2 + 4);
                    break;
                }
            case SupportedTypes.Guid:
                {
                    byte[] a = new byte[16];
                    Array.Copy(buf, index, a, 0, 16);
                    result = new Guid(a);
                    index += 16;
                    break;
                }

            default: return null;
        }

        return result;
    }

    private static SupportedTypes ToSupportedType(Type type)
    {

        if (type.IsEnum)
        {
            return SupportedTypes.Unsupported;
            //#if !MF
            //                type = Enum.GetUnderlyingType(type);
            //#else
            //                return SupportedTypes.Unsupported;
            //#endif
        }

        switch (type.Name)
        {
            case "SByte": return SupportedTypes.SByte;
            case "Byte": return SupportedTypes.Byte;
            case "Int16": return SupportedTypes.Int16;
            case "UInt16": return SupportedTypes.UInt16;
            case "Int32": return SupportedTypes.Int32;
            case "UInt32": return SupportedTypes.UInt32;
            case "Int64": return SupportedTypes.Int64;
            case "UInt64": return SupportedTypes.UInt64;
            case "Single": return SupportedTypes.Single;
            case "Double": return SupportedTypes.Double;
            case "Boolean": return SupportedTypes.Boolean;
            case "Char": return SupportedTypes.Char;
            case "DateTime": return SupportedTypes.DateTime;
            case "TimeSpan": return SupportedTypes.TimeSpan;
            case "String": return SupportedTypes.String;
            case "Guid": return SupportedTypes.Guid;

            default: return SupportedTypes.Unsupported;
        }
    }

    private static Type FromSupportedType(SupportedTypes type)
    {
        switch (type)
        {
            case SupportedTypes.SByte: return typeof(SByte);
            case SupportedTypes.Byte: return typeof(Byte);
            case SupportedTypes.Int16: return typeof(Int16);
            case SupportedTypes.UInt16: return typeof(UInt16);
            case SupportedTypes.Int32: return typeof(Int32);
            case SupportedTypes.UInt32: return typeof(UInt32);
            case SupportedTypes.Int64: return typeof(Int64);
            case SupportedTypes.UInt64: return typeof(UInt64);
            case SupportedTypes.Single: return typeof(Single);
            case SupportedTypes.Double: return typeof(Double);
            case SupportedTypes.Boolean: return typeof(Boolean);
            case SupportedTypes.Char: return typeof(Char);
            case SupportedTypes.DateTime: return typeof(DateTime);
            case SupportedTypes.TimeSpan: return typeof(TimeSpan);
            case SupportedTypes.String: return typeof(String);
            case SupportedTypes.Guid: return typeof(Guid);
            default: return null;
        }
    }

    private static object CreateInstance(Type type)
    {
        /// for specified types without default constructor, like string
        SupportedTypes st = ToSupportedType(type);
        switch (st)
        {
            case SupportedTypes.SByte: return (SByte)0;
            case SupportedTypes.Byte: return (Byte)0;
            case SupportedTypes.Int16: return (Int16)0;
            case SupportedTypes.UInt16: return (UInt16)0;
            case SupportedTypes.Int32: return (Int32)0;
            case SupportedTypes.UInt32: return (UInt32)0;
            case SupportedTypes.Int64: return (Int64)0;
            case SupportedTypes.UInt64: return (UInt64)0;
            case SupportedTypes.Single: return (Single)0;
            case SupportedTypes.Double: return (Double)0;
            case SupportedTypes.Boolean: return false;
            case SupportedTypes.Char: return '\0';
            case SupportedTypes.DateTime: return new DateTime();
            case SupportedTypes.TimeSpan: return new TimeSpan();
            case SupportedTypes.String: return string.Empty;
            case SupportedTypes.Guid: return Guid.Empty;
        }

        /// For types with default constructor
#if MF || TINYCLR
        ConstructorInfo ci = type.GetConstructor(new Type[] { });

        if (ci == null)
        {
            throw new NotSupportedException("Can't find default constructor.");
        }

        return ci.Invoke(new object[] { });
#else
            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception)
            {
                throw new NotSupportedException("Can't find default constructor.");
            }
#endif
    }

    private static FieldInfo[] GetSerializableFileds(object obj)
    {
        FieldInfo[] fields;
#if MF || TINYCLR
        fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#else
        fields = new List<FieldInfo>(obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).ToArray();
#endif
        FieldInfo fi;
        int nonSerialized = 0;

        for (int i = 0; i < fields.Length - nonSerialized; i++)
        {
            //Debug.WriteLine(fields[i].Name);
#if MF || TINYCLR

            /// TODO: NETMF doesn't support NotSerialized attribute, add this check later
            //if (!fields[i].FieldType.IsSerializable)
            if (fields[i].Name.IndexOf("_") == 0)
            {
                fields[i] = fields[fields.Length - nonSerialized - 1];
                nonSerialized++;
            }
#else
            if (fields[i].Attributes.HasFlag(FieldAttributes.NotSerialized) || (fields[i].Attributes.HasFlag(FieldAttributes.Static)))
            {
                fields[i] = fields[fields.Length - nonSerialized - 1];
                nonSerialized++;
            }
#endif
            for (int j = i + 1; j < fields.Length - nonSerialized; j++)
            {
                for (int k = 0; k < fields[i].Name.Length; k++)
                {
                    if (k >= fields[j].Name.Length)
                    {
                        break;
                    }

                    if (fields[j].Name[k] < fields[i].Name[k])
                    {
                        fi = fields[i];
                        fields[i] = fields[j];
                        fields[j] = fi;
                        break;
                    }

                    if (fields[j].Name[k] == fields[i].Name[k])
                    {
                        continue;
                    }

                    if (fields[j].Name[k] > fields[i].Name[k])
                    {
                        break;
                    }
                }
            }
        }

        FieldInfo[] res = new FieldInfo[fields.Length - nonSerialized];
        Array.Copy(fields, 0, res, 0, fields.Length - nonSerialized);

        //foreach (var item in res)
        //{
        //    Debug.WriteLine(item.Name);
        //}

        return res;
    }
}
//}