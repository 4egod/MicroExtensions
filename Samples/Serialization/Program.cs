using System.Diagnostics;

namespace Serialization
{
    class Program
    {
        static void Main(string[] args)
        {
            SomeClass test = new SomeClass()
            {
                IntValue = 100,
                UIntValue = 200,
                ShortValue = 300,
                UShortValue = 400,
                StringValue = "Some string value",
                SecondStringValue = "Another string value",
                SomeEnum = SomeEnum.Item2, // Won't serialize
                SomeEnumAsByte = SomeEnum.Item3
            };

            var buf = test.Serialize();
            string hex = buf.ToHex();
            Debug.WriteLine(hex);

            test = (SomeClass)buf.Deserialize(typeof(SomeClass));

            hex = "640000001400000041006E006F007400680065007200200073007400720069006E0067002000760061006C00750065002C011100000053006F006D006500200073007400720069006E0067002000760061006C0075006500C8000000900103";
            buf = hex.FromHex();
            test = (SomeClass)buf.Deserialize(test.GetType());
        }
    }
}
