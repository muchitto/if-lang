using Compiler.Syntax.Nodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Semantics.SemanticPasses;

public class ControlFlowConstructionVisitor(SemanticContext semanticContext) : BaseNodeVisitor(semanticContext)
{
    public override AnnotationNode VisitAnnotationNode(AnnotationNode annotationNode)
    {
        return base.VisitAnnotationNode(annotationNode);
    }

    public override BodyBlockNode VisitBodyBlockNode(BodyBlockNode bodyBlockNode)
    {
        return base.VisitBodyBlockNode(bodyBlockNode);
    }
}