using System.Collections;

public static class IListExtension
{
    public static bool AreElementsEqual(this IList instance, IList value, int offset, int length)
    {
        try
        {
            for (int i = 0; i < length; i++)
            {
                if (!instance[offset + i].Equals(value[offset + i]))
                {
                    return false;
                }
            }
        }
        catch
        {
            return false;
        }

        return true;
    }
}
