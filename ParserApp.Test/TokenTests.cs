using NUnit.Framework.Constraints;
using ParserApp;

namespace ParserApp.Test
{
    [TestFixture]
    public class TokenTests
    {
        private static IEnumerable<TestCaseData> ValidExpressionTokens()
        {
            yield return new TestCaseData(new Token("d1", Token.TokenType.Expression), Is.EqualTo(1));
            yield return new TestCaseData(new Token("12d1", Token.TokenType.Expression), Is.EqualTo(12));
            yield return new TestCaseData(new Token("1D6", Token.TokenType.Expression), Is.InRange(1, 6));
            yield return new TestCaseData(new Token("D6", Token.TokenType.Expression), Is.InRange(1, 6));
        }

        private static IEnumerable<Token> InvalidExpressionTokens()
        {
            yield return new Token("1", Token.TokenType.Expression);
            yield return new Token("12d", Token.TokenType.Expression);
        }

        private static IEnumerable<TestCaseData> ValidValueTokens()
        {
            yield return new TestCaseData(new Token("1", Token.TokenType.Value), Is.EqualTo(1));
        }

        private static IEnumerable<Token> InvalidTokenToCallToValue()
        {
            yield return new Token("", Token.TokenType.Eof);
            yield return new Token("+", Token.TokenType.Operation);
        }

        [TestCaseSource(nameof(ValidExpressionTokens))]
        public void ToValue_ValidExpression_ReturnsInt(Token token, Constraint constraint)
        {
            Assert.That(token.ToValue(), constraint);
        }

        [TestCaseSource(nameof(InvalidExpressionTokens))]
        public void ToValue_InvalidExpression_ThrowsException(Token token)
        {
            Assert.That(() => token.ToValue(), Throws.Exception);
        }

        [TestCaseSource(nameof(ValidValueTokens))]
        public void ToValue_ValidValueToken_ReturnsInt(Token token, Constraint constraint)
        {
            Assert.That(token.ToValue(), constraint);
        }

        [TestCaseSource(nameof(InvalidTokenToCallToValue))]
        public void ToValue_InvalidToken_ThrowsException(Token token)
        {
            Assert.That(() => token.ToValue(), Throws.Exception);
        }
    }
}