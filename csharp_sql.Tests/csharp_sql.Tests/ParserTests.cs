using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_sql.Tests
{
    public class ParserTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(TokenType.Numeric, "123")]
        [TestCase(TokenType.Identifier, "id")]
        [TestCase(TokenType.String, "'string'")]
        public void ParserTest_ParseExpression_ReturnsTrueOkIfExpressionTokenType(TokenType tokenType, string value)
        { 
            var token = new Token { TokenType = tokenType, Value = value, Location = new Location { Column = 1, Row = 1 } };

            var parser = new Parser();
            var parseExpressionResponse = parser.ParseExpression(new[] { token }, 0);

            Assert.IsTrue(parseExpressionResponse.Ok);
            Assert.IsTrue(parseExpressionResponse.Expression.TokenLiteral.TokenType == token.TokenType);
            Assert.IsTrue(parseExpressionResponse.Expression.TokenLiteral.Value == token.Value);
            Assert.IsTrue(parseExpressionResponse.Expression.TokenLiteral.Location == token.Location);

            Assert.IsTrue(parseExpressionResponse.NextCursor == 1);
        }

        [TestCase(TokenType.Int)]
        [TestCase(TokenType.Semicolon)]
        [TestCase(TokenType.Into)]
        [TestCase(TokenType.Comma)]
        [TestCase(TokenType.Asterisk)]
        [TestCase(TokenType.Create)]
        [TestCase(TokenType.Insert)]
        public void ParserTest_ParseExpression_ReturnsFalseOkIfNotExpressionTokenType(TokenType tokenType)
        { 
            var token = new Token { TokenType = tokenType, Value = "", Location = new Location { Column = 1, Row = 1 } };

            var parser = new Parser();
            var parseExpressionResponse = parser.ParseExpression(new[] { token }, 0);

            Assert.IsTrue(parseExpressionResponse.Ok == false);
        }
    }
}
