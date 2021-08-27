using DoubleParserLib;
using Xunit;

namespace DoubleParserUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void DoubleParser_Normal()
        {
            var s = "11.11";
            var r = DoubleParser.Parse(s);

            Assert.True((r - 11.11) <= double.Epsilon);
        }

        [Fact]
        public void DoubleParser_Integer()
        {
            var s = "11";
            var r = DoubleParser.Parse(s);

            Assert.True((r - 11) <= double.Epsilon);
        }
    }
}
