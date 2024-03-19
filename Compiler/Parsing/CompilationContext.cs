using Compiler.ErrorHandling;

namespace Compiler.Parsing;

public class CompilationContext(PositionData positionData)
{
    public PositionData PositionData { get; set; } = positionData;
}