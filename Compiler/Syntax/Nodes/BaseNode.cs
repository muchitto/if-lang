using Compiler.Semantics.TypeInformation;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public abstract class BaseNode(NodeContext nodeContext)
{
    public Scope? Scope { get; set; }
    public virtual TypeRef TypeRef { get; set; } = new(TypeInfo.Unknown);

    public NodeContext NodeContext { get; } = nodeContext;

    public virtual void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitBaseNode(this);
    }
}