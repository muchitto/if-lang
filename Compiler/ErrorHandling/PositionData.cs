using System.Text;

namespace Compiler.ErrorHandling;

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

    public PositionData PositionDataTo(PositionData other)
    {
        return this with { ColTo = other.ColTo };
    }

    public (int Line, int Column) GetLineAndColumn()
    {
        var line = 1;
        var column = 1;
        for (var i = 0; i < ColFrom; i++)
        {
            if (SourceCode[i] == '\n')
            {
                line++;
                column = 1;
            }
            else
            {
                column++;
            }
        }

        return (line, column);
    }

    public string GetCurrentLine()
    {
        var lineStart = ColFrom;
        var lineEnd = ColTo;

        while (lineStart > 0 && SourceCode[lineStart - 1] != '\n')
        {
            lineStart--;
        }

        while (lineEnd < SourceCode.Length && SourceCode[lineEnd] != '\n')
        {
            lineEnd++;
        }

        return SourceCode.Substring(lineStart, lineEnd - lineStart);
    }

    public string GetErrorArrow(string prefix)
    {
        var arrow = new StringBuilder();

        var (_, col) = GetLineAndColumn();

        // make prefixes from the start of the line to the error position
        for (var i = 0; i < col - 1; i++)
        {
            arrow.Append(prefix);
        }

        // make the arrow
        arrow.Append('^', ColTo - ColFrom);

        return arrow.ToString();
    }
}