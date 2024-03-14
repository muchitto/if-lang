using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class TypeInfoStructureNode(Dictionary<string, TypeInfoNode> fields) : TypeInfoNode
{
    public Dictionary<string, TypeInfoNode> Fields { get; } = fields;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitTypeInfoStructureNode(this);
    }
}