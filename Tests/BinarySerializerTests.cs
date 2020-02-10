using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    [TestClass]
    public class BinarySerializerTests
    {
        [TestMethod]
        public void DateTimeTest()
        {
            DateTime date = DateTime.Now;

            byte[] buf = date.Serialize();

            date = (DateTime)buf.Deserialize(typeof(DateTime));
        }
    }
}
