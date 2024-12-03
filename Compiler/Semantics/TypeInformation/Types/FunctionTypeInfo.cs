namespace Compiler.Semantics.TypeInformation.Types;

public class FunctionTypeInfo(TypeRef returnType, List<FunctionParameterTypeInfo> parameters) : TypeInfo
{
    public TypeRef ReturnType { get; } = returnType;
    public List<FunctionParameterTypeInfo> Parameters { get; } = parameters;

    public override string? TypeName => null;

    public override string ToString()
    {
        return $"Function() -> {ReturnType}";
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitFunctionTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareFunctionTypeInfo(this, other);
    }
}