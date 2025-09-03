namespace ParserApp
{
    class Program
    {
        static List<string> expressions = new List<string>()
        {
            "1*2+3*4",
            "12*1d6-2d4/3",
            "(d6*2)+3",
            "d6*(2+3)",
            "9*d6-1/d4"
        };

        // ugly main loop but just wrapping entire main runner in a try/catch
        static void Main(string[] args)
        {
            try
            {
                Parser parser = new Parser();
                while (true)
                {
                    Console.Write("Input expression (or exit)> ");
                    string? expression = Console.ReadLine();
                    if (expression == null)
                    {
                        throw new Exception("Null/blank expression input.");
                    }

                    if (expression.Equals("exit"))
                    {
                        break;
                    }
                    int res = parser.Evaluate(expression);
                    Console.WriteLine($"Result: {res}");
                }
                Console.WriteLine("Exiting.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exiting with exception: {e}");
                // any non-zero exit code.
                Environment.Exit(1);
            }
        }
    }

}