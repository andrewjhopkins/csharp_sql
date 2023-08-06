using csharp_sql.Statements;

namespace csharp_sql.Memory
{
    public class MemoryBackend
    {
        public Dictionary<string, Table> Tables { get; set; } = new Dictionary<string, Table>();

        public string CreateTable(CreateTableStatement createTableStatement)
        {
            var table = new Table();
            for (var i = 0; i < createTableStatement.Columns.Count(); i++)
            {
                var column = createTableStatement.Columns.ElementAt(i);
                
                table.Columns.Add(column.Name.Value);

                ColumnType columnType;

                switch (column.DataType.TokenType)
                {
                    case TokenType.Int:
                        columnType = ColumnType.Int;
                        break;
                    case TokenType.Text:
                        columnType = ColumnType.Text;
                        break;
                    default:
                        throw new Exception($"Invalid column type loc: {column.DataType.Location.Row}:{column.DataType.Location.Column}");
                }

                table.ColumnTypes.Add(columnType);
            }

            Tables[createTableStatement.Name.Value] = table;

            return createTableStatement.Name.Value;
        }

        public InsertResponse Insert(InsertStatement insertStatement) 
        {
            var insertedValues = new List<string>();

            if (!Tables.ContainsKey(insertStatement.Table.Value))
            {
                throw new Exception($"Table: {insertStatement.Table.Value} does not exist");
            }

            var table = Tables[insertStatement.Table.Value];

            var row = new List<string>();

            if (insertStatement.Values.Count() != table.Columns.Count()) 
            {
                throw new Exception($"Missing values. {insertStatement.Values.Count()} given. {table.Columns.Count()} needed");
            }

            for (var i = 0; i <  insertStatement.Values.Count(); i++) 
            {
                var value = insertStatement.Values.ElementAt(i);
                row.Add(value.TokenLiteral.Value);

                insertedValues.Add(value.TokenLiteral.Value);
            }

            table.Rows.Add(row);

            return new InsertResponse
            {
                Table = insertStatement.Table.Value,
                Values = insertedValues
            };
        }

        public SelectResponse Select(SelectStatement selectStatement)
        { 
            if (!Tables.ContainsKey(selectStatement.From.Table.Value))
            {
                throw new Exception("Table does not exist");
            }

            var table = Tables[selectStatement.From.Table.Value];
            var results = new List<List<Cell>>();
            var columns = new List<ResultColumn>();

            // Handle asterisks MySQL equivalent. Asterisk must be first, populate missing fields but only display fields once
            if (selectStatement.Items.Count() > 0 && selectStatement.Items.ElementAt(0).Asterisk)
            {
                var selectItems = new List<SelectItem>();
                foreach(var column in table.Columns)
                {
                    var selectItem = new SelectItem()
                    {
                        Expression = new Expression()
                        {
                            TokenLiteral = new Token()
                            {
                                TokenType = TokenType.Identifier,
                                // must always be the first identifier after "SELECT"
                                Location = new Location()
                                {
                                    Row = 0,
                                    Column = "SELECT".Count() + 1
                                },
                                Value = column,
                            }
                        },
                        Asterisk = false,
                        AsToken = null
                    };

                    selectItems.Add(selectItem);
                }

                selectStatement.Items = selectItems;
            }

            for (var i = 0; i < table.Rows.Count(); i++)
            {
                var row = table.Rows.ElementAt(i);

                var result = new List<Cell>();
                var isFirstRow = i == 0;

                for (var j = 0; j < selectStatement.Items.Count(); j++) 
                { 
                    var selectItem = selectStatement.Items.ElementAt(j);

                    // Skip other asterisks
                    if (selectItem.Asterisk)
                    {
                        continue;
                    }

                    var literal = selectItem.Expression.TokenLiteral;

                    if (literal.TokenType == TokenType.Identifier)
                    {
                        var found = false;
                        for (var k = 0; k < table.Columns.Count(); k++)
                        {
                            var tableColumn = table.Columns.ElementAt(k);

                            if (tableColumn == literal.Value)
                            {
                                if (isFirstRow)
                                {
                                    columns.Add(new ResultColumn { Name = literal.Value, Type = table.ColumnTypes.ElementAt(k) });
                                }

                                if (table.ColumnTypes.ElementAt(k) == ColumnType.Int)
                                {
                                    result.Add(new Cell { IntValue = Int32.Parse(row.ElementAt(k)) });
                                }
                                else
                                { 
                                result.Add(new Cell { StringValue = row.ElementAt(k) });
                                }

                                found = true;
                                break;
                            }

                        }

                        if (!found)
                        {
                            throw new Exception($"Column: {literal.Value} does not exist");
                        }

                        continue;
                    }

                    throw new Exception($"Column: {literal.Value} does not exist");
                }

                results.Add(result);
            }

            return new SelectResponse
            {
                Rows = results,
                Columns = columns
            };
        }

        public byte[] TokenToCell(Token token)
        {
            if (token.TokenType == TokenType.Numeric)
            {
                var buffer = new List<byte>();

                if (Int32.TryParse(token.Value, out var integer))
                {
                    return BitConverter.GetBytes(integer);
                }
                else
                {
                    throw new Exception("Unable to parse to INT");
                }
            }

            return GetBytes(token.Value);
        }

        public static byte[] GetBytes(string value)
        {
            return Array.ConvertAll(value.Split('-'), s => byte.Parse(s, System.Globalization.NumberStyles.HexNumber));
        }
    }
}
