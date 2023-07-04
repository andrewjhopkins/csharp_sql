using System;

namespace csharp_sql
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lexer = new Lexer("Testing\nsource\ncode\n");
            lexer.Lex();
        }
    }
}
