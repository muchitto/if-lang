using Compiler.ErrorHandling;
using Compiler.Lexing;
using Compiler.Syntax;
using Compiler.Syntax.Nodes;

namespace Compiler.Parsing;

public class Parser(CompilationContext context)
{
    private Token? _peekedToken;

    private Stack<bool> RecordNewLines { get; } = new();

    private bool ShouldRecordNewLines => RecordNewLines.Count > 0 && RecordNewLines.Peek();

    private CompilationContext Context { get; } = context;
    private Lexer Lexer { get; } = new(context);

    private CompileError.ParseError NextSyntaxError(string message)
    {
        return new CompileError.ParseError(message, PeekToken().PositionData);
    }

    private Token GetToken()
    {
        Token token;

        if (_peekedToken != null)
        {
            token = _peekedToken.Value;
            _peekedToken = null;
            return token;
        }


        token = Lexer.GetToken();

        while (token.Is(TokenType.NewLine) && !ShouldRecordNewLines)
        {
            token = Lexer.GetToken();
        }

        return token;
    }

    private Token PeekToken()
    {
        if (_peekedToken != null)
        {
            return _peekedToken.Value;
        }

        var token = GetToken();

        while (token.Is(TokenType.NewLine) && !ShouldRecordNewLines)
        {
            token = GetToken();
        }

        _peekedToken = token;

        return token;
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

    private void ExpectValue(TokenType type, string value, string? msg = null)
    {
        if (IsNext(type, value))
        {
            return;
        }

        var message = msg ??
                      $"expected {type} with value {value} but got {PeekToken().Type} with value {PeekToken().Value}";

        throw NextSyntaxError(message);
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

    private void ExpectNewLine()
    {
        if (!IsNewLine())
        {
            throw NextSyntaxError("expected newline");
        }
    }

    private void ExpectEatSentenceEnd(string? msg = null)
    {
        ExpectSentenceEnd(msg);
        GetToken();
    }

    private void ExpectSentenceEnd(string? msg = null)
    {
        if (IsSentenceEnd())
        {
            return;
        }

        var message = msg ?? "expected newline or semicolon";
        throw NextSyntaxError(message);
    }

    private List<FunctionDeclarationParameterNode> ParseFunctionDeclarationArguments()
    {
        var parameters = new List<FunctionDeclarationParameterNode>();

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

                var parameterContext = new NodeContext(parameterName.NodeContext.PositionData);
                parameters.Add(new FunctionDeclarationParameterNode(parameterContext,
                    parameterName.ToDeclarationNameNode(), type));

                if (IsNextEat(TokenType.Symbol, ","))
                {
                    continue;
                }

                break;
            }

            ExpectEatValue(TokenType.Symbol, ")");
        }

        return parameters;
    }

    private FunctionDeclarationNode ParseFunctionDeclaration(List<AnnotationNode> annotations)
    {
        var name = ParseSingleIdentifier();

        var parameters = ParseFunctionDeclarationArguments();

        TypeInfoNode? returnType = null;
        if (IsNextEat(TokenType.Symbol, "->"))
        {
            returnType = ParseTypeInfo();
        }

        var body = ParseBlock();

        var functionDeclarationContext = new NodeContext(name.NodeContext.PositionData);

        return new FunctionDeclarationNode(functionDeclarationContext, name.ToDeclarationNameNode(), parameters,
            returnType, body, annotations);
    }

    private BodyBlockNode ParseBlock()
    {
        ExpectEatValue(TokenType.Symbol, "{", "expected a start of a block using {");

        var statements = new List<BaseNode>();

        // Should start recording newlines

        while (!IsNext(TokenType.Symbol, "}"))
        {
            var statement = ParseBlockStatementOrDeclaration();

            statements.Add(statement);
        }

        ExpectEatValue(TokenType.Symbol, "}");

        NodeContext blocNodeContext;

        if (statements.Count == 0)
        {
            blocNodeContext = new NodeContext(PeekToken().PositionData);
        }
        else
        {
            blocNodeContext = new NodeContext(statements.First().NodeContext, statements.Last().NodeContext);
        }

        return new BodyBlockNode(blocNodeContext, statements);
    }

