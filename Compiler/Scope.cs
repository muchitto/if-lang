using Compiler.Syntax.Nodes;

namespace Compiler;

public enum ScopeType
{
    Program,
    BlockBody,
    Function,
    Object
}

public class Scope(Scope? parent, ScopeType type, BaseNode attachedNode)
{
    public Scope? Parent { get; } = parent;
    public ScopeType Type { get; } = type;

    public BaseNode AttachedNode { get; } = attachedNode;

    public List<Symbol> Symbols { get; } = [];

    public bool ReturnStatementFound { get; set; } = false;
}