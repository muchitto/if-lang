using Compiler.Syntax;

namespace Compiler.Semantics.ControlFlow;

public class BasicBlock : ControlFlowNode
{
    public BasicBlock(NodeContext nodeContext, List<ControlFlowNode> nodes) : base(nodeContext)
    {
        Nodes = nodes;
    }

    public List<ControlFlowNode> Nodes { get; set; } = [];
}