    private BaseNode? ParseStatement()
    {
        var nextToken = PeekToken();

        if (IsNextEat(TokenType.Keyword, "return"))
        {
            var returnContext = new NodeContext(nextToken.PositionData);

            if (IsSentenceEnd())
            {
                return new ReturnStatementNode(returnContext, null);
            }

            var expression = ParseExpression();
            return new ReturnStatementNode(returnContext, expression);
        }

        if (IsNextEat(TokenType.Keyword, "break"))
        {
            return new BreakStatementNode(new NodeContext(nextToken.PositionData));
        }

        if (IsNextEat(TokenType.Keyword, "continue"))
        {
            return new ContinueStatementNode(new NodeContext(nextToken.PositionData));
        }

        if (IsNext(TokenType.Identifier))
        {
            var identifier = ParseIdentifier();

            if (PeekToken().IsAssignmentOperator())
            {
                var op = GetToken().ToOperator();

                var value = ParseExpression();

                var assignmentNodeContext = new NodeContext(identifier.NodeContext, value.NodeContext);

                return new AssignmentNode(assignmentNodeContext, identifier, value, op);
            }

            return ParseFunctionCall(identifier);
        }

        throw NextSyntaxError("expected a statement");
    }

    private BaseNode ParseBlockStatementOrDeclaration()
    {
        if (IsNext(TokenType.Keyword, "var") || IsNext(TokenType.Keyword, "def") || IsNext(TokenType.Keyword, "new"))
        {
            return ParseDeclaration();
        }

        if (IsNext(TokenType.Keyword, "if") || IsNext(TokenType.Keyword, "for") || IsNext(TokenType.Keyword, "while"))
        {
            return ParseControlFlowStatement();
        }

        StartRecordNewLinesMode(true);

        var statement = ParseStatement();

        ExpectEatSentenceEnd();

        EndRecordNewLinesMode();

        return statement;
    }

    private BaseNode? ParseControlFlowStatement()
    {
        BaseNode? controlFlowStatement = null;

        if (IsNextEat(TokenType.Keyword, "if"))
        {
            controlFlowStatement = ParseIfStatement();
        }
        else if (IsNextEat(TokenType.Keyword, "for"))
        {
            controlFlowStatement = ParseForStatement();
        }
        else if (IsNextEat(TokenType.Keyword, "while"))
        {
            controlFlowStatement = ParseWhileStatement();
        }

        if (controlFlowStatement == null)
        {
            throw NextSyntaxError("expected a control flow statement");
        }

        return controlFlowStatement;
    }

    private void StartRecordNewLinesMode(bool mode)
    {
        RecordNewLines.Push(mode);
    }

    private void EndRecordNewLinesMode()
    {
        RecordNewLines.Pop();
    }

    private DeclarationNode ParseDeclaration()
    {
        var annotations = CollectAnnotations();

        DeclarationNode declaration;
        if (IsNextEat(TokenType.Keyword, "var"))
        {
            StartRecordNewLinesMode(true);

            declaration = ParseVariableDeclaration(annotations);

            ExpectEatSentenceEnd();

            EndRecordNewLinesMode();
        }
        else if (IsNextEat(TokenType.Keyword, "def"))
        {
            declaration = ParseFunctionDeclaration(annotations);
        }
        else if (IsNextEat(TokenType.Keyword, "new"))
        {
            declaration = ParseObjectDeclaration(annotations);
        }
        else if (IsNextEat(TokenType.Keyword, "enum"))
        {
            declaration = ParseEnumDeclaration(annotations);
        }
        else
        {
            throw NextSyntaxError("expected a declaration");
        }

        return declaration;
    }

