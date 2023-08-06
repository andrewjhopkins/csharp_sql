using csharp_sql.Statements;

namespace csharp_sql
{
    public class Parser
    {
        public IEnumerable<IStatement> Parse(IEnumerable<Token> tokens)
        {
            if (tokens.Count() == 0)
            {
                throw new Exception("No tokens to parse");
            }

            // append semicolon if needed
            if (tokens.Last().TokenType != TokenType.Semicolon)
            {
                tokens.Concat(new Token[] { new Token { TokenType = TokenType.Semicolon, Value = ";" } });
            }

            var ast = new List<IStatement>();

            var cursor = 0;

            while (cursor < tokens.Count()) 
            {
                if (tokens.ElementAt(cursor).TokenType == TokenType.Semicolon)
                {
                    return ast;
                }

                var parseStatementResponse = ParseStatement(tokens, cursor);
                ast.Add(parseStatementResponse.Statement);
                cursor = parseStatementResponse.NextCursor;
            }

            return ast;
        }

        public ParseStatementResponse ParseStatement(IEnumerable<Token> tokens, int initialCursor)
        {
            var cursor = initialCursor;

            switch (tokens.ElementAt(cursor).TokenType)
            {
                case TokenType.Select:
                {
                    var parseSelectResponse = ParseSelectStatement(tokens, cursor);
                    return new ParseStatementResponse
                    {
                        Statement = parseSelectResponse.SelectStatement,
                        NextCursor = parseSelectResponse.NextCursor
                    };
                }
                case TokenType.Insert:
                { 
                    var parseInsertReponse = ParseInsertStatement(tokens, cursor);
                    return new ParseStatementResponse
                    {
                        Statement = parseInsertReponse.InsertStatement,
                        NextCursor = parseInsertReponse.NextCursor
                    };
                }
                case TokenType.Create:
                {
                    var parseCreateTableResponse = ParseCreateTableStatement(tokens, cursor);

                    return new ParseStatementResponse 
                    {
                        Statement = parseCreateTableResponse.CreateTableStatement,
                        NextCursor = parseCreateTableResponse.NextCursor
                    };
                }
                default:
                    throw new Exception("Could not parse statement");
            }
        }

        public ParseSelectStatementResponse ParseSelectStatement(IEnumerable<Token> tokens, int initialCursor)
        {
            var cursor = initialCursor;
            if (tokens.ElementAt(cursor).TokenType != TokenType.Select) 
            {
                return new ParseSelectStatementResponse
                {
                    Ok = false,
                    NextCursor = cursor
                };
            }

            cursor += 1;

            var selectStatement = new SelectStatement();

            var parseSelectItemsResponse = ParseSelectItems(tokens, cursor);

            selectStatement.Items = parseSelectItemsResponse.SelectItems;
            cursor = parseSelectItemsResponse.NextCursor;

            ExpectTokenType(tokens.ElementAt(cursor), TokenType.From);

            cursor += 1;

            var identifierToken = tokens.ElementAt(cursor);

            ExpectTokenType(identifierToken, TokenType.Identifier);

            selectStatement.From = new FromItem { Table = identifierToken };

            return new ParseSelectStatementResponse
            {
                SelectStatement = selectStatement,
                Ok = true,
                NextCursor = cursor + 1
            };
        }

        public ParseInsertStatementResponse ParseInsertStatement(IEnumerable<Token> tokens, int initialCursor)
        {
            var cursor = initialCursor;

            ExpectTokenType(tokens.ElementAt(cursor), TokenType.Insert);
            cursor += 1;

            ExpectTokenType(tokens.ElementAt(cursor), TokenType.Into);
            cursor += 1;

            var table = tokens.ElementAt(cursor);
            ExpectTokenType(table, TokenType.Identifier);

            cursor += 1;

            ExpectTokenType(tokens.ElementAt(cursor), TokenType.Values);
            cursor += 1;

            ExpectTokenType(tokens.ElementAt(cursor), TokenType.LeftParen);
            cursor += 1;

            var parseExpressionsResponse = ParseExpressions(tokens, cursor);
            if (!parseExpressionsResponse.Ok)
            {
                throw new Exception("Could not parse expressions");
            }

            cursor = parseExpressionsResponse.NextCursor;

            ExpectTokenType(tokens.ElementAt(cursor), TokenType.RightParen);
            cursor += 1;

            var insertStatement = new InsertStatement
            {
                Values = parseExpressionsResponse.Expressions,
                Table = table,
            };

            return new ParseInsertStatementResponse
            {
                InsertStatement = insertStatement,
                Ok = true,
                NextCursor = cursor
            };
        }

        public ParseCreateTableStatementResponse ParseCreateTableStatement(IEnumerable<Token> tokens, int initialCursor)
        {
            var cursor = initialCursor;

            ExpectTokenType(tokens.ElementAt(cursor), TokenType.Create);
            cursor += 1;

            ExpectTokenType(tokens.ElementAt(cursor), TokenType.Table);
            cursor += 1;

            var tableNameToken = tokens.ElementAt(cursor);
            ExpectTokenType(tableNameToken, TokenType.Identifier);
            cursor += 1;

            ExpectTokenType(tokens.ElementAt(cursor), TokenType.LeftParen);
            cursor += 1;

            var parseColumnDefinitionResponse = ParseColumnDefinitions(tokens, cursor);

            cursor = parseColumnDefinitionResponse.NextCursor;
            var columnDefinitions = parseColumnDefinitionResponse.ColumnDefinitions;

            ExpectTokenType(tokens.ElementAt(cursor), TokenType.RightParen);
            cursor += 1;

            var createTableStatement = new CreateTableStatement
            {
                Name = tableNameToken,
                Columns = columnDefinitions
            };

            return new ParseCreateTableStatementResponse
            {
                Ok = true,
                NextCursor = cursor,
                CreateTableStatement = createTableStatement
            };
        }

