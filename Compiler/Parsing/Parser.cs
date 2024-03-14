using Compiler.ErrorHandling;
using Compiler.Lexing;
using Compiler.Syntax;
using Compiler.Syntax.Nodes;

namespace Compiler.Parsing;

public class Parser
{
    public Parser(CompilationContext context)
    {
        Context = context;
        Lexer = new Lexer(context, new LexerOptions());
    }

    private CompilationContext Context { get; }
    private Lexer Lexer { get; }

    private CompileError.SyntaxError NextSyntaxError(string message)
    {
        return new CompileError.SyntaxError(message, PeekToken().PositionData);
    }

    private Token GetToken()
    {
        return Lexer.GetToken();
    }

    private Token PeekToken()
    {
        return Lexer.PeekToken();
    }

    private bool IsNext(TokenType type, string? value = null)
    {
        if (value != null)
        {
            return PeekToken().Is(type, value);
        }

        return PeekToken().Is(type);
    }

    private bool IsNextOr(params TokenType[] types)
    {
        return types.Contains(PeekToken().Type);
    }

    private void OptionalNewLine()
    {
        Optional(TokenType.NewLine);
    }

    private void Optional(params TokenType[] types)
    {
        if (IsNextOr(types))
        {
            GetToken();
        }
    }

    private void Expect(TokenType type, string? msg = null)
    {
        if (!IsNext(type))
        {
            var message = msg ?? $"expected {type} but got {PeekToken().Type}";

            throw NextSyntaxError(message);
        }
    }

    private void ExpectEat(TokenType type, string? msg = null)
    {
        Expect(type);
        GetToken();
    }

    private void ExpectValue(TokenType type, string value, string? msg = null)
    {
        if (!IsNext(type, value))
        {
            var message = msg ?? $"expected {type} but got {PeekToken().Type}";

            throw NextSyntaxError(message);
        }
    }

    private void ExpectEatValue(TokenType type, string value, string? msg = null)
    {
        ExpectValue(type, value);
        GetToken();
    }

    private void Optional(TokenType type, string value)
    {
        if (!IsNext(type, value))
        {
            return;
        }

        GetToken();
    }

    private void Optional(string value, params TokenType[] types)
    {
        if (!IsNextOr(types))
        {
            return;
        }

        if (PeekToken().Value == value)
        {
            GetToken();
        }
    }

    private bool IsNextEat(TokenType type)
    {
        if (!IsNext(type))
        {
            return false;
        }

        GetToken();
        return true;
    }

    private bool IsNextEat(TokenType type, string value)
    {
        if (!IsNext(type, value))
        {
            return false;
        }

        GetToken();
        return true;
    }

    private bool IsNewLine()
    {
        return IsNext(TokenType.NewLine);
    }

    private bool IsSentenceEnd()
    {
        return IsNewLine() || IsNext(TokenType.Symbol, ";");
    }

    // Newline or semicolon
    private void SentenceEnd()
    {
        if (IsSentenceEnd())
        {
            GetToken();
        }
        else
        {
            throw NextSyntaxError("expected newline or semicolon");
        }
    }

    private void ExpectNewLine()
    {
        if (!IsNewLine())
        {
            throw NextSyntaxError("expected newline");
        }
    }

    private FunctionDeclarationNode ParseFunctionDeclaration()
    {
        var name = ParseSingleIdentifier();

        var parameters = new List<FunctionDeclarationArgumentNode>();

        if (IsNextEat(TokenType.Symbol, "("))
        {
            while (!IsNext(TokenType.Symbol, ")"))
            {
                var parameterName = ParseSingleIdentifier();

                TypeInfoNode type;
                if (IsNextEat(TokenType.Symbol, ":"))
                {
                    type = ParseTypeInfo();
                }
                else
                {
                    throw NextSyntaxError("expected type");
                }

                parameters.Add(new FunctionDeclarationArgumentNode(parameterName, type));

                if (IsNextEat(TokenType.Symbol, ","))
                {
                    continue;
                }

                break;
            }

            ExpectEatValue(TokenType.Symbol, ")");
        }

        TypeInfoNode returnType = null;
        if (IsNextEat(TokenType.Symbol, "->"))
        {
            returnType = ParseTypeInfo();
        }

        var body = ParseBlock();

        return new FunctionDeclarationNode(name, parameters, returnType, body);
    }

