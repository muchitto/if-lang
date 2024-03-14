namespace Compiler.Lexing;

public record struct PositionData(string FilePath, string SourceCode, int ColFrom, int ColTo)
{
    public PositionData WithColFrom(int colFrom)
    {
        return this with { ColFrom = colFrom };
    }

    public PositionData WithColTo(int colTo)
    {
        return this with { ColTo = colTo };
    }

    public PositionData WithColFromAndColTo(int colFrom, int colTo)
    {
        return this with { ColFrom = colFrom, ColTo = colTo };
    }

    public PositionData MoveColFrom(int offset)
    {
        return this with { ColFrom = ColFrom + offset };
    }

    public PositionData MoveColTo(int offset)
    {
        return this with { ColTo = ColTo + offset };
    }

    public PositionData PositionDataSpan(int colToOffset)
    {
        return this with { ColTo = ColFrom + colToOffset };
    }
}