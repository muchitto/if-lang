namespace Compiler.Semantics.TypeInformation.Types;

public class FunctionTypeInfo(TypeRef returnType, Dictionary<string, TypeRef> parameters) : TypeInfo
{
    public TypeRef ReturnType { get; } = returnType;
    public Dictionary<string, TypeRef> Parameters { get; } = parameters;

    public override string ToString()
    {
        return $"Function<{ReturnType}>";
    }

    public override bool HasDeferredTypes()
    {
        return ReturnType.TypeInfo.HasDeferredTypes() || Parameters.Values.Any(x => x.TypeInfo.HasDeferredTypes());
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitFunctionTypeInfo(this);
    }
}