using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ProgramNode(List<DeclarationNode> declarations) : BaseNode
{
    public List<DeclarationNode> Declarations { get; } = declarations;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitProgramNode(this);
    }
}