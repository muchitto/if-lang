using Compiler.ErrorHandling;
using Compiler.Semantics.TypeInformation.TypeComparer;

namespace Compiler.Semantics.TypeInformation;

public class TypeRef
{
    public TypeRef(TypeInfo typeInfo)
    {
        TypeInfo = typeInfo;
    }

    public TypeInfo TypeInfo { get; set; }

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
}