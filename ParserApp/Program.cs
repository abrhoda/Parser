namespace PrattParser
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
        static void Main(string[] args)
        {
            try
            {
                Parser parser = new Parser();
                foreach (string expression in expressions)
                {
                    Console.WriteLine($"Expression: {expression}");
                    int res = parser.Evaluate(expression);
                    Console.WriteLine($"Result: {res}");
                }
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