    private EnumDeclarationNode ParseEnumDeclaration(List<AnnotationNode> annotations)
    {
        var name = ParseSingleIdentifier();

        var items = new List<EnumDeclarationItemNode>();

        ExpectEatValue(TokenType.Symbol, "{");

        while (!IsNext(TokenType.Symbol, "}"))
        {
            var itemName = ParseSingleIdentifier();

            var parameters = new List<EnumDeclarationItemParameterNode>();

            if (parameters.Any(p => p.Name.Name == itemName.Name))
            {
                throw NextSyntaxError($"parameter {itemName.Name} already exists in the enum");
            }

            if (IsNextEat(TokenType.Symbol, "("))
            {
                while (!IsNext(TokenType.Symbol, ")"))
                {
                    var parameterName = ParseSingleIdentifier();

                    ExpectEatValue(TokenType.Symbol, ":");

                    var parameterType = ParseTypeInfo();

                    parameters.Add(new EnumDeclarationItemParameterNode(parameterName.NodeContext,
                        parameterName.ToDeclarationNameNode(),
                        parameterType));

                    if (IsNextEat(TokenType.Symbol, ","))
                    {
                        continue;
                    }

                    break;
                }

                ExpectEatValue(TokenType.Symbol, ")");
            }

            var enumItem =
                new EnumDeclarationItemNode(itemName.NodeContext, itemName.ToDeclarationNameNode(), parameters);

            items.Add(enumItem);

            if (IsNextEat(TokenType.Symbol, ","))
            {
                continue;
            }

            break;
        }

        ExpectEatValue(TokenType.Symbol, "}");

        var enumDeclarationNodeContext = new NodeContext(name.NodeContext.PositionData);

        return new EnumDeclarationNode(
            enumDeclarationNodeContext,
            name.ToDeclarationNameNode(),
            items,
            annotations
        );
    }

    private AnnotationNode ParseAnnotation()
    {
        var name = ParseSingleIdentifier();

        var arguments = new List<BaseNode>();

        var usesParentheses = IsNextEat(TokenType.Symbol, "(");

        if (usesParentheses)
        {
            while (!IsNext(TokenType.Symbol, ")"))
            {
                var argument = ParseExpression();

                arguments.Add(argument);

                if (IsNextEat(TokenType.Symbol, ","))
                {
                    continue;
                }

                break;
            }

            ExpectEatValue(TokenType.Symbol, ")");
        }
        else
        {
            while (!IsSentenceEnd())
            {
                var argument = ParseExpression();

                arguments.Add(argument);

                if (IsNextEat(TokenType.Symbol, ","))
                {
                    continue;
                }

                break;
            }
        }

        var annotationNodeContext = new NodeContext(name.NodeContext.PositionData);

        return new AnnotationNode(annotationNodeContext, name, arguments);
    }

    private ExternNode ParseExternDeclaration()
    {
        if (IsNextEat(TokenType.Keyword, "var"))
        {
            return ParseExternVariableDeclaration();
        }

        if (IsNextEat(TokenType.Keyword, "def"))
        {
            return ParseExternFunctionDeclaration();
        }

        throw NextSyntaxError("expected an extern declaration. either a function or a variable");
    }

    private ExternVariableNode ParseExternVariableDeclaration()
    {
        var name = ParseSingleIdentifier();

        ExpectEatValue(TokenType.Symbol, ":");

        var type = ParseTypeInfo();

        var externVariableNodeContext = new NodeContext(name.NodeContext.PositionData);

        return new ExternVariableNode(externVariableNodeContext, name.ToDeclarationNameNode(), type);
    }

    private ExternFunctionNode ParseExternFunctionDeclaration()
    {
        var name = ParseSingleIdentifier();

        var parameters = ParseFunctionDeclarationArguments();

        TypeInfoNode? returnType = null;
        if (IsNextEat(TokenType.Symbol, "->"))
        {
            returnType = ParseTypeInfo();
        }

        var externFunctionNodeContext = new NodeContext(name.NodeContext.PositionData);

        return new ExternFunctionNode(externFunctionNodeContext, name.ToDeclarationNameNode(), parameters, returnType);
    }


    private ObjectDeclarationNode ParseObjectDeclaration(List<AnnotationNode> annotations)
    {
        var isImmediatelyInstanced = !IsNextEat(TokenType.Keyword, "type");

        // It should get the first identifier and IF after that it finds the
        // "called" identifier, it should use the first identifier as the base name
        // and the second as the class name. But if it doesn't find the "called"
        // identifier, it should use the first identifier as the class name and
        // the base name should be null.
        var firstIdentifier = ParseSingleIdentifier();

        IdentifierNode name;
        TypeInfoNameNode? baseName = null;

        var objectDeclarationNodeContext = new NodeContext(firstIdentifier.NodeContext.PositionData);
        if (IsNextEat(TokenType.Identifier, "called"))
        {
            baseName = new TypeInfoNameNode(firstIdentifier.NodeContext, firstIdentifier.Name);
            name = ParseSingleIdentifier();

            objectDeclarationNodeContext = name.NodeContext;
        }
        else
        {
            name = firstIdentifier;
        }

        ExpectEatValue(TokenType.Symbol, "{");

        var fields = new List<DeclarationNode>();

        while (!IsNext(TokenType.Symbol, "}"))
        {
            var field = ParseObjectLevelDeclaration();

            fields.Add(field);
        }

        ExpectEatValue(TokenType.Symbol, "}");

        return new ObjectDeclarationNode(
            objectDeclarationNodeContext,
            isImmediatelyInstanced,
            baseName,
            name.ToDeclarationNameNode(),
            fields,
            annotations
        );
    }

