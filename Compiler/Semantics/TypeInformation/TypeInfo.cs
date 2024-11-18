using Compiler.Semantics.TypeInformation.Types;
using Compiler.Semantics.TypeInformation.TypeVisitor;

namespace Compiler.Semantics.TypeInformation;

public abstract class TypeInfo
{
    public static readonly TypeInfo Void = new VoidTypeInfo();
    public static readonly TypeInfo String = new StringTypeInfo();
    public static readonly TypeInfo Int16 = new NumberTypeInfo(NumberType.Int16);
    public static readonly TypeInfo Int32 = new NumberTypeInfo(NumberType.Int32);
    public static readonly TypeInfo Int64 = new NumberTypeInfo(NumberType.Int64);
    public static readonly TypeInfo UInt16 = new NumberTypeInfo(NumberType.UInt16);
    public static readonly TypeInfo UInt32 = new NumberTypeInfo(NumberType.UInt32);
    public static readonly TypeInfo UInt64 = new NumberTypeInfo(NumberType.UInt64);
    public static readonly TypeInfo Float32 = new NumberTypeInfo(NumberType.Float32);
    public static readonly TypeInfo Float64 = new NumberTypeInfo(NumberType.Float64);

    public static readonly TypeInfo Boolean = new BooleanTypeInfo();
    public static readonly TypeInfo Unknown = new UnknownTypeInfo();
    public static readonly TypeInfo Deferred = new DeferredTypeInfo();
    public static readonly TypeInfo Object = new ObjectTypeInfo(null, null, "object", []);
    public static readonly TypeRef ObjectRef = new(Object);

    public static Dictionary<string, TypeInfo> StringToTypeInfo = new()
    {
        { "void", Void },
        { "string", String },
        { "int", Int32 },
        { "int16", Int16 },
        { "int32", Int32 },
        { "int64", Int64 },
        { "uint16", UInt16 },
        { "uint32", UInt32 },
        { "uint64", UInt64 },
        { "float", Float64 },
        { "float32", Float32 },
        { "float64", Float64 },
        { "bool", Boolean },
        { "object", Object }
    };

    public abstract string? TypeName { get; }

    public bool IsIncomplete => IncompleteVisitor.IsIncompleteType(this);

    public bool IsUnknown => UnknownVisitor.HasUnknownType(this);

    public static bool TryGetBuiltInType(string name, out TypeInfo typeInfo)
    {
        if (StringToTypeInfo.TryGetValue(name, out typeInfo))
        {
            return true;
        }

        typeInfo = null;

        return false;
    }

    public virtual void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitTypeInfo(this);
    }

    public virtual bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        throw new NotImplementedException();
    }
}