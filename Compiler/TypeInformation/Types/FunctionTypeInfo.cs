namespace Compiler.TypeInformation.Types;

public class FunctionTypeInfo(TypeRef returnType, Dictionary<string, TypeRef> parameters) : TypeInfo
{
    public TypeRef ReturnType { get; } = returnType;
    public Dictionary<string, TypeRef> Parameters { get; } = parameters;

    public override string ToString()
    {
        return $"Function<{ReturnType}>";
    }

    public override bool Compare(TypeInfo other)
    {
        if (other is not FunctionTypeInfo functionTypeInfo || !ReturnType.Compare(functionTypeInfo.ReturnType) ||
            Parameters.Count != functionTypeInfo.Parameters.Count)
        {
            return false;
        }

        for (var i = 0; i < Parameters.Count; i++)
        {
            if (!Parameters.ElementAt(i).Value.Compare(functionTypeInfo.Parameters.ElementAt(i).Value))
            {
                return false;
            }
        }

        return true;
    }

    public override bool HasDeferredTypes()
    {
        return ReturnType.TypeInfo.HasDeferredTypes() || Parameters.Values.Any(x => x.TypeInfo.HasDeferredTypes());
    }
}