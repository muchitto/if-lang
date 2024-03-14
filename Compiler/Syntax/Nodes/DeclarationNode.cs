using Compiler.TypeInformation;

namespace Compiler.Syntax.Nodes;

public abstract class DeclarationNode(IdentifierNode name) : BaseNode
{
    private TypeRef _typeRef = new(TypeInfo.Unknown);
    public IdentifierNode Name { get; } = name;

    public override TypeRef TypeRef
    {
        get => _typeRef;
        set => Name.TypeRef = _typeRef = value;
    }
}