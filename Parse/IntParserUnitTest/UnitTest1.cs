using Xunit;
using IntParserLib;
using System;
using System.Globalization;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test_ParseAllotThousands()
        {
            var s = "1,000";
            int.TryParse(s, NumberStyles.AllowThousands, provider: null, out var i);
            // 
            Assert.True(i == 1000);
        }


        [Fact]
        public void Test_CustomParse_MidleWhiteSpace()
        {
            var s = " 1 0 ";
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }

        [Fact]
        public void Test_CustomParse_Dot()
        {
            var s = "10.0";
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }

        [Fact]
        public void Test_CustomParse_NormalSpan()
        {
            var s = "10".AsSpan();
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }

        [Fact]
        public void Test_CustomParse_WhiteSpaceSpan()
        {
            var s = " 10 ";
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }

        [Fact]
        public void Test_CustomParse_DotSpan()
        {
            var s = "10.0".AsSpan();
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }

        [Fact]
        public void Test_CustomParse_PositveSpan()
        {
            var s = "+10".AsSpan();
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }

        [Fact]
        public void Test_CustomParse_NegativeSpan()
        {
            var s = "-10".AsSpan();
            var i = IntParser.Parse(s);
            Assert.True(i == -10);
        }
    }
}
