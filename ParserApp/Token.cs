using System.Data;

namespace PrattParser
{

    public class Token
    {
        private static readonly Random random = new Random();
        public enum TokenType
        {
            Expression,
            Value,
            Operation,
            Eof
        }

        public string Input { get; }
        public TokenType Type { get; }

        public Token(string input, TokenType tokenType)
        {
            this.Input = input;
            this.Type = tokenType;
        }

        public int ToValue()
        {
            int result = 0;
            if (this.Type == TokenType.Expression)
            {
                result = ParseExpression(this.Input);
            }
            else if (this.Type == TokenType.Value)
            {
                if (!int.TryParse(this.Input, out result))
                {
                    throw new Exception($"Could not convert token.Input ({this.Input}) to an integer.");
                }
            }
            else
            {
                throw new Exception($"TokenType.Operation and TokenType.Eof do not have a value. Found TokenType of {this.Type}");
            }

            return result;
        }

        private int ParseExpression(string expression)
        {
            int result = 0;
            int amount = 1;
            
            int index = expression.IndexOf('d');
            // double check that d/D are in the expression. Though it shouldn't be possible to get here without it.
            if (index == -1)
            {
                index = expression.IndexOf('D');
            }
            if (index == -1)
            {
                throw new Exception("Expression did not contain 'd' or 'D'. Cannot parse.");
            }

            if (index != 0 && !int.TryParse(expression.AsSpan(0, index), out amount))
            {
                throw new Exception("Could not parse 'amount' from expression as it wasn't an integer.");
            }

            int size;
            if (!int.TryParse(expression.AsSpan(index + 1), out size))
            {
                throw new Exception("Could not parse 'size' from expression as it wasn't an integer.");
            }

            for (int i = 0; i < amount; ++i)
            {
                result += random.Next(1, size+1);
            }
            Console.WriteLine($"Rolled {result} for expression {expression}");
            return result;
        }
    }
}