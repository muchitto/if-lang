namespace Compiler.TypeInformation;

public class TypeRef(TypeInfo typeInfo)
{
    public TypeInfo TypeInfo { get; set; } = typeInfo;

    public bool Compare(TypeRef other)
    {
        return TypeInfo.Compare(other.TypeInfo);
    }

    public bool Compare(TypeInfo other)
    {
        return TypeInfo.Compare(other);
    }
}