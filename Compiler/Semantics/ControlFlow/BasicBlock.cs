namespace Compiler.Semantics.ControlFlow;

public class BasicBlock
{
    public BasicBlock(List<ControlFlowNode> nodes)
    {
        Nodes = nodes;
    }

    public List<ControlFlowNode> Nodes { get; set; } = [];
}