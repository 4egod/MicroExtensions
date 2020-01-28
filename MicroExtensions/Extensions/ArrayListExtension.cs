using System.Collections;

//namespace Extensions
//{
    public static class ArrayListExtension
    {
        public static void AddRange(this ArrayList value, ArrayList values)
        {
            foreach (var item in values)
            {
                value.Add(item);
            }
        }
    }
//}
