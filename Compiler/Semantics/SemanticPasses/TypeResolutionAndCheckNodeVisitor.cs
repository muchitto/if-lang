using Compiler.ErrorHandling;
using Compiler.Semantics.TypeInference;
using Compiler.Semantics.TypeInformation;
using Compiler.Semantics.TypeInformation.TypeComparer;
using Compiler.Semantics.TypeInformation.Types;
using Compiler.Syntax;
using Compiler.Syntax.Nodes;

namespace Compiler.Semantics.SemanticPasses;

public class TypeResolutionAndCheckNodeVisitor(SemanticContext semanticContext, SemanticHandler semanticHandler)
    : SemanticPassBaseNodeVisitor(semanticContext, semanticHandler)
{
    public override ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        SemanticHandler.RecallNodeScope(programNode);

        base.VisitProgramNode(programNode);

        SemanticHandler.PopScope();

        return programNode;
    }

    public override ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        if (objectDeclarationNode.BaseName != null)
        {
            VisitTypeNameNode(objectDeclarationNode.BaseName);
        }

        SemanticHandler.RecallNodeScope(objectDeclarationNode);

        var name = objectDeclarationNode.Name.Name;

        if (objectDeclarationNode.TypeRef.TypeInfo is not ObjectTypeInfo objectTypeInfo)
        {
            throw new CompileError.SemanticError(
                "object type info is null",
                objectDeclarationNode.NodeContext.PositionData
            );
        }

        foreach (var declarationNode in objectDeclarationNode.Fields)
        {
            var visitedDeclarationNode = VisitDeclarationNode(declarationNode);

            if (visitedDeclarationNode.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError(
                    $"field {visitedDeclarationNode.Name.Name} type not resolved",
                    visitedDeclarationNode.NodeContext.PositionData
                );
            }

            objectTypeInfo.Fields[visitedDeclarationNode.Name.Name] = visitedDeclarationNode.TypeRef;
        }

        SemanticHandler.PopScope();

        if (objectDeclarationNode.TypeRef.Compare(TypeInfo.Unknown))
        {
            throw new CompileError.SemanticError(
                $"object {objectDeclarationNode.Name.Name} type not resolved",
                objectDeclarationNode.NodeContext.PositionData
            );
        }

        return objectDeclarationNode;
    }

    public override TypeInfoNameNode VisitTypeNameNode(TypeInfoNameNode typeInfoNameNode)
    {
        if (SemanticHandler.TryGetInNodeScope(typeInfoNameNode.Name, SymbolType.Type, out var nodeScopeSymbol))
        {
            typeInfoNameNode.TypeRef = nodeScopeSymbol.TypeRef;

            return typeInfoNameNode;
        }

        if (TypeInfo.TryGetBuiltInType(typeInfoNameNode.Name, out var typeInfo))
        {
            typeInfoNameNode.TypeRef.TypeInfo = typeInfo;
        }
        else
        {
            if (SemanticHandler.TryLookupType(typeInfoNameNode.Name, out var symbol))
            {
                typeInfoNameNode.TypeRef.TypeInfo = symbol.TypeRef.TypeInfo;
            }
            else
            {
                throw new CompileError.SemanticError(
                    $"type {typeInfoNameNode.Name} not found",
                    typeInfoNameNode.NodeContext.PositionData
                );
            }
        }

        return typeInfoNameNode;
    }

    public override ArrayLiteralNode VisitArrayLiteralNode(ArrayLiteralNode arrayLiteralNode)
    {
        if (arrayLiteralNode.Elements.Count <= 0)
        {
            arrayLiteralNode.TypeRef.TypeInfo = new GenericTypeInfo("Array", [new TypeRef(TypeInfo.Deferred)]);

            return base.VisitArrayLiteralNode(arrayLiteralNode);
        }

        var firstElementTypeRef = VisitBaseNode(arrayLiteralNode.Elements[0]).TypeRef;

        if (firstElementTypeRef.Compare(TypeInfo.Unknown))
        {
            throw new CompileError.SemanticError(
                "array element type not resolved",
                arrayLiteralNode.NodeContext.PositionData
            );
        }

        foreach (var element in arrayLiteralNode.Elements)
        {
            var visitedElement = VisitBaseNode(element);

            if (visitedElement.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError(
                    "array element type not resolved",
                    visitedElement.NodeContext.PositionData
                );
            }

            if (!visitedElement.TypeRef.Compare(firstElementTypeRef))
            {
                throw new CompileError.SemanticError(
                    "array element type mismatch",
                    visitedElement.NodeContext.PositionData
                );
            }
        }

        arrayLiteralNode.TypeRef.TypeInfo = new GenericTypeInfo("Array", [firstElementTypeRef]);

        return base.VisitArrayLiteralNode(arrayLiteralNode);
    }

    public override TypeInfoArrayNode VisitTypeInfoArrayNode(TypeInfoArrayNode typeInfoArrayNode)
    {
        base.VisitTypeInfoArrayNode(typeInfoArrayNode);

        typeInfoArrayNode.TypeRef.TypeInfo = new GenericTypeInfo("Array", [typeInfoArrayNode.BaseType.TypeRef]);

        return typeInfoArrayNode;
    }

    public override StructureLiteralNode VisitStructureLiteralNode(StructureLiteralNode structureLiteralNode)
    {
        var fields = new Dictionary<string, TypeRef>();
        foreach (var field in structureLiteralNode.Fields)
        {
            var visitedField = VisitBaseNode(field.Field);

            if (visitedField.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError(
                    $"field {field.Name.Name} type not resolved",
                    field.NodeContext.PositionData
                );
            }

            fields.Add(field.Name.Name, visitedField.TypeRef);
        }

        structureLiteralNode.TypeRef.TypeInfo = new StructureTypeInfo(fields);

        return base.VisitStructureLiteralNode(structureLiteralNode);
    }

    public override StructureLiteralFieldNode VisitStructureLiteralFieldNode(
        StructureLiteralFieldNode structureLiteralFieldNode)
    {
        structureLiteralFieldNode.Field.Accept(this);

        structureLiteralFieldNode.TypeRef = structureLiteralFieldNode.Field.TypeRef;
        structureLiteralFieldNode.Name.TypeRef = structureLiteralFieldNode.TypeRef;

        return structureLiteralFieldNode;
    }

    public override TypeInfoStructureNode VisitTypeInfoStructureNode(TypeInfoStructureNode typeInfoStructureNode)
    {
        var fields = new Dictionary<string, TypeRef>();
        foreach (var field in typeInfoStructureNode.Fields)
        {
            var visitedField = VisitTypeInfoNode(field.Value);

            if (visitedField.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError(
                    $"field {field.Key} type not resolved",
                    visitedField.NodeContext.PositionData
                );
            }

            fields.Add(field.Key, visitedField.TypeRef);
        }

        typeInfoStructureNode.TypeRef.TypeInfo = new StructureTypeInfo(fields);

        return base.VisitTypeInfoStructureNode(typeInfoStructureNode);
    }

    public override FunctionDeclarationParameterNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationParameterNode functionDeclarationParameterNode)
    {
        if (!functionDeclarationParameterNode.TypeRef.Compare(TypeInfo.Unknown))
        {
            return base.VisitFunctionDeclarationParameterNode(functionDeclarationParameterNode);
        }

        var visitedTypeNode = VisitTypeInfoNode(functionDeclarationParameterNode.TypeInfoNode);

        functionDeclarationParameterNode.TypeRef = visitedTypeNode.TypeRef;

        SemanticHandler.SetSymbol(
            new Symbol(
                functionDeclarationParameterNode.Name.Name,
                SemanticHandler.CurrentScope,
                functionDeclarationParameterNode,
                SymbolType.Identifier
            ),
            !SemanticHandler.CanSetAlreadyDeclaredSymbol
        );

        return functionDeclarationParameterNode;
    }

    public override ObjectVariableOverride VisitObjectVariableOverride(ObjectVariableOverride objectVariableOverride)
    {
        if (!semanticHandler.TryGetScopeOfType(ScopeType.Object, out var objectScope) ||
            objectScope.AttachedNode is not ObjectDeclarationNode objectDeclarationNode)
        {
            throw new CompileError.SemanticError(
                "object variable override not in object scope",
                objectVariableOverride.NodeContext.PositionData
            );
        }

        objectVariableOverride.Value.Accept(this);

        if (SemanticHandler.TryLookupIdentifier(objectVariableOverride.Name.Name, out var variableSymbol))
        {
            if (variableSymbol.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError(
                    $"object variable override {objectVariableOverride.Name.Name} type not resolved",
                    objectVariableOverride.NodeContext.PositionData
                );
            }

            if (!variableSymbol.TypeRef.Compare(objectVariableOverride.Value.TypeRef))
            {
                throw new CompileError.SemanticError(
                    $"object variable override {objectVariableOverride.Name.Name} type mismatch",
                    objectVariableOverride.NodeContext.PositionData
                );
            }

            objectVariableOverride.TypeRef = variableSymbol.TypeRef;
            objectVariableOverride.Name.TypeRef = variableSymbol.TypeRef;
        }
        else
        {
            throw new CompileError.SemanticError(
                $"object variable override {objectVariableOverride.Name.Name} not found",
                objectVariableOverride.NodeContext.PositionData
            );
        }

        return objectVariableOverride;
    }

    public override IdentifierNode VisitIdentifierNode(IdentifierNode identifierNode)
    {
        if (identifierNode.Name == "this")
        {
            if (!SemanticHandler.TryGetScopeOfType(ScopeType.Object, out var objectScope))
            {
                throw new CompileError.SemanticError(
                    "this can only be used in object scope",
                    identifierNode.NodeContext.PositionData
                );
            }

            identifierNode.TypeRef = objectScope.AttachedNode.TypeRef;

            return identifierNode;
        }

        if (identifierNode.Name == "super")
        {
            if (!SemanticHandler.TryGetScopeOfType(ScopeType.Object, out var objectScope))
            {
                throw new CompileError.SemanticError(
                    "super can only be used in object scope",
                    identifierNode.NodeContext.PositionData
                );
            }

            if (objectScope.AttachedNode.TypeRef.TypeInfo is not ObjectTypeInfo objectTypeInfo)
            {
                throw new CompileError.SemanticError(
                    "super can only be used in object scope",
                    identifierNode.NodeContext.PositionData
                );
            }

            if (objectTypeInfo.BaseClass == null)
            {
                throw new CompileError.SemanticError(
                    "object has no base class",
                    identifierNode.NodeContext.PositionData
                );
            }

            identifierNode.TypeRef = objectTypeInfo.BaseClass;

            return identifierNode;
        }

        if (SemanticHandler.TryLookupIdentifier(identifierNode.Name, out var symbol))
        {
            identifierNode.TypeRef = symbol.TypeRef;
        }
        else
        {
            throw new CompileError.SemanticError(
                $"identifier {identifierNode.Name} not found",
                identifierNode.NodeContext.PositionData
            );
        }

        return identifierNode;
    }

    public override EnumDeclarationNode VisitEnumDeclarationNode(EnumDeclarationNode enumDeclarationNode)
    {
        if (!enumDeclarationNode.TypeRef.Compare(TypeInfo.Unknown))
        {
            foreach (var item in enumDeclarationNode.Items)
            {
                VisitEnumDeclarationItemNode(item);
            }

            return enumDeclarationNode;
        }

        throw new NotImplementedException();

        return base.VisitEnumDeclarationNode(enumDeclarationNode);
    }

    public override EnumDeclarationItemNode VisitEnumDeclarationItemNode(
        EnumDeclarationItemNode enumDeclarationItemNode)
    {
        if (!enumDeclarationItemNode.TypeRef.Compare(TypeInfo.Unknown))
        {
            foreach (var parameter in enumDeclarationItemNode.ParameterNodes)
            {
                VisitEnumDeclarationItemParameterNode(parameter);
            }

            return enumDeclarationItemNode;
        }

        throw new NotImplementedException();
    }

    public override EnumDeclarationItemParameterNode VisitEnumDeclarationItemParameterNode(
        EnumDeclarationItemParameterNode enumDeclarationItemParameterNode)
    {
        if (!enumDeclarationItemParameterNode.TypeRef.Compare(TypeInfo.Unknown))
        {
            return base.VisitEnumDeclarationItemParameterNode(enumDeclarationItemParameterNode);
        }

        throw new NotImplementedException();

        return base.VisitEnumDeclarationItemParameterNode(enumDeclarationItemParameterNode);
    }

    public override EnumShortHandNode VisitEnumShortHandNode(EnumShortHandNode enumShortHandNode)
    {
        throw new NotImplementedException();

        return base.VisitEnumShortHandNode(enumShortHandNode);
    }

    public override ExternFunctionNode VisitExternFunctionNode(ExternFunctionNode externFunctionNode)
    {
        return externFunctionNode;
    }

    public override IfStatementNode VisitIfStatementNode(IfStatementNode ifStatementNode)
    {
        if (ifStatementNode.Expression != null)
        {
            VisitBaseNode(ifStatementNode.Expression);

            if (!ifStatementNode.Expression.TypeRef.Compare<ExpressionComparer>(TypeInfo.Boolean))
            {
                throw new CompileError.SemanticError(
                    "if statement condition must be boolean",
                    ifStatementNode.Expression.NodeContext.PositionData
                );
            }
        }

        return ifStatementNode;
    }

    public override UnaryExpressionNode VisitUnaryExpressionNode(UnaryExpressionNode unaryExpressionNode)
    {
        VisitBaseNode(unaryExpressionNode.Value);

        unaryExpressionNode.TypeRef = unaryExpressionNode.Value.TypeRef;

        return base.VisitUnaryExpressionNode(unaryExpressionNode);
    }

    public override ExpressionNode VisitExpressionNode(ExpressionNode expressionNode)
    {
        var visitedLeft = VisitBaseNode(expressionNode.Left);
        var visitedRight = VisitBaseNode(expressionNode.Right);

        if (expressionNode.Operator == Operator.Is)
        {
            // TODO: Need to implement a IS comparer so
            // we can for instance check if a type is a subtype of another

            // We want to check that the right side is the current type OR a subtype of the current type

            if (!visitedLeft.TypeRef.Compare<IsComparer>(visitedRight.TypeRef))
            {
                throw new CompileError.SemanticError(
                    "expression type mismatch",
                    expressionNode.NodeContext.PositionData
                );
            }

            expressionNode.TypeRef = new TypeRef(TypeInfo.Boolean);

            return expressionNode;
        }

        if (visitedLeft.TypeRef.Compare<ExpressionComparer>(TypeInfo.Void) ||
            visitedRight.TypeRef.Compare<ExpressionComparer>(TypeInfo.Void))
        {
            throw new CompileError.SemanticError(
                "cannot use void in expression",
                expressionNode.NodeContext.PositionData
            );
        }

        if (!visitedLeft.TypeRef.Compare<ExpressionComparer>(visitedRight.TypeRef))
        {
            throw new CompileError.SemanticError(
                "expression type mismatch",
                expressionNode.NodeContext.PositionData
            );
        }

        expressionNode.TypeRef = visitedLeft.TypeRef;

        return expressionNode;
    }

    public override FunctionCallNode VisitFunctionCallNode(FunctionCallNode functionCallNode)
    {
        var visitedName = VisitIdentifierNode(functionCallNode.Name);

        if (visitedName.TypeRef.TypeInfo is not FunctionTypeInfo functionTypeInfo)
        {
            throw new CompileError.SemanticError(
                $"trying to call non-function {functionCallNode.Name.Name}",
                functionCallNode.NodeContext.PositionData
            );
        }

        if (functionCallNode.Arguments.Count != functionTypeInfo.Parameters.Count)
        {
            throw new CompileError.SemanticError(
                $"function {functionCallNode.Name.Name} requires {functionTypeInfo.Parameters.Count} arguments, you gave {functionCallNode.Arguments.Count}",
                functionCallNode.NodeContext.PositionData
            );
        }

        for (var a = 0; a < functionCallNode.Arguments.Count; a++)
        {
            var argument = functionCallNode.Arguments[a];
            var argumentNode = VisitFunctionCallArgumentNode(argument);

            if (functionTypeInfo.Parameters.Count <= a)
            {
                throw new CompileError.SemanticError(
                    $"too many arguments for function {functionCallNode.Name.Name}",
                    functionCallNode.NodeContext.PositionData
                );
            }

            var (functionSignatureArgumentName, functionSignatureArgumentType) =
                functionTypeInfo.Parameters.ElementAt(a);

            if (!argumentNode.TypeRef.Compare(functionSignatureArgumentType))
            {
                throw new CompileError.SemanticError(
                    $"argument {functionSignatureArgumentName} type mismatch",
                    functionCallNode.NodeContext.PositionData
                );
            }
        }

        functionCallNode.TypeRef = functionTypeInfo.ReturnType;

        return functionCallNode;
    }

    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode)
    {
        TypeRef? typeRef = null;

        if (variableDeclarationNode.TypeName != null)
        {
            var visitedTypeName = VisitTypeInfoNode(variableDeclarationNode.TypeName);

            typeRef = visitedTypeName.TypeRef;

            variableDeclarationNode.TypeRef = typeRef;
        }

        if (variableDeclarationNode.Value != null)
        {
            var visitedValue = VisitBaseNode(variableDeclarationNode.Value);

            if (visitedValue.TypeRef.TypeInfo == TypeInfo.Void)
            {
                throw new CompileError.SemanticError(
                    "cannot assign void to variable"
                    ,
                    variableDeclarationNode.NodeContext.PositionData
                );
            }

            if (visitedValue.TypeRef.TypeInfo.HasDeferredTypes())
            {
                if (typeRef == null)
                {
                    throw new CompileError.SemanticError(
                        "variable type cannot be resolved",
                        variableDeclarationNode.NodeContext.PositionData
                    );
                }

                var inferenceVisitor = new InferenceVisitor();
                inferenceVisitor.VisitVariableDeclarationNode(variableDeclarationNode);
            }

            if (typeRef != null && !typeRef.Compare(visitedValue.TypeRef))
            {
                throw new CompileError.SemanticError(
                    "variable type mismatch",
                    variableDeclarationNode.NodeContext.PositionData
                );
            }

            if (typeRef == null)
            {
                typeRef = visitedValue.TypeRef;
            }
        }

        if (typeRef == null)
        {
            throw new CompileError.SemanticError(
                "variable type not resolved",
                variableDeclarationNode.NodeContext.PositionData
            );
        }

        variableDeclarationNode.TypeRef = typeRef;

        SemanticHandler.SetSymbol(
            new Symbol(
                variableDeclarationNode.Name.Name,
                SemanticHandler.CurrentScope,
                variableDeclarationNode,
                SymbolType.Identifier
            ),
            !SemanticHandler.CanSetAlreadyDeclaredSymbol
        );

        return variableDeclarationNode;
    }

    public override AssignmentNode VisitAssignmentNode(AssignmentNode assignmentNode)
    {
        base.VisitAssignmentNode(assignmentNode);

        // Check for void assignments in one consolidated block
        if (assignmentNode.Name.TypeRef.TypeInfo == TypeInfo.Void ||
            assignmentNode.Value.TypeRef.TypeInfo == TypeInfo.Void)
        {
            var errorMessage = assignmentNode.Name.TypeRef.TypeInfo == TypeInfo.Void
                ? "cannot assign to void"
                : "cannot assign void to variable";
            throw new CompileError.SemanticError(
                errorMessage,
                assignmentNode.NodeContext.PositionData
            );
        }

        if (assignmentNode.Name.TypeRef.TypeInfo is ObjectTypeInfo && assignmentNode.Value is NullLiteralNode)
        {
            assignmentNode.Value.TypeRef = assignmentNode.Name.TypeRef;
        }

        if (!assignmentNode.Name.TypeRef.Compare(assignmentNode.Value.TypeRef))
        {
            throw new CompileError.SemanticError(
                $"assignment type mismatch, expected {assignmentNode.Name.TypeRef.TypeInfo}, got {assignmentNode.Value.TypeRef.TypeInfo}",
                assignmentNode.NodeContext.PositionData
            );
        }

        assignmentNode.TypeRef = assignmentNode.Value.TypeRef;

        return assignmentNode;
    }

    public override MemberAccessNode VisitMemberAccessNode(MemberAccessNode memberAccessNode)
    {
        VisitBaseNode(memberAccessNode.BaseObject);

        if (memberAccessNode.BaseObject.TypeRef.TypeInfo is not ObjectTypeInfo objectTypeInfo)
        {
            throw new CompileError.SemanticError(
                "trying to access member of non-object",
                memberAccessNode.NodeContext.PositionData
            );
        }

        var name = "";

        if (memberAccessNode.Member is IdentifierNode identifierNode)
        {
            name = identifierNode.Name;
        }
        else if (memberAccessNode.Member is FunctionCallNode functionCallNode)
        {
            name = functionCallNode.Name.Name;
        }
        else
        {
            throw new CompileError.SemanticError(
                "member access node member is not identifier or function call",
                memberAccessNode.NodeContext.PositionData
            );
        }

        if (!objectTypeInfo.Fields.TryGetValue(name, out var typeRef))
        {
            throw new CompileError.SemanticError(
                $"object {objectTypeInfo.Name} has no member {name}",
                memberAccessNode.NodeContext.PositionData
            );
        }

        memberAccessNode.Member.TypeRef = typeRef;

        memberAccessNode.TypeRef = typeRef;

        return memberAccessNode;
    }

    public override ReturnStatementNode VisitReturnStatementNode(ReturnStatementNode returnStatementNode)
    {
        base.VisitReturnStatementNode(returnStatementNode);

        if (!semanticHandler.TryGetScopeOfType(ScopeType.Function, out var functionScope)
            || functionScope.AttachedNode is not FunctionDeclarationNode functionDeclarationNode
            || functionScope.AttachedNode.TypeRef.TypeInfo is not FunctionTypeInfo functionTypeInfo)
        {
            throw new CompileError.SemanticError(
                "return statement not in function scope",
                returnStatementNode.NodeContext.PositionData
            );
        }

        functionScope.ReturnStatementFound = true;

        returnStatementNode.TypeRef = returnStatementNode.Value?.TypeRef ?? new TypeRef(TypeInfo.Void);

        if (functionTypeInfo.ReturnType.TypeInfo == TypeInfo.Void && returnStatementNode.Value != null)
        {
            throw new CompileError.SemanticError(
                "returning value in void function",
                returnStatementNode.NodeContext.PositionData
            );
        }

        if (!functionTypeInfo.ReturnType.Compare(returnStatementNode.TypeRef))
        {
            if (functionTypeInfo.ReturnType.TypeInfo != TypeInfo.Void && returnStatementNode.Value == null)
            {
                throw new CompileError.SemanticError(
                    $"function {functionDeclarationNode.Name.Name} requires a return value of type {functionTypeInfo.ReturnType.TypeInfo}",
                    returnStatementNode.NodeContext.PositionData
                );
            }

            throw new CompileError.SemanticError(
                $"return type mismatch, expected {functionTypeInfo.ReturnType.TypeInfo}, got {returnStatementNode.TypeRef.TypeInfo}",
                returnStatementNode.NodeContext.PositionData
            );
        }

        return returnStatementNode;
    }

    public override FunctionCallArgumentNode VisitFunctionCallArgumentNode(
        FunctionCallArgumentNode functionCallArgumentNode)
    {
        var visitedValue = VisitBaseNode(functionCallArgumentNode.Value);

        functionCallArgumentNode.TypeRef = visitedValue.TypeRef;

        return functionCallArgumentNode;
    }

    public override NumberLiteralNode VisitNumberNode(NumberLiteralNode numberLiteralNode)
    {
        if (numberLiteralNode.Number.Contains('.'))
        {
            numberLiteralNode.TypeRef = new TypeRef(TypeInfo.Float);
        }
        else
        {
            numberLiteralNode.TypeRef = new TypeRef(TypeInfo.Int);
        }

        return numberLiteralNode;
    }

    public override StringLiteralNode VisitStringLiteralNode(StringLiteralNode stringLiteralNode)
    {
        stringLiteralNode.TypeRef = new TypeRef(TypeInfo.String);

        return base.VisitStringLiteralNode(stringLiteralNode);
    }

    public override BooleanLiteralNode VisitBooleanLiteralNode(BooleanLiteralNode booleanLiteralNode)
    {
        booleanLiteralNode.TypeRef = new TypeRef(TypeInfo.Boolean);

        return base.VisitBooleanLiteralNode(booleanLiteralNode);
    }

    public override FunctionDeclarationNode VisitFunctionDeclarationNode(
        FunctionDeclarationNode functionDeclarationNode)
    {
        if (functionDeclarationNode.Scope == null)
        {
            throw new CompileError.SemanticError(
                $"function {functionDeclarationNode.Name.Name} has no scope",
                functionDeclarationNode.NodeContext.PositionData
            );
        }

        if (SemanticHandler.InScopeType(ScopeType.Function))
        {
            SemanticHandler.NewScope(ScopeType.Function, functionDeclarationNode);
        }
        else
        {
            SemanticHandler.RecallNodeScope(functionDeclarationNode);
        }

        var parameters = new Dictionary<string, TypeRef>();
        foreach (var parameterNode in functionDeclarationNode.ParameterNodes)
        {
            VisitFunctionDeclarationParameterNode(parameterNode);

            if (parameterNode.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError(
                    $"function parameter {parameterNode.Name.Name} type not resolved",
                    parameterNode.NodeContext.PositionData
                );
            }

            parameters.Add(parameterNode.Name.Name, parameterNode.TypeRef);
        }

        var visitedBlockNode = VisitBodyBlockNode(functionDeclarationNode.Body);

        SemanticHandler.PopScope();

        if (functionDeclarationNode.ReturnTypeInfo != null)
        {
            VisitTypeInfoNode(functionDeclarationNode.ReturnTypeInfo);

            if (functionDeclarationNode.ReturnTypeInfo.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError(
                    "function return type not resolved",
                    functionDeclarationNode.ReturnTypeInfo.NodeContext.PositionData
                );
            }
        }

        var returnTypeRef = functionDeclarationNode.ReturnTypeInfo?.TypeRef ?? new TypeRef(TypeInfo.Void);

        functionDeclarationNode.TypeRef.TypeInfo = new FunctionTypeInfo(returnTypeRef, parameters);

        if (!returnTypeRef.Compare(TypeInfo.Void))
        {
            if (visitedBlockNode.Statements.Count > 0 && visitedBlockNode.Statements.Last() is not ReturnStatementNode)
            {
                throw new CompileError.SemanticError(
                    $"last statement in function {functionDeclarationNode.Name.Name} must be a return statement",
                    functionDeclarationNode.NodeContext.PositionData
                );
            }

            if (!functionDeclarationNode.Scope.ReturnStatementFound)
            {
                throw new CompileError.SemanticError(
                    $"function {functionDeclarationNode.Name.Name} requires a return statement",
                    functionDeclarationNode.NodeContext.PositionData
                );
            }
        }

        SemanticHandler.SetSymbol(
            new Symbol(
                functionDeclarationNode.Name.Name,
                SemanticHandler.CurrentScope,
                functionDeclarationNode,
                SymbolType.Identifier
            ),
            !SemanticHandler.CanSetAlreadyDeclaredSymbol
        );

        return functionDeclarationNode;
    }

    public override BodyBlockNode VisitBodyBlockNode(BodyBlockNode bodyBlockNode)
    {
        SemanticHandler.NewScope(ScopeType.BlockBody, bodyBlockNode);

        foreach (var statement in bodyBlockNode.Statements)
        {
            var visitedStatement = VisitBaseNode(statement);
        }

        SemanticHandler.PopScope();

        return bodyBlockNode;
    }
}