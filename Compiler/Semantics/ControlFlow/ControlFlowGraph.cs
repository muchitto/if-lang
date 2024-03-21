namespace Compiler.Semantics.ControlFlow;

public class ControlFlowGraph
{
    public ControlFlowGraph(BasicBlock entryBlock, BasicBlock exitBlock, List<BasicBlock> blocks)
    {
        EntryBlock = entryBlock;
        ExitBlock = exitBlock;
        Blocks = blocks;
    }

    public BasicBlock EntryBlock { get; set; }

    public List<BasicBlock> Blocks { get; set; } = [];

    public BasicBlock ExitBlock { get; set; }
}