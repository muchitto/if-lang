using Compiler.ErrorHandling;
using Compiler.Semantics.TypeInference;
using Compiler.Semantics.TypeInformation;
using Compiler.Semantics.TypeInformation.TypeComparer;
using Compiler.Semantics.TypeInformation.Types;
using Compiler.Syntax;
using Compiler.Syntax.Nodes;
using Compiler.Syntax.Nodes.TypeInfoNodes;

namespace Compiler.Semantics.SemanticPasses;

public class TypeResolutionNodeVisitor(SemanticContext semanticContext)
    : SemanticPassBaseNodeVisitor(semanticContext)
{
    // The current declarations we are resolving, so that we do not get into infinite loops
    // when resolving types.
    private readonly Stack<DeclarationNode> _currentDeclarations = [];

    public string CurrentFunctionName { get; private set; } = string.Empty;
    public string CurrentObjectName { get; private set; } = string.Empty;

    public override ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        RecallNodeScope(programNode);

        base.VisitProgramNode(programNode);

        PopScope();

        return programNode;
    }

    public override OptionalTypeInfoNode VisitOptionalTypeInfoNode(OptionalTypeInfoNode optionalTypeInfoNode)
    {
        optionalTypeInfoNode.TypeRef.TypeInfo =
            new GenericTypeInfo("Optional", [optionalTypeInfoNode.TypeInfo.TypeRef]);

        return base.VisitOptionalTypeInfoNode(optionalTypeInfoNode);
    }

    private bool IsCurrentlyResolving(DeclarationNode declarationNode)
    {
        if (!_currentDeclarations.Contains(declarationNode))
        {
            _currentDeclarations.Push(declarationNode);

            return false;
        }

        return true;
    }

    public void FreeCurrentResolving()
    {
        _currentDeclarations.Pop();
    }

    public override ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        AddObjectDeclarationNode(objectDeclarationNode);

        if (objectDeclarationNode.BaseName != null)
        {
            VisitReferenceNameNode(objectDeclarationNode.BaseName);
        }

        RecallNodeScope(objectDeclarationNode);

        if (objectDeclarationNode.TypeRef.TypeInfo is not ObjectTypeInfo objectTypeInfo)
        {
            throw new CompileError.SemanticError(
                "object type info is null",
                objectDeclarationNode.NodeContext.PositionData
            );
        }

        foreach (var variableDeclarationNode in objectDeclarationNode.Fields.OfType<VariableDeclarationNode>())
        {
            var variableName = variableDeclarationNode.Named.Name;

            VisitVariableDeclarationNode(variableDeclarationNode);

            objectTypeInfo.Fields[variableName] = variableDeclarationNode.TypeRef;
        }

        var notVariableDeclaration = objectDeclarationNode.Fields.Where(
            d => d is not VariableDeclarationNode);

        foreach (var functionDeclarationNode in notVariableDeclaration)
        {
            var visitedDeclarationNode = VisitDeclarationNode(functionDeclarationNode);

            if (visitedDeclarationNode.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError(
                    $"field {visitedDeclarationNode.Named.Name} type not resolved",
                    visitedDeclarationNode.NodeContext.PositionData
                );
            }

            objectTypeInfo.Fields[visitedDeclarationNode.Named.Name] = visitedDeclarationNode.TypeRef;
        }

        PopScope();

        if (objectDeclarationNode.TypeRef.Compare(TypeInfo.Unknown))
        {
            throw new CompileError.SemanticError(
                $"object {objectDeclarationNode.Named.Name} type not resolved",
                objectDeclarationNode.NodeContext.PositionData
            );
        }

        PopObjectDeclarationNode();

        return objectDeclarationNode;
    }

    /*
    public override TypeInfoNameNode VisitTypeInfoNameNode(TypeInfoNameNode typeInfoNameNode)
    {
        if (TryGetInNodeScope(typeInfoNameNode.Name, SymbolType.Type, out var nodeScopeSymbol))
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
            if (TryLookupType(typeInfoNameNode.Name, out var symbol))
            {
                if (symbol.TypeRef.TypeInfo.IsUnknown)
                {
                    if (symbol.TypeRef.TypeInfo is AbstractStructuralTypeInfo abstractStructuralTypeInfo)
                    {
                        NewScope(ScopeType.Object, symbol.Node);

                        foreach (var (name, typeRef) in abstractStructuralTypeInfo.Fields)
                        {
                            if (!typeRef.TypeInfo.IsUnknown)
                            {
                                continue;
                            }

                            if (!TryLookupIdentifier(name, out symbol))
                            {
                                throw new CompileError.SemanticError(
                                    $"field {name} not found",
                                    typeInfoNameNode.NodeContext.PositionData
                                );
                            }

                            RecallScope(symbol.Scope);

                            VisitBaseNode(symbol.Node);

                            PopScope();

                            if (symbol.TypeRef.TypeInfo.IsUnknown)
                            {
                                throw new CompileError.SemanticError(
                                    $"field {name} type not resolved",
                                    typeInfoNameNode.NodeContext.PositionData
                                );
                            }
                        }

                        PopScope();
                    }


                    VisitBaseNode(symbol.Node);
                }

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
    */

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
        var scope = NewScope(ScopeType.Object, structureLiteralNode);

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

        structureLiteralNode.TypeRef.TypeInfo =
            new StructureTypeInfo(
                scope,
                fields
            );

        PopScope();

        return base.VisitStructureLiteralNode(structureLiteralNode);
    }

    public override StructureLiteralFieldNode VisitStructureLiteralFieldNode(
        StructureLiteralFieldNode structureLiteralFieldNode
    )
    {
        structureLiteralFieldNode.Field.Accept(this);

        structureLiteralFieldNode.TypeRef = structureLiteralFieldNode.Field.TypeRef;
        structureLiteralFieldNode.Name.TypeRef = structureLiteralFieldNode.TypeRef;

        return structureLiteralFieldNode;
    }

    public override TypeInfoStructureNode VisitTypeInfoStructureNode(TypeInfoStructureNode typeInfoStructureNode)
    {
        var scope = NewScope(ScopeType.Object, typeInfoStructureNode);
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

        typeInfoStructureNode.TypeRef.TypeInfo = new StructureTypeInfo(scope, fields);

        PopScope();

        return base.VisitTypeInfoStructureNode(typeInfoStructureNode);
    }

    public override FunctionDeclarationParameterNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationParameterNode functionDeclarationParameterNode
    )
    {
        if (!functionDeclarationParameterNode.TypeRef.Compare(TypeInfo.Unknown))
        {
            return base.VisitFunctionDeclarationParameterNode(functionDeclarationParameterNode);
        }

        var visitedTypeNode = VisitTypeInfoNode(functionDeclarationParameterNode.TypeInfoNode);

        functionDeclarationParameterNode.TypeRef = visitedTypeNode.TypeRef;

        SetSymbol(
            new Symbol(
                functionDeclarationParameterNode.Named.Name,
                CurrentScope,
                functionDeclarationParameterNode,
                SymbolType.Identifier
            ),
            !CanSetAlreadyDeclaredSymbol
        );

        return functionDeclarationParameterNode;
    }

    public override ObjectVariableOverride VisitObjectVariableOverride(ObjectVariableOverride objectVariableOverride)
    {
        if (!TryGetScopeOfType(ScopeType.Object, out var objectScope) ||
            objectScope.AttachedNode is not ObjectDeclarationNode)
        {
            throw new CompileError.SemanticError(
                "object variable override not in object scope",
                objectVariableOverride.NodeContext.PositionData
            );
        }

        if (!TryLookupIdentifier(objectVariableOverride.Named.Name, out var variableSymbol))
        {
            throw new CompileError.SemanticError(
                $"object variable override {objectVariableOverride.Named.Name} not found",
                objectVariableOverride.NodeContext.PositionData
            );
        }

        if (variableSymbol.TypeRef.TypeInfo == TypeInfo.Unknown)
        {
            throw new CompileError.SemanticError(
                $"object variable override {objectVariableOverride.Named.Name} type not resolved",
                objectVariableOverride.NodeContext.PositionData
            );
        }

        objectVariableOverride.TypeRef = variableSymbol.TypeRef;

        objectVariableOverride.Value.Accept(this);

        if (objectVariableOverride.Value.TypeRef.HasDeferredTypes())
        {
            var inferenceVisitor = new InferenceVisitor(SemanticContext);
            inferenceVisitor.VisitObjectVariableOverride(objectVariableOverride);
        }

        if (!variableSymbol.TypeRef.Compare(objectVariableOverride.Value.TypeRef))
        {
            throw new CompileError.SemanticError(
                $"object variable override {objectVariableOverride.Named.Name} type mismatch",
                objectVariableOverride.NodeContext.PositionData
            );
        }

        objectVariableOverride.TypeRef = variableSymbol.TypeRef;

        return objectVariableOverride;
    }

    public override ArrayAccessNode VisitArrayAccessNode(ArrayAccessNode arrayAccessNode)
    {
        arrayAccessNode.Array.Accept(this);
        arrayAccessNode.Accessor.Accept(this);

        // TODO: Maybe in the future we can support array access with non-array types.
        // After we allow making accessors and generic types, we can allow this.

        if (arrayAccessNode.Array.TypeRef.TypeInfo is not GenericTypeInfo { Name: "Array" })
        {
            throw new CompileError.SemanticError(
                "trying to access non-array",
                arrayAccessNode.NodeContext.PositionData
            );
        }

        if (arrayAccessNode.Accessor.TypeRef.TypeInfo != TypeInfo.Int)
        {
            throw new CompileError.SemanticError(
                "array accessor must be int",
                arrayAccessNode.NodeContext.PositionData
            );
        }

        arrayAccessNode.TypeRef = arrayAccessNode.Array.TypeRef;

        return arrayAccessNode;
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
        EnumDeclarationItemNode enumDeclarationItemNode
    )
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
        EnumDeclarationItemParameterNode enumDeclarationItemParameterNode
    )
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
        enumShortHandNode.TypeRef.TypeInfo = TypeInfo.Deferred;

        foreach (var parameter in enumShortHandNode.Parameters)
        {
            VisitBaseNode(parameter);
        }

        return enumShortHandNode;
    }

    public override ExternFunctionNode VisitExternFunctionNode(ExternFunctionNode externFunctionNode)
    {
        return externFunctionNode;
    }

    public override ExternVariableNode VisitExternVariableNode(ExternVariableNode externVariableNode)
    {
        return externVariableNode;
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

        VisitBodyBlockNode(ifStatementNode.Body);

        if (ifStatementNode.NextIf != null)
        {
            VisitIfStatementNode(ifStatementNode.NextIf);
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

        if (visitedLeft.TypeRef.HasDeferredTypes() || visitedRight.TypeRef.HasDeferredTypes())
        {
            var inferenceVisitor = new InferenceVisitor(SemanticContext);
            inferenceVisitor.VisitExpressionNode(expressionNode);

            if (visitedLeft.TypeRef.HasDeferredTypes() || visitedRight.TypeRef.HasDeferredTypes())
            {
                throw new CompileError.SemanticError(
                    "expression type not resolved",
                    expressionNode.NodeContext.PositionData
                );
            }
        }

        if (visitedLeft.TypeRef.Compare<ExpressionComparer>(TypeInfo.Void) ||
            visitedRight.TypeRef.Compare<ExpressionComparer>(TypeInfo.Void))
        {
            throw new CompileError.SemanticError(
                "cannot use void in expression",
                expressionNode.NodeContext.PositionData
            );
        }

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

            expressionNode.TypeRef.TypeInfo = TypeInfo.Boolean;

            return expressionNode;
        }

        if (expressionNode.Operator.IsLogicalOperator())
        {
            bool compare;

            if (expressionNode.Operator == Operator.Is)
            {
                compare = visitedLeft.TypeRef.Compare<IsComparer>(visitedRight.TypeRef);
            }
            else
            {
                compare = visitedLeft.TypeRef.Compare<ExpressionComparer>(visitedRight.TypeRef);
            }

            if (!compare)
            {
                throw new CompileError.SemanticError(
                    "expression type mismatch",
                    expressionNode.NodeContext.PositionData
                );
            }

            expressionNode.TypeRef.TypeInfo = TypeInfo.Boolean;

            return expressionNode;
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
        // Check if the function is a class initialization
        if (TryLookupType(functionCallNode.Name.Name, out var symbol)
            && symbol.TypeRef.TypeInfo is ObjectTypeInfo)
        {
            if (functionCallNode.Arguments.Count != 0)
            {
                throw new CompileError.SemanticError(
                    "class initialization cannot have arguments",
                    functionCallNode.NodeContext.PositionData
                );
            }

            functionCallNode.TypeRef = symbol.TypeRef;

            return functionCallNode;
        }

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
        VariableDeclarationNode variableDeclarationNode
    )
    {
        if (IsCurrentlyResolving(variableDeclarationNode))
        {
            return variableDeclarationNode;
        }

        TypeRef? typeRef = null;

        if (variableDeclarationNode.TypeInfo != null)
        {
            var visitedTypeName = VisitTypeInfoNode(variableDeclarationNode.TypeInfo);

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

            if (visitedValue.TypeRef.HasDeferredTypes())
            {
                if (typeRef == null)
                {
                    throw new CompileError.SemanticError(
                        "variable type cannot be resolved",
                        variableDeclarationNode.NodeContext.PositionData
                    );
                }

                var inferenceVisitor = new InferenceVisitor(SemanticContext);
                inferenceVisitor.VisitVariableDeclarationNode(variableDeclarationNode);
            }

            /*
            if (typeRef != null && !typeRef.Compare(visitedValue.TypeRef))
            {
                throw new CompileError.SemanticError(
                    "variable type mismatch",
                    variableDeclarationNode.NodeContext.PositionData
                );
            }
            */

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

        SetSymbol(
            new Symbol(
                variableDeclarationNode.Named.Name,
                CurrentScope,
                variableDeclarationNode,
                SymbolType.Identifier
            ),
            !CanSetAlreadyDeclaredSymbol
        );

        FreeCurrentResolving();

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
        else if (assignmentNode.Value is EnumShortHandNode)
        {
            var inferenceVisitor = new InferenceVisitor(SemanticContext);
            inferenceVisitor.VisitAssignmentNode(assignmentNode);
        }

        /*
        if (!assignmentNode.Name.TypeRef.Compare(assignmentNode.Value.TypeRef))
        {
            throw new CompileError.SemanticError(
                $"assignment type mismatch, expected {assignmentNode.Name.TypeRef.TypeInfo}, got {assignmentNode.Value.TypeRef.TypeInfo}",
                assignmentNode.NodeContext.PositionData
            );
        }
        */

        assignmentNode.TypeRef = assignmentNode.Value.TypeRef;

        return assignmentNode;
    }

    public override ReturnStatementNode VisitReturnStatementNode(ReturnStatementNode returnStatementNode)
    {
        base.VisitReturnStatementNode(returnStatementNode);

        if (!TryGetScopeOfType(ScopeType.Function, out var functionScope)
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

        /*
        if (!functionTypeInfo.ReturnType.Compare(returnStatementNode.TypeRef))
        {
            if (functionTypeInfo.ReturnType.TypeInfo != TypeInfo.Void && returnStatementNode.Value == null)
            {
                throw new CompileError.SemanticError(
                    $"function {functionDeclarationNode.Named.Name} requires a return value of type {functionTypeInfo.ReturnType.TypeInfo}",
                    returnStatementNode.NodeContext.PositionData
                );
            }

            throw new CompileError.SemanticError(
                $"return type mismatch, expected {functionTypeInfo.ReturnType.TypeInfo}, got {returnStatementNode.TypeRef.TypeInfo}",
                returnStatementNode.NodeContext.PositionData
            );
        }
        */

        returnStatementNode.TypeRef = functionTypeInfo.ReturnType;

        return returnStatementNode;
    }

    public override FunctionCallArgumentNode VisitFunctionCallArgumentNode(
        FunctionCallArgumentNode functionCallArgumentNode
    )
    {
        var visitedValue = VisitBaseNode(functionCallArgumentNode.Value);

        functionCallArgumentNode.TypeRef = visitedValue.TypeRef;

        return functionCallArgumentNode;
    }

    public override FunctionDeclarationNode VisitFunctionDeclarationNode(
        FunctionDeclarationNode functionDeclarationNode
    )
    {
        if (IsCurrentlyResolving(functionDeclarationNode))
        {
            return functionDeclarationNode;
        }

        if (functionDeclarationNode.Scope == null)
        {
            throw new CompileError.SemanticError(
                $"function {functionDeclarationNode.Named.Name} has no scope",
                functionDeclarationNode.NodeContext.PositionData
            );
        }

        if (InScopeType(ScopeType.Function))
        {
            NewScope(ScopeType.Function, functionDeclarationNode);
        }
        else
        {
            RecallNodeScope(functionDeclarationNode);
        }

        var parameters = new Dictionary<string, TypeRef>();
        foreach (var parameterNode in functionDeclarationNode.ParameterNodes)
        {
            VisitFunctionDeclarationParameterNode(parameterNode);

            if (parameterNode.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError(
                    $"function parameter {parameterNode.Named.Name} type not resolved",
                    parameterNode.NodeContext.PositionData
                );
            }

            parameters.Add(parameterNode.Named.Name, parameterNode.TypeRef);
        }

        var visitedBlockNode = VisitBodyBlockNode(functionDeclarationNode.Body);

        PopScope();

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
                    $"last statement in function {functionDeclarationNode.Named.Name} must be a return statement",
                    functionDeclarationNode.NodeContext.PositionData
                );
            }

            if (!functionDeclarationNode.Scope.ReturnStatementFound)
            {
                throw new CompileError.SemanticError(
                    $"function {functionDeclarationNode.Named.Name} requires a return statement",
                    functionDeclarationNode.NodeContext.PositionData
                );
            }
        }

        SetSymbol(
            new Symbol(
                functionDeclarationNode.Named.Name,
                CurrentScope,
                functionDeclarationNode,
                SymbolType.Identifier
            ),
            !CanSetAlreadyDeclaredSymbol
        );

        FreeCurrentResolving();

        return functionDeclarationNode;
    }

    public override BodyBlockNode VisitBodyBlockNode(BodyBlockNode bodyBlockNode)
    {
        NewScope(ScopeType.BlockBody, bodyBlockNode);

        foreach (var statement in bodyBlockNode.Statements)
        {
            var visitedStatement = VisitBaseNode(statement);
        }

        PopScope();

        return bodyBlockNode;
    }

    public override TypeInfoAnonymousEnumNode VisitTypeInfoAnonymousEnumNode(
        TypeInfoAnonymousEnumNode typeInfoAnonymousEnumNode
    )
    {
        var fields = new Dictionary<string, TypeRef>();
        foreach (var field in typeInfoAnonymousEnumNode.Fields)
        {
            VisitTypeInfoEnumFieldNode(field);

            if (field.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError(
                    $"enum field {field.Named.Name} type not resolved",
                    field.NodeContext.PositionData
                );
            }

            fields.Add(field.Named.Name, field.TypeRef);
        }

        typeInfoAnonymousEnumNode.TypeRef.TypeInfo = new InlineEnumTypeInfo(fields);

        return typeInfoAnonymousEnumNode;
    }

    public override TypeInfoEnumFieldNode VisitTypeInfoEnumFieldNode(TypeInfoEnumFieldNode typeInfoEnumFieldNode)
    {
        var parameters = new Dictionary<string, TypeRef>();
        foreach (var parameter in typeInfoEnumFieldNode.Parameters)
        {
            VisitTypeInfoEnumFieldParamNode(parameter);

            if (parameter.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError(
                    $"enum field parameter {parameter.Named.Name} type not resolved",
                    parameter.NodeContext.PositionData
                );
            }

            parameters.Add(parameter.Named.Name, parameter.TypeRef);
        }

        typeInfoEnumFieldNode.TypeRef.TypeInfo = new EnumItemTypeInfo(CurrentScope,
            typeInfoEnumFieldNode.Named.Name, parameters);

        return typeInfoEnumFieldNode;
    }

    public override TypeInfoEnumFieldParamNode VisitTypeInfoEnumFieldParamNode(
        TypeInfoEnumFieldParamNode typeInfoEnumFieldParamNode
    )
    {
        VisitTypeInfoNode(typeInfoEnumFieldParamNode.TypeInfoNode);

        typeInfoEnumFieldParamNode.TypeRef = typeInfoEnumFieldParamNode.TypeInfoNode.TypeRef;

        return typeInfoEnumFieldParamNode;
    }

    public override ReferenceNamedNode VisitReferenceNameNode(ReferenceNamedNode referenceNamedNode)
    {
        if (TryLookupType(referenceNamedNode.Name, out var symbol))
        {
            referenceNamedNode.TypeRef = symbol.TypeRef;
        }
        else
        {
            throw new CompileError.SemanticError(
                $"type {referenceNamedNode.Name} not found",
                referenceNamedNode.NodeContext.PositionData
            );
        }

        return base.VisitReferenceNameNode(referenceNamedNode);
    }
}