using System;
using TimetableLoader;
using Xunit;

namespace TimetableLoaderTest
{
    public class DayMaskConverterTest
    {
        [Theory]
        [InlineData("0000000", 0)]
        [InlineData("1000000", 1)]
        [InlineData("0100000", 2)]
        [InlineData("0010000", 4)]
        [InlineData("0001000", 8)]
        [InlineData("0000100", 16)]
        [InlineData("0000010", 32)]
        [InlineData("0000001", 64)]
        [InlineData("1111111", 127)]
        public void ConvertMask(string mask, byte expected)
        {
            Assert.Equal(expected, DayMaskConverter.Convert(mask));
        }

        [Theory]
        [InlineData("")]
        [InlineData("       ")]
        [InlineData(null)]
        public void ConvertEmpty(string mask)
        {
            Assert.Equal(DBNull.Value, DayMaskConverter.Convert(mask));            
        }
    }
}