    private List<AnnotationNode> CollectAnnotations()
    {
        var annotations = new List<AnnotationNode>();

        if (IsNext(TokenType.Symbol, "@"))
        {
            StartRecordNewLinesMode(true);
            while (IsNextEat(TokenType.Symbol, "@"))
            {
                var annotation = ParseAnnotation();

                ExpectEatSentenceEnd("expected newline after annotation");

                annotations.Add(annotation);
            }

            EndRecordNewLinesMode();
        }

        return annotations;
    }

    private DeclarationNode ParseObjectLevelDeclaration()
    {
        var annotations = CollectAnnotations();

        DeclarationNode declaration;
        if (IsNextEat(TokenType.Keyword, "var"))
        {
            StartRecordNewLinesMode(true);

            declaration = ParseVariableDeclaration(annotations);

            ExpectEatSentenceEnd();

            EndRecordNewLinesMode();
        }
        else if (IsNextEat(TokenType.Keyword, "def"))
        {
            declaration = ParseFunctionDeclaration(annotations);
        }
        else if (IsNext(TokenType.Identifier))
        {
            var name = ParseSingleIdentifier();

            ExpectEatValue(TokenType.Symbol, "=");

            var value = ParseExpression();

            var objectVariableOverrideNodeContext = new NodeContext(name.NodeContext, value.NodeContext);

            declaration =
                new ObjectVariableOverride(objectVariableOverrideNodeContext, name.ToDeclarationNameNode(), value);
        }
        else
        {
            throw NextSyntaxError("expected a declaration");
        }

        return declaration;
    }

    private WhileStatementNode ParseWhileStatement()
    {
        var condition = ParseExpression();

        var body = ParseBlock();

        var whileNodeContext = new NodeContext(condition.NodeContext, body.NodeContext);

        return new WhileStatementNode(whileNodeContext, condition, body);
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

        var forNodeContext = new NodeContext(iteratorVariable.NodeContext, body.NodeContext);

        return new ForStatementNode(forNodeContext, iterable, iteratorVariable, body);
    }

    private IfStatementNode ParseIfStatement()
    {
        var condition = ParseExpression();

        var body = ParseBlock();

        IfStatementNode? nextIf = null;

        if (!IsNextEat(TokenType.Keyword, "else"))
        {
            var ifNodeContext = new NodeContext(condition.NodeContext, body.NodeContext);

            return new IfStatementNode(ifNodeContext, condition, body, nextIf);
        }

        if (IsNextEat(TokenType.Keyword, "if"))
        {
            nextIf = ParseIfStatement();
        }
        else
        {
            var elseBody = ParseBlock();

            var ifNodeContext = new NodeContext(condition.NodeContext, body.NodeContext);

            var elseNodeContext = elseBody.NodeContext;
            var elseStatement = new IfStatementNode(elseNodeContext, null, elseBody, null);

            return new IfStatementNode(ifNodeContext, condition, body, elseStatement);
        }

        return new IfStatementNode(
            new NodeContext(condition.NodeContext, nextIf.NodeContext),
            condition,
            body,
            nextIf
        );
    }

    private List<TypeInfoEnumFieldParamNode> ParseTypeInfoEnumFieldParams()
    {
        var parameters = new List<TypeInfoEnumFieldParamNode>();

        if (IsNextEat(TokenType.Symbol, "("))
        {
            while (!IsNext(TokenType.Symbol, ")"))
            {
                var parameterName = ParseSingleIdentifier();

                ExpectEatValue(TokenType.Symbol, ":");

                var parameterType = ParseTypeInfo();

                var parameterNodeContext = new NodeContext(parameterName.NodeContext, parameterType.NodeContext);

                parameters.Add(new TypeInfoEnumFieldParamNode(parameterNodeContext,
                    parameterName.ToDeclarationNameNode(), parameterType));

                if (IsNextEat(TokenType.Symbol, ","))
                {
                    continue;
                }

                break;
            }

            ExpectEatValue(TokenType.Symbol, ")");
        }

        return parameters;
    }

