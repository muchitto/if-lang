using Compiler.ErrorHandling;
using Compiler.Semantics.TypeInformation.Types;

namespace Compiler.Semantics.TypeInformation.TypeComparer;

public class IsComparer : BaseComparer
{
    public override bool CompareFunctionTypeInfo(FunctionTypeInfo typeInfo, TypeInfo other)
    {
        return Compare(typeInfo.ReturnType.TypeInfo, other);
    }

    public override bool CompareObjectTypeInfo(ObjectTypeInfo typeInfo, TypeInfo other)
    {
        if (other is not ObjectTypeInfo objectTypeInfo)
        {
            return false;
        }

        return base.CompareObjectTypeInfo(typeInfo, other)
               || typeInfo.IsChildOf(objectTypeInfo)
               || typeInfo.IsParentOf(objectTypeInfo);
    }
}