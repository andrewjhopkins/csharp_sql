namespace csharp_sql.Tests
{
    public class ParserTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParserTest_ParseInsertStatement_ReturnsExpectedInsertStatementResponse()
        { 
            var values = new[] { "123", "username", "'password'" };

            var tokenTypes = new[] { TokenType.Numeric, TokenType.Identifier, TokenType.String };

            var tokens = new[] {
                new Token { TokenType = TokenType.Insert, Value = "INSERT", Location = new Location {  Column = 1, Row = 1} },
                new Token { TokenType = TokenType.Into, Value = "INTO", Location = new Location {  Column = 1, Row = 1} },

                new Token { TokenType = TokenType.Identifier, Value = "users", Location = new Location {  Column = 1, Row = 1} },

                new Token { TokenType = TokenType.Values, Value = "VALUES", Location = new Location {  Column = 1, Row = 1} },

                new Token { TokenType = TokenType.LeftParen, Value = "(", Location = new Location {  Column = 1, Row = 1} },

                new Token { TokenType = tokenTypes[0], Value = values[0], Location = new Location {  Column = 1, Row = 1} },
                new Token { TokenType = TokenType.Comma, Value = ",", Location = new Location {  Column = 1, Row = 1} },

                new Token { TokenType = tokenTypes[1], Value = values[1], Location = new Location {  Column = 1, Row = 1} },
                new Token { TokenType = TokenType.Comma, Value = ",", Location = new Location {  Column = 1, Row = 1} },

                new Token { TokenType = tokenTypes[2], Value = values[2], Location = new Location {  Column = 1, Row = 1} },

                new Token { TokenType = TokenType.RightParen, Value = ")", Location = new Location {  Column = 1, Row = 1} },
            };

            var parser = new Parser();
            var parseInsertStatementResponse = parser.ParseInsertStatement(tokens, 0);

            Assert.IsTrue(parseInsertStatementResponse.Ok);
            Assert.IsTrue(parseInsertStatementResponse.NextCursor == tokens.Count());
            Assert.IsTrue(parseInsertStatementResponse.InsertStatement.Values.Count() == 3);

            for (var i = 0; i < values.Count(); i++)
            { 
                parseInsertStatementResponse.InsertStatement.Values.ElementAt(i).TokenLiteral.Value = values[i];
                parseInsertStatementResponse.InsertStatement.Values.ElementAt(i).TokenLiteral.TokenType = tokenTypes[i];
            }

        }

        [Test]
        public void ParserTest_ParseSelectStatement_ReturnsExpectedSelectStatementResponse()
        { 
            var values = new[] { "id", "username", "password" };

            var tokens = new[] {
                new Token { TokenType = TokenType.Select, Value = "SELECT", Location = new Location { Column = 0, Row = 1 } },

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

                new Token { TokenType = TokenType.From, Value = "FROM", Location = new Location { Column = 12, Row = 1 } },
                new Token { TokenType = TokenType.Identifier, Value = "Users", Location = new Location { Column = 13, Row = 1 } },
            };

            var parser = new Parser();
            var selectStatementResponse = parser.ParseSelectStatement(tokens, 0);

            Assert.IsTrue(selectStatementResponse.Ok);
            Assert.IsTrue(selectStatementResponse.SelectStatement.Items.Count() == values.Count());

            for (var i = 0; i < selectStatementResponse.SelectStatement.Items.Count(); i++)
            {
                var item = selectStatementResponse.SelectStatement.Items.ElementAt(i);
                Assert.IsTrue(item.Expression.TokenLiteral.TokenType == TokenType.Identifier);
                Assert.IsTrue(item.Expression.TokenLiteral.Value == values[i]);
                Assert.IsTrue(item.Asterisk == false);
                Assert.IsTrue(item.AsToken.TokenType == TokenType.Identifier);
                Assert.IsTrue(item.AsToken.Value == values[i]);
            }

            Assert.IsTrue(selectStatementResponse.SelectStatement.From.Table.TokenType == TokenType.Identifier);
            Assert.IsTrue(selectStatementResponse.SelectStatement.From.Table.Value == "Users");
        }

        [Test]
        public void ParserTest_ParseCreateTableStatement_ReturnsExpectedCreateTableStatementResponse()
        { 
            var values = new[] { "id", "age", "username" };
            var tokenDataTypes = new[] { TokenType.Text, TokenType.Int, TokenType.Text };

            var tokens = new[] {
                new Token { TokenType = TokenType.Create, Value = "CREATE", Location = new Location { Column = 0, Row = 1 } },
                new Token { TokenType = TokenType.Table, Value = "TABLE", Location = new Location { Column = 0, Row = 1 } },

                new Token { TokenType = TokenType.Identifier, Value = "USERS", Location = new Location { Column = 0, Row = 1 } },

                new Token { TokenType = TokenType.LeftParen, Value = "(", Location = new Location { Column = 0, Row = 1 } },

                new Token { TokenType = TokenType.Identifier, Value = values[0], Location = new Location { Column = 0, Row = 1 } },
                new Token { TokenType = tokenDataTypes[0], Value = "text", Location = new Location { Column = 1, Row = 1 } },
                new Token { TokenType = TokenType.Comma, Value = ",", Location = new Location { Column = 2, Row = 1 } },

                new Token { TokenType = TokenType.Identifier, Value = values[1], Location = new Location { Column = 3, Row = 1 } },
                new Token { TokenType = tokenDataTypes[1], Value = "int", Location = new Location { Column = 4, Row = 1 } },
                new Token { TokenType = TokenType.Comma, Value = ",", Location = new Location { Column = 5, Row = 1 } },

                new Token { TokenType = TokenType.Identifier, Value = values[2], Location = new Location { Column = 6, Row = 1 } },
                new Token { TokenType = tokenDataTypes[2], Value = "text", Location = new Location { Column = 7, Row = 1 } },

                new Token { TokenType = TokenType.RightParen, Value = ")", Location = new Location { Column = 0, Row = 1 } },
            };

            var parser = new Parser();
            var createTableStatementResponse = parser.ParseCreateTableStatement(tokens, 0);

            Assert.IsTrue(createTableStatementResponse.Ok);

            Assert.IsTrue(createTableStatementResponse.CreateTableStatement.Name.TokenType == TokenType.Identifier);
            Assert.IsTrue(createTableStatementResponse.CreateTableStatement.Name.Value == "USERS");

            var columnDefinitions = createTableStatementResponse.CreateTableStatement.Columns;

            Assert.IsTrue(columnDefinitions.Count() == values.Count());

            for (var i = 0; i < columnDefinitions.Count(); i++)
            {
                Assert.IsTrue(columnDefinitions.ElementAt(i).Name.Value == values[i]);
                Assert.IsTrue(columnDefinitions.ElementAt(i).Name.TokenType == TokenType.Identifier);
                Assert.IsTrue(columnDefinitions.ElementAt(i).DataType.TokenType == tokenDataTypes[i]);
            }
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
        public void ParserTest_ParseSelectItems_ReturnsExpectedSelectItemsResponseWithTrueAsterisk()
        {
            var tokens = new[] {
                new Token { TokenType = TokenType.Asterisk, Value = "*", Location = new Location { Column = 1, Row = 1 } },
            };

            var parser = new Parser();
            var selectItemsResponse = parser.ParseSelectItems(tokens, 0);

            Assert.IsTrue(selectItemsResponse.Ok == true);
            Assert.IsTrue(selectItemsResponse.SelectItems.Count() == 1);

            var item = selectItemsResponse.SelectItems.ElementAt(0);
            Assert.IsTrue(item.Asterisk == true);
            Assert.IsTrue(item.Expression == null);
            Assert.IsTrue(item.AsToken == null);
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
                Assert.IsTrue(item.AsToken.TokenType == TokenType.Identifier);
                Assert.IsTrue(item.AsToken.Value == values[i]);
            }
        }

        [TestCase]
        public void ParserTest_ParseColumnDefinitions_ReturnsExpectedColumnDefinitions()
        {
            var values = new[] { "id", "age", "username" };
            var tokenDataTypes = new[] { TokenType.Text, TokenType.Int, TokenType.Text };

            var tokens = new[] {
                new Token { TokenType = TokenType.Identifier, Value = values[0], Location = new Location { Column = 0, Row = 1 } },
                new Token { TokenType = tokenDataTypes[0], Value = "text", Location = new Location { Column = 1, Row = 1 } },
                new Token { TokenType = TokenType.Comma, Value = ",", Location = new Location { Column = 2, Row = 1 } },

                new Token { TokenType = TokenType.Identifier, Value = values[1], Location = new Location { Column = 3, Row = 1 } },
                new Token { TokenType = tokenDataTypes[1], Value = "int", Location = new Location { Column = 4, Row = 1 } },
                new Token { TokenType = TokenType.Comma, Value = ",", Location = new Location { Column = 5, Row = 1 } },

                new Token { TokenType = TokenType.Identifier, Value = values[2], Location = new Location { Column = 6, Row = 1 } },
                new Token { TokenType = tokenDataTypes[2], Value = "text", Location = new Location { Column = 7, Row = 1 } },
            };

            var parser = new Parser();
            var columnDefinitions = parser.ParseColumnDefinitions(tokens, 0);

            Assert.IsTrue(columnDefinitions.ColumnDefinitions.Count() == values.Count());

            for (var i = 0; i < columnDefinitions.ColumnDefinitions.Count(); i++)
            {
                Assert.IsTrue(columnDefinitions.ColumnDefinitions.ElementAt(i).Name.Value == values[i]);
                Assert.IsTrue(columnDefinitions.ColumnDefinitions.ElementAt(i).Name.TokenType == TokenType.Identifier);
                Assert.IsTrue(columnDefinitions.ColumnDefinitions.ElementAt(i).DataType.TokenType == tokenDataTypes[i]);
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

        
        [Test]
        public void ParserTest_ParseExpressions_ReturnsExpectedExpressionsResponse()
        {
            var values = new[] { "123", "username", "'password'" };
            var tokenTypes = new[] { TokenType.Numeric, TokenType.Identifier, TokenType.String };

            var tokens = new[] {
                new Token { TokenType = tokenTypes[0], Value = values[0], Location = new Location {  Column = 1, Row = 1} },
                new Token { TokenType = TokenType.Comma, Value = ",", Location = new Location {  Column = 1, Row = 1} },

                new Token { TokenType = tokenTypes[1], Value = values[1], Location = new Location {  Column = 1, Row = 1} },
                new Token { TokenType = TokenType.Comma, Value = ",", Location = new Location {  Column = 1, Row = 1} },

                new Token { TokenType = tokenTypes[2], Value = values[2], Location = new Location {  Column = 1, Row = 1} },
            };

            var parser = new Parser();
            var parseExpressionsResponse = parser.ParseExpressions(tokens, 0);

            Assert.IsTrue(parseExpressionsResponse.Ok);
            Assert.IsTrue(parseExpressionsResponse.NextCursor == tokens.Count());
            Assert.IsTrue(parseExpressionsResponse.Expressions.Count() == 3);

            for (var i = 0; i < values.Count(); i++)
            { 
                parseExpressionsResponse.Expressions.ElementAt(i).TokenLiteral.Value = values[i];
            }
        }
    }
}
