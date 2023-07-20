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
                        Console.WriteLine("Create Table Statement");
                    }

                    else if (statement.GetType() == typeof(InsertStatement))
                    {
                        Console.WriteLine("Insert Statement");
                    }
                }
            }
        }
    }
}
