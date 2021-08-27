using Xunit;
using IntParserLib;
using System;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test_CustomParse_Normal()
        {
            var s = "10";
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }

        [Fact]
        public void Test_Fail()
        {
            var s = "10t";
            var i = IntParser.Parse(s);
            Assert.False(i == 10);
        }

        [Fact]
        public void Test_CustomParse_WhiteSpace()
        {
            var s = " 10 ";
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
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
    }
}
