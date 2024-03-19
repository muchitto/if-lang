using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ProgramNode(NodeContext nodeContext, List<DeclarationNode> declarations) : BaseNode(nodeContext)
{
    public List<DeclarationNode> Declarations { get; } = declarations;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitProgramNode(this);
    }
}