    private BodyBlockNode ParseBlock()
    {
        ExpectEatValue(TokenType.Symbol, "{");

        var statements = new List<BaseNode>();

        // Should start recording newlines

        while (!IsNext(TokenType.Symbol, "}"))
        {
            Lexer.LexerOptions.RecordNewLines.Push(true);

            var statement = ParseStatement();

            SentenceEnd();

            Lexer.LexerOptions.RecordNewLines.Pop();

            statements.Add(statement);
        }


        ExpectEatValue(TokenType.Symbol, "}");

        return new BodyBlockNode(statements);
    }

    private BaseNode ParseStatement()
    {
        if (IsNext(TokenType.Keyword, "def"))
        {
            return ParseDeclaration();
        }

        if (IsNextEat(TokenType.Keyword, "var"))
        {
            return ParseVariableDeclaration();
        }

        if (IsNextEat(TokenType.Keyword, "return"))
        {
            if (IsSentenceEnd())
            {
                return new ReturnStatementNode(null);
            }

            var expression = ParseExpression();
            return new ReturnStatementNode(expression);
        }

        if (IsNextEat(TokenType.Keyword, "if"))
        {
            return ParseIfStatement();
        }

        if (IsNextEat(TokenType.Keyword, "for"))
        {
            return ParseForStatement();
        }

        if (IsNextEat(TokenType.Keyword, "while"))
        {
            return ParseWhileStatement();
        }

        if (IsNextEat(TokenType.Keyword, "break"))
        {
            return new BreakStatementNode();
        }

        if (IsNextEat(TokenType.Keyword, "continue"))
        {
            return new ContinueStatementNode();
        }

        if (IsNext(TokenType.Identifier))
        {
            var identifier = ParseIdentifier();

            if (PeekToken().IsAssignmentOperator())
            {
                var op = GetToken().ToOperator();

                var value = ParseExpression();

                return new AssignmentNode(identifier, value, op);
            }

            return ParseFunctionCall(identifier);
        }

        throw NextSyntaxError("expected statement");
    }

    private DeclarationNode ParseDeclaration()
    {
        Lexer.LexerOptions.RecordNewLines.Push(false);

        DeclarationNode declaration;
        if (IsNextEat(TokenType.Keyword, "var"))
        {
            declaration = ParseVariableDeclaration();
        }
        else if (IsNextEat(TokenType.Keyword, "def"))
        {
            declaration = ParseFunctionDeclaration();
        }
        else if (IsNextEat(TokenType.Keyword, "new"))
        {
            declaration = ParseObjectDeclaration();
        }
        else
        {
            throw NextSyntaxError("expected a declaration");
        }

        Lexer.LexerOptions.RecordNewLines.Pop();

        return declaration;
    }

    private ObjectDeclarationNode ParseObjectDeclaration()
    {
        var isImmediatelyInstanced = !IsNextEat(TokenType.Keyword, "type");

        var baseName = ParseSingleIdentifier();

        ExpectEatValue(TokenType.Identifier, "called");

        var name = ParseSingleIdentifier();

        ExpectEatValue(TokenType.Symbol, "{");

        var fields = new List<DeclarationNode>();

        while (!IsNext(TokenType.Symbol, "}"))
        {
            var field = ParseDeclaration();

            fields.Add(field);
        }

        ExpectEatValue(TokenType.Symbol, "}");

        return new ObjectDeclarationNode(isImmediatelyInstanced, new TypeInfoNameNode(baseName.Name), name, fields);
    }

    private WhileStatementNode ParseWhileStatement()
    {
        var condition = ParseExpression();

        var body = ParseBlock();

        return new WhileStatementNode(condition, body);
    }

    private ForStatementNode ParseForStatement()
    {
        var usesParentheses = IsNextEat(TokenType.Symbol, "(");

        var iteratorVariable = ParseSingleIdentifier();

        ExpectEatValue(TokenType.Keyword, "in");

        var iterable = ParseExpression();

        if (usesParentheses)
        {
            ExpectEatValue(TokenType.Symbol, ")");
        }

        var body = ParseBlock();

        return new ForStatementNode(iterable, iteratorVariable, body);
    }

