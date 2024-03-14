using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class AnnotationNode : BaseNode
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitAnnotationNode(this);
    }
}