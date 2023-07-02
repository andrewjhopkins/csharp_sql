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
    }
}