    private TypeInfoEnumNode ParseTypeInfoEnum(IdentifierNode firstName)
    {
        var fields = new List<TypeInfoEnumFieldNode>();


        List<TypeInfoEnumFieldParamNode> parameters = [];
        if (IsNext(TokenType.Symbol, "("))
        {
            parameters = ParseTypeInfoEnumFieldParams();
        }

        fields.Add(new TypeInfoEnumFieldNode(
            firstName.NodeContext,
            firstName.ToDeclarationNameNode(),
            parameters
        ));

        while (IsNextEat(TokenType.Symbol, "|"))
        {
            var name = ParseSingleIdentifier();

            parameters = [];

            if (IsNext(TokenType.Symbol, "("))
            {
                parameters = ParseTypeInfoEnumFieldParams();
            }

            var enumFieldNodeContext = new NodeContext(name.NodeContext.PositionData);

            fields.Add(new TypeInfoEnumFieldNode(enumFieldNodeContext, name.ToDeclarationNameNode(), parameters));
        }

        var enumNodeContext = new NodeContext(fields.First().NodeContext, fields.Last().NodeContext);

        return new TypeInfoEnumNode(enumNodeContext, fields);
    }

    private TypeInfoNode ParseTypeInfo()
    {
        if (IsNext(TokenType.Identifier))
        {
            var identifier = ParseSingleIdentifier();

            // These are possible examples or any combination of these:
            // First | Second | Third
            // First(value : string) | Second | Third(value : string)
            if (IsNext(TokenType.Symbol, "|") || IsNext(TokenType.Symbol, "("))
            {
                return ParseTypeInfoEnum(identifier);
            }

            var typeName = identifier.ToTypeInfoNameNode();

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

            var structureNodeContext =
                new NodeContext(fields.First().Value.NodeContext, fields.Last().Value.NodeContext);

            return ParseTypeInfoPart(new TypeInfoStructureNode(structureNodeContext, fields));
        }

        throw NextSyntaxError("expected type info");
    }

    private TypeInfoNode ParseTypeInfoPart(TypeInfoNode typeInfo)
    {
        if (IsNextEat(TokenType.Symbol, "["))
        {
            ExpectEatValue(TokenType.Symbol, "]");

            return ParseTypeInfoPart(new TypeInfoArrayNode(typeInfo.NodeContext, typeInfo));
        }

        if (IsNextEat(TokenType.Symbol, "?"))
        {
            return ParseTypeInfoPart(new OptionalTypeInfoNode(typeInfo.NodeContext, typeInfo));
        }

        return typeInfo;
    }

    private VariableDeclarationNode ParseVariableDeclaration(List<AnnotationNode> annotationNodes)
    {
        var name = ParseSingleIdentifier();

        TypeInfoNode? type = null;
        if (IsNextEat(TokenType.Symbol, ":"))
        {
            type = ParseTypeInfo();
        }

        BaseNode? value = null;
        if (IsNextEat(TokenType.Symbol, "="))
        {
            value = ParseExpression();
        }

        if (value == null && type == null)
        {
            throw NextSyntaxError("expected type or value");
        }

        NodeContext variableDeclarationNodeContext;

        if (value != null)
        {
            variableDeclarationNodeContext = new NodeContext(name.NodeContext, value.NodeContext);
        }
        else
        {
            variableDeclarationNodeContext = name.NodeContext;
        }

        return new VariableDeclarationNode(variableDeclarationNodeContext, name.ToDeclarationNameNode(), type, value,
            annotationNodes);
    }

    private BaseNode ParseExpressionContinue(BaseNode lhs, int minPrecedence)
    {
        var nextToken = PeekToken();

        if (!nextToken.IsOperator())
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

        BaseNode rhs;

        if (op == Operator.Is)
        {
            rhs = ParseTypeInfo();
        }
        else
        {
            rhs = ParseExpression(precedence);
        }

        var expressionNodeContext = new NodeContext(lhs.NodeContext, rhs.NodeContext);

        return new ExpressionNode(
            expressionNodeContext,
            lhs,
            rhs,
            op
        );
    }

