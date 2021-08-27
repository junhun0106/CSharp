using Xunit;
using IntParserLib;
using System;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test_Normal()
        {
            var s = "10";
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }

        [Fact]
        public void Test_WhiteSpace()
        {
            var s = " 10 ";
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }

        [Fact]
        public void Test_Dot()
        {
            var s = "10.0";
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }

        [Fact]
        public void Test_NormalSpan()
        {
            var s = "10".AsSpan();
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }

        [Fact]
        public void Test_WhiteSpaceSpan()
        {
            var s = " 10 ";
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }

        [Fact]
        public void Test_DotSpan()
        {
            var s = "10.0".AsSpan();
            var i = IntParser.Parse(s);
            Assert.True(i == 10);
        }
    }
}