    private IfStatementNode ParseIfStatement()
    {
        var condition = ParseExpression();

        var body = ParseBlock();

        IfStatementNode? nextIf = null;

        if (!IsNextEat(TokenType.Keyword, "else"))
        {
            return new IfStatementNode(condition, body, nextIf);
        }

        if (IsNextEat(TokenType.Keyword, "if"))
        {
            nextIf = ParseIfStatement();
        }
        else
        {
            var elseBody = ParseBlock();

            var elseStatement = new IfStatementNode(null, elseBody, null);

            return new IfStatementNode(condition, body, elseStatement);
        }

        return new IfStatementNode(condition, body, nextIf);
    }

    private TypeInfoNode ParseTypeInfo()
    {
        if (IsNext(TokenType.Identifier))
        {
            var typeName = new TypeInfoNameNode(GetToken().Value);

            return ParseTypeInfoPart(typeName);
        }

        if (IsNextEat(TokenType.Symbol, "{"))
        {
            var fields = new Dictionary<string, TypeInfoNode>();

            while (!IsNext(TokenType.Symbol, "}"))
            {
                var fieldName = ParseSingleIdentifier();

                ExpectEatValue(TokenType.Symbol, ":");

                var fieldType = ParseTypeInfo();

                fields.Add(fieldName.Name, fieldType);

                if (IsNextEat(TokenType.Symbol, ","))
                {
                    continue;
                }

                break;
            }

            ExpectEatValue(TokenType.Symbol, "}");

            return ParseTypeInfoPart(new TypeInfoStructureNode(fields));
        }

        throw NextSyntaxError("expected type info");
    }

    private TypeInfoNode ParseTypeInfoPart(TypeInfoNode typeInfo)
    {
        if (IsNextEat(TokenType.Symbol, "["))
        {
            ExpectEatValue(TokenType.Symbol, "]");

            return ParseTypeInfoPart(new TypeInfoArrayNode(typeInfo));
        }

        return typeInfo;
    }

    private VariableDeclarationNode ParseVariableDeclaration()
    {
        var name = new IdentifierNode(GetToken().Value);

        TypeInfoNode type = null;
        if (IsNextEat(TokenType.Symbol, ":"))
        {
            type = ParseTypeInfo();
        }

        BaseNode value = null;
        if (IsNextEat(TokenType.Symbol, "="))
        {
            value = ParseExpression();
        }

        if (value == null && type == null)
        {
            throw NextSyntaxError("expected type or value");
        }

        return new VariableDeclarationNode(name, type, value);
    }

    private BaseNode ParseExpressionContinue(BaseNode lhs, int minPrecedence)
    {
        var nextToken = PeekToken();

        if (nextToken.Type == TokenType.Operator)
        {
            return lhs;
        }

        var op = nextToken.ToOperator();
        var precedence = op.Precedence();

        if (precedence <= minPrecedence)
        {
            return lhs;
        }

        GetToken();

        var rhs = ParseExpression(precedence);

        return new ExpressionNode(
            lhs,
            lhs,
            op
        );
    }

    private BaseNode ParseExpression(int minPrecedence = 0)
    {
        var lhs = ParsePrimary();

        if (!IsNext(TokenType.Operator))
        {
            return lhs;
        }

        while (true)
        {
            var node = ParseExpressionContinue(lhs, minPrecedence);

            if (node == lhs)
            {
                break;
            }

            lhs = node;
        }

        return lhs;
    }

    private BaseNode ParsePrimary()
    {
        if (IsNext(TokenType.Number))
        {
            return new NumberLiteralNode(GetToken().Value);
        }

        if (IsNext(TokenType.String))
        {
            return new StringLiteralNode(GetToken().Value);
        }

        if (IsNext(TokenType.Keyword, "true") || IsNext(TokenType.Keyword, "false"))
        {
            return new BooleanLiteralNode(GetToken().Value == "true");
        }

        if (IsNextEat(TokenType.Keyword, "null"))
        {
            return new NullLiteralNode();
        }

        if (IsNext(TokenType.Symbol, "{"))
        {
            return ParseStructureLiteral();
        }

        if (IsNext(TokenType.Symbol, "["))
        {
            return ParseArrayLiteral();
        }

        if (!IsNext(TokenType.Identifier))
        {
            throw NextSyntaxError($"unexpected token {PeekToken().Type}");
        }

        var identifier = new IdentifierNode(GetToken().Value);

        if (IsNextEat(TokenType.Symbol, "("))
        {
            return ParseFunctionCallWithParentheses(identifier);
        }

        if (IsNextEat(TokenType.Symbol, "."))
        {
            return ParseMemberAccess(identifier);
        }

        if (IsNextOr(TokenType.Number, TokenType.String, TokenType.Identifier))
        {
            return ParseFunctionCallWithoutParenthesis(identifier);
        }

        return identifier;
    }

