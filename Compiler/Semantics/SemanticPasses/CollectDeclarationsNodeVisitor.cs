using Compiler.ErrorHandling;
using Compiler.Semantics.TypeInformation;
using Compiler.Semantics.TypeInformation.Types;
using Compiler.Syntax.Nodes;

namespace Compiler.Semantics.SemanticPasses;

public class CollectDeclarationsNodeVisitor(SemanticContext semanticContext, SemanticHandler semanticHandler)
    : SemanticPassBaseNodeVisitor(semanticContext, semanticHandler)
{
    public override ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        if (objectDeclarationNode.IsImmediatelyInstanced)
        {
            SemanticHandler.SetSymbol(
                new Symbol(
                    objectDeclarationNode.Name.Name,
                    SemanticHandler.CurrentScope,
                    objectDeclarationNode,
                    SymbolType.Identifier
                ),
                true
            );
        }

        var baseTypeRef = TypeInfo.ObjectRef;
        if (objectDeclarationNode.BaseName != null)
        {
            if (SemanticHandler.TryLookupType(objectDeclarationNode.BaseName.Name, out var symbol))
            {
                baseTypeRef = symbol.TypeRef;
            }
            else
            {
                baseTypeRef = new TypeRef(TypeInfo.Unknown);
            }

            objectDeclarationNode.BaseName.TypeRef = baseTypeRef;
        }

        var objectScope = SemanticHandler.NewScope(ScopeType.Object, objectDeclarationNode);

        var name = objectDeclarationNode.Name.Name;

        objectDeclarationNode.Name.TypeRef = new TypeRef(TypeInfo.Unknown);

        var fields = new Dictionary<string, TypeRef>();
        objectDeclarationNode.TypeRef.TypeInfo =
            new ObjectTypeInfo(baseTypeRef, name, fields, objectScope);

        foreach (var declaration in objectDeclarationNode.Fields)
        {
            VisitDeclarationNode(declaration);

            fields.Add(declaration.Name.Name, declaration.TypeRef);
        }

        SemanticHandler.PopScope();

        SemanticHandler.SetSymbol(
            new Symbol(
                objectDeclarationNode.Name.Name,
                objectScope,
                objectDeclarationNode,
                SymbolType.Type
            ),
            true
        );

        return objectDeclarationNode;
    }

    public override FunctionDeclarationParameterNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationParameterNode functionDeclarationParameterNode)
    {
        base.VisitFunctionDeclarationParameterNode(functionDeclarationParameterNode);

        functionDeclarationParameterNode.TypeRef = functionDeclarationParameterNode.TypeInfoNode.TypeRef;

        return functionDeclarationParameterNode;
    }

    public override TypeInfoNameNode VisitTypeNameNode(TypeInfoNameNode typeInfoNameNode)
    {
        TypeRef typeRef = new(TypeInfo.Unknown);

        if (TypeInfo.TryGetBuiltInType(typeInfoNameNode.Name, out var typeInfo))
        {
            typeRef = new TypeRef(typeInfo);
        }
        else
        {
            if (SemanticHandler.TryLookupType(typeInfoNameNode.Name, out var symbol))
            {
                typeRef = symbol.TypeRef;
            }
        }

        typeInfoNameNode.TypeRef = typeRef;

        return typeInfoNameNode;
    }

    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode)
    {
        var name = variableDeclarationNode.Name.Name;

        if (variableDeclarationNode.TypeInfo != null)
        {
            VisitTypeInfoNode(variableDeclarationNode.TypeInfo);

            variableDeclarationNode.TypeRef = variableDeclarationNode.TypeInfo.TypeRef;
        }

        if (variableDeclarationNode.Value != null)
        {
            VisitBaseNode(variableDeclarationNode.Value);
        }

        SemanticHandler.SetSymbol(
            new Symbol(name, SemanticHandler.CurrentScope, variableDeclarationNode, SymbolType.Identifier),
            true
        );

        return variableDeclarationNode;
    }

    public override FunctionDeclarationNode VisitFunctionDeclarationNode(
        FunctionDeclarationNode functionDeclarationNode)
    {
        SemanticHandler.NewScope(ScopeType.Function, functionDeclarationNode);

        var name = functionDeclarationNode.Name.Name;

        var parameters = new Dictionary<string, TypeRef>();
        foreach (var parameter in functionDeclarationNode.ParameterNodes)
        {
            var parameterName = parameter.Name.Name;
            var parameterTypeRef = VisitTypeInfoNode(parameter.TypeInfoNode).TypeRef;

            parameter.TypeRef = parameterTypeRef;
            parameters.Add(parameterName, parameterTypeRef);

            SemanticHandler.SetSymbol(
                new Symbol(parameterName, SemanticHandler.CurrentScope, parameter, SymbolType.Identifier),
                true
            );
        }

        SemanticHandler.PopScope();

        var returnTypeRef = new TypeRef(TypeInfo.Void);

        if (functionDeclarationNode.ReturnTypeInfo != null)
        {
            VisitTypeInfoNode(functionDeclarationNode.ReturnTypeInfo);

            returnTypeRef = functionDeclarationNode.ReturnTypeInfo.TypeRef;
        }

        var functionTypeInfo = new FunctionTypeInfo(returnTypeRef, parameters);

        functionDeclarationNode.TypeRef.TypeInfo = functionTypeInfo;

        SemanticHandler.SetSymbol(
            new Symbol(name, SemanticHandler.CurrentScope, functionDeclarationNode, SymbolType.Identifier),
            true
        );

        return functionDeclarationNode;
    }

    public override ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        SemanticHandler.NewScope(ScopeType.Program, programNode);

        base.VisitProgramNode(programNode);

        SemanticHandler.PopScope();

        return programNode;
    }

    public override ExternFunctionNode VisitExternFunctionNode(ExternFunctionNode externFunctionNode)
    {
        var name = externFunctionNode.Name.Name;

        var parameters = new Dictionary<string, TypeRef>();

        foreach (var parameter in externFunctionNode.ParameterNodes)
        {
            VisitFunctionDeclarationParameterNode(parameter);

            var parameterName = parameter.Name.Name;
            var parameterTypeRef = parameter.TypeRef;

            if (parameterTypeRef.Compare(TypeInfo.Unknown))
            {
                throw new CompileError.SemanticError(
                    "extern function or variable types cannot be unknown at the time of declaration",
                    parameter.TypeInfoNode.NodeContext.PositionData
                );
            }

            parameter.TypeRef = parameterTypeRef;
            parameters.Add(parameterName, parameterTypeRef);
        }

        var returnTypeRef = new TypeRef(TypeInfo.Void);

        if (externFunctionNode.ReturnType != null)
        {
            VisitTypeInfoNode(externFunctionNode.ReturnType);

            returnTypeRef = externFunctionNode.ReturnType.TypeRef;
        }

        var functionTypeInfo = new FunctionTypeInfo(returnTypeRef, parameters);

        externFunctionNode.TypeRef.TypeInfo = functionTypeInfo;

        SemanticHandler.SetSymbol(
            new Symbol(name, SemanticHandler.CurrentScope, externFunctionNode, SymbolType.Identifier),
            true
        );

        return externFunctionNode;
    }

    public override EnumDeclarationNode VisitEnumDeclarationNode(EnumDeclarationNode enumDeclarationNode)
    {
        SemanticHandler.NewScope(ScopeType.Enum, enumDeclarationNode);

        var name = enumDeclarationNode.Name.Name;

        var items = new Dictionary<string, TypeRef>();
        foreach (var item in enumDeclarationNode.Items)
        {
            item.Accept(this);

            items.Add(item.Name.Name, item.TypeRef);
        }

        SemanticHandler.PopScope();

        SemanticHandler.SetSymbol(
            new Symbol(name, SemanticHandler.CurrentScope, enumDeclarationNode, SymbolType.Type),
            true
        );

        enumDeclarationNode.TypeRef.TypeInfo = new EnumTypeInfo(name, items);

        return enumDeclarationNode;
    }

    public override EnumDeclarationItemNode VisitEnumDeclarationItemNode(EnumDeclarationItemNode enumItemNode)
    {
        var parameters = new Dictionary<string, TypeRef>();
        foreach (var parameter in enumItemNode.ParameterNodes)
        {
            VisitEnumDeclarationItemParameterNode(parameter);

            parameters.Add(parameter.Name.Name, parameter.TypeRef);
        }

        enumItemNode.TypeRef.TypeInfo = new EnumItemTypeInfo(enumItemNode.Name.Name, parameters);

        SemanticHandler.SetSymbol(
            new Symbol(enumItemNode.Name.Name, SemanticHandler.CurrentScope, enumItemNode, SymbolType.Type),
            true
        );

        return enumItemNode;
    }

    public override EnumDeclarationItemParameterNode VisitEnumDeclarationItemParameterNode(
        EnumDeclarationItemParameterNode enumDeclarationItemParameterNode)
    {
        enumDeclarationItemParameterNode.TypeInfoNode.Accept(this);

        enumDeclarationItemParameterNode.TypeRef = enumDeclarationItemParameterNode.TypeInfoNode.TypeRef;
        enumDeclarationItemParameterNode.Name.TypeRef = enumDeclarationItemParameterNode.TypeRef;

        return enumDeclarationItemParameterNode;
    }

    public override ExternVariableNode VisitExternVariableNode(ExternVariableNode externVariableNode)
    {
        var name = externVariableNode.Name.Name;

        VisitTypeInfoNode(externVariableNode.TypeInfoNode);

        if (externVariableNode.TypeRef.Compare(TypeInfo.Unknown))
        {
            throw new CompileError.SemanticError(
                "extern function or variable types cannot be unknown at the time of declaration",
                externVariableNode.TypeInfoNode.NodeContext.PositionData
            );
        }

        externVariableNode.TypeRef = externVariableNode.TypeInfoNode.TypeRef;

        SemanticHandler.SetSymbol(
            new Symbol(name, SemanticHandler.CurrentScope, externVariableNode, SymbolType.Identifier),
            true
        );

        return externVariableNode;
    }
}