        public ParseColumnDefinitionResponse ParseColumnDefinitions(IEnumerable<Token> tokens, int initialCursor)
        {
            var cursor = initialCursor;
            var columnDefinitions = new List<ColumnDefinition>();

            while (cursor < tokens.Count() && tokens.ElementAt(cursor).TokenType != TokenType.RightParen)
            {
                if (columnDefinitions.Count() > 0)
                { 
                    var commaToken = tokens.ElementAt(cursor);
                    ExpectTokenType(commaToken, TokenType.Comma);
                    cursor += 1;
                }

                var identifierToken = tokens.ElementAt(cursor);
                ExpectTokenType(identifierToken, TokenType.Identifier);
                cursor += 1;

                var dataTypeToken = tokens.ElementAt(cursor);

                if (dataTypeToken.TokenType != TokenType.Text && dataTypeToken.TokenType != TokenType.Int)
                {
                    throw new Exception($"Expected TEXT or INT loc: {dataTypeToken.Location.Row}:{dataTypeToken.Location.Column}");
                }
                cursor += 1;

                var columnDefinition = new ColumnDefinition
                {
                    DataType = dataTypeToken,
                    Name = identifierToken
                };

                columnDefinitions.Add(columnDefinition);
            }

            return new ParseColumnDefinitionResponse
            {
                ColumnDefinitions = columnDefinitions,
                NextCursor = cursor,
                Ok = true
            };
        }

        public ParseSelectItemsResponse ParseSelectItems(IEnumerable<Token> tokens, int initialCursor)
        {
            var cursor = initialCursor;
            var selectItems = new List<SelectItem>();

            while (true) 
            {
                if (cursor >= tokens.Count() || tokens.ElementAt(cursor).TokenType == TokenType.From)
                {
                    return new ParseSelectItemsResponse
                    {
                        SelectItems = selectItems,
                        NextCursor = cursor,
                        Ok = true,
                    };
                }

                var current = tokens.ElementAt(cursor);

                if (selectItems.Count() > 0)
                {

                    ExpectTokenType(current, TokenType.Comma);
                    cursor += 1;
                }

                var selectItem = new SelectItem();

                current = tokens.ElementAt(cursor);

                if (current.TokenType == TokenType.Asterisk)
                {
                    selectItem.Asterisk = true;
                    cursor += 1;
                }
                else
                {
                    var parseExpressionResponse = ParseExpression(tokens, cursor);
                    if (!parseExpressionResponse.Ok)
                    {
                        throw new Exception($"Expected expression loc: {current.Location.Row}:{current.Location.Column}");
                    }

                    cursor = parseExpressionResponse.NextCursor;
                    selectItem.Expression = parseExpressionResponse.Expression;

                    if (cursor < tokens.Count() && tokens.ElementAt(cursor).TokenType == TokenType.As)
                    {
                        cursor += 1;
                        var identifierToken = tokens.ElementAt(cursor);
                        ExpectTokenType(identifierToken, TokenType.Identifier);

                        selectItem.AsToken = identifierToken;
                        cursor += 1;
                    }
                }

                selectItems.Add(selectItem);
            }
        }

        public ParseExpressionsResponse ParseExpressions(IEnumerable<Token> tokens, int initialCursor)
        {
            var cursor = initialCursor;
            var expressions = new List<Expression>();

            while (cursor < tokens.Count() && tokens.ElementAt(cursor).TokenType != TokenType.RightParen) 
            { 
                var current = tokens.ElementAt(cursor);

                if (expressions.Count() > 0)
                {
                    ExpectTokenType(current, TokenType.Comma);
                    cursor += 1;
                }

                var parseExpressionResponse = ParseExpression(tokens, cursor);
                if (!parseExpressionResponse.Ok)
                {
                    throw new Exception($"Expected expression loc: {current.Location.Row}:{current.Location.Column}");
                }

                cursor = parseExpressionResponse.NextCursor;
                expressions.Add(parseExpressionResponse.Expression);
            }

            return new ParseExpressionsResponse
            {
                Expressions = expressions,
                NextCursor = cursor,
                Ok = true
            };
        }

        public ParseExpressionResponse ParseExpression(IEnumerable<Token> tokens, int initialCursor)
        {
            var cursor = initialCursor;

            var current = tokens.ElementAt(cursor);
            var kinds = new[] { TokenType.Identifier, TokenType.Numeric, TokenType.String };

            if (kinds.Contains(current.TokenType))
            {
                var expression = new Expression
                {
                    TokenLiteral = current,
                };

                cursor += 1;

                var response = new ParseExpressionResponse()
                {
                    Expression = expression,
                    NextCursor = cursor,
                    Ok = true
                };

                return response;
            }

            return new ParseExpressionResponse()
            {
                NextCursor = initialCursor,
                Ok = false,
            };
        }

        private void ExpectTokenType(Token token, TokenType type) 
        {
            if (token.TokenType != type)
            {
                throw new Exception($"Expected {type} loc: {token.Location.Row}:{token.Location.Column}");
            }
        }
    }
}
