using Compiler.Syntax;

namespace Compiler.Semantics.ControlFlow;

public abstract class ControlFlowNode(NodeContext nodeContext)
{
    public NodeContext NodeContext { get; set; } = nodeContext;
}