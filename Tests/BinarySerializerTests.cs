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

        [TestMethod]
        public void VersionTest()
        {
            Version data = new Version(1, 2, 3, 4);
            byte[] buf = data.Serialize();
            data = (Version)buf.Deserialize(typeof(Version));
        }
    }
}
