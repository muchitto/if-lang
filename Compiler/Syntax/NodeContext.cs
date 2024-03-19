using Compiler.ErrorHandling;

namespace Compiler.Syntax;

public record NodeContext(PositionData PositionData)
{
    public NodeContext(NodeContext nodeContext, NodeContext otherNodeContext)
        : this(nodeContext.PositionData.PositionDataTo(otherNodeContext.PositionData))
    {
    }
}