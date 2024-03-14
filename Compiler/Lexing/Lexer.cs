using Compiler.ErrorHandling;
using Compiler.Parsing;

namespace Compiler.Lexing;

public class Lexer(CompilationContext context, LexerOptions lexerOptions)
{
    public static readonly string[] AssignmentOperators =
    [
        "=", "+=", "-=", "*=", "/=", "%="
    ];

    public static readonly string[] Keywords =
    [
        "new", "def", "var", "is", "if", "else", "while", "for", "in", "break", "continue", "return", "type",
        "attr", "import", "from", "static", "extend", "on", "off", "true", "false", "null"
    ];

    public static readonly string[] Operators =
    [
        "+", "-", "*", "/", "%", ">", "<", ">=", "<=", "==", "!=", "&&", "||", "!", "++", "--"
    ];

    public static readonly string[] Symbols = ["{", "}", "(", ")", "[", "]", "@", "->", ".", ":", ","];

    private bool _lastTokenWasNewLine = true;

    private Token? _peekedToken;
    private CompilationContext Context { get; } = context;

    private PositionData PositionData => Context.PositionData;

    private string[] SymbolsAndOperators => [..Symbols, ..Operators, ..AssignmentOperators];

    public LexerOptions LexerOptions { get; } = lexerOptions;

    private char GetChar()
    {
        if (IsEnd())
        {
            return '\0';
        }

        var chr = PositionData.SourceCode[PositionData.ColFrom];

        Context.PositionData = Context.PositionData with
        {
            ColFrom = PositionData.ColFrom + 1
        };

        return chr;
    }

    private char PeekChar(int offset = 0)
    {
        if (PositionData.ColFrom + offset >= PositionData.SourceCode.Length)
        {
            return '\0';
        }

        return PositionData.SourceCode[PositionData.ColFrom + offset];
    }

    public bool IsEnd()
    {
        return PositionData.ColFrom >= PositionData.SourceCode.Length;
    }

    private string GetWhile(Func<char, bool> fn)
    {
        var str = "";
        while (!IsEnd() && fn(PeekChar()))
        {
            str += GetChar();
        }

        return str;
    }

    private string GetUntil(Func<char, bool> fn)
    {
        var str = "";
        while (!IsEnd() && !fn(PeekChar()))
        {
            str += GetChar();
        }

        return str;
    }

    private string GetUntil(char c)
    {
        return GetUntil(ch => ch == c);
    }

    private string GetUntil(string s)
    {
        var str = "";
        while (!IsEnd() && !IsNext(s, false))
        {
            str += GetChar();
        }

        return str;
    }

    private bool IsNext(string s, bool shouldEat)
    {
        for (var i = 0; i < s.Length; i++)
        {
            if (PeekChar(i) != s[i])
            {
                return false;
            }
        }

        if (shouldEat)
        {
            Context.PositionData = Context.PositionData with
            {
                ColFrom = PositionData.ColFrom + s.Length
            };
        }

        return true;
    }

    public Token PeekToken()
    {
        if (_peekedToken != null)
        {
            return _peekedToken.Value;
        }

        var token = GetToken();

        _peekedToken = token;

        return token;
    }

    public Token GetToken()
    {
        if (_peekedToken != null)
        {
            var token = _peekedToken.Value;
            _peekedToken = null;
            return token;
        }

        while (true)
        {
            if (IsEnd())
            {
                return new Token(TokenType.EndOfFile, PositionData);
            }

            if (char.IsWhiteSpace(PeekChar()))
            {
                if (LexerOptions.ShouldRecordNewLines && !_lastTokenWasNewLine && PeekChar() == '\n')
                {
                    GetWhile(char.IsWhiteSpace);

                    if (IsEnd())
                    {
                        return new Token(TokenType.EndOfFile, "", PositionData);
                    }

                    return new Token(TokenType.NewLine, "", PositionData);
                }

                GetChar();
            }
            else if (IsNext("//", true))
            {
                GetWhile(ch => ch != '\n');
            }
            else if (IsNext("/*", true))
            {
                var commentNesting = 0;

                while (true)
                {
                    if (IsNext("/*", true))
                    {
                        commentNesting++;
                    }
                    else if (IsNext("*/", true))
                    {
                        if (commentNesting == 0)
                        {
                            break;
                        }

                        commentNesting--;
                    }

                    GetChar();
                }
            }
            else
            {
                break;
            }
        }

        if (IsEnd())
        {
            return new Token(TokenType.EndOfFile, PositionData);
        }

        _lastTokenWasNewLine = false;

        var startingPosition = PositionData;

        if (PeekChar() == '_' || char.IsLetter(PeekChar()))
        {
            var identifier = GetWhile(ch => ch == '_' || char.IsLetterOrDigit(ch));

            startingPosition = startingPosition.PositionDataSpan(identifier.Length);

            if (Keywords.Contains(identifier))
            {
                return new Token(TokenType.Keyword, identifier, startingPosition);
            }

            return new Token(TokenType.Identifier, identifier, startingPosition);
        }

        if (char.IsDigit(PeekChar()))
        {
            var number = GetWhile(char.IsDigit);

            if (PeekChar() == '.')
            {
                GetChar();
                number += "." + GetWhile(char.IsDigit);
            }

            startingPosition = startingPosition.PositionDataSpan(number.Length);

            return new Token(TokenType.Number, number, startingPosition);
        }

        if (PeekChar() == '"' || PeekChar() == '\'' || PeekChar() == '`')
        {
            var quote = GetChar();
            var str = GetUntil(quote);
            GetChar();
            return new Token(TokenType.String, str, startingPosition.PositionDataSpan(str.Length + 1));
        }

        var longestMatch = "";
        foreach (var symbolOrOperator in SymbolsAndOperators)
        {
            if (IsNext(symbolOrOperator, false) && symbolOrOperator.Length > longestMatch.Length)
            {
                longestMatch = symbolOrOperator;
            }
        }

        if (longestMatch == "")
        {
            throw new Exception("Unexpected character");
        }

        if (Symbols.Contains(longestMatch) || AssignmentOperators.Contains(longestMatch))
        {
            Context.PositionData = Context.PositionData.MoveColFrom(longestMatch.Length);

            return new Token(TokenType.Symbol, longestMatch, startingPosition.PositionDataSpan(longestMatch.Length));
        }

        if (Operators.Contains(longestMatch))
        {
            Context.PositionData = Context.PositionData.MoveColFrom(longestMatch.Length);

            return new Token(TokenType.Operator, longestMatch, startingPosition.PositionDataSpan(longestMatch.Length));
        }

        throw new CompileError.LexingError("Operator not implemented", PositionData);
    }
}