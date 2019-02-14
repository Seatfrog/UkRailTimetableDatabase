using System;
using System.Collections.Generic;
using System.Text;
using TimetableLoader;
using Xunit;

namespace TimetableLoaderTest
{
    public class SequenceTest
    {
        [Fact]
        public void FirstValueIsOne()
        {
            var sequence = new Sequence();

            Assert.Equal(1, sequence.GetNext());
        }

        [Fact]
        public void IncrementsstValueByOneEachtime()
        {
            var sequence = new Sequence();

            Assert.Equal(1, sequence.GetNext());
            Assert.Equal(2, sequence.GetNext());
            Assert.Equal(3, sequence.GetNext());
        }
    }
}
