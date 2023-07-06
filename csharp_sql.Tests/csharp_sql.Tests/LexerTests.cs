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
        public void LexTest_TestLexingSymbols_ReturnsExpectedTokenWithSymbolType(string source)
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

        [TestCase("''")]
        [TestCase("'string'")]
        [TestCase("'s t r i n g'")]
        [TestCase("'test ing'")]
        [TestCase("'*dje*$%@#$'")]
        public void LexTest_TestLexingString_ReturnsExpectedTokenWithStringType(string source)
        {
            var expectedTokens = new List<Token> {
                    new Token { Location = new Location{ Column = 0, Row = 0 }, TokenType = TokenType.String, Value = $"{source}" }
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

        public static object[] TestLexingStringsCases =
        {
            /*
            new object[] 
            {
                "'test' 'test2'",
                new List<Token>
                {
                    new Token { Location = new Location{ Column = 0, Row = 0 }, TokenType = TokenType.String, Value = "'test'" },
                    new Token { Location = new Location{ Column = 6, Row = 0 }, TokenType = TokenType.String, Value = "'test2'" },
                }
            },
            new object[] 
            { 
                "'test tes testing'  'more testing'  'evenmore testing'  '' ",
                new List<Token>
                { 
                    new Token { Location = new Location{ Column = 0, Row = 0 }, TokenType = TokenType.String, Value = "'test tes testing'" },
                    new Token { Location = new Location{ Column = 19, Row = 0 }, TokenType = TokenType.String, Value = "'more testing'" },
                    new Token { Location = new Location{ Column = 35, Row = 0 }, TokenType = TokenType.String, Value = "'evenmore testing'" },
                    new Token { Location = new Location{ Column = 55, Row = 0 }, TokenType = TokenType.String, Value = "''" },
                }
            },
            */
            new object[] 
            { 
                "'testing'\n'testing2'",
                new List<Token>
                { 
                    new Token { Location = new Location{ Column = 0, Row = 0 }, TokenType = TokenType.String, Value = "'testing'" },
                    new Token { Location = new Location{ Column = 0, Row = 1 }, TokenType = TokenType.String, Value = "'testing2'" },
                }
            }
        };

        [TestCaseSource(nameof(TestLexingStringsCases))]
        public void LexTest_TestLexingStrings_ReturnsExpectedTokensWithStringType(string source, IEnumerable<Token> expectedTokens)
        { 
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

        [TestCase("'testing")]
        //[TestCase("'testing' 'testing")]
        public void LexTest_TestLexingStrings_ReturnsNoTokensIfMatchingQuoteNotFound(string source)
        { 
            var lexer = new Lexer(source);
            var tokens = lexer.Lex();

            Assert.That(tokens.Count(), Is.EqualTo(0));
        }
    }
}