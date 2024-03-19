using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class AnnotationNode(NodeContext nodeContext, IdentifierNode name, List<BaseNode> arguments)
    : BaseNode(nodeContext)
{
    public IdentifierNode Name { get; } = name;
    public List<BaseNode> Arguments { get; } = arguments;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitAnnotationNode(this);
    }
}