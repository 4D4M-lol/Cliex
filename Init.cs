using Spectre.Console;
using Spectre.Console.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cliex
{
    public static class Init
    {
        public static void Main(string[] args)
        {
            string source = "{\"A\": \"Apple\", \"B\": \"Banana\"}";
            Lexer lexer = new(source);
            List<Token> result = lexer.Analyse();
            JsonSerializerOptions options = new()
            {
                Converters = { new JsonStringEnumConverter(), new JsonTokenConverter(), },
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(result, options);
            JsonText text = new JsonText(json);

            text.BracketsStyle = new(Color.Yellow);
            text.BracesStyle = new(Color.Yellow);
            
            AnsiConsole.Write(text);
        }
    }
}