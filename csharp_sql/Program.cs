using csharp_sql.Memory;
using csharp_sql.Statements;
using System.Text;

namespace csharp_sql
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var memory = new MemoryBackend();

            Console.WriteLine("Welcome to C# sql");
            Console.WriteLine("Press ctrl + c to quit");

            while (true)
            {
                Console.Write("# ");
                var text = Console.ReadLine();

                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }

                try
                {
                    var lexer = new Lexer(text);
                    var tokens = lexer.Lex();

                    var parser = new Parser();
                    var ast = parser.Parse(tokens);

                    foreach (var statement in ast)
                    {
                        if (statement.GetType() == typeof(SelectStatement))
                        {
                            var results = memory.Select((SelectStatement)statement);

                            foreach (var column in results.Columns)
                            {
                                Console.Write($"| {column.Name} ");
                            }

                            Console.WriteLine("|");

                            for (var i = 0; i < 20; i++)
                            {
                                Console.Write("=");
                            }

                            Console.WriteLine();

                            foreach (var row in results.Rows)
                            {
                                Console.Write("|");
                                for (var i = 0; i < row.Count(); i++)
                                {
                                    var cell = row.ElementAt(i);

                                    var type = results.Columns.ElementAt(i).Type;
                                    var output = "";

                                    switch (type)
                                    {
                                        case ColumnType.Int:
                                            output = cell.IntValue.ToString();
                                            break;
                                        case ColumnType.Text:
                                            output = cell.StringValue;
                                            break;
                                    }

                                    Console.Write($" {output} | "); ;
                                }

                                Console.WriteLine();

                            }

                            Console.WriteLine("ok");
                        }

                        else if (statement.GetType() == typeof(CreateTableStatement))
                        {
                            var tableName = memory.CreateTable((CreateTableStatement) statement);
                            Console.WriteLine($"table: {tableName} created");
                        }

                        else if (statement.GetType() == typeof(InsertStatement))
                        {
                            var insertResponse = memory.Insert((InsertStatement) statement);

                            var infoString = new StringBuilder();

                            for (var i = 0; i < insertResponse.Values.Count(); i++)
                            {
                                var value = insertResponse.Values.ElementAt(i);
                                if (i == (insertResponse.Values.Count() - 1))
                                {
                                    infoString.Append($"{value}");
                                }
                                else
                                { 
                                    infoString.Append($"{value}, ");
                                }
                            }

                            Console.WriteLine($"Values ({infoString}) inserted into table: {insertResponse.Table}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }
    }
}
