using Compiler.ErrorHandling;
using Compiler.Semantics.TypeInformation.TypeComparer;
using Compiler.Semantics.TypeInformation.Types;
using Compiler.Semantics.TypeInformation.TypeVisitor;

namespace Compiler.Semantics.TypeInformation;

public class TypeRef(TypeInfo typeInfo)
{
    public TypeInfo TypeInfo { get; set; } = typeInfo;

    public bool IsOptionalType => TypeInfo is GenericTypeInfo { Name: "Optional" };

    public bool IsUnknown => typeInfo.IsUnknown;

    public bool Compare<TComparer>(TypeRef other, out List<CompileError> errors)
        where TComparer : ITypeComparer<CompileError>, new()
    {
        var comparer = new TComparer();
        var result = comparer.Compare(TypeInfo, other.TypeInfo);
        errors = comparer.Errors;
        return result;
    }

    public bool Compare<TComparer>(TypeInfo other)
        where TComparer : ITypeComparer<CompileError>, new()
    {
        var comparer = new TComparer();
        return comparer.Compare(TypeInfo, other);
    }

    public bool Compare<TComparer>(TypeRef other)
        where TComparer : ITypeComparer<CompileError>, new()
    {
        return Compare<TComparer>(other.TypeInfo);
    }

    public bool Compare(TypeRef other)
    {
        return Compare(other.TypeInfo);
    }

    public bool Compare(TypeInfo other)
    {
        return Compare<BasicComparer>(other);
    }

    public bool HasDeferredTypes()
    {
        if (TypeInfo is DeferredTypeInfo)
        {
            return true;
        }

        var visitor = new DeferredTypesVisitor();
        visitor.VisitTypeInfo(TypeInfo);
        return visitor.HasDeferredTypes;
    }

    public override string ToString()
    {
        return TypeInfo?.ToString() ?? "null";
    }
}