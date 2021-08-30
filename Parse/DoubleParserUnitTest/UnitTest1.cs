using DoubleParserLib;
using Xunit;

namespace DoubleParserUnitTest
{
    public class UnitTest1
    {
        // https://gist.github.com/samcragg/1680386

        [Fact]
        public void DoubleParser_NullOrEmpty()
        {
            DoubleParser_Test(null, 0d);
            DoubleParser_Test(string.Empty, 0d);
        }

        [Fact]
        public void DoubleParse_AllowThousands()
        {
            double.TryParse("1,000.12", out var r);
            Assert.True(r == 1000.12d);
        }

        [Fact]
        public void DoubleParser_Normal()
        {
            DoubleParser_Test("11", 11);
            DoubleParser_Test("11e1", 110);
            DoubleParser_Test("11e2", 1100);
            DoubleParser_Test("11.11", 11.11d);
            DoubleParser_Test(".11", 0.11d);
            DoubleParser_Test("+11.11", 11.11d);
            DoubleParser_Test("-11.11", -11.11d);
            DoubleParser_Test("11.1e1", 111);
            DoubleParser_Test("11.1e2", 1110);
            DoubleParser_Test("+11.1e2", 1110);
            DoubleParser_Test("-11.1e2", -1110);
        }

        [Fact]
        public void DoubleParser_Contant()
        {
            DoubleParser_Test(double.MaxValue.ToString(), double.MaxValue);
            DoubleParser_Test(double.MinValue.ToString(), double.MinValue);
            DoubleParser_Test(double.Epsilon.ToString(), double.Epsilon);
            DoubleParser_Test((-double.Epsilon).ToString(), -double.Epsilon);

            DoubleParser_Test("+1E+309", double.PositiveInfinity);
            DoubleParser_Test("-1E+309", double.NegativeInfinity);

            DoubleParser_Test("+1E-325", +0d);
            DoubleParser_Test("-1E-325", -0d);
        }

        public void DoubleParser_Test(string s, double expect)
        {
            double.TryParse(s, out var r);
            var r_span = DoubleParser.Parse(s);

            if (double.IsPositiveInfinity(expect)) {
                Assert.True(double.IsPositiveInfinity(r) && double.IsPositiveInfinity(r_span));
            } else if (double.IsNegativeInfinity(expect)) {  
                Assert.True(double.IsNegativeInfinity(r) && double.IsNegativeInfinity(r_span));
            } else {
                Assert.True(r - r_span <= double.Epsilon);
                Assert.True(r_span - expect <= double.Epsilon);
            }
        }
    }
}
