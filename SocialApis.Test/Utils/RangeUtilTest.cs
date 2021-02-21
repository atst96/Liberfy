using System.Collections.Generic;
using SocialApis.Utils;
using Xunit;

namespace SocialApis.Test.Utils
{
    public class RangeUtilTest
    {
        [Fact]
        public void Enumerate_Byte_ZeroLength()
        {
            IEnumerable<byte> actual, expected;

            actual = RangeUtil.Enumerate((byte)0, (byte)0);
            expected = new byte[] { 0 };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Enumerate_Byte_Up()
        {
            IEnumerable<byte> actual, expected;

            actual = RangeUtil.Enumerate((byte)0, (byte)5);
            expected = new byte[] { 0, 1, 2, 3, 4, 5 };

            Assert.Equal(expected, actual);

            actual = RangeUtil.Enumerate((byte)254, (byte)255);
            expected = new byte[] { 254, 255 };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Enumerate_Byte_Down()
        {
            IEnumerable<byte> actual, expected;

            actual = RangeUtil.Enumerate((byte)5, (byte)0);
            expected = new byte[] { 5, 4, 3, 2, 1, 0 };

            Assert.Equal(expected, actual);
        }
    }
}
