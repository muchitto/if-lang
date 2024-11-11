using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class AnnotationNode(NodeContext nodeContext, IdentifierNode name, List<BaseNode> arguments)
    : BaseNode(nodeContext), IEquatable<BaseNode>
{
    public IdentifierNode Name { get; } = name;
    public List<BaseNode> Arguments { get; } = arguments;

    public bool Equals(BaseNode? other)
    {
        return other is AnnotationNode annotationNode &&
               Name.Equals(annotationNode.Name) &&
               Arguments.SequenceEqual(annotationNode.Arguments);
    }

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitAnnotationNode(this);
    }
}