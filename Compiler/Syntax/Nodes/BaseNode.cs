using Compiler.Syntax.Visitor;
using Compiler.TypeInformation;

namespace Compiler.Syntax.Nodes;

public abstract class BaseNode
{
    public Scope? Scope { get; set; }
    public virtual TypeRef TypeRef { get; set; } = new(TypeInfo.Unknown);

    public virtual void Accept(INodeVisitor nodeVisitor)
    {
    }
}