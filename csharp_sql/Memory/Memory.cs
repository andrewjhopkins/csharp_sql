using csharp_sql.Statements;

namespace csharp_sql.Memory
{
    public class MemoryBackend
    {
        public Dictionary<string, Table> Tables { get; set; } = new Dictionary<string, Table>();

        public void CreateTable(CreateTableStatement createTableStatement)
        {
            var table = new Table();

            if (createTableStatement.Columns == null)
            {
                return;
            }

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
                        throw new Exception("Invalid column type");
                }

                table.ColumnTypes.Add(columnType);
            }

            Tables[createTableStatement.Name.Value] = table;

            return;
        }

        public void Insert(InsertStatement insertStatement) 
        {
            if (!Tables.ContainsKey(insertStatement.Table.Value))
            {
                throw new Exception("Table does not exist");
            }

            var table = Tables[insertStatement.Table.Value];

            var row = new List<string>();

            if (insertStatement.Values.Count() != table.Columns.Count()) 
            {
                throw new Exception("missing values");
            }

            for (var i = 0; i <  insertStatement.Values.Count(); i++) 
            {
                var value = insertStatement.Values.ElementAt(i);
                row.Add(value.TokenLiteral.Value);
            }

            table.Rows.Add(row);

            return;
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

            for (var i = 0; i < table.Rows.Count(); i++)
            {
                var row = table.Rows.ElementAt(i);

                var result = new List<Cell>();
                var isFirstRow = i == 0;

                for (var j = 0; j < selectStatement.Items.Count(); j++) 
                { 
                    var selectItem = selectStatement.Items.ElementAt(j);

                    // TODO: support asterisks

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
                            throw new Exception("Column does not exist");
                        }

                        continue;
                    }

                    throw new Exception("Column does not exist");
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
