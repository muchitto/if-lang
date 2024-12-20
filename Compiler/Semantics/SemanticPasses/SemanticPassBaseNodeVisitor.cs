using Compiler.ErrorHandling;
using Compiler.Semantics.ScopeHandling;
using Compiler.Semantics.TypeInformation;
using Compiler.Semantics.TypeInformation.Types;
using Compiler.Syntax.Nodes;
using Compiler.Syntax.Nodes.TypeInfoNodes;

namespace Compiler.Semantics.SemanticPasses;

public class SemanticPassBaseNodeVisitor(SemanticHandler semanticHandler, BaseScopeHandler baseScopeHandler)
    : SemanticHelperBaseNodeVisitor(semanticHandler, baseScopeHandler)
{
    private readonly List<ObjectDeclarationNode> _objectDeclarationNodes = [];

    protected ObjectDeclarationNode? CurrentObjectDeclarationNode => _objectDeclarationNodes.Last();

    public override BooleanLiteralNode VisitBooleanLiteralNode(BooleanLiteralNode booleanLiteralNode)
    {
        booleanLiteralNode.TypeRef = new TypeRef(TypeInfo.Boolean);

        return base.VisitBooleanLiteralNode(booleanLiteralNode);
    }

    public override NumberLiteralNode VisitNumberLiteralNode(NumberLiteralNode numberLiteralNode)
    {
        if (numberLiteralNode.IsFloat())
        {
            numberLiteralNode.TypeRef = new TypeRef(TypeInfo.Float32);
        }
        else
        {
            numberLiteralNode.TypeRef = new TypeRef(TypeInfo.Int32);
        }

        return numberLiteralNode;
    }

    public override StringLiteralNode VisitStringLiteralNode(StringLiteralNode stringLiteralNode)
    {
        stringLiteralNode.TypeRef = new TypeRef(TypeInfo.String);

        return base.VisitStringLiteralNode(stringLiteralNode);
    }

    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode
    )
    {
        if (variableDeclarationNode.TypeInfoNode != null)
        {
            VisitTypeInfoNode(variableDeclarationNode.TypeInfoNode);
        }

        return base.VisitVariableDeclarationNode(variableDeclarationNode);
    }

    public override TypeInfoNode VisitTypeInfoNode(TypeInfoNode typeInfoNode)
    {
        return base.VisitTypeInfoNode(typeInfoNode);
    }

    public override TypeInfoNameNode VisitTypeInfoNameNode(TypeInfoNameNode typeInfoNameNode)
    {
        if (SemanticHandler.TryGetInNodeScope(typeInfoNameNode.Name, SymbolType.Type, out var nodeScopeSymbol))
        {
            typeInfoNameNode.TypeRef = nodeScopeSymbol.TypeRef;
        }
        else if (TypeInfo.TryGetBuiltInType(typeInfoNameNode.Name, out var typeInfo))
        {
            typeInfoNameNode.TypeRef = new TypeRef(typeInfo);
        }
        else
        {
            if (SemanticHandler.TryLookupType(typeInfoNameNode.Name, out var symbol))
            {
                typeInfoNameNode.TypeRef = symbol.TypeRef;
            }
            else
            {
                typeInfoNameNode.TypeRef = new TypeRef(TypeInfo.Unknown);
            }
        }

        return base.VisitTypeInfoNameNode(typeInfoNameNode);
    }

    public override TypeInfoInlineEnumNode VisitTypeInfoInlineEnumNode(
        TypeInfoInlineEnumNode typeInfoInlineEnumNode
    )
    {
        using (EnterScope(ScopeType.Enum, typeInfoInlineEnumNode, out var scope))
        {
            var fields = new List<AbstractStructuralFieldTypeInfo>();
            foreach (var field in typeInfoInlineEnumNode.Fields)
            {
                VisitTypeInfoNode(field);

                fields.Add(
                    new AbstractStructuralFieldTypeInfo(
                        field.Named.Name,
                        field.TypeRef
                    )
                );
            }

            typeInfoInlineEnumNode.TypeRef.TypeInfo = new InlineEnumTypeInfo(scope, fields);
        }

        return typeInfoInlineEnumNode;
    }


    public override TypeInfoEnumFieldNode VisitTypeInfoEnumFieldNode(TypeInfoEnumFieldNode typeInfoEnumFieldNode)
    {
        var parameters = new List<EnumItemParameterTypeInfo>();
        foreach (var parameter in typeInfoEnumFieldNode.Parameters)
        {
            VisitTypeInfoNode(parameter);

            parameters.Add(
                new EnumItemParameterTypeInfo(parameter.Named.Name, parameter.TypeRef)
            );
        }

        typeInfoEnumFieldNode.TypeRef.TypeInfo = new EnumItemTypeInfo(
            CurrentScope,
            typeInfoEnumFieldNode.Named.Name,
            parameters
        );

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


    public override TypeInfoArrayNode VisitTypeInfoArrayNode(TypeInfoArrayNode typeInfoArrayNode)
    {
        base.VisitTypeInfoNode(typeInfoArrayNode.BaseType);

        typeInfoArrayNode.TypeRef = new TypeRef(
            new GenericTypeInfo("Array", [typeInfoArrayNode.BaseType.TypeRef])
        );

        return base.VisitTypeInfoArrayNode(typeInfoArrayNode);
    }

    public override FieldAccessNode VisitFieldAccessNode(FieldAccessNode fieldAccessNode)
    {
        VisitIdentifierNode(fieldAccessNode.BaseObjectName);

        if (SemanticHandler.TryLookupIdentifier(fieldAccessNode.BaseObjectName.GetName(),
                out var baseObjectIdentifierSymbol)
            && baseObjectIdentifierSymbol.Scope.Parent != null)
        {
            if (baseObjectIdentifierSymbol.TypeRef.TypeInfo is not ObjectTypeInfo objectTypeInfo)
            {
                throw new CompileError.SemanticError(
                    "cannot access a non object",
                    baseObjectIdentifierSymbol.Node
                );
            }

            using (MustRecallScope(objectTypeInfo.Scope))
            {
                VisitBaseNode(fieldAccessNode.Member);
            }
        }
        else
        {
            throw new CompileError.SemanticError(
                $"could not find the identifier {fieldAccessNode.BaseObjectName.GetName()}",
                fieldAccessNode.BaseObjectName
            );
        }

        fieldAccessNode.TypeRef = fieldAccessNode.Member.TypeRef;

        return fieldAccessNode;
    }

    public override TypeFieldAccessNode VisitTypeFieldAccessNode(TypeFieldAccessNode typeFieldAccessNode)
    {
        VisitTypeInfoNameNode(typeFieldAccessNode.BaseTypeNode);

        if (typeFieldAccessNode.BaseTypeNode.TypeRef.TypeInfo is not AbstractStructuralTypeInfo
            baseObjectStructuralTypeInfo)
        {
            throw new CompileError.SemanticError(
                "the base object should be a structural type",
                typeFieldAccessNode
            );
        }

        using (MustRecallScope(baseObjectStructuralTypeInfo.Scope))
        {
            VisitBaseNode(typeFieldAccessNode.Member);
        }

        return typeFieldAccessNode;
    }

    public override IdentifierNode VisitIdentifierNode(IdentifierNode identifierNode)
    {
        if (identifierNode.Name == "this")
        {
            if (CurrentObjectDeclarationNode == null)
            {
                throw new CompileError.SemanticError(
                    "cannot access this in a non object context",
                    identifierNode
                );
            }

            identifierNode.TypeRef = CurrentObjectDeclarationNode.TypeRef;

            return identifierNode;
        }

        if (identifierNode.Name == "super")
        {
            if (!SemanticHandler.TryGetScopeOfType(ScopeType.Object, out var objectScope))
            {
                throw new CompileError.SemanticError(
                    "super can only be used in object scope",
                    identifierNode
                );
            }

            if (objectScope.AttachedNode.TypeRef.TypeInfo is not ObjectTypeInfo objectTypeInfo)
            {
                throw new CompileError.SemanticError(
                    "super can only be used in object scope",
                    identifierNode
                );
            }

            if (objectTypeInfo.BaseClass == null)
            {
                throw new CompileError.SemanticError(
                    "object has no base class",
                    identifierNode
                );
            }

            identifierNode.TypeRef = objectTypeInfo.BaseClass;

            return identifierNode;
        }

        if (CurrentScope.Type == ScopeType.Enum && SemanticHandler.TryLookupType(identifierNode.Name, out var symbol))
        {
            if (symbol.TypeRef.TypeInfo.IsUnknown)
            {
                throw new CompileError.SemanticError(
                    "unknown type",
                    symbol.Node
                );
            }

            identifierNode.TypeRef = symbol.TypeRef;
        }
        else if (SemanticHandler.TryLookupIdentifier(identifierNode.Name, out symbol))
        {
            if (symbol.TypeRef.TypeInfo.IsUnknown && symbol.Node is not VariableDeclarationNode)
            {
                VisitBaseNode(symbol.Node);
            }

            identifierNode.TypeRef = symbol.TypeRef;
        }
        else
        {
            throw new CompileError.SemanticError(
                $"identifier {identifierNode.Name} not found",
                identifierNode
            );
        }

        return identifierNode;
    }

    public override ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        AddObjectDeclarationNode(objectDeclarationNode);

        objectDeclarationNode = base.VisitObjectDeclarationNode(objectDeclarationNode);

        PopObjectDeclarationNode();

        return objectDeclarationNode;
    }

    protected void AddObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        _objectDeclarationNodes.Add(objectDeclarationNode);
    }

    protected ObjectDeclarationNode PopObjectDeclarationNode()
    {
        var objectDeclarationNode = _objectDeclarationNodes.Last();

        _objectDeclarationNodes.RemoveAt(_objectDeclarationNodes.Count - 1);

        return objectDeclarationNode;
    }
}