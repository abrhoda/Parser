using NUnit.Framework.Constraints;

namespace ParserApp.Test
{
    [TestFixture]
    public class ParserTests
    {
        private Parser parser = new Parser();

        [TestCase("")]
        [TestCase("-")]
        [TestCase("abc123")]
        [TestCase("(d6")]
        public void Evalute_InvalidInput_ThrowsException(string input)
        {
            Assert.That(() => parser.Evaluate(input), Throws.Exception);
        }

        private static IEnumerable<TestCaseData> ValidInputs()
        {
            yield return new TestCaseData("1", Is.EqualTo(1));
            yield return new TestCaseData("d2", Is.InRange(1, 2));
            yield return new TestCaseData("2*(2+3)", Is.EqualTo(10));
            yield return new TestCaseData("12-d6*2", Is.InRange(0, 10));
            yield return new TestCaseData("1d6+2d4*2", Is.InRange(5, 22));

        }


        [TestCaseSource(nameof(ValidInputs))]
        public void Evalute_ValidInputs_ReturnsInt(string input, Constraint constraint)
        {
            Assert.That(parser.Evaluate(input), constraint);
        }
    }
}