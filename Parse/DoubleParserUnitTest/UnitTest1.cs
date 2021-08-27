using DoubleParserLib;
using Xunit;

namespace DoubleParserUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var s = "11.11";
            var r = DoubleParser.Parse(s);

            Assert.True((r - 11.11) <= double.Epsilon);
        }
    }
}
