using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class TypeCastNode(NodeContext nodeContext, TypeInfoNameNode fromTypeInfoName, TypeInfoNameNode toTypeInfoName)
    : BaseNode(nodeContext)
{
    public TypeInfoNameNode FromTypeInfoName = fromTypeInfoName;

    public TypeInfoNameNode ToTypeInfoName = toTypeInfoName;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitTypeCastNode(this);
    }
}