    private ArrayLiteralNode ParseArrayLiteral()
    {
        var elements = new List<BaseNode>();

        ExpectEatValue(TokenType.Symbol, "[");

        while (!IsNext(TokenType.Symbol, "]"))
        {
            var element = ParseExpression();

            elements.Add(element);

            if (IsNextEat(TokenType.Symbol, ","))
            {
                continue;
            }

            break;
        }

        ExpectEatValue(TokenType.Symbol, "]");

        return new ArrayLiteralNode(elements);
    }

    private StructureLiteralNode ParseStructureLiteral()
    {
        var fields = new List<StructureLiteralFieldNode>();

        ExpectEatValue(TokenType.Symbol, "{");

        while (!IsNext(TokenType.Symbol, "}"))
        {
            var identifierPosition = PeekToken().PositionData;
            var fieldName = ParseSingleIdentifier();

            var fieldExists = fields.Any(f => f.Name.Name == fieldName.Name);

            if (fieldExists)
            {
                throw new CompileError.SyntaxError(
                    $"field {fieldName.Name} already exists in the structure",
                    identifierPosition
                );
            }

            ExpectEatValue(TokenType.Symbol, ":");

            var fieldValue = ParseExpression();

            fields.Add(new StructureLiteralFieldNode(fieldName, fieldValue));

            if (IsNextEat(TokenType.Symbol, ","))
            {
                continue;
            }

            break;
        }

        ExpectEatValue(TokenType.Symbol, "}");

        return new StructureLiteralNode(fields);
    }

    private FunctionCallNode ParseFunctionCall(IdentifierNode functionName)
    {
        if (IsNextEat(TokenType.Symbol, "("))
        {
            return ParseFunctionCallWithParentheses(functionName);
        }

        return ParseFunctionCallWithoutParenthesis(functionName);
    }

    private FunctionCallNode ParseFunctionCallWithParentheses(IdentifierNode functionName)
    {
        var arguments = new List<FunctionCallArgumentNode>();

        while (!IsNext(TokenType.Symbol, ")") && !IsEnd())
        {
            var expression = ParseExpression();

            arguments.Add(new FunctionCallArgumentNode(expression));

            if (IsNextEat(TokenType.Symbol, ","))
            {
                continue;
            }

            break;
        }

        ExpectEatValue(TokenType.Symbol, ")");

        return new FunctionCallNode(functionName, arguments);
    }

    // Function call can be used with parentheses or without
    private FunctionCallNode ParseFunctionCallWithoutParenthesis(IdentifierNode functionName)
    {
        var arguments = new List<FunctionCallArgumentNode>();

        while (!IsSentenceEnd() && !IsNext(TokenType.EndOfFile))
        {
            var expression = ParseExpression();

            arguments.Add(new FunctionCallArgumentNode(expression));

            if (IsNextEat(TokenType.Symbol, ","))
            {
                continue;
            }

            break;
        }

        return new FunctionCallNode(functionName, arguments);
    }

    private MemberAccessNode ParseMemberAccess(IdentifierNode baseObject)
    {
        while (true)
        {
            var member = ParseSingleIdentifier();

            var memberAccess = new MemberAccessNode(baseObject, member);

            if (IsNextEat(TokenType.Symbol, "."))
            {
                baseObject = memberAccess;
                continue;
            }


            return memberAccess;
        }
    }

    private IdentifierNode ParseSingleIdentifier()
    {
        if (PeekToken().Type != TokenType.Identifier)
        {
            throw NextSyntaxError("expected an identifier");
        }

        return new IdentifierNode(GetToken().Value);
    }

    private IdentifierNode ParseIdentifier()
    {
        var identifier = ParseSingleIdentifier();

        if (!IsNextEat(TokenType.Symbol, "."))
        {
            return identifier;
        }

        return ParseMemberAccess(identifier);
    }

    public ProgramNode Parse()
    {
        var declarations = new List<DeclarationNode>();

        while (!IsEnd())
        {
            var declaration = ParseDeclaration();

            declarations.Add(declaration);
        }

        return new ProgramNode(declarations);
    }

    private bool IsEnd()
    {
        return Lexer.IsEnd() || PeekToken().Is(TokenType.EndOfFile);
    }
}