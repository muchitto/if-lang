using Compiler.ErrorHandling;
using Compiler.Semantics.TypeInformation.Types;

namespace Compiler.Semantics.TypeInformation.TypeComparer;

public class ExpressionComparer : BaseComparer
{
    public override bool CompareFunctionTypeInfo(FunctionTypeInfo typeInfo, TypeInfo other)
    {
        return Compare(typeInfo.ReturnType.TypeInfo, other);
    }
}