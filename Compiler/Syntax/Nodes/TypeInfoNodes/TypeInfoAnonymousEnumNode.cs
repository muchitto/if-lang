using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes.TypeInfoNodes;

/**
 * Represents and inline enum type definition.
 */
public class TypeInfoAnonymousEnumNode(
    NodeContext nodeContext,
    List<TypeInfoEnumFieldNode> fields
) : TypeInfoNode(nodeContext), IEquatable<BaseNode>
{
    public List<TypeInfoEnumFieldNode> Fields { get; } = fields;

    public bool Equals(BaseNode? other)
    {
        return other is TypeInfoAnonymousEnumNode anonymousEnumTypeInfoNode &&
               Fields.SequenceEqual(anonymousEnumTypeInfoNode.Fields);
    }

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitTypeInfoAnonymousEnumNode(this);
    }
}