using csharp_sql.Statements;
using System.Security.Cryptography;

namespace csharp_sql.Memory
{
    public class Memory
    {
        public Dictionary<string, Table> Tables { get; set; }

        public void CreateTable(CreateTableStatement createTableStatement)
        {
            var table = new Table();
            Tables[createTableStatement.Name.Value] = table;

            if (createTableStatement.Columns == null)
            {
                return;
            }

            for (var i = 0; i < createTableStatement.Columns.Count(); i++)
            {
                var column = createTableStatement.Columns.ElementAt(i);
                
                table.Columns.Append(column.Name.Value);

                ColumnType columnType;

                switch (column.DataType.Value)
                {
                    case "int":
                        columnType = ColumnType.Int;
                        break;
                    case "text":
                        columnType = ColumnType.Text;
                        break;
                    default:
                        throw new Exception("Invalid column type");
                }

                table.ColumnTypes.Append(columnType);
            }

            return;
        }

        public void Insert(InsertStatement insertStatement) 
        {
            // TODO: check if exists first
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

                // TODO: Skip non literal kinds

                row.Add(value.TokenLiteral.Value);
            }

            table.Rows.Append(row);

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

                    // TODO: skip literal

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

                                result.Add(new Cell { StringValue = row.ElementAt(k) });
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
