using csharp_sql.Statements;

namespace csharp_sql
{
    public class Parser
    {
        public IEnumerable<IStatement> Parse(IEnumerable<Token> tokens)
        {
            if (tokens.Count() == 0)
            {
                throw new Exception();
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
                var parseStatementResponse = ParseStatement(tokens, cursor);
                ast.Add(parseStatementResponse.Statement);
                cursor = parseStatementResponse.NextCursor;
            }

            return ast;
        }

        public ParseStatementResponse ParseStatement(IEnumerable<Token> tokens, int initialCursor)
        {
            var cursor = initialCursor;
            var parseSelectResponse = ParseSelectStatement(tokens, cursor);

            if (parseSelectResponse.Ok)
            {
                return new ParseStatementResponse
                {
                    Statement = parseSelectResponse.SelectStatement,
                    NextCursor = parseSelectResponse.NextCursor
                };
            }


            return null;
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

            if (tokens.ElementAt(cursor).TokenType != TokenType.From)
            {
                throw new Exception("Expected From");
            }

            cursor += 1;

            var identifierToken = tokens.ElementAt(cursor);

            if (identifierToken.TokenType != TokenType.Identifier)
            {
                throw new Exception("Expected Identifier");
            }

            selectStatement.From = new FromItem { Table = identifierToken };

            return new ParseSelectStatementResponse
            {
                SelectStatement = selectStatement,
                Ok = true,
                NextCursor = cursor + 1
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
                    if (current.TokenType != TokenType.Comma)
                    {
                        throw new Exception("Expected comma");
                    }

                    cursor += 1;
                }

                var selectItem = new SelectItem();

                if (tokens.ElementAt(cursor).TokenType == TokenType.Asterisk)
                {
                    selectItem.Asterisk = true;
                    cursor += 1;
                }
                else
                {
                    var parseExpressionResponse = ParseExpression(tokens, cursor);
                    if (!parseExpressionResponse.Ok)
                    {
                        throw new Exception("Expected expression");
                    }

                    cursor = parseExpressionResponse.NextCursor;
                    selectItem.Expression = parseExpressionResponse.Expression;

                    if (cursor < tokens.Count() && tokens.ElementAt(cursor).TokenType == TokenType.As)
                    {
                        cursor += 1;
                        var identifierToken = tokens.ElementAt(cursor);
                        if (identifierToken.TokenType != TokenType.Identifier)
                        {
                            throw new Exception("Expected identifier");
                        }

                        selectItem.AsToken = identifierToken;
                        cursor += 1;
                    }
                }

                selectItems.Add(selectItem);
            }
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
    }
}
