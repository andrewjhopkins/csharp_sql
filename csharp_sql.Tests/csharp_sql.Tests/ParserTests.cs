namespace csharp_sql.Tests
{
    public class ParserTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParserTest_ParseSelectItems_ReturnsExpectedSelectItemsResponse()
        {
            var values = new[] { "id", "username", "password" };

            var tokens = new[] {
                new Token { TokenType = TokenType.Identifier, Value = values[0], Location = new Location { Column = 1, Row = 1 } },
                new Token { TokenType = TokenType.Comma, Value = ",", Location = new Location { Column = 2, Row = 1 } },

                new Token { TokenType = TokenType.Identifier, Value = values[1], Location = new Location { Column = 3, Row = 1 } },
                new Token { TokenType = TokenType.Comma, Value = ",", Location = new Location { Column = 4, Row = 1} },

                new Token { TokenType = TokenType.Identifier, Value = values[2], Location = new Location { Column = 5, Row = 1 } },
            };

            var parser = new Parser();
            var selectItemsResponse = parser.ParseSelectItems(tokens, 0);

            Assert.IsTrue(selectItemsResponse.Ok == true);
            Assert.IsTrue(selectItemsResponse.SelectItems.Count() == 3);

            for (var i = 0; i < selectItemsResponse.SelectItems.Count(); i++)
            {
                var item = selectItemsResponse.SelectItems.ElementAt(i);
                Assert.IsTrue(item.Expression.TokenLiteral.TokenType == TokenType.Identifier);
                Assert.IsTrue(item.Expression.TokenLiteral.Value == values[i]);
                Assert.IsTrue(item.Asterisk == false);
                Assert.IsTrue(item.AsToken == null);
            }
        }

        [Test]
        public void ParserTest_ParseSelectItems_ReturnsExpectedSelectItemsResponseWithAsIdentifier()
        {
            var values = new[] { "id", "username", "password" };

            var tokens = new[] {
                new Token { TokenType = TokenType.Identifier, Value = values[0], Location = new Location { Column = 1, Row = 1 } },
                new Token { TokenType = TokenType.As, Value = values[0], Location = new Location { Column = 2, Row = 1 } },
                new Token { TokenType = TokenType.Identifier, Value = values[0], Location = new Location { Column = 3, Row = 1 } },
                new Token { TokenType = TokenType.Comma, Value = ",", Location = new Location { Column = 4, Row = 1 } },

                new Token { TokenType = TokenType.Identifier, Value = values[1], Location = new Location { Column = 5, Row = 1 } },
                new Token { TokenType = TokenType.As, Value = values[1], Location = new Location { Column = 6, Row = 1 } },
                new Token { TokenType = TokenType.Identifier, Value = values[1], Location = new Location { Column = 7, Row = 1 } },
                new Token { TokenType = TokenType.Comma, Value = ",", Location = new Location { Column = 8, Row = 1} },

                new Token { TokenType = TokenType.Identifier, Value = values[2], Location = new Location { Column = 9, Row = 1 } },
                new Token { TokenType = TokenType.As, Value = values[2], Location = new Location { Column = 10, Row = 1 } },
                new Token { TokenType = TokenType.Identifier, Value = values[2], Location = new Location { Column = 11, Row = 1 } },
            };

            var parser = new Parser();
            var selectItemsResponse = parser.ParseSelectItems(tokens, 0);

            Assert.IsTrue(selectItemsResponse.Ok == true);
            Assert.IsTrue(selectItemsResponse.SelectItems.Count() == 3);

            for (var i = 0; i < selectItemsResponse.SelectItems.Count(); i++)
            {
                var item = selectItemsResponse.SelectItems.ElementAt(i);
                Assert.IsTrue(item.Expression.TokenLiteral.TokenType == TokenType.Identifier);
                Assert.IsTrue(item.Expression.TokenLiteral.Value == values[i]);
                Assert.IsTrue(item.Asterisk == false);
                Assert.IsTrue(item.AsToken == null);
            }
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
