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

    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode
    )
    {
        if (variableDeclarationNode.TypeInfo != null)
        {
            VisitTypeInfoNode(variableDeclarationNode.TypeInfo);
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

    public override TypeInfoAnonymousEnumNode VisitTypeInfoAnonymousEnumNode(
        TypeInfoAnonymousEnumNode typeInfoAnonymousEnumNode
    )
    {
        using (EnterScope(ScopeType.Enum, typeInfoAnonymousEnumNode, out var scope))
        {
            var fields = new Dictionary<string, TypeRef>();
            foreach (var field in typeInfoAnonymousEnumNode.Fields)
            {
                VisitTypeInfoNode(field);

                fields.Add(field.Named.Name, field.TypeRef);
            }

            typeInfoAnonymousEnumNode.TypeRef.TypeInfo = new AnonymousEnumTypeInfo(scope, fields);
        }

        return typeInfoAnonymousEnumNode;
    }


    public override TypeInfoEnumFieldNode VisitTypeInfoEnumFieldNode(TypeInfoEnumFieldNode typeInfoEnumFieldNode)
    {
        var parameters = new Dictionary<string, TypeRef>();
        foreach (var parameter in typeInfoEnumFieldNode.Parameters)
        {
            VisitTypeInfoNode(parameter);

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


    public override TypeInfoArrayNode VisitTypeInfoArrayNode(TypeInfoArrayNode typeInfoArrayNode)
    {
        base.VisitTypeInfoNode(typeInfoArrayNode.BaseType);

        typeInfoArrayNode.TypeRef = new TypeRef(
            new GenericTypeInfo("Array", [typeInfoArrayNode.BaseType.TypeRef])
        );

        return base.VisitTypeInfoArrayNode(typeInfoArrayNode);
    }

    public override MemberAccessNode VisitMemberAccessNode(MemberAccessNode memberAccessNode)
    {
        VisitIdentifierNode(memberAccessNode.BaseObject);

        if (SemanticHandler.TryLookupIdentifier(memberAccessNode.BaseObject.GetName(),
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
                VisitBaseNode(memberAccessNode.Member);
            }
        }
        else
        {
            throw new CompileError.SemanticError(
                $"could not find the identifier {memberAccessNode.BaseObject.GetName()}",
                memberAccessNode.BaseObject
            );
        }

        if (memberAccessNode.Member.TypeRef.IsUnknown)
        {
            throw new CompileError.SemanticError(
                $"could not find the type of {memberAccessNode.Member.GetName()}",
                memberAccessNode.Member
            );
        }

        memberAccessNode.TypeRef = memberAccessNode.Member.TypeRef;

        return memberAccessNode;
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

        if (SemanticHandler.TryLookupIdentifier(identifierNode.Name, out var symbol))
        {
            if (symbol.TypeRef.TypeInfo.IsUnknown)
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