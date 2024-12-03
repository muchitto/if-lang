using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ProgramNode(
    NodeContext nodeContext,
    List<DeclarationNode> declarations,
    List<BaseNode> generatedMainFunctionStatements
) : BaseNode(nodeContext)
{
    public List<DeclarationNode> Declarations { get; set; } = declarations;

    public List<BaseNode> GeneratedMainFunctionStatements { get; set; } = generatedMainFunctionStatements;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitProgramNode(this);
    }
}