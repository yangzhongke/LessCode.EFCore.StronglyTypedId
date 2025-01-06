using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessCode.EFCore.StronglyTypedIdGenerator.UnitTests.CodeAnalysisHelperTests
{
    public class ResolveTypeWithRoslynShould
    {
        [Theory]
        [InlineData("int", typeof(int))]
        [InlineData("string", typeof(string))]
        [InlineData("long", typeof(long))]
        [InlineData("short", typeof(short))]
        [InlineData("byte", typeof(byte))]
        [InlineData("uint", typeof(uint))]
        [InlineData("ulong", typeof(ulong))]
        [InlineData("ushort", typeof(ushort))]
        [InlineData("sbyte", typeof(sbyte))]
        [InlineData("float", typeof(float))]
        [InlineData("double", typeof(double))]
        [InlineData("decimal", typeof(decimal))]
        [InlineData("char", typeof(char))]
        [InlineData("bool", typeof(bool))]
        [InlineData("object", typeof(object))]
        [InlineData("System.Int32", typeof(int))]
        [InlineData("System.String", typeof(string))]
        [InlineData("System.Int64", typeof(long))]
        [InlineData("System.Int16", typeof(short))]
        [InlineData("System.Byte", typeof(byte))]
        [InlineData("System.UInt32", typeof(uint))]
        [InlineData("System.UInt64", typeof(ulong))]
        [InlineData("System.UInt16", typeof(ushort))]
        [InlineData("System.SByte", typeof(sbyte))]
        [InlineData("System.Single", typeof(float))]
        [InlineData("System.Double", typeof(double))]
        [InlineData("System.Decimal", typeof(decimal))]
        [InlineData("System.Char", typeof(char))]
        [InlineData("System.Boolean", typeof(bool))]
        [InlineData("System.Object", typeof(object))]
        public void ResolveType_Correctly(string typeName, Type expectedType)
        {
            var type = CodeAnalysisHelper.ResolveTypeFromName(typeName);
            type.Should().Be(expectedType);
        }
    }
}
