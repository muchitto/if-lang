using Compiler.ErrorHandling;
using Compiler.ScopeHandler;
using Compiler.Syntax.Nodes;
using Compiler.TypeInformation;
using Compiler.TypeInformation.Types;

namespace Compiler.SemanticPasses;

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
        SemanticHandler.RecallNodeScope(objectDeclarationNode);

        VisitTypeNameNode(objectDeclarationNode.BaseName);

        var name = objectDeclarationNode.Name.Name;

        if (objectDeclarationNode.TypeRef.TypeInfo is not ObjectTypeInfo objectTypeInfo)
        {
            throw new CompileError.SemanticError("object type info is null");
        }

        foreach (var declarationNode in objectDeclarationNode.Fields)
        {
            var visitedDeclarationNode = VisitDeclarationNode(declarationNode);

            if (visitedDeclarationNode.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError($"field {visitedDeclarationNode.Name.Name} type not resolved");
            }

            objectTypeInfo.Fields[visitedDeclarationNode.Name.Name] = visitedDeclarationNode.TypeRef;
        }

        SemanticHandler.PopScope();

        if (objectDeclarationNode.TypeRef.Compare(TypeInfo.Unknown))
        {
            throw new CompileError.SemanticError($"object {objectDeclarationNode.Name.Name} type not resolved");
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

        var typeRef = new TypeRef(TypeInfo.Unknown);

        if (TypeInfo.TryGetBuiltInType(typeInfoNameNode.Name, out var typeInfo))
        {
            typeRef.TypeInfo = typeInfo;
        }
        else
        {
            if (SemanticHandler.TryLookupType(typeInfoNameNode.Name, out var symbol))
            {
                typeRef = symbol.TypeRef;
            }
            else
            {
                throw new CompileError.SemanticError($"type {typeInfoNameNode.Name} not found");
            }
        }

        typeInfoNameNode.TypeRef = typeRef;

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
            throw new CompileError.SemanticError("array element type not resolved");
        }

        foreach (var element in arrayLiteralNode.Elements)
        {
            var visitedElement = VisitBaseNode(element);

            if (visitedElement.TypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError("array element type not resolved");
            }

            if (!visitedElement.TypeRef.Compare(firstElementTypeRef))
            {
                throw new CompileError.SemanticError("array element type mismatch");
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
                throw new CompileError.SemanticError($"field {field.Name.Name} type not resolved");
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
                throw new CompileError.SemanticError($"field {field.Key} type not resolved");
            }

            fields.Add(field.Key, visitedField.TypeRef);
        }

        typeInfoStructureNode.TypeRef.TypeInfo = new StructureTypeInfo(fields);

        return base.VisitTypeInfoStructureNode(typeInfoStructureNode);
    }

    public override FunctionDeclarationArgumentNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationArgumentNode functionDeclarationArgumentNode)
    {
        if (!functionDeclarationArgumentNode.TypeRef.Compare(TypeInfo.Unknown))
        {
            return base.VisitFunctionDeclarationParameterNode(functionDeclarationArgumentNode);
        }

        var visitedTypeNode = VisitTypeInfoNode(functionDeclarationArgumentNode.TypeInfoNode);

        functionDeclarationArgumentNode.TypeRef = visitedTypeNode.TypeRef;

        SemanticHandler.SetSymbol(
            new Symbol(
                functionDeclarationArgumentNode.Name.Name,
                SemanticHandler.CurrentScope,
                functionDeclarationArgumentNode,
                SymbolType.Identifier
            ),
            !SemanticHandler.CanSetAlreadyDeclaredSymbol
        );

        return functionDeclarationArgumentNode;
    }

    public override IdentifierNode VisitIdentifierNode(IdentifierNode identifierNode)
    {
        if (identifierNode.Name == "this")
        {
            if (!SemanticHandler.TryGetScopeOfType(ScopeType.Object, out var objectScope))
            {
                throw new CompileError.SemanticError("this can only be used in object scope");
            }

            identifierNode.TypeRef = objectScope.AttachedNode.TypeRef;

            return identifierNode;
        }

        if (SemanticHandler.TryLookupIdentifier(identifierNode.Name, out var symbol))
        {
            identifierNode.TypeRef = symbol.TypeRef;
        }
        else
        {
            throw new CompileError.SemanticError($"identifier {identifierNode.Name} not found");
        }

        return identifierNode;
    }

    public override FunctionCallNode VisitFunctionCallNode(FunctionCallNode functionCallNode)
    {
        var visitedName = VisitIdentifierNode(functionCallNode.Name);

        if (visitedName.TypeRef.TypeInfo is not FunctionTypeInfo functionTypeInfo)
        {
            throw new CompileError.SemanticError($"trying to call non-function {functionCallNode.Name.Name}");
        }

        if (functionCallNode.Arguments.Count != functionTypeInfo.Parameters.Count)
        {
            throw new CompileError.SemanticError(
                $"function {functionCallNode.Name.Name} requires {functionTypeInfo.Parameters.Count} arguments, you gave {functionCallNode.Arguments.Count}"
            );
        }

        for (var a = 0; a < functionCallNode.Arguments.Count; a++)
        {
            var argument = functionCallNode.Arguments[a];
            var argumentNode = VisitFunctionCallArgumentNode(argument);

            if (functionTypeInfo.Parameters.Count <= a)
            {
                throw new CompileError.SemanticError($"too many arguments for function {functionCallNode.Name.Name}");
            }

            var (functionSignatureArgumentName, functionSignatureArgumentType) =
                functionTypeInfo.Parameters.ElementAt(a);
            if (!argumentNode.TypeRef.Compare(functionSignatureArgumentType))
            {
                throw new CompileError.SemanticError($"argument {functionSignatureArgumentName} type mismatch");
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
                throw new CompileError.SemanticError("cannot assign void to variable");
            }

            if (visitedValue.TypeRef.TypeInfo.HasDeferredTypes())
            {
                if (typeRef == null)
                {
                    throw new CompileError.SemanticError("variable type cannot be resolved");
                }

                var inferenceVisitor = new InferenceVisitor();
                inferenceVisitor.VisitVariableDeclarationNode(variableDeclarationNode);
            }

            if (typeRef != null && !typeRef.Compare(visitedValue.TypeRef))
            {
                throw new CompileError.SemanticError("variable type mismatch");
            }

            if (typeRef == null)
            {
                typeRef = visitedValue.TypeRef;
            }
        }

        if (typeRef == null)
        {
            throw new CompileError.SemanticError("variable type not resolved");
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
            throw new CompileError.SemanticError(errorMessage);
        }

        if (assignmentNode.Name.TypeRef.TypeInfo is ObjectTypeInfo && assignmentNode.Value is NullLiteralNode)
        {
            assignmentNode.Value.TypeRef = assignmentNode.Name.TypeRef;
        }

        if (!assignmentNode.Name.TypeRef.Compare(assignmentNode.Value.TypeRef))
        {
            throw new CompileError.SemanticError(
                $"assignment type mismatch, expected {assignmentNode.Name.TypeRef.TypeInfo}, got {assignmentNode.Value.TypeRef.TypeInfo}");
        }

        assignmentNode.TypeRef = assignmentNode.Value.TypeRef;

        return assignmentNode;
    }

    public override MemberAccessNode VisitMemberAccessNode(MemberAccessNode memberAccessNode)
    {
        VisitBaseNode(memberAccessNode.BaseObject);

        if (memberAccessNode.BaseObject.TypeRef.TypeInfo is not ObjectTypeInfo objectTypeInfo)
        {
            throw new CompileError.SemanticError("trying to access member of non-object");
        }

        // This is a problem, the member should be an IdentifierNode and the baseObject can be a MemberAccessNode

        if (!objectTypeInfo.Fields.TryGetValue(memberAccessNode.Member.Name, out var typeRef))
        {
            throw new CompileError.SemanticError(
                $"object {objectTypeInfo.Name} has no member {memberAccessNode.Member.Name}");
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
            throw new CompileError.SemanticError("return statement not in function scope");
        }

        functionScope.ReturnStatementFound = true;

        returnStatementNode.TypeRef = returnStatementNode.Value?.TypeRef ?? new TypeRef(TypeInfo.Void);

        if (functionTypeInfo.ReturnType.TypeInfo == TypeInfo.Void && returnStatementNode.Value != null)
        {
            throw new CompileError.SemanticError("returning value in void function");
        }

        if (!functionTypeInfo.ReturnType.Compare(returnStatementNode.TypeRef))
        {
            if (functionTypeInfo.ReturnType.TypeInfo != TypeInfo.Void && returnStatementNode.Value == null)
            {
                throw new CompileError.SemanticError(
                    $"function {functionDeclarationNode.Name.Name} requires a return value of type {functionTypeInfo.ReturnType.TypeInfo}");
            }

            throw new CompileError.SemanticError(
                $"return type mismatch, expected {functionTypeInfo.ReturnType.TypeInfo}, got {returnStatementNode.TypeRef.TypeInfo}"
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
        numberLiteralNode.TypeRef = new TypeRef(TypeInfo.Number);

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
            throw new CompileError.SemanticError($"function {functionDeclarationNode.Name.Name} has no scope");
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
                throw new CompileError.SemanticError($"function parameter {parameterNode.Name.Name} type not resolved");
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
                throw new CompileError.SemanticError("function return type not resolved");
            }
        }

        var returnTypeRef = functionDeclarationNode.ReturnTypeInfo?.TypeRef ?? new TypeRef(TypeInfo.Void);

        functionDeclarationNode.TypeRef.TypeInfo = new FunctionTypeInfo(returnTypeRef, parameters);

        if (!returnTypeRef.Compare(TypeInfo.Void))
        {
            if (visitedBlockNode.Statements.Count > 0 && visitedBlockNode.Statements.Last() is not ReturnStatementNode)
            {
                throw new CompileError.SemanticError(
                    $"last statement in function {functionDeclarationNode.Name.Name} must be a return statement"
                );
            }

            if (!functionDeclarationNode.Scope.ReturnStatementFound)
            {
                throw new CompileError.SemanticError(
                    $"function {functionDeclarationNode.Name.Name} requires a return statement");
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