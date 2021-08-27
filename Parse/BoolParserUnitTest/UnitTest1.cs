using Xunit;
using BoolParseLib;
using System;

namespace BoolParserUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void BoolParseSpan_True()
        {
            var s = "true";
            var r = BoolParser.Parse(s.AsSpan());
            Assert.True(r);
        }

        [Fact]
        public void BoolParseSpan_False()
        {
            var s = "false";
            var r = BoolParser.Parse(s.AsSpan());
            Assert.False(r);
        }

        [Fact]
        public void BoolParseSpan_TrueWithWhiteSpace()
        {
            var s = " true ";
            var r = BoolParser.Parse(s.AsSpan());
            Assert.True(r);
        }

        [Fact]
        public void BoolParseSpan_FalseWithWhiteSpace()
        {
            var s = " false ";
            var r = BoolParser.Parse(s.AsSpan());
            Assert.False(r);
        }

        [Fact]
        public void BoolParse_TrueWithMiddleWhiteSpace()
        {
            var s = " t r u e ";
            bool.TryParse(s, out var r);
            Assert.False(r);
        }

        [Fact]
        public void BoolParseSpan_TrueWithMiddleWhiteSpace()
        {
            var s = " t r u e ";
            var r = BoolParser.Parse(s.AsSpan());
            Assert.False(r);
        }
    }
}
