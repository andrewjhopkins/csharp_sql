using csharp_sql.Memory;

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

                Console.WriteLine(ast.Count());
            }
        }
    }
}
