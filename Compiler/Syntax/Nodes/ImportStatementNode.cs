using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ImportStatementNode : BaseNode
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitImportStatementNode(this);
    }
}