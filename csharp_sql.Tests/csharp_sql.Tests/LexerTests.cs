using csharp_sql;

namespace csharp_sql.Tests
{
    public class LexerTests 
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("")]
        [TestCase(null)]
        public void LexTest_SourceNullorEmpty_ReturnsNoTokens(string source)
        {
            var lexer = new Lexer(source);
            var tokens = lexer.Lex();

            Assert.IsNotNull(tokens);
            Assert.IsTrue(tokens.Count() == 0);
        }

        [TestCase(",")]
        [TestCase("(")]
        [TestCase(")")]
        [TestCase(";")]
        public void LexTest_TestLexingSymbols_ReturnsTokenWithSymbolType(string source)
        {
            var expectedTokens = new List<Token> {
                    new Token { Location = new Location{ Column = 0, Row = 0 }, TokenType = TokenType.Symbol, Value = $"{source}" }
                };

            var lexer = new Lexer(source);
            var tokens = lexer.Lex();

            Assert.That(tokens.Count(), Is.EqualTo(expectedTokens.Count()));
            for (var i = 0; i < expectedTokens.Count(); i++) 
            {
                var expectedToken = expectedTokens.ElementAt(i);
                var actualToken = tokens.ElementAt(i);

                Assert.That(expectedToken.TokenType, Is.EqualTo(actualToken.TokenType));
                Assert.That(expectedToken.Value, Is.EqualTo(actualToken.Value));
                Assert.That(expectedToken.Location.Column, Is.EqualTo(actualToken.Location.Column));
                Assert.That(expectedToken.Location.Row, Is.EqualTo(actualToken.Location.Row));
            }
        }
    }
}