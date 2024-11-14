using Compiler.Syntax.Nodes;

namespace Compiler.Semantics.ScopeHandling;

public enum ScopeType
{
    Program,
    BodyBlock,
    Function,
    Object,
    Enum
}

public class Scope(Scope? parent, ScopeType type, BaseNode attachedNode)
{
    public Scope? Parent { get; } = parent;
    public ScopeType Type { get; } = type;

    public BaseNode AttachedNode { get; } = attachedNode;

    public List<Symbol> Symbols { get; } = [];

    public bool ReturnStatementFound { get; set; } = false;

    public override string ToString()
    {
        return $"Scope {{Type = {Type}, AttachedNode = {attachedNode}}}";
    }
}