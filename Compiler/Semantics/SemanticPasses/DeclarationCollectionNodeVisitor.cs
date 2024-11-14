using Compiler.ErrorHandling;
using Compiler.Semantics.ScopeHandling;
using Compiler.Semantics.TypeInformation;
using Compiler.Semantics.TypeInformation.Types;
using Compiler.Syntax.Nodes;

namespace Compiler.Semantics.SemanticPasses;

public class DeclarationCollectionNodeVisitor(SemanticHandler semanticHandler)
    : SemanticPassBaseNodeVisitor(semanticHandler, new DeclarationCollectionScopeHandler(semanticHandler))
{
    public override ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        AddObjectDeclarationNode(objectDeclarationNode);

        using (EnterScope(ScopeType.Object, objectDeclarationNode, out var objectScope))
        {
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

            var name = objectDeclarationNode.Named.Name;

            objectDeclarationNode.Named.TypeRef = new TypeRef(TypeInfo.Unknown);

            var fields = new Dictionary<string, TypeRef>();
            objectDeclarationNode.TypeRef.TypeInfo =
                new ObjectTypeInfo(objectScope, baseTypeRef, name, fields);

            foreach (var variableDeclaration in objectDeclarationNode.Fields.OfType<VariableDeclarationNode>())
            {
                VisitVariableDeclarationNode(variableDeclaration);

                fields.Add(variableDeclaration.Named.Name, variableDeclaration.TypeRef);
            }


            foreach (var functionDeclaration in objectDeclarationNode.Fields.OfType<FunctionDeclarationNode>())
            {
                VisitFunctionDeclarationNode(functionDeclaration);

                fields.Add(functionDeclaration.Named.Name, functionDeclaration.TypeRef);
            }
        }

        PopObjectDeclarationNode();

        return objectDeclarationNode;
    }

    public override FunctionDeclarationParameterNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationParameterNode functionDeclarationParameterNode
    )
    {
        base.VisitFunctionDeclarationParameterNode(functionDeclarationParameterNode);

        functionDeclarationParameterNode.TypeRef = functionDeclarationParameterNode.TypeInfoNode.TypeRef;

        return functionDeclarationParameterNode;
    }

    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode
    )
    {
        var name = variableDeclarationNode.Named.Name;

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
            new Symbol(name, CurrentScope, variableDeclarationNode, SymbolType.Identifier),
            true
        );

        return variableDeclarationNode;
    }

    public override FunctionDeclarationNode VisitFunctionDeclarationNode(
        FunctionDeclarationNode functionDeclarationNode
    )
    {
        var parameters = new List<FunctionTypeInfoParameter>();
        var name = functionDeclarationNode.Named.Name;

        using (EnterScope(ScopeType.Function, functionDeclarationNode))
        {
            foreach (var parameter in functionDeclarationNode.ParameterNodes)
            {
                var parameterName = parameter.Named.Name;
                var parameterTypeRef = VisitTypeInfoNode(parameter.TypeInfoNode).TypeRef;

                parameter.TypeRef = parameterTypeRef;
                parameters.Add(new FunctionTypeInfoParameter(parameterName, parameterTypeRef));

                SemanticHandler.SetSymbol(
                    new Symbol(
                        parameterName,
                        CurrentScope,
                        parameter,
                        SymbolType.Identifier
                    ),
                    true
                );
            }
        }

        var returnTypeRef = new TypeRef(TypeInfo.Void);

        if (functionDeclarationNode.ReturnTypeInfo != null)
        {
            VisitTypeInfoNode(functionDeclarationNode.ReturnTypeInfo);

            returnTypeRef = functionDeclarationNode.ReturnTypeInfo.TypeRef;
        }

        var functionTypeInfo = new FunctionTypeInfo(returnTypeRef, parameters);

        functionDeclarationNode.TypeRef.TypeInfo = functionTypeInfo;

        SemanticHandler.SetSymbol(
            new Symbol(name, CurrentScope, functionDeclarationNode, SymbolType.Identifier),
            true
        );

        return functionDeclarationNode;
    }

    public override ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        using (EnterScope(ScopeType.Program, programNode))
        {
            foreach (var declaration in programNode.Declarations)
            {
                if (declaration is ObjectDeclarationNode objectDeclarationNode)
                {
                    if (objectDeclarationNode.IsImmediatelyInstanced)
                    {
                        SemanticHandler.SetSymbol(
                            new Symbol(
                                objectDeclarationNode.Named.Name,
                                CurrentScope,
                                objectDeclarationNode,
                                SymbolType.Identifier
                            ),
                            true
                        );
                    }

                    SemanticHandler.SetSymbol(
                        new Symbol(
                            objectDeclarationNode.Named.Name,
                            CurrentScope,
                            objectDeclarationNode,
                            SymbolType.Type
                        ),
                        true
                    );
                }
            }

            base.VisitProgramNode(programNode);
        }

        return programNode;
    }

    public override ExternFunctionNode VisitExternFunctionNode(ExternFunctionNode externFunctionNode)
    {
        var name = externFunctionNode.Named.Name;

        var parameters = new List<FunctionTypeInfoParameter>();

        foreach (var parameter in externFunctionNode.ParameterNodes)
        {
            VisitFunctionDeclarationParameterNode(parameter);

            var parameterName = parameter.Named.Name;
            var parameterTypeRef = parameter.TypeRef;

            parameters.Add(new FunctionTypeInfoParameter(parameterName, parameterTypeRef));
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
            new Symbol(name, CurrentScope, externFunctionNode, SymbolType.Identifier),
            true
        );

        return externFunctionNode;
    }

    public override EnumDeclarationNode VisitEnumDeclarationNode(EnumDeclarationNode enumDeclarationNode)
    {
        Scope enumScope;
        var name = enumDeclarationNode.Named.Name;
        var items = new Dictionary<string, TypeRef>();

        using (EnterScope(ScopeType.Enum, enumDeclarationNode, out enumScope))
        {
            foreach (var item in enumDeclarationNode.Items)
            {
                item.Accept(this);

                items.Add(item.Named.Name, item.TypeRef);
            }
        }

        SemanticHandler.SetSymbol(
            new Symbol(
                name,
                CurrentScope,
                enumDeclarationNode,
                SymbolType.Type
            ),
            true
        );

        enumDeclarationNode.TypeRef.TypeInfo = new EnumTypeInfo(enumScope, name, items);

        return enumDeclarationNode;
    }

    public override EnumDeclarationItemNode VisitEnumDeclarationItemNode(EnumDeclarationItemNode enumItemNode)
    {
        var parameters = new Dictionary<string, TypeRef>();
        foreach (var parameter in enumItemNode.ParameterNodes)
        {
            VisitEnumDeclarationItemParameterNode(parameter);

            parameters.Add(parameter.Named.Name, parameter.TypeRef);
        }

        enumItemNode.TypeRef.TypeInfo =
            new EnumItemTypeInfo(CurrentScope, enumItemNode.Named.Name, parameters);

        SemanticHandler.SetSymbol(
            new Symbol(enumItemNode.Named.Name, CurrentScope, enumItemNode, SymbolType.Type),
            true
        );

        return enumItemNode;
    }

    public override EnumDeclarationItemParameterNode VisitEnumDeclarationItemParameterNode(
        EnumDeclarationItemParameterNode enumDeclarationItemParameterNode
    )
    {
        enumDeclarationItemParameterNode.TypeInfoNode.Accept(this);

        enumDeclarationItemParameterNode.TypeRef = enumDeclarationItemParameterNode.TypeInfoNode.TypeRef;
        enumDeclarationItemParameterNode.Named.TypeRef = enumDeclarationItemParameterNode.TypeRef;

        return enumDeclarationItemParameterNode;
    }

    public override ExternVariableNode VisitExternVariableNode(ExternVariableNode externVariableNode)
    {
        var name = externVariableNode.Named.Name;

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
            new Symbol(name, CurrentScope, externVariableNode, SymbolType.Identifier),
            true
        );

        return externVariableNode;
    }
}