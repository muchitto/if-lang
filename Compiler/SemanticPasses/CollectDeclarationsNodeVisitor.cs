using Compiler.ScopeHandler;
using Compiler.Syntax.Nodes;
using Compiler.TypeInformation;
using Compiler.TypeInformation.Types;

namespace Compiler.SemanticPasses;

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

        SemanticHandler.NewScope(ScopeType.Object, objectDeclarationNode);

        var name = objectDeclarationNode.Name.Name;

        var fields = new Dictionary<string, TypeRef>();

        foreach (var declaration in objectDeclarationNode.Fields)
        {
            VisitDeclarationNode(declaration);

            fields.Add(declaration.Name.Name, declaration.TypeRef);

            objectDeclarationNode.TypeRef.TypeInfo = new ObjectTypeInfo(name, fields);
        }

        SemanticHandler.PopScope();

        objectDeclarationNode.TypeRef.TypeInfo = new ObjectTypeInfo(name, fields);
        objectDeclarationNode.Name.TypeRef = objectDeclarationNode.TypeRef;

        SemanticHandler.SetSymbol(
            new Symbol(
                objectDeclarationNode.Name.Name,
                SemanticHandler.CurrentScope,
                objectDeclarationNode,
                SymbolType.Type
            ),
            true
        );

        return objectDeclarationNode;
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

        return base.VisitTypeNameNode(typeInfoNameNode);
    }

    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode)
    {
        var name = variableDeclarationNode.Name.Name;

        if (variableDeclarationNode.TypeName != null)
        {
            VisitTypeInfoNode(variableDeclarationNode.TypeName);

            variableDeclarationNode.TypeRef = variableDeclarationNode.TypeName.TypeRef;
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
        functionDeclarationNode.Name.TypeRef = functionDeclarationNode.TypeRef;

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
}