    private BaseNode ParseExpression(int minPrecedence = 0)
    {
        if (IsNextEat(TokenType.Symbol, "("))
        {
            GetToken();
            var expression = ParseExpression();
            ExpectEatValue(TokenType.Symbol, ")");
            return expression;
        }

        if (IsNext(TokenType.Operator, "!"))
        {
            var operatorToken = GetToken();
            var op = operatorToken.ToOperator();
            var value = ParseExpression();
            var unaryExpressionNodeContext = new NodeContext(operatorToken.PositionData);
            return new UnaryExpressionNode(unaryExpressionNodeContext, op, value);
        }

        var lhs = ParsePrimary();

        if (!PeekToken().IsOperator())
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
        var nextToken = PeekToken();
        var nextNodeContext = new NodeContext(nextToken.PositionData);
        if (IsNext(TokenType.Number))
        {
            return new NumberLiteralNode(nextNodeContext, GetToken().Value);
        }

        if (IsNext(TokenType.String))
        {
            return new StringLiteralNode(nextNodeContext, GetToken().Value);
        }

        if (IsNext(TokenType.Keyword, "true") || IsNext(TokenType.Keyword, "false"))
        {
            return new BooleanLiteralNode(nextNodeContext, GetToken().Value == "true");
        }

        if (IsNextEat(TokenType.Keyword, "null"))
        {
            return new NullLiteralNode(nextNodeContext);
        }

        if (IsNext(TokenType.Symbol, "{"))
        {
            return ParseStructureLiteral();
        }

        if (IsNext(TokenType.Symbol, "["))
        {
            return ParseArrayLiteral();
        }

        if (IsNextEat(TokenType.Symbol, "."))
        {
            return ParseEnumShortHand();
        }

        if (!IsNext(TokenType.Identifier))
        {
            throw NextSyntaxError($"unexpected token {PeekToken().Type}");
        }

        var identifier = ParseSingleIdentifier();

        if (IsNextEat(TokenType.Symbol, "("))
        {
            return ParseFunctionCallWithParentheses(identifier);
        }

        if (IsNext(TokenType.Symbol, "["))
        {
            return ParseArrayAccess(identifier);
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

    private ArrayAccessNode ParseArrayAccess(IdentifierNode baseObject)
    {
        BaseNode current = baseObject;
        while (IsNextEat(TokenType.Symbol, "["))
        {
            var index = ParseExpression();

            ExpectEatValue(TokenType.Symbol, "]");

            var arrayAccessNodeContext = new NodeContext(baseObject.NodeContext, index.NodeContext);
            current = new ArrayAccessNode(arrayAccessNodeContext, current, index);
        }

        return (ArrayAccessNode)current;
    }

    private EnumShortHandNode ParseEnumShortHand()
    {
        var enumName = ParseSingleIdentifier();

        var parameters = new List<BaseNode>();
        if (IsNextEat(TokenType.Symbol, "("))
        {
            while (!IsNext(TokenType.Symbol, ")"))
            {
                var parameter = ParseExpression();

                parameters.Add(parameter);

                if (IsNextEat(TokenType.Symbol, ","))
                {
                    continue;
                }

                break;
            }

            ExpectEatValue(TokenType.Symbol, ")");
        }

        var enumShortHandNodeContext = new NodeContext(enumName.NodeContext.PositionData);

        return new EnumShortHandNode(enumShortHandNodeContext, enumName.ToDeclarationNameNode(), parameters);
    }

    private ArrayLiteralNode ParseArrayLiteral()
    {
        var elements = new List<BaseNode>();

        StartRecordNewLinesMode(true);

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

        NodeContext arrayLiteralNodeContext;

        if (elements.Count == 0)
        {
            arrayLiteralNodeContext = new NodeContext(PeekToken().PositionData);
        }
        else
        {
            arrayLiteralNodeContext = new NodeContext(elements.First().NodeContext, elements.Last().NodeContext);
        }

        EndRecordNewLinesMode();

        return new ArrayLiteralNode(arrayLiteralNodeContext, elements);
    }

    private StructureLiteralNode ParseStructureLiteral()
    {
        var fields = new List<StructureLiteralFieldNode>();

        StartRecordNewLinesMode(false);

        ExpectEatValue(TokenType.Symbol, "{");

        while (!IsNext(TokenType.Symbol, "}"))
        {
            var identifierPosition = PeekToken().PositionData;
            var fieldName = ParseSingleIdentifier();

            var fieldExists = fields.Any(f => f.Name.Name == fieldName.Name);

            if (fieldExists)
            {
                throw new CompileError.ParseError(
                    $"field {fieldName.Name} already exists in the structure",
                    identifierPosition
                );
            }

            ExpectEatValue(TokenType.Symbol, ":");

            var fieldValue = ParseExpression();

            var fieldNodeContext = new NodeContext(fieldName.NodeContext, fieldValue.NodeContext);

            fields.Add(new StructureLiteralFieldNode(fieldNodeContext, fieldName, fieldValue));

            if (IsNextEat(TokenType.Symbol, ","))
            {
                continue;
            }

            break;
        }

        ExpectEatValue(TokenType.Symbol, "}");

        EndRecordNewLinesMode();

        var structureLiteralNodeContext = new NodeContext(fields.First().NodeContext, fields.Last().NodeContext);
        return new StructureLiteralNode(structureLiteralNodeContext, fields);
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

            arguments.Add(new FunctionCallArgumentNode(expression.NodeContext, expression));

            if (IsNextEat(TokenType.Symbol, ","))
            {
                continue;
            }

            break;
        }

        ExpectEatValue(TokenType.Symbol, ")");

        return new FunctionCallNode(functionName.NodeContext, functionName, arguments);
    }

    // Function call can be used with parentheses or without
    private FunctionCallNode ParseFunctionCallWithoutParenthesis(IdentifierNode functionName)
    {
        var arguments = new List<FunctionCallArgumentNode>();

        while (!IsSentenceEnd() && !IsNext(TokenType.EndOfFile))
        {
            var expression = ParseExpression();

            arguments.Add(new FunctionCallArgumentNode(expression.NodeContext, expression));

            if (IsNextEat(TokenType.Symbol, ","))
            {
                continue;
            }

            break;
        }

        var functionCallNodeContext = new NodeContext(functionName.NodeContext, arguments.Last().NodeContext);
        return new FunctionCallNode(functionCallNodeContext, functionName, arguments);
    }

    private MemberAccessNode ParseMemberAccess(IdentifierNode baseObject)
    {
        while (true)
        {
            BaseNode member = ParseSingleIdentifier();

            var memberAccessNodeContext = new NodeContext(baseObject.NodeContext, member.NodeContext);
            var memberAccess = new MemberAccessNode(memberAccessNodeContext, baseObject, member);

            if (IsNextEat(TokenType.Symbol, "."))
            {
                baseObject = memberAccess;
                continue;
            }

            if (member is IdentifierNode identifierMember && IsNextEat(TokenType.Symbol, "("))
            {
                member = ParseFunctionCallWithParentheses(identifierMember);
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

        var identifierNodeContext = new NodeContext(PeekToken().PositionData);

        return new IdentifierNode(identifierNodeContext, GetToken().Value);
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

    private DeclarationNode ParseProgramLevelDeclaration()
    {
        if (IsNextEat(TokenType.Keyword, "extern"))
        {
            StartRecordNewLinesMode(true);

            var externDeclaration = ParseExternDeclaration();

            ExpectEatSentenceEnd();

            EndRecordNewLinesMode();

            return externDeclaration;
        }

        return ParseDeclaration();
    }

    public ProgramNode Parse()
    {
        var declarations = new List<DeclarationNode>();

        var topLevelDeclarationsOver = false;

        while (!IsEnd())
        {
            var declaration = ParseProgramLevelDeclaration();

            if (declaration is ExternNode)
            {
                if (topLevelDeclarationsOver)
                {
                    throw NextSyntaxError("extern declarations should be at the top of the file");
                }
            }
            else
            {
                topLevelDeclarationsOver = true;
            }

            declarations.Add(declaration);
        }

        var programNodeContext = new NodeContext(declarations.First().NodeContext, declarations.Last().NodeContext);
        return new ProgramNode(programNodeContext, declarations);
    }

    private bool IsEnd()
    {
        return Lexer.IsEnd() || PeekToken().Is(TokenType.EndOfFile);
    }
}