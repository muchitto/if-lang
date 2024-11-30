using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ProgramNode(NodeContext nodeContext, List<DeclarationNode> declarations) : BaseNode(nodeContext)
{
    public List<DeclarationNode> Declarations { get; set; } = declarations;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitProgramNode(this);
    }
}