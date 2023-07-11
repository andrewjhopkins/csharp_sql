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
            var expectedToken = new Token { Location = new Location { Column = 0, Row = 0 }, TokenType = Helper.SymbolToTokenType(source[0]), Value = $"{source}" };

            var lexer = new Lexer(source);
            var tokens = lexer.Lex();

            Assert.That(tokens.Count(), Is.EqualTo(1));
            var actualToken = tokens.ElementAt(0);

            Assert.That(expectedToken.TokenType, Is.EqualTo(actualToken.TokenType));
            Assert.That(expectedToken.Value, Is.EqualTo(actualToken.Value));
            Assert.That(expectedToken.Location.Column, Is.EqualTo(actualToken.Location.Column));
            Assert.That(expectedToken.Location.Row, Is.EqualTo(actualToken.Location.Row));
        }

        [TestCase("''")]
        [TestCase("'string'")]
        [TestCase("'s t r i n g'")]
        [TestCase("'test ing'")]
        [TestCase("'*dje*$%@#$'")]
        public void LexTest_TestLexingString_ReturnsExpectedTokenWithStringType(string source)
        {
            var expectedToken = new Token { Location = new Location { Column = 0, Row = 0 }, TokenType = TokenType.String, Value = $"{source}" };

            var lexer = new Lexer(source);
            var tokens = lexer.Lex();

            Assert.That(tokens.Count(), Is.EqualTo(1));
            var actualToken = tokens.ElementAt(0);

            Assert.That(expectedToken.TokenType, Is.EqualTo(actualToken.TokenType));
            Assert.That(expectedToken.Value, Is.EqualTo(actualToken.Value));
            Assert.That(expectedToken.Location.Column, Is.EqualTo(actualToken.Location.Column));
            Assert.That(expectedToken.Location.Row, Is.EqualTo(actualToken.Location.Row));
        }

        [TestCase("'testing")]
        [TestCase("'testing' 'testing")]
        public void LexTest_TestLexingStrings_ReturnsNoTokensIfMatchingQuoteNotFound(string source)
        { 
            var lexer = new Lexer(source);
            var tokens = lexer.Lex();

            Assert.That(tokens.Count(), Is.EqualTo(0));
        }

        [TestCase("select")]
        [TestCase("from")]
        [TestCase("as")]
        [TestCase("table")]
        [TestCase("create")]
        [TestCase("insert")]
        [TestCase("into")]
        [TestCase("values")]
        [TestCase("int")]
        [TestCase("text")]
        public void LexTest_TestLexingKeywords_ReturnsTokenWithKeywordType(string source)
        { 
            var lexer = new Lexer(source);
            var tokens = lexer.Lex();

            var expectedToken = new Token { Location = new Location { Column = 0, Row = 0 }, TokenType = TokenType.Keyword, Value = $"{source}" };
            Assert.That(tokens.Count(), Is.EqualTo(1));

            var actualToken = tokens.ElementAt(0);

            Assert.That(expectedToken.TokenType, Is.EqualTo(actualToken.TokenType));
            Assert.That(expectedToken.Value, Is.EqualTo(actualToken.Value));
            Assert.That(expectedToken.Location.Column, Is.EqualTo(actualToken.Location.Column));
            Assert.That(expectedToken.Location.Row, Is.EqualTo(actualToken.Location.Row));
        }

        [TestCase("423")]
        [TestCase("123")]
        [TestCase("12")]
        public void LexTest_TestLexingNumeric_ReturnsTokenWithNumerType(string source)
        {
            var lexer = new Lexer(source);
            var tokens = lexer.Lex();

            var expectedToken = new Token { Location = new Location { Column = 0, Row = 0 }, TokenType = TokenType.Numeric, Value = $"{source}" };
            Assert.That(tokens.Count(), Is.EqualTo(1));
            var actualToken = tokens.ElementAt(0);

            Assert.That(expectedToken.TokenType, Is.EqualTo(actualToken.TokenType));
            Assert.That(expectedToken.Value, Is.EqualTo(actualToken.Value));
            Assert.That(expectedToken.Location.Column, Is.EqualTo(actualToken.Location.Column));
            Assert.That(expectedToken.Location.Row, Is.EqualTo(actualToken.Location.Row));
        }

        public static object[] TestLexingStringsCases =
        {
            new object[] 
            {
                "'test' 'test2'",
                new List<Token>
                {
                    new Token { Location = new Location{ Column = 0, Row = 0 }, TokenType = TokenType.String, Value = "'test'" },
                    new Token { Location = new Location{ Column = 7, Row = 0 }, TokenType = TokenType.String, Value = "'test2'" },
                }
            },
            new object[] 
            { 
                "'test tes testing'  'more testing'  'evenmore testing'  '' ",
                new List<Token>
                { 
                    new Token { Location = new Location{ Column = 0, Row = 0 }, TokenType = TokenType.String, Value = "'test tes testing'" },
                    new Token { Location = new Location{ Column = 20, Row = 0 }, TokenType = TokenType.String, Value = "'more testing'" },
                    new Token { Location = new Location{ Column = 36, Row = 0 }, TokenType = TokenType.String, Value = "'evenmore testing'" },
                    new Token { Location = new Location{ Column = 56, Row = 0 }, TokenType = TokenType.String, Value = "''" },
                }
            },
            new object[] 
            { 
                "'testing'\n'testing2'",
                new List<Token>
                { 
                    new Token { Location = new Location{ Column = 0, Row = 0 }, TokenType = TokenType.String, Value = "'testing'" },
                    new Token { Location = new Location{ Column = 0, Row = 1 }, TokenType = TokenType.String, Value = "'testing2'" },
                }
            },
            new object[] 
            { 
                "SELECT id FROM Users;",
                new List<Token>
                { 
                    new Token { Location = new Location { Column = 0, Row = 0}, TokenType = TokenType.Keyword, Value = "SELECT" },
                    new Token { Location = new Location { Column = 7, Row = 0}, TokenType = TokenType.Identifier, Value = "id" },
                    new Token { Location = new Location { Column = 10, Row = 0}, TokenType = TokenType.Keyword, Value = "FROM" },
                    new Token { Location = new Location { Column = 15, Row = 0}, TokenType = TokenType.Identifier, Value = "Users" },
                    new Token { Location = new Location { Column = 20, Row = 0}, TokenType = TokenType.Semicolon, Value = ";" },
                }
            },
            new object[] 
            { 
                "InSeRt  INTO earnings (quarter, revenue, cogs) VALUES ('Q42023', 13241, 89302);",
                new List<Token>
                { 
                    new Token { Location = new Location { Column = 0, Row = 0}, TokenType = TokenType.Keyword, Value = "InSeRt" },
                    new Token { Location = new Location { Column = 8, Row = 0}, TokenType = TokenType.Keyword, Value = "INTO" },
                    new Token { Location = new Location { Column = 13, Row = 0}, TokenType = TokenType.Identifier, Value = "earnings" },
                    new Token { Location = new Location { Column = 22, Row = 0}, TokenType = TokenType.LeftParen, Value = "(" },
                    new Token { Location = new Location { Column = 23, Row = 0}, TokenType = TokenType.Identifier, Value = "quarter" },
                    new Token { Location = new Location { Column = 30, Row = 0}, TokenType = TokenType.Comma, Value = "," },
                    new Token { Location = new Location { Column = 32, Row = 0}, TokenType = TokenType.Identifier, Value = "revenue" },
                    new Token { Location = new Location { Column = 39, Row = 0}, TokenType = TokenType.Comma, Value = "," },
                    new Token { Location = new Location { Column = 41, Row = 0}, TokenType = TokenType.Identifier, Value = "cogs" },
                    new Token { Location = new Location { Column = 45, Row = 0}, TokenType = TokenType.RightParen, Value = ")" },
                    new Token { Location = new Location { Column = 47, Row = 0}, TokenType = TokenType.Keyword, Value = "VALUES" },
                    new Token { Location = new Location { Column = 54, Row = 0}, TokenType = TokenType.LeftParen, Value = "(" },
                    new Token { Location = new Location { Column = 55, Row = 0}, TokenType = TokenType.String, Value = "'Q42023'" },
                    new Token { Location = new Location { Column = 63, Row = 0}, TokenType = TokenType.Comma, Value = "," },
                    new Token { Location = new Location { Column = 65, Row = 0}, TokenType = TokenType.Numeric, Value = "13241" },
                    new Token { Location = new Location { Column = 70, Row = 0}, TokenType = TokenType.Comma, Value = "," },
                    new Token { Location = new Location { Column = 72, Row = 0}, TokenType = TokenType.Numeric, Value = "89302" },
                    new Token { Location = new Location { Column = 77, Row = 0}, TokenType = TokenType.RightParen, Value = ")" },
                    new Token { Location = new Location { Column = 78, Row = 0}, TokenType = TokenType.Semicolon, Value = ";" },
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
    }
}