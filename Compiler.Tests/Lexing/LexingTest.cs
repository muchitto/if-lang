using Compiler.Lexing;

namespace Compiler.Tests.Lexing;

public class LexingTest : CompilationTest
{
    [Fact]
    public void ShouldLexSimpleFunction()
    {
        var source = @"
            func add(a: Int, b: Int) -> Int {
                return a + b
            }
        ";

        var lexer = CreateLexer("ShouldLexSimpleFunction", source);

        try
        {
            var tokens = GetTokenTypeAndValue(lexer);

            Assert.Equal(23, tokens.Length);
            Assert.Equal(tokens, new[]
            {
                (TokenType.NewLine, ""),
                (TokenType.Keyword, "func"),
                (TokenType.Identifier, "add"),
                (TokenType.Symbol, "("),
                (TokenType.Identifier, "a"),
                (TokenType.Symbol, ":"),
                (TokenType.Identifier, "Int"),
                (TokenType.Symbol, ","),
                (TokenType.Identifier, "b"),
                (TokenType.Symbol, ":"),
                (TokenType.Identifier, "Int"),
                (TokenType.Symbol, ")"),
                (TokenType.Symbol, "->"),
                (TokenType.Identifier, "Int"),
                (TokenType.Symbol, "{"),
                (TokenType.NewLine, ""),
                (TokenType.Keyword, "return"),
                (TokenType.Identifier, "a"),
                (TokenType.Operator, "+"),
                (TokenType.Identifier, "b"),
                (TokenType.NewLine, ""),
                (TokenType.Symbol, "}"),
                (TokenType.EndOfFile, "")
            });
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public void ShouldLexSimpleVariableDeclaration()
    {
        var source = @"
            var a = 1
        ";

        var lexer = CreateLexer("ShouldLexSimpleVariableDeclaration", source);

        try
        {
            var tokens = GetTokenTypeAndValue(lexer);

            Assert.Equal(6, tokens.Length);
            Assert.Equal(tokens, new[]
            {
                (TokenType.NewLine, ""),
                (TokenType.Keyword, "var"),
                (TokenType.Identifier, "a"),
                (TokenType.Symbol, "="),
                (TokenType.Number, "1"),
                (TokenType.EndOfFile, "")
            });
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public void ShouldLexTwoVariableDeclarations()
    {
        var source = @"
            var a = 1
            var b = 2
        ";

        var lexer = CreateLexer("ShouldLexTwoVariableDeclarations", source);

        try
        {
            var tokens = GetTokenTypeAndValue(lexer);

            Assert.Equal(11, tokens.Length);
            Assert.Equal(tokens, new[]
            {
                (TokenType.NewLine, ""),
                (TokenType.Keyword, "var"),
                (TokenType.Identifier, "a"),
                (TokenType.Symbol, "="),
                (TokenType.Number, "1"),
                (TokenType.NewLine, ""),
                (TokenType.Keyword, "var"),
                (TokenType.Identifier, "b"),
                (TokenType.Symbol, "="),
                (TokenType.Number, "2"),
                (TokenType.EndOfFile, "")
            });
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public void ShouldLexSimpleClass()
    {
        var source = @"
            class A {
            }
        ";

        var lexer = CreateLexer("ShouldLexSimpleClass", source);

        try
        {
            var tokens = GetTokenTypeAndValue(lexer);

            Assert.Equal(7, tokens.Length);
            Assert.Equal(tokens, new[]
            {
                (TokenType.NewLine, ""),
                (TokenType.Keyword, "class"),
                (TokenType.Identifier, "A"),
                (TokenType.Symbol, "{"),
                (TokenType.NewLine, ""),
                (TokenType.Symbol, "}"),
                (TokenType.EndOfFile, "")
            });
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    private Lexer CreateLexer(string testName, string source)
    {
        var compilationContext = CreateCompilationContext(testName, source);

        return new Lexer(compilationContext);
    }

    private Token[] GetTokens(Lexer lexer)
    {
        var tokens = new List<Token>();
        while (!lexer.IsEnd())
        {
            tokens.Add(lexer.GetToken());
        }

        return tokens.ToArray();
    }

    private (TokenType Type, string Value)[] GetTokenTypeAndValue(Lexer lexer)
    {
        return GetTokens(lexer).Select(t => (t.Type, t.Value)).ToArray();
    }
}