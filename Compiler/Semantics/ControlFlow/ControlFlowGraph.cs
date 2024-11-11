namespace Compiler.Semantics.ControlFlow;

public class ControlFlowGraph
{
    public ControlFlowGraph(List<BasicBlock> blocks)
    {
        Blocks = blocks;
        EntryBlock = Blocks[0];
        ExitBlock = Blocks[^1];
    }

    public List<Symbol> InvolvedSymbols { get; set; } = [];

    public BasicBlock EntryBlock { get; set; }

    public List<BasicBlock> Blocks { get; set; }

    public BasicBlock ExitBlock { get; set; }
}