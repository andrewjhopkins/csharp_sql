using csharp_sql.Memory;
using csharp_sql.Statements;

namespace csharp_sql
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var memory = new MemoryBackend();

            Console.WriteLine("Welcome to csharp sql.");
            Console.WriteLine("Press ctrl + c to quit.");

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
                            Console.WriteLine("Select Statement");
                        }

                        else if (statement.GetType() == typeof(CreateTableStatement))
                        {
                            memory.CreateTable((CreateTableStatement) statement);
                            Console.WriteLine("ok, table created");
                        }

                        else if (statement.GetType() == typeof(InsertStatement))
                        {
                            memory.Insert((InsertStatement) statement);
                            Console.WriteLine("ok, new values inserted");
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
