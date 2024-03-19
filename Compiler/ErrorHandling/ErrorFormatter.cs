using System.Text;

namespace Compiler.ErrorHandling;

public class ErrorFormatter
{
    public static string FormatError(string message, PositionData positionData)
    {
        var (line, col) = positionData.GetLineAndColumn();

        var positionStr = new StringBuilder();

        positionStr.AppendLine($"in file {positionData.FilePath} on line {line} and column {col}:");
        positionStr.AppendLine(message);

        positionStr.AppendLine('\t' + positionData.GetCurrentLine());
        positionStr.Append('\t' + positionData.GetErrorArrow("-"));

        return positionStr.ToString();
    }
}