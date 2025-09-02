namespace PrattParser
{
    public class Node
    {
        public Token Value { get; init; }
        public Node? Right { get; set; }
        public Node? Left { get; set; }

        public Node(Token token)
        {
            this.Value = token;
        }
    }
}