#if MF || TINYCLR

using System;

public static class EnumExtension
{
    public static bool HasFlag(this Enum @enum, Enum flag)
    {
        if (flag == null)
        {
            throw new ArgumentNullException("flag");
        }

        if (@enum.GetType() != flag.GetType())
        {
            throw new ArgumentException("flag");
        }

        int i = (int)@enum.GetType().GetField("value__").GetValue(@enum);
        int j = (int)@flag.GetType().GetField("value__").GetValue(flag);

        //if ((i != 0) && (j == 0))
        //{
        //    return false;
        //}

        return (i & j) == j;
    }
}

#endif