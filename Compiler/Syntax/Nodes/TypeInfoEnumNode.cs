using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class TypeInfoEnumNode(NodeContext nodeContext, List<TypeInfoEnumFieldNode> fields) : TypeInfoNode(nodeContext)
{
    public List<TypeInfoEnumFieldNode> Fields { get; } = fields;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitTypeInfoEnumNode(this);
    }
}