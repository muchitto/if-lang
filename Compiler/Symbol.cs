using Compiler.Semantics.TypeInformation;
using Compiler.Syntax.Nodes;

namespace Compiler;

public enum SymbolType
{
    Identifier,
    Type
}

public class Symbol
{
    public Symbol(string name, Scope scope, BaseNode node, SymbolType type)
    {
        Name = name;
        Scope = scope;
        Node = node;
        Type = type;
    }

    public string Name { get; init; }

    public TypeRef TypeRef => Node.TypeRef;

    public Scope Scope { get; init; }

    public BaseNode Node { get; init; }

    public SymbolType Type { get; init; }
}