
namespace Serialization
{
    public class SomeClass
    {
        public int IntValue { get; set; }

        public uint UIntValue { get; set; }

        public string StringValue { get; set; }

        public short ShortValue { get; set; }

        public ushort UShortValue { get; set; }

        public string SecondStringValue { get; set; }

        /// <summary>
        /// Can't be serialized
        /// </summary>
        public SomeEnum SomeEnum { get; set; }


        private byte someEnumAsByte;
        public SomeEnum SomeEnumAsByte
        {
            get { return (SomeEnum)someEnumAsByte; }
            set { someEnumAsByte = (byte)value; }
        }
    }
}
