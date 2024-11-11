using Compiler.Semantics.TypeInformation;
using Compiler.Syntax.Nodes;

namespace Compiler;

public enum SymbolType
{
    Identifier,
    Type
}

public class Symbol(string name, Scope scope, BaseNode node, SymbolType type)
{
    public string Name { get; init; } = name;

    public TypeRef TypeRef => Node.TypeRef;

    public Scope Scope { get; init; } = scope;

    public BaseNode Node { get; init; } = node;

    public SymbolType Type { get; init; } = type;
}