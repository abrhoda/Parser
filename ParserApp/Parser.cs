
namespace ParserApp
{

    public class Parser
    {
        // Fields are useful for testing. No real reason to keep these otherwise.
        // public string? Input { get; private set; }
        // public List<Token>? Tokens { get; private set; }
        // public Node? Nodes { get; private set; }

        // Could be a static readonly map but I like this functional style more.
        private static (double, double) InfixBindingPower(string op) => op switch
        {
            "+" or "-" => (1.0, 1.1),
            "*" or "/" => (2.0, 2.1),
            _ => throw new Exception($"{op} is not a supported operation. Only +, -, *, and / are supported."),
        };

        private List<Token> Tokenize(string input)
        {
            List<Token> acc = new List<Token>();
            int cur = 0;
            int end = 0;
            bool isExpression = false;
            while (end < input.Length)
            {
                char c = input[end];
                if (char.IsDigit(c))
                {
                    end++;
                }
                else if (c == 'd' || c == 'D')
                {
                    if (isExpression)
                    {
                        throw new Exception($"Multiple d/D in the same expression at position {end}.");
                    }
                    end++;
                    isExpression = true;
                }
                else if (c == '+' || c == '-' || c == '*' || c == '/' || c == '(' || c == ')')
                {
                    if (cur != end)
                    {
                        if (isExpression)
                        {
                            if (input[end - 1] == 'd' || input[end - 1] == 'D')
                            {
                                // chunk ends in d/D which is invalid
                                throw new Exception($"Token cannot end in d/D. Token text: {input[cur..end]}.");
                            }
                            acc.Add(new Token(input[cur..end], Token.TokenType.Expression));
                            isExpression = false;
                        }
                        else
                        {
                            acc.Add(new Token(input[cur..end], Token.TokenType.Value));
                        }
                    }

                    acc.Add(new Token(c.ToString(), Token.TokenType.Operation));
                    end++;
                    cur = end;
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (cur != end)
                    {
                        if (isExpression)
                        {
                            if (input[end - 1] == 'd' || input[end - 1] == 'D')
                            {
                                // chunk ends in d/D which is invalid
                                throw new Exception($"Token cannot end in d/D. Token text: {input[cur..end]}.");
                            }
                            acc.Add(new Token(input[cur..end], Token.TokenType.Expression));
                            isExpression = false;
                        }
                        else
                        {
                            acc.Add(new Token(input[cur..end], Token.TokenType.Value));
                        }
                    }
                    end++;
                    cur = end;
                }
                else
                {
                    throw new Exception("Unknown/invalid character in input string.");
                }
            }
            if (cur != end)
            {
                if (isExpression)
                {
                    if (input[end - 1] == 'd' || input[end - 1] == 'D')
                    {
                        // chunk ends in d/D which is invalid
                        throw new Exception($"Token cannot end in d/D. Token text: {input[cur..end]}.");
                    }
                    acc.Add(new Token(input[cur..end], Token.TokenType.Expression));
                    isExpression = false;
                }
                else
                {
                    acc.Add(new Token(input[cur..end], Token.TokenType.Value));
                }
            }
            acc.Add(new Token("", Token.TokenType.Eof));
            return acc;
        }

        /// <summary>
        /// Returns the next (head) token from the list while removing it, shrinking the list of tokens.
        /// </summary>
        /// <param name="tokens">List of tokens returned by the Tokenize method.</param>
        /// <returns>the first (head) token of the list</returns>
        /// <exception cref="Exception">Thrown when the list is null or empty</exception>
        private Token Next(List<Token> tokens)
        {
            if (tokens == null || tokens.Count == 0)
            {
                throw new Exception("Cannot get next token in a null or empty list.");
            }

            Token next = tokens.First();
            tokens.RemoveAt(0);
            return next;
        }

        /// <summary>
        /// Returns the next (head) token from the list without removing it.
        /// </summary>
        /// <param name="tokens">List of tokens returned by the Tokenize method.</param>
        /// <returns>the first (head) token of the list or null if the list is null or empty.</returns>
        private Token? Peek(List<Token> tokens)
        {
            if (tokens == null || tokens.Count == 0)
            {
                return null;
            }
            Token first = tokens.First();
            return tokens.First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="minimumBindingPower"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Node NodeTreeFromTokens(List<Token> tokens, double minimumBindingPower)
        {
            Node root = new Node(Next(tokens));
            if (root.Value.Type == Token.TokenType.Operation && root.Value.Input == "(")
            {
                root = NodeTreeFromTokens(tokens, 0.0);
                if (Next(tokens).Input != ")")
                {
                    throw new Exception("Expression should have a closing paren. One was not found.");
                }
            }
            else if ((root.Value.Type != Token.TokenType.Value) && (root.Value.Type != Token.TokenType.Expression))
            {
                throw new Exception($"Expression must start with an expression or value. Found {root.Value.Type}");
            }


            while (true)
            {
                Token? op = Peek(tokens);
                if (op == null || op.Type == Token.TokenType.Eof || op.Input == ")")
                {
                    return root;
                }
                else if (op.Type == Token.TokenType.Expression || op.Type == Token.TokenType.Value)
                {
                    throw new Exception($"Expected end of file or operation token. Found {op.Type} with value {op.Input}");
                }


                (double lbp, double rbp) = InfixBindingPower(op.Input);
                if (lbp < minimumBindingPower)
                {
                    break;
                }

                // already peeked so remove
                Next(tokens);
                Node rhs = NodeTreeFromTokens(tokens, rbp);
                Node opNode = new Node(op);
                opNode.Left = root;
                opNode.Right = rhs;
                root = opNode;
            }
            return root;
        }

        private int EvaluateTree(Node node)
        {
            if (node.Value.Type == Token.TokenType.Operation)
            {
                if (node.Left == null || node.Right == null)
                {
                    throw new Exception("EvaluateTree found token with TokenType.Operation and no right or left.");
                }
                return node.Value.Input switch
                {
                    "+" => EvaluateTree(node.Left) + EvaluateTree(node.Right),
                    "-" => EvaluateTree(node.Left) - EvaluateTree(node.Right),
                    "*" => EvaluateTree(node.Left) * EvaluateTree(node.Right),
                    "/" => EvaluateTree(node.Left) / EvaluateTree(node.Right),
                    _ => throw new Exception($"EvaluteTree found token with TokenType.Operation and unexpected operation value ({node.Value.Input}).")
                };
            }
            return node.Value.ToValue();
        }

        /// <summary>
        /// Standard depth first traversal of a binary tree and prints a preOrder notation of the expression.
        /// </summary>
        /// <param name="root">Root node of the expression tree.</param>
        public void PrintExpressionFromTree(Node? node)
        {
            if (node == null)
            {
                return;
            }
            Console.Write($"{node.Value.Input}");
            PrintExpressionFromTree(node.Left);
            PrintExpressionFromTree(node.Right);
        }

        /// <summary>
        /// Takes a dice expression string and evalutes it to an int.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public int Evaluate(string input)
        {
            List<Token> tokens = Tokenize(input);
            Node expressionTree = NodeTreeFromTokens(tokens, 0.0);
            //PrintExpressionFromTree(expressionTree);
            return EvaluateTree(expressionTree);
        }
    }
}