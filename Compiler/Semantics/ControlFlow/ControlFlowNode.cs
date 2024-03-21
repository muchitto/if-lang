using Compiler.Syntax;

namespace Compiler.Semantics.ControlFlow;

public class ControlFlowNode
{
    public ControlFlowNode(NodeContext nodeContext)
    {
        NodeContext = nodeContext;
    }

    public NodeContext NodeContext { get; set; }
}