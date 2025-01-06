using FluentAssertions;
using System.Linq.Expressions;

namespace LessCode.EFCore.StronglyTypedIdGenerator.UnitTests.CodeAnalysisHelperTests
{
    public class SupportsBinaryOperatorShould
    {
        [Theory]
        [InlineData(typeof(int), ExpressionType.Equal, true)]
        [InlineData(typeof(int), ExpressionType.NotEqual, true)]
        [InlineData(typeof(int), ExpressionType.LessThan, true)]
        [InlineData(typeof(int), ExpressionType.GreaterThan, true)]
        [InlineData(typeof(int), ExpressionType.LessThanOrEqual, true)]
        [InlineData(typeof(int), ExpressionType.GreaterThanOrEqual, true)]
        [InlineData(typeof(uint), ExpressionType.Equal, true)]
        [InlineData(typeof(uint), ExpressionType.NotEqual, true)]
        [InlineData(typeof(uint), ExpressionType.LessThan, true)]
        [InlineData(typeof(uint), ExpressionType.GreaterThan, true)]
        [InlineData(typeof(uint), ExpressionType.LessThanOrEqual, true)]
        [InlineData(typeof(uint), ExpressionType.GreaterThanOrEqual, true)]
        [InlineData(typeof(string), ExpressionType.Equal, true)]
        [InlineData(typeof(string), ExpressionType.NotEqual, true)]
        [InlineData(typeof(string), ExpressionType.LessThan, false)]
        [InlineData(typeof(string), ExpressionType.GreaterThan, false)]
        public void ReturnExpected_ForNumericType(Type type, ExpressionType operation, bool expectedReturnValue)
        {
            var actual = CodeAnalysisHelper.SupportsBinaryOperator(type, operation);
            actual.Should().Be(expectedReturnValue);
